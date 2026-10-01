// Caution! Be sure you understand the caveats before publishing an application with
// offline support. See https://aka.ms/blazor-offline-considerations

self.importScripts('./service-worker-assets.js');
self.addEventListener('install', event => event.waitUntil(onInstall(event)));
self.addEventListener('activate', event => event.waitUntil(onActivate(event)));
self.addEventListener('fetch', event => event.respondWith(onFetch(event)));

const cacheNamePrefix = 'offline-cache-';
const cacheName = `${cacheNamePrefix}${self.assetsManifest.version}`;

// Маркер, который onActivate кладёт в кэш, когда офлайн-версия полностью загружена.
// По нему страница определяет, можно ли уже отключать интернет.
const readyMarkerUrl = '/__sunduk_offline_ready__';

const offlineAssetsInclude = [ /\.dll$/, /\.pdb$/, /\.wasm/, /\.html/, /\.js$/, /\.json$/, /\.css$/, /\.woff$/, /\.woff2$/, /\.svg$/, /\.png$/, /\.jpe?g$/, /\.gif$/, /\.ico$/, /\.blat$/, /\.dat$/ ];

// Обязательно включаем манифест приложения: без него установленный PWA без сети
// не сможет прочитать start_url/иконки.
offlineAssetsInclude.push(/\.webmanifest$/);

// Мусор, который в офлайн-кэше только жрёт место и время загрузки:
// исходники less/scss, метаданные, sourcemaps, неиспользуемые форматы шрифтов
// (приложение грузит self-hosted fonts/*.woff2) и не-minified дубликаты
// FontAwesome-стилей и скриптов.
const offlineAssetsExclude = [
    /^service-worker\.js$/,
    /\.(less|scss|yml|map)$/,
    /^metadata\//,
    /^webfonts\/.*\.(eot|ttf|woff|svg)$/,
    /^css\/(all|brands|fontawesome|regular|solid|svg-with-js|v4-shims)\.css$/,
    /^js\/(all|brands|conflict-detection|fontawesome|regular|solid|v4-shims)\.js$/,
];

// Сколько файлов качать одновременно. Последовательная загрузка 2000+ файлов
// занимает минуты на мобильном интернете, и install frequently не успевает
// завершиться — тогда офлайн-кэша не появляется вообще.
const installBatchSize = 16;

async function onInstall(event) {
    console.info('Service worker: Install');

    // Применяем новую версию сразу, а не после нескольких перезагрузок.
    self.skipWaiting();

    // Fetch and cache all matching items from the assets manifest
    const assetsRequests = self.assetsManifest.assets
        .filter(asset => offlineAssetsInclude.some(pattern => pattern.test(asset.url)))
        .filter(asset => !offlineAssetsExclude.some(pattern => pattern.test(asset.url)))
        .map(asset => new Request(asset.url, { integrity: asset.hash }));

    const total = assetsRequests.length;
    const cache = await caches.open(cacheName);
    const failed = [];
    let done = 0;

    for (let i = 0; i < assetsRequests.length; i += installBatchSize) {
        const batch = assetsRequests.slice(i, i + installBatchSize);
        const results = await Promise.allSettled(batch.map(req => cache.add(req)));
        for (let j = 0; j < results.length; j++) {
            done++;
            if (results[j].status === 'rejected') failed.push(batch[j].url);
        }
        await notifyClients({ type: 'offline-progress', cached: done - failed.length, total, failed: failed.length });
    }

    if (failed.length) {
        // Раньше эти ошибки молча терялись в console.warn, и пропущенные ассеты
        // было невозможно заметить — именно так и не заметили сломанные ?v= ссылки.
        console.error(`Service worker: не удалось закэшировать ${failed.length} из ${total} ассетов:`, failed);
        await notifyClients({ type: 'offline-progress', cached: total - failed.length, total, failed: failed.length, failedUrls: failed });
    } else {
        console.info(`Service worker: офлайн-кэш собран, ${total} файлов`);
    }
}

async function onActivate(event) {
    console.info('Service worker: Activate');

    // Delete unused caches
    const cacheKeys = await caches.keys();
    await Promise.all(cacheKeys
        .filter(key => key.startsWith(cacheNamePrefix) && key !== cacheName)
        .map(key => caches.delete(key)));

    // Сразу забираем управление, чтобы свежая версия работала с первой загрузки.
    await self.clients.claim();

    // Маркер готовности: пока его нет, офлайн-версия считается недокачанной.
    const cache = await caches.open(cacheName);
    const marker = new Response(
        JSON.stringify({ version: self.assetsManifest.version, installedAt: Date.now() }),
        { headers: { 'Content-Type': 'application/json' } });
    await cache.put(new Request(readyMarkerUrl), marker);

    // Сообщаем открытым страницам версию: если она отличается от той, с которой
    // страница загрузилась, значит обновление установлено и пора перезагрузиться.
    await notifyClients({ type: 'offline-activated', version: self.assetsManifest.version });
}

async function onFetch(event) {
    if (event.request.method !== 'GET') return fetch(event.request);

    // For all navigation requests, try to serve index.html from cache
    // If you need some URLs to be server-rendered, edit the following check to exclude those URLs
    const isNavigation = event.request.mode === 'navigate';
    const request = isNavigation ? 'index.html' : event.request;

    const cache = await caches.open(cacheName);

    // ignoreSearch обязателен: index.html может запрашивать ассеты с ?v=N, а в
    // манифесте лежат URL без query. Без ignoreSearch Cache Storage считает их
    // разными ключами, и ассет молча не находится в офлайне.
    let cachedResponse = await cache.match(request, { ignoreSearch: true });

    if (!cachedResponse) {
        try {
            return await fetch(event.request);
        } catch (e) {
            if (isNavigation) {
                // Последний шанс отдать приложение, а не страницу ошибки браузера.
                cachedResponse = await cache.match('index.html', { ignoreSearch: true });
            }
            if (cachedResponse) return cachedResponse;

            console.warn('Service worker: ассет недоступен офлайн:', event.request.url, e);
            return new Response('', { status: 504, statusText: 'Offline' });
        }
    }

    return cachedResponse;
}

async function notifyClients(message) {
    const clients = await self.clients.matchAll({ includeUncontrolled: true, type: 'window' });
    for (const client of clients) client.postMessage(message);
}
