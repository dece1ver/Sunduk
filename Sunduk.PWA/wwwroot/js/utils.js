window.triggerAutoGrow = () => {
    document.querySelectorAll('textarea').forEach(t => t.dispatchEvent(new Event('input')));
};

// Состояние офлайн-режима для индикатора в MudAppBar.
// Service worker кладёт маркер готовности в Cache Storage после полной загрузки
// офлайн-версии и шлёт postMessage с прогрессом и с фактом активации.
window.sundukOffline = (() => {
    const readyMarkerUrl = '/__sunduk_offline_ready__';
    const cachePrefix = 'offline-cache-';
    // Версия, с которой страница реально загрузилась. Нужна, чтобы отличить
    // «обновление пришло» от обычного первого открытия.
    const loadedVersionKey = 'sunduk.swVersion';

    let dotNetRef = null;
    let handlersBound = false;
    let progress = null;

    async function readMarker() {
        if (!('caches' in window)) return null;
        const response = await caches.match(readyMarkerUrl);
        if (!response) return null;
        try {
            return await response.json();
        } catch {
            return null;
        }
    }

    async function countCached() {
        const keys = await caches.keys();
        const current = keys.filter(k => k.startsWith(cachePrefix)).sort().pop();
        if (!current) return 0;
        const cache = await caches.open(current);
        const requests = await cache.keys();
        return requests.filter(r => !r.url.endsWith(readyMarkerUrl)).length;
    }

    async function getState() {
        if (!('serviceWorker' in navigator) || !('caches' in window)) {
            return {
                supported: false, online: navigator.onLine, swState: 'unsupported',
                cacheReady: false, version: null, cached: 0, total: 0, failed: 0
            };
        }

        const registration = await navigator.serviceWorker.getRegistration();
        let swState = 'none';
        if (registration) {
            if (registration.installing) swState = 'installing';
            else if (registration.waiting) swState = 'waiting';
            else if (registration.active) swState = 'active';
        }

        const marker = await readMarker();

        // Пока идёт установка, прогресс приходит от SW — не пересчитываем ключи
        // кэша на каждом батче, это слишком дорого.
        let cached = 0;
        let total = 0;
        let failed = 0;
        if (progress) {
            cached = progress.cached;
            total = progress.total;
            failed = progress.failed;
        } else {
            cached = await countCached();
        }

        return {
            supported: true,
            online: navigator.onLine,
            swState,
            cacheReady: !!marker,
            version: marker ? marker.version : null,
            cached,
            total,
            failed
        };
    }

    function loadedVersion() {
        try {
            return sessionStorage.getItem(loadedVersionKey);
        } catch {
            return null;
        }
    }

    function rememberVersion(version) {
        try {
            if (version) sessionStorage.setItem(loadedVersionKey, version);
        } catch {
            // sessionStorage может быть недоступен в приватном режиме — не критично.
        }
    }

    function notify() {
        if (dotNetRef) dotNetRef.invokeMethodAsync('OnOfflineStateChanged').catch(() => { });
    }

    function bindHandlers() {
        if (handlersBound) return;
        handlersBound = true;

        window.addEventListener('online', notify);
        window.addEventListener('offline', notify);

        navigator.serviceWorker.addEventListener('message', event => {
            const data = event.data;
            if (!data || typeof data !== 'object') return;

            if (data.type === 'offline-progress') {
                progress = { cached: data.cached, total: data.total, failed: data.failed };
                notify();
                return;
            }

            if (data.type === 'offline-activated') {
                // Установка завершена — дальше состояние считаем по маркеру.
                progress = null;
                // Версию НЕ запоминаем здесь: если она новее той, с которой страница
                // загрузилась, обновление уже установлено и нужно перезагрузиться.
                // Если бы мы её записали, подсказка бы пропала.
                notify();
            }
        });

        navigator.serviceWorker.addEventListener('updatefound', () => {
            navigator.serviceWorker.getRegistration().then(() => notify());
        });
    }

    return {
        getState: getState,

        loadedVersion: loadedVersion,

        // Вызывается из .NET. Прокидывает наружу текущее состояние и начинает
        // следить за изменениями (сеть, активация SW, прогресс загрузки кэша).
        watch: async dotnetRef => {
            dotNetRef = dotnetRef;
            bindHandlers();

            const state = await getState();
            if (state.cacheReady) rememberVersion(state.version);
            return state;
        },

        stop: () => {
            dotNetRef = null;
        }
    };
})();
