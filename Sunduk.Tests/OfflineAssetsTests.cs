using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Sunduk.PWA.Infrastructure.Offline;
using Xunit;

namespace Sunduk.Tests
{
    /// <summary>
    /// Тест, который требует наличия publish-папки. Если её нет — тест пропускается
    /// на этапе discovery, а не падает: «dotnet test» без предварительной публикации
    /// должен оставаться зелёным.
    /// </summary>
    public sealed class PublishFactAttribute : FactAttribute
    {
        public const string PublishHint =
            "Нет publish-папки. Выполните: dotnet publish Sunduk.PWA/Sunduk.PWA.csproj -c Release --output release";

        public PublishFactAttribute()
        {
            if (OfflineAssetsTests.FindPublishedWwwRoot() is null)
                Skip = PublishHint;
        }
    }

    /// <summary>
    /// Регресс-тесты офлайн-режима PWA.
    ///
    /// История: в index.html стояли ссылки вида "_content/MudBlazor/MudBlazor.min.css?v=1".
    /// Service worker складывает в кэш URL из манифеста, где query-строки нет, а
    /// Cache.match по умолчанию не игнорирует query — в офлайне эти файлы не находились
    /// никогда. Приложение открывалось без разметки и без работающих диалогов MudBlazor.
    /// </summary>
    public class OfflineAssetsTests
    {
        #region Пути

        public static string RepoRoot()
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Sunduk.sln")))
                dir = dir.Parent;

            return dir?.FullName
                ?? throw new InvalidOperationException("Не найден корень репозитория (Sunduk.sln)");
        }

        private static string PwaProjectRoot => Path.Combine(RepoRoot(), "Sunduk.PWA");

        private static string IndexHtmlPath => Path.Combine(PwaProjectRoot, "wwwroot", "index.html");

        private static string PublishedServiceWorkerPath =>
            Path.Combine(PwaProjectRoot, "wwwroot", "service-worker.published.js");

        /// <summary>
        /// Ищет wwwroot публикации. AGENTS.md и CI публикуют в «release» от корня репозитория.
        /// </summary>
        public static string? FindPublishedWwwRoot()
        {
            try
            {
                var root = RepoRoot();
                var candidates = new[]
                {
                    Path.Combine(root, "release", "wwwroot"),
                    Path.Combine(root, "release"),
                    Path.Combine(PwaProjectRoot, "release", "wwwroot"),
                    Path.Combine(PwaProjectRoot, "release"),
                    Path.Combine(PwaProjectRoot, "bin", "Release", "net10.0", "publish", "wwwroot"),
                };

                return candidates.FirstOrDefault(c =>
                    File.Exists(Path.Combine(c, "index.html"))
                    && File.Exists(Path.Combine(c, "service-worker-assets.js")));
            }
            catch (IOException)
            {
                return null;
            }
        }

        private static string PublishedFile(string name)
        {
            var root = FindPublishedWwwRoot()
                ?? throw new InvalidOperationException(PublishFactAttribute.PublishHint);
            return File.ReadAllText(Path.Combine(root, name));
        }

        #endregion

        private static string ReadIndexHtml() => File.ReadAllText(IndexHtmlPath);

        private static string ReadPublishedServiceWorker() => File.ReadAllText(PublishedServiceWorkerPath);

        /// <summary>Все локальные ресурсы, на которые ссылается index.html.</summary>
        private static string[] LocalReferences(string html)
        {
            return Regex.Matches(html, "(?:href|src)\\s*=\\s*\"([^\"]+)\"")
                .Select(m => m.Groups[1].Value)
                .Where(u => !string.IsNullOrWhiteSpace(u))
                .Where(u => !u.StartsWith("#")
                    && !u.StartsWith("mailto:")
                    && !u.StartsWith("http://")
                    && !u.StartsWith("https://")
                    && !u.StartsWith("//")
                    && !u.StartsWith("data:"))
                .ToArray();
        }

        #region Тесты исходников (работают всегда)

        [Fact]
        public void IndexHtml_НеСодержитCacheBustingQuery()
        {
            var offenders = LocalReferences(ReadIndexHtml())
                .Where(u => u.Contains('?'))
                .ToArray();

            Assert.True(offenders.Length == 0,
                "index.html не должен запрашивать ассеты с query-строкой: service worker " +
                "кэширует URL из манифеста (без query), а Cache.match по умолчанию не " +
                "игнорирует query, поэтому в офлайне такие файлы не находятся. " +
                "Уберите '?v=' из: " + string.Join(", ", offenders));
        }

        [Fact]
        public void ServiceWorker_ИспользуетIgnoreSearch()
        {
            Assert.Contains("ignoreSearch", ReadPublishedServiceWorker(), StringComparison.Ordinal);
        }

        [Fact]
        public void ServiceWorker_КэшируетWebmanifest()
        {
            Assert.Contains("webmanifest", ReadPublishedServiceWorker(), StringComparison.Ordinal);
        }

        [Fact]
        public void ServiceWorker_НеТеряетОшибкиКэширования()
        {
            var sw = ReadPublishedServiceWorker();

            // Пропущенные ассеты должны попадать в postMessage/console.error, а не
            // молча теряться в try/catch: именно так раньше не заметили сломанные ссылки.
            Assert.Contains("offline-progress", sw, StringComparison.Ordinal);
            Assert.Contains("failed", sw, StringComparison.Ordinal);
        }

        [Fact]
        public void ServiceWorker_НеТеряетНавигациюПриОшибкеФетча()
        {
            var sw = ReadPublishedServiceWorker();

            // Без fallback ветки офлайн-промайс реджектится и пользователь получает
            // страницу ошибки браузера вместо приложения.
            Assert.Contains("status: 504", sw, StringComparison.Ordinal);
        }

        #endregion

        #region Маппинг состояний индикатора

        [Theory]
        // Ещё не спросили браузер — не врём «не поддерживается» на первом кадре.
        [InlineData(false, true, "none", false, null, null, OfflineAvailability.Unknown)]
        // Браузер не умеет service worker.
        [InlineData(false, false, "unsupported", false, null, null, OfflineAvailability.Unsupported)]
        // Первичная загрузка офлайн-версии.
        [InlineData(true, true, "installing", false, null, null, OfflineAvailability.Preparing)]
        [InlineData(true, true, "active", false, null, null, OfflineAvailability.Preparing)]
        // Готово, можно отключать интернет.
        [InlineData(true, true, "active", true, "abc", "abc", OfflineAvailability.Ready)]
        // SW зарегистрирован, новая версия ждёт активации — смысл тот же, что и перезагрузка.
        [InlineData(true, true, "waiting", false, null, null, OfflineAvailability.UpdateReady)]
        // Новая версия установлена, страница ещё на старой.
        [InlineData(true, true, "active", true, "new", "old", OfflineAvailability.UpdateReady)]
        // Нет сети.
        [InlineData(true, false, "active", true, "abc", "abc", OfflineAvailability.Offline)]
        [InlineData(true, false, "installing", false, null, null, OfflineAvailability.OfflineNoCache)]
        public void Availability_Маппится_ИзСостоянияБраузера(
            bool supported, bool online, string swState, bool cacheReady,
            string? version, string? loadedVersion, OfflineAvailability expected)
        {
            var state = new OfflineState
            {
                Supported = supported,
                Online = online,
                SwState = swState,
                CacheReady = cacheReady,
                Version = version,
                LoadedVersion = loadedVersion
            };

            Assert.Equal(expected, OfflineStatusMonitor.Map(state));
        }

        [Fact]
        public void Availability_ОшибкаОпросаПоказывается()
        {
            // Раньше здесь был молчаливый catch, и устройство с устаревшей копией
            // utils.js в кэше вечно показывало «проверяю…» без всякой диагностики.
            var state = OfflineState.Error("устаревшая копия utils.js");

            Assert.Equal(OfflineAvailability.ProbeFailed, OfflineStatusMonitor.Map(state));
            Assert.Equal("устаревшая копия utils.js", state.Diagnostic);
        }

        [Fact]
        public void Percent_СчитаетГотовностьКэша()
        {
            Assert.Equal(0, new OfflineState().Percent);                                   // total неизвестен
            Assert.Equal(0, new OfflineState { Cached = 10, Total = 0 }.Percent);
            Assert.Equal(50, new OfflineState { Cached = 500, Total = 1000 }.Percent);
            Assert.Equal(100, new OfflineState { Cached = 1000, Total = 1000 }.Percent);
            Assert.Equal(100, new OfflineState { Cached = 2000, Total = 1000 }.Percent);   // клампим
        }

        [Fact]
        public void Availability_БезСетиПоказываетОфлайнДажеЕслиЕстьОбновление()
        {
            var state = new OfflineState
            {
                Supported = true,
                Online = false,
                SwState = "active",
                CacheReady = true,
                Version = "new",
                LoadedVersion = "old"
            };

            // Обновление без сети не установить — пользователю важнее знать, что
            // приложение продолжает работать.
            Assert.Equal(OfflineAvailability.Offline, OfflineStatusMonitor.Map(state));
        }

        #endregion

        #region Тесты публикации

        [PublishFact]
        public void ОпубликованныйIndexHtml_НеСодержитCacheBustingQuery()
        {
            var offenders = LocalReferences(PublishedFile("index.html"))
                .Where(u => u.Contains('?'))
                .ToArray();

            Assert.True(offenders.Length == 0,
                "В опубликованном index.html остались ссылки с query: " + string.Join(", ", offenders));
        }

        [PublishFact]
        public void ОпубликованныйIndexHtml_НеСодержитНераскрытыхПлейсхолдеров()
        {
            var html = PublishedFile("index.html");

            // #[.{fingerprint}] должен быть подставлен на этапе публикации. Если он
            // остался в файле, браузер запросит _framework/blazor.webassembly#[.{fingerprint}].js,
            // получит 404 и приложение не запустится вообще. Такое бывает при
            // инкрементальной публикации поверх устаревшего obj/Release.
            Assert.DoesNotContain("#.{fingerprint}", html, StringComparison.Ordinal);
            Assert.DoesNotContain("#.", html, StringComparison.Ordinal);
        }

        [PublishFact]
        public void ОпубликованныеАссеты_КаждыйЗапрошенныйПопадаетВОфлайнКэш()
        {
            var cached = CacheableUrls(ReadPublishedServiceWorker(), PublishedAssets());

            // Ассеты из index.html запрашиваются по URL, а service worker ищет их в кэше
            // с ignoreSearch — сравниваем по URL без query.
            var requested = LocalReferences(PublishedFile("index.html"))
                .Where(u => !u.StartsWith("/_framework/"))   // fingerprinted рантайм, проверяем отдельно
                .Select(u => u.Split('?')[0].TrimStart('/'))
                .Distinct()
                .ToArray();

            var missing = requested
                .Where(u => u.Length > 0 && !cached.Contains(u))
                .ToArray();

            Assert.True(missing.Length == 0,
                "Эти файлы запрашиваются index.html, но не попадают в офлайн-кэш: "
                + string.Join(", ", missing));
        }

        [PublishFact]
        public void ОпубликованныйМанифест_СодержитОбязательныеФайлыЗапуска()
        {
            var cached = CacheableUrls(ReadPublishedServiceWorker(), PublishedAssets());

            foreach (var required in new[]
                     {
                         "index.html",
                         "manifest.webmanifest",
                         "Sunduk.styles.css",
                         "css/all.min.css",
                         "css/fonts.css",
                         "css/site.css",
                         "css/mudblazor-override.css",
                         "_content/MudBlazor/MudBlazor.min.css",
                         "_content/MudBlazor/MudBlazor.min.js",
                         "_content/Blazor.Common/blazorCommon.js",
                         "_content/Blazor.Text.Editor/blazorTextEditor.js",
                         "_content/CodeBeam.MudBlazor.Extensions/MudExtensions.min.js",
                         "js/utils.js",
                     })
            {
                Assert.True(cached.Contains(required),
                    $"Обязательный для запуска файл «{required}» не попадает в офлайн-кэш.");
            }
        }

        [PublishFact]
        public void ОпубликованныйРантайм_ЗагружаемыеМодулиСуществуют()
        {
            var root = FindPublishedWwwRoot()!;
            var framework = Path.Combine(root, "_framework");
            var loader = Directory.GetFiles(framework, "blazor.webassembly*.js").FirstOrDefault()
                ?? throw new InvalidOperationException("Нет blazor.webassembly*.js в публикации");

            // blazor.webassembly.js приезжает из runtime-пакета Blazor и при публикации
            // НЕ пересобирается под hard-fingerprinting. Он жёстко запрашивает
            // "_framework/dotnet.js". Если в публикации только fingerprinted
            // dotnet.<hash>.js, рантайм не стартует: "Failed to start platform".
            // Проверяем инвариант, а не конкретное имя файла — тогда тест останется
            // верным и при soft, и при hard fingerprinting.
            var requested = Regex.Matches(File.ReadAllText(loader), "[\"'`]_framework/[^\"'`]+[\"'`]")
                .Select(m => m.Value.Trim('"', '\'', '`'))
                .Where(u => !u.Contains("{", StringComparison.Ordinal))
                .Distinct()
                .ToArray();

            Assert.True(requested.Length > 0,
                $"Не удалось определить, какие модули запрашивает {Path.GetFileName(loader)}.");

            var missing = requested
                .Where(u => !File.Exists(Path.Combine(root, u.Replace('/', Path.DirectorySeparatorChar))))
                .ToArray();

            Assert.True(missing.Length == 0,
                $"{Path.GetFileName(loader)} запрашивает файлы, которых нет в публикации: "
                + string.Join(", ", missing)
                + ". Проверьте WasmFingerprintDotnetJs в Sunduk.PWA.csproj.");
        }

        #endregion

        #region Разбор артефактов

        private static HashSet<string> PublishedAssets()
        {
            var js = PublishedFile("service-worker-assets.js");
            var start = js.IndexOf('{');
            var end = js.LastIndexOf('}');
            if (start < 0 || end < start)
                throw new InvalidOperationException("Не удалось разобрать service-worker-assets.js");

            var json = js.Substring(start, end - start + 1);
            return Regex.Matches(json, "\"url\"\\s*:\\s*\"([^\"]+)\"")
                .Select(m => m.Groups[1].Value)
                .ToHashSet(StringComparer.Ordinal);
        }

        /// <summary>
        /// Прогоняет include/exclude регулярные выражения из published service worker
        /// по ассетам манифеста — ровно то, что делает сам service worker.
        /// </summary>
        private static HashSet<string> CacheableUrls(string serviceWorker, HashSet<string> assets)
        {
            var include = ExtractPatterns(serviceWorker, "offlineAssetsInclude");
            var exclude = ExtractPatterns(serviceWorker, "offlineAssetsExclude");

            return assets
                .Where(a => include.Any(p => Regex.IsMatch(a, p)))
                .Where(a => !exclude.Any(p => Regex.IsMatch(a, p)))
                .ToHashSet(StringComparer.Ordinal);
        }

        private static List<string> ExtractPatterns(string source, string arrayName)
        {
            // Массив может быть объявлен литералом и дополнен через push — собираем
            // все литералы регулярных выражений рядом с именем массива.
            var declaration = Regex.Match(source,
                $@"{Regex.Escape(arrayName)}\s*=\s*\[(?<body>.*?)\]\s*;",
                RegexOptions.Singleline);

            var patterns = new List<string>();

            void Collect(string body)
            {
                foreach (Match m in Regex.Matches(body, @"/((?:\\.|[^/\\\n])*)/[gimsuy]*"))
                    patterns.Add(m.Groups[1].Value);
            }

            if (declaration.Success)
                Collect(declaration.Groups["body"].Value);

            // push(...) в тот же массив — отдельная операция.
            foreach (Match push in Regex.Matches(source,
                         $@"{Regex.Escape(arrayName)}\.push\s*\(\s*(?<body>/(?:\\.|[^/\\\n])*/[gimsuy]*)\s*\)"))
                Collect(push.Groups["body"].Value);

            if (patterns.Count == 0)
                throw new InvalidOperationException($"В service-worker.published.js не найдены регулярные выражения {arrayName}");

            return patterns;
        }

        #endregion
    }
}
