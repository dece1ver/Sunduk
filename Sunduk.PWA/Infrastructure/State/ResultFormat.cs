using System;
using System.Globalization;
using System.Linq;

namespace Sunduk.PWA.Infrastructure.State
{
    /// <summary>
    /// Как показывать дробные числа в результатах: сколько знаков после точки и нужны ли хвостовые нули.
    /// Записывается шаблоном: «0.000» — три знака, нули остаются (0.2 → 0.200);
    /// «0.###» — до трёх знаков, лишние нули убираются (0.2 → 0.2).
    /// </summary>
    public readonly record struct ResultFormat(int Decimals, bool TrailingZeros)
    {
        public const int MaxDecimals = 6;

        /// <summary> Общий формат по умолчанию: до четырёх знаков без хвостовых нулей. </summary>
        public static ResultFormat DefaultGeneral => new(4, false);

        /// <summary> Формат SunduCAM по умолчанию: до трёх знаков без хвостовых нулей, как всегда было в G-коде. </summary>
        public static ResultFormat DefaultCam => new(3, false);

        /// <summary> Шаблон в виде «0.000», «0.###» или «0». </summary>
        public string Pattern => Decimals == 0
            ? "0"
            : "0." + new string(TrailingZeros ? '0' : '#', Decimals);

        public static bool TryParse(string? pattern, out ResultFormat format)
        {
            format = DefaultGeneral;
            if (string.IsNullOrWhiteSpace(pattern)) return false;

            pattern = pattern.Trim();
            if (pattern == "0")
            {
                format = new ResultFormat(0, false);
                return true;
            }
            if (!pattern.StartsWith("0.", StringComparison.Ordinal)) return false;

            var tail = pattern[2..];
            if (tail.Length is 0 or > MaxDecimals) return false;

            bool zeros = tail[0] == '0';
            if (tail.Any(c => c != (zeros ? '0' : '#'))) return false;

            format = new ResultFormat(tail.Length, zeros);
            return true;
        }

        /// <summary> Форматирует число с запятой-точкой «как в программе» (разделитель — всегда точка). </summary>
        /// <param name="decimals"> Знаков после точки; <c>null</c> — взять из формата. </param>
        public string Format(double value, int? decimals = null)
        {
            int d = Math.Clamp(decimals ?? Decimals, 0, 15);
            string result = value.ToString($"F{d}", CultureInfo.InvariantCulture);

            // «-0.000» после округления — это просто ноль
            if (result[0] == '-' && result.All(c => c is '-' or '0' or '.')) result = result[1..];

            if (!TrailingZeros && result.Contains('.')) result = result.TrimEnd('0').TrimEnd('.');
            return result;
        }
    }

    /// <summary>
    /// Формат, которым сейчас пользуются <c>ToPrettyString</c> и <c>NC</c>. Два формата: общий и для SunduCAM.
    /// Какой из них действует, определяется тем, на какой странице пользователь: см. <see cref="IsCam"/>.
    /// </summary>
    public static class ResultFormats
    {
        public static ResultFormat General { get; set; } = ResultFormat.DefaultGeneral;
        public static ResultFormat Cam { get; set; } = ResultFormat.DefaultCam;

        /// <summary> Открыт ли сейчас SunduCAM. Выставляет главный макет при смене адреса. </summary>
        public static bool IsCam { get; set; }

        public static ResultFormat Active => IsCam ? Cam : General;
    }
}
