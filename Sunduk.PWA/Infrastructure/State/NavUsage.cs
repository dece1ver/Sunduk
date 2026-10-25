using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using MudBlazor;

namespace Sunduk.PWA.Infrastructure.State
{
    /// <summary> Раздел, который может оказаться в нижней панели навигации. </summary>
    public record NavSection(string Href, string Title, string Icon);

    /// <summary>
    /// Счётчик заходов в разделы. По нему нижняя панель на телефоне подбирает самые частые
    /// разделы пользователя. Данные хранятся только в LocalStorage этого устройства.
    /// </summary>
    public class NavUsage
    {
        private const string StorageKey = "nav-usage";

        /// <summary> Сколько раз нужно зайти в раздел, чтобы он начал влиять на порядок. </summary>
        private const int MinVisitsToChange = 3;

        /// <summary> Сколько разделов показывается в панели (ещё одна кнопка «Ещё» есть всегда). </summary>
        public const int Slots = 4;

        /// <summary> Все разделы. Первые <see cref="Slots"/> — набор по умолчанию, в нужном порядке. </summary>
        public static readonly IReadOnlyList<NavSection> Sections = new NavSection[]
        {
            new("/calculator", "Режимы", Icons.Material.Filled.Calculate),
            new("/stuff", "Хрень", Icons.Material.Filled.RoundedCorner),
            new("/tolerance", "Допуски", Icons.Material.Filled.Straighten),
            new("/knowledge", "База", Icons.Material.Filled.MenuBook),
            new("/converters", "Конверт.", Icons.Material.Filled.Transform),
            new("/timecalc", "Время", Icons.Material.Filled.Timer),
            new("/dynamic-templates", "Шаблоны", Icons.Material.Outlined.Code),
            new("/sunducam", "CAM", Icons.Material.Outlined.FeaturedPlayList),
        };

        private readonly ILocalStorageService _storage;
        private Dictionary<string, int> _visits = new();
        private string? _lastSection;

        public NavUsage(ILocalStorageService storage) => _storage = storage;

        public async Task LoadAsync()
        {
            try
            {
                _visits = await _storage.GetItemAsync<Dictionary<string, int>>(StorageKey) ?? new();
            }
            catch
            {
                _visits = new();
            }
        }

        /// <summary> Находит раздел по относительному пути (например, «knowledge/formulas»). </summary>
        public static NavSection? Find(string relativePath)
        {
            var path = "/" + relativePath.Split('?', '#')[0].TrimStart('/');
            return Sections.FirstOrDefault(s => path.Equals(s.Href, StringComparison.OrdinalIgnoreCase)
                                             || path.StartsWith(s.Href + "/", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary> Засчитывает заход. Переходы внутри одного раздела (вкладки, статьи) считаются одним заходом. </summary>
        public async Task RecordAsync(string relativePath)
        {
            var section = Find(relativePath);
            if (section is null || section.Href == _lastSection) return;
            _lastSection = section.Href;

            _visits[section.Href] = _visits.GetValueOrDefault(section.Href) + 1;
            try
            {
                await _storage.SetItemAsync(StorageKey, _visits);
            }
            catch
            {
                // Счётчик не критичен: без хранилища панель просто останется по умолчанию
            }
        }

        /// <summary>
        /// Разделы для панели: по убыванию заходов, при равенстве — в порядке по умолчанию.
        /// Раздел учитывается, только если зашли в него не меньше <see cref="MinVisitsToChange"/> раз,
        /// чтобы случайные переходы не меняли панель: до этого остаётся порядок по умолчанию.
        /// </summary>
        public IReadOnlyList<NavSection> Top()
        {
            int Score(NavSection s)
            {
                var visits = _visits.GetValueOrDefault(s.Href);
                return visits >= MinVisitsToChange ? visits : 0;
            }

            return Sections
                .Select((s, i) => (Section: s, Index: i, Score: Score(s)))
                .OrderByDescending(x => x.Score)
                .ThenBy(x => x.Index)
                .Take(Slots)
                .Select(x => x.Section)
                .ToList();
        }
    }
}
