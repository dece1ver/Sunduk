using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazored.LocalStorage;

namespace Sunduk.PWA.Infrastructure.State
{
    /// <summary> Как упорядочены разделы в нижней панели навигации. </summary>
    public enum NavOrderMode
    {
        /// <summary> Фиксированный набор и порядок. </summary>
        Default,

        /// <summary> По частоте заходов пользователя (см. <see cref="NavUsage"/>). </summary>
        ByUsage,
    }

    /// <summary>
    /// Параметры приложения, которые пользователь меняет в диалоге «Параметры». Каждый хранится
    /// в LocalStorage отдельным ключом. Ключ темы остался прежним, чтобы выбор не сбросился.
    /// </summary>
    public class AppSettings
    {
        private const string ThemeKey = "theme-dark";
        private const string NavOrderKey = "app-nav-order";
        private const string CalculatorKey = "app-calculator-enabled";
        private const string ResultFormatKey = "app-result-format";
        private const string CamFormatKey = "app-cam-format";

        private readonly ILocalStorageService _storage;

        public AppSettings(ILocalStorageService storage) => _storage = storage;

        public bool IsDark { get; private set; } = true;
        public NavOrderMode NavOrder { get; private set; } = NavOrderMode.Default;

        /// <summary> Показывать ли калькулятор выражений на странице режимов. </summary>
        public bool CalculatorEnabled { get; private set; } = true;

        /// <summary> Формат дробных чисел в результатах. Хранится шаблоном вида «0.###». </summary>
        public ResultFormat ResultFormat { get; private set; } = ResultFormat.DefaultGeneral;

        /// <summary> То же для SunduCAM: в G-коде обычно нужен другой набор знаков и нулей. </summary>
        public ResultFormat CamFormat { get; private set; } = ResultFormat.DefaultCam;

        /// <summary> Растёт при каждой смене формата: по нему страницы перерисовываются с новыми числами. </summary>
        public int FormatVersion { get; private set; }

        public event Action? Changed;

        public async Task LoadAsync()
        {
            try
            {
                IsDark = await _storage.GetItemAsync<bool?>(ThemeKey) ?? true;
                NavOrder = await _storage.GetItemAsync<NavOrderMode?>(NavOrderKey) ?? NavOrderMode.Default;
                CalculatorEnabled = await _storage.GetItemAsync<bool?>(CalculatorKey) ?? true;
                ResultFormat = ParseFormat(await _storage.GetItemAsync<string>(ResultFormatKey), ResultFormat.DefaultGeneral);
                CamFormat = ParseFormat(await _storage.GetItemAsync<string>(CamFormatKey), ResultFormat.DefaultCam);
            }
            catch
            {
                // Повреждённое значение в хранилище не должно мешать запуску: остаются значения по умолчанию
            }
            ApplyFormats();
            Changed?.Invoke();
        }

        private static ResultFormat ParseFormat(string? pattern, ResultFormat fallback) =>
            ResultFormat.TryParse(pattern, out var format) ? format : fallback;

        /// <summary> Передаёт форматы статическим методам <c>ToPrettyString</c> и <c>NC</c>. </summary>
        private void ApplyFormats()
        {
            if (ResultFormats.General != ResultFormat || ResultFormats.Cam != CamFormat) FormatVersion++;
            ResultFormats.General = ResultFormat;
            ResultFormats.Cam = CamFormat;
        }

        public Task SetDarkAsync(bool value) => SetAsync(v => IsDark = v, value, ThemeKey, IsDark);
        public Task SetNavOrderAsync(NavOrderMode value) => SetAsync(v => NavOrder = v, value, NavOrderKey, NavOrder);
        public Task SetCalculatorEnabledAsync(bool value) => SetAsync(v => CalculatorEnabled = v, value, CalculatorKey, CalculatorEnabled);

        public Task SetResultFormatAsync(ResultFormat value) => SetFormatAsync(v => ResultFormat = v, value, ResultFormat, ResultFormatKey);
        public Task SetCamFormatAsync(ResultFormat value) => SetFormatAsync(v => CamFormat = v, value, CamFormat, CamFormatKey);

        private async Task SetFormatAsync(Action<ResultFormat> assign, ResultFormat value, ResultFormat current, string key)
        {
            if (current == value) return;
            assign(value);
            ApplyFormats();
            Changed?.Invoke();
            await _storage.SetItemAsync(key, value.Pattern);
        }

        private async Task SetAsync<T>(Action<T> assign, T value, string key, T current)
        {
            if (EqualityComparer<T>.Default.Equals(current, value)) return;
            assign(value);
            Changed?.Invoke();
            await _storage.SetItemAsync(key, value);
        }

        /// <summary>
        /// Стирает всё, что приложение хранит на устройстве: параметры, станки и инструменты,
        /// настройки и введённые значения калькуляторов. После этого нужно перезагрузить страницу,
        /// чтобы сбросилось и состояние в памяти.
        /// </summary>
        public Task ResetAllAsync() => _storage.ClearAsync().AsTask();
    }
}
