// In development, always fetch from the network and do not enable offline support.
// This is because caching would make development more difficult (changes would not
// be reflected on the first load after each change).
//
// Важно: если на этом origin (например, https://localhost:5001) раньше запускалась
// опубликованная (кэширующая) версия приложения, её service worker и офлайн-кэш
// могли «застрять»: вкладка тогда бесконечно крутится на загрузке, ничего не
// отображается, и помогает только несколько перезапусков. Здесь мы сразу:
//   1) вызываем skipWaiting — dev-воркер активируется, не дожидаясь освобождения
//      страниц старым воркером;
//   2) удаляем все офлайн-кэши (offline-cache-*) от опубликованных сборок;
//   3) вызываем clients.claim() — забираем управление origin себе.
// Так development всегда грузится из сети и не наследует устаревшие ассеты.
self.addEventListener('install', () => self.skipWaiting());
self.addEventListener('activate', event => event.waitUntil((async () => {
    const keys = await caches.keys();
    await Promise.all(keys.filter(k => k.startsWith('offline-cache-')).map(k => caches.delete(k)));
    await self.clients.claim();
})()));
