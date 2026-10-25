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

        private readonly ILocalStorageService _storage;

        public AppSettings(ILocalStorageService storage) => _storage = storage;

        public bool IsDark { get; private set; } = true;
        public NavOrderMode NavOrder { get; private set; } = NavOrderMode.Default;

        /// <summary> Показывать ли калькулятор выражений на странице режимов. </summary>
        public bool CalculatorEnabled { get; private set; } = true;

        public event Action? Changed;

        public async Task LoadAsync()
        {
            try
            {
                IsDark = await _storage.GetItemAsync<bool?>(ThemeKey) ?? true;
                NavOrder = await _storage.GetItemAsync<NavOrderMode?>(NavOrderKey) ?? NavOrderMode.Default;
                CalculatorEnabled = await _storage.GetItemAsync<bool?>(CalculatorKey) ?? true;
            }
            catch
            {
                // Повреждённое значение в хранилище не должно мешать запуску: остаются значения по умолчанию
            }
            Changed?.Invoke();
        }

        public Task SetDarkAsync(bool value) => SetAsync(v => IsDark = v, value, ThemeKey, IsDark);
        public Task SetNavOrderAsync(NavOrderMode value) => SetAsync(v => NavOrder = v, value, NavOrderKey, NavOrder);
        public Task SetCalculatorEnabledAsync(bool value) => SetAsync(v => CalculatorEnabled = v, value, CalculatorKey, CalculatorEnabled);

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
