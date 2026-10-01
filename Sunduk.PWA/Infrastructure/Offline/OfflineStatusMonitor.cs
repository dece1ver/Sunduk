using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Sunduk.PWA.Infrastructure.Offline
{
    /// <summary>
    /// Следит за состоянием service worker и офлайн-кэша и отдаёт его UI.
    /// Состояние приходит из window.sundukOffline (wwwroot/js/utils.js), который
    /// подписан на online/offline, активацию SW и прогресс загрузки кэша.
    /// </summary>
    public sealed class OfflineStatusMonitor : IAsyncDisposable
    {
        private const string Module = "sundukOffline";

        private readonly IJSRuntime _js;
        private DotNetObjectReference<OfflineStatusMonitor>? _selfRef;
        private bool _started;

        public OfflineState State { get; private set; } = OfflineState.Unknown;

        /// <summary>Готово ли офлайн-приложение, насколько это вообще возможно.</summary>
        public bool OfflineReady => Availability is OfflineAvailability.Ready or OfflineAvailability.Offline;

        public event Action? StateChanged;

        public OfflineStatusMonitor(IJSRuntime js)
        {
            _js = js;
        }

        public OfflineAvailability Availability => Map(State);

        /// <summary>Вызывается один раз из MainLayout после первого рендера.</summary>
        public async Task StartAsync()
        {
            if (_started) return;
            _started = true;

            _selfRef = DotNetObjectReference.Create(this);
            try
            {
                State = await _js.InvokeAsync<OfflineState>($"{Module}.watch", _selfRef)
                    ?? OfflineState.Unknown;
            }
            catch (JSException)
            {
                // Например, prerender или нестандартная сборка — тихо остаёмся в Unsupported.
                State = OfflineState.Unknown;
            }

            StateChanged?.Invoke();
        }

        [JSInvokable]
        public async Task OnOfflineStateChanged()
        {
            try
            {
                var state = await _js.InvokeAsync<OfflineState>($"{Module}.getState");
                var loadedVersion = await _js.InvokeAsync<string?>($"{Module}.loadedVersion");

                State = state is null
                    ? OfflineState.Unknown
                    : new OfflineState
                    {
                        Supported = state.Supported,
                        Online = state.Online,
                        SwState = state.SwState,
                        CacheReady = state.CacheReady,
                        Version = state.Version,
                        Cached = state.Cached,
                        Total = state.Total,
                        Failed = state.Failed,
                        LoadedVersion = loadedVersion
                    };
            }
            catch (JSException)
            {
                return;
            }
            catch (JSDisconnectedException)
            {
                return;
            }

            StateChanged?.Invoke();
        }

        /// <summary>Переводит состояние браузера в статус для индикатора. Чистая функция.</summary>
        public static OfflineAvailability Map(OfflineState s)
        {
            // Пока браузер не ответил, не показываем «не поддерживается» — это было бы
            // враньём на первом кадре, до завершения JS-вызова.
            if (!s.IsKnown) return OfflineAvailability.Unknown;

            if (!s.Supported) return OfflineAvailability.Unsupported;

            var updateInstalled = !string.IsNullOrEmpty(s.Version)
                && !string.IsNullOrEmpty(s.LoadedVersion)
                && s.Version != s.LoadedVersion;

            // Нет сети — обновление всё равно не установить, важнее сказать, что
            // приложение продолжает работать (или не работает) из кэша.
            if (!s.Online)
                return s.CacheReady ? OfflineAvailability.Offline : OfflineAvailability.OfflineNoCache;

            // Service worker ещё не зарегистрирован: офлайн не заработает, пока
            // страница не откроется с сетью.
            if (s.SwState == "none") return OfflineAvailability.NotActivated;

            // SW переключился на новую версию, а мы продолжаем работать на старой.
            if (updateInstalled) return OfflineAvailability.UpdateReady;

            // Новая версия скачана и ждёт активации. В норме не наблюдается —
            // service worker вызывает skipWaiting, — но если вдруг ждёт, смысл тот же.
            if (s.SwState == "waiting") return OfflineAvailability.UpdateReady;

            // Кэш ещё не собран: идёт первичная загрузка офлайн-версии.
            if (!s.CacheReady) return OfflineAvailability.Preparing;

            if (s.SwState == "active") return OfflineAvailability.Ready;

            return OfflineAvailability.Preparing;
        }

        public async ValueTask DisposeAsync()
        {
            if (_selfRef is not null)
            {
                try
                {
                    await _js.InvokeVoidAsync($"{Module}.stop");
                }
                catch (JSDisconnectedException)
                {
                    // Страница уже закрыта — ничего делать не нужно.
                }
                catch (JSException)
                {
                }

                _selfRef.Dispose();
                _selfRef = null;
            }
        }
    }
}
