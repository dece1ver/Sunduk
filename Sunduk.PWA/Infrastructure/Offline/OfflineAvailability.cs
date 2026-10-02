using System;

namespace Sunduk.PWA.Infrastructure.Offline
{
    /// <summary>
    /// Состояние офлайн-режима приложения. Показывается значком в MudAppBar
    /// и подробной строкой статуса в диалоге «Описание».
    /// </summary>
    public enum OfflineAvailability
    {
        /// <summary>Состояние ещё не запрошено у браузера.</summary>
        Unknown,

        /// <summary>Опрос браузера не удался — скорее всего устаревшая копия utils.js в кэше.</summary>
        ProbeFailed,

        /// <summary>Браузер не умеет service worker / Cache Storage — офлайн невозможен.</summary>
        Unsupported,

        /// <summary>Офлайн-версия ещё скачивается, кэш не готов. Не отключать интернет.</summary>
        Preparing,

        /// <summary>Service worker зарегистрирован, но ещё ни разу не активировался.</summary>
        NotActivated,

        /// <summary>Новая версия установлена, но страница работает на старой — пора перезагрузить.</summary>
        UpdateReady,

        /// <summary>Сеть есть, офлайн-кэш собран, можно отключать интернет.</summary>
        Ready,

        /// <summary>Сети нет, приложение работает из офлайн-кэша.</summary>
        Offline,

        /// <summary>Сети нет, а офлайн-кэш не собран — приложение работает не полностью.</summary>
        OfflineNoCache
    }

    /// <summary>
    /// Состояние офлайна, пришедшее из JS. Сериализуется из window.sundukOffline.getState().
    /// </summary>
    public sealed class OfflineState
    {
        public bool Supported { get; init; }
        public bool Online { get; init; }

        /// <summary>none | installing | waiting | active | unsupported</summary>
        public string SwState { get; init; } = "none";

        public bool CacheReady { get; init; }
        public string? Version { get; init; }
        public int Cached { get; init; }
        public int Total { get; init; }
        public int Failed { get; init; }

        /// <summary>Версия service worker, с которой эта страница реально загрузилась.</summary>
        public string? LoadedVersion { get; init; }

        /// <summary>Состояние ещё не получено из браузера.</summary>
        public bool IsKnown => Supported || SwState != "none";

        public static readonly OfflineState Unknown = new();

        /// <summary>Опрос не удался. Причина — в Diagnostic, показывается пользователю.</summary>
        public static OfflineState Error(string diagnostic) => new() { ProbeFailed = true, Diagnostic = diagnostic };

        /// <summary>Внутренний флаг «опрос упал». Не приходит из JS.</summary>
        internal bool ProbeFailed { get; init; }

        /// <summary>Техническая причина сбоя опроса — для показа пользователю.</summary>
        public string? Diagnostic { get; init; }

        /// <summary>Готовность офлайн-кэша в процентах.</summary>
        public int Percent => Total > 0 ? (int)Math.Clamp(Cached * 100L / Total, 0, 100) : 0;
    }
}
