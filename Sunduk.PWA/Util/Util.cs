using Sunduk.PWA.Infrastructure.Tools;
using System;
using System.Globalization;

namespace Sunduk.PWA.Util
{
    public static class Util
    {
        
        public enum GetNumberOption { Any, OnlyPositive }
        public enum PrettyStringOption { AsIs, ZeroToEmpty }
        public enum ToolDescriptionOption { General, L230, GoodwayLeft, GoodwayRight }

        /// <summary>
        /// Получает число из строки
        /// </summary>
        /// <param name="stringNumber">Строка для получения</param>
        /// <param name="defaultValue">Значение по умолчанию</param>
        /// <param name="numberOption">Возвращаемое значение: только положительное или любое</param>
        /// <returns>Значение Double, при неудаче возвращает значение по умолчанию</returns>
        public static double GetDouble(this string stringNumber, double defaultValue = 0, GetNumberOption numberOption = GetNumberOption.OnlyPositive)
        {
            NumberFormatInfo numberFomat = new() { NumberDecimalSeparator = "," };
            if (Double.TryParse(stringNumber, NumberStyles.Any, numberFomat, out double result))
            {
                if (numberOption == GetNumberOption.OnlyPositive && result >= 0)
                {
                    return result;
                }
                else if (numberOption == GetNumberOption.Any)
                {
                    return result;
                }
                else
                {
                    return defaultValue;
                }
                
            }
            return defaultValue;
        }

        /// <summary>
        /// Получает число из строки
        /// </summary>
        /// <param name="stringNumber">Строка для получения</param>
        /// <param name="defaultValue">Значение по умолчанию</param>
        /// <param name="numberOption">Возвращаемое значение: только положительное или любое</param>
        /// <returns>Значение Int32, при неудаче возвращает значение по умолчанию</returns>
        public static int GetInt(this string stringNumber, int defaultValue = 0, GetNumberOption numberOption = GetNumberOption.OnlyPositive)
        {
            NumberFormatInfo numberFomat = new() { NumberDecimalSeparator = "," };
            if (Int32.TryParse(stringNumber, NumberStyles.Any, numberFomat, out int result))
            {
                if (numberOption == GetNumberOption.OnlyPositive && result > 0)
                {
                    return result;
                }
                else
                {
                    return defaultValue;
                }
            }
            return defaultValue;
        }

        /// <summary>
        /// Переводит радианы в угол</summary>
        /// <param name="radians">Значение в радианах</param>
        /// <returns>Угловое значение</returns>
        public static double Degrees(this double radians)
        {
            return radians * 180 / Math.PI;
        }

        /// <summary>
        /// Округляет
        /// </summary>
        /// <param name="rounder">Значение округления</param>
        /// <returns>Радиан</returns>
        public static int Round(this int value, int rounder = 10)
        {
            if (value < rounder) return value;
            return value / rounder * rounder;
        }

        /// <summary>
        /// Округляет
        /// </summary>
        /// <param name="rounder">Значение округления</param>
        /// <returns>Радиан</returns>
        public static int Round(this double value, int rounder = 10)
        {
            if (value < rounder) return (int)value;
            return (int)Math.Round(value / rounder) * rounder;
        }

        /// <summary>
        /// Переводит угол в радианы
        /// </summary>
        /// <param name="degrees">Угловое значение</param>
        /// <returns>Радиан</returns>
        public static double Radians(this double degrees)
        {
            return degrees * Math.PI / 180;
        }

        /// <summary>
        /// Форматирует число в строку подходящую для СЧПУ
        /// </summary>
        /// <param name="value">Число</param>
        /// <param name="precision">Точность</param>
        /// <returns>Отформатированную строку</returns>
        public static string NC(this double value, int precision = 3)
        {
            return value.ToString($"F{precision}").Replace(",", ".").Contains('.')
                ? value.ToString($"F{precision}").Replace(",", ".").TrimEnd('0')
                : value.ToString($"F{precision}");
        }

        /// <summary>
        /// Форматирует число в номер инструмента
        /// </summary>
        /// <param name="value">Число</param>
        /// <returns>Отформатированную строку</returns>
        public static string ToolNumber(this int value)
        {
            return value.ToString($"D{4}");
        }

        /// <summary>
        /// Миллиметры в микроны
        /// </summary>
        /// <param name="value">Число в миллиметрах</param>
        /// <returns>Отформатированную строку</returns>
        public static int Microns(this double value)
        {
            return (value * 1000).Round(10);
        }

        /// <summary>
        /// Миллиметры в микроны
        /// </summary>
        /// <param name="value">Число в миллиметрах</param>
        /// <returns>Отформатированную строку</returns>
        public static int Microns(this int value)
        {
            return (value * 1000).Round(10);
        }

        public static string Description(this Tool tool, ToolDescriptionOption option = ToolDescriptionOption.General)
        {
            return tool switch
            {
                SpecialTool specialTool => option switch
                {
                    ToolDescriptionOption.General => $"T{specialTool.Position.ToolNumber()} ({specialTool.Name})".Replace(',', '.'),
                    ToolDescriptionOption.L230 => $"T{specialTool.Position.ToolNumber()}({specialTool.Name})".Replace(',', '.'),
                    ToolDescriptionOption.GoodwayLeft => $"T{specialTool.Position.ToolNumber()}G54M58({specialTool.Name})".Replace(',', '.'),
                    ToolDescriptionOption.GoodwayRight => $"T{specialTool.Position.ToolNumber()}G55M58({specialTool.Name})".Replace(',', '.'),
                    _ => string.Empty,
                },
                TurningExternalTool turningTool => option switch
                {
                    ToolDescriptionOption.General => $"T{turningTool.Position.ToolNumber()} ({turningTool.Name} {turningTool.Angle} R{turningTool.Radius})".Replace(',', '.'),
                    ToolDescriptionOption.L230 => $"T{turningTool.Position.ToolNumber()}({turningTool.Name} {turningTool.Angle} R{turningTool.Radius})".Replace(',', '.'),
                    ToolDescriptionOption.GoodwayLeft => $"T{turningTool.Position.ToolNumber()}G54M58({turningTool.Name} {turningTool.Angle} R{turningTool.Radius})".Replace(',', '.'),
                    ToolDescriptionOption.GoodwayRight => $"T{turningTool.Position.ToolNumber()}G55M58({turningTool.Name} {turningTool.Angle} R{turningTool.Radius})".Replace(',', '.'),
                    _ => string.Empty,
                },
                DrillingTool drillingTool => option switch
                {
                    ToolDescriptionOption.General => $"T{drillingTool.Position.ToolNumber()} ({drillingTool.Name} D{drillingTool.Diameter})".Replace(',', '.'),
                    ToolDescriptionOption.L230 => $"T{drillingTool.Position.ToolNumber()}({drillingTool.Name} D{drillingTool.Diameter})".Replace(',', '.'),
                    ToolDescriptionOption.GoodwayLeft => $"T{drillingTool.Position.ToolNumber()}G54M58({drillingTool.Name} D{drillingTool.Diameter})".Replace(',', '.'),
                    ToolDescriptionOption.GoodwayRight => $"T{drillingTool.Position.ToolNumber()}G55M58({drillingTool.Name} D{drillingTool.Diameter})".Replace(',', '.'),
                    _ => string.Empty,
                },
                TappingTool tappingTool => option switch
                {
                    ToolDescriptionOption.General => $"T{tappingTool.Position.ToolNumber()} ({tappingTool.Name} M{tappingTool.Diameter}x{tappingTool.Pitch})".Replace(',', '.'),
                    ToolDescriptionOption.L230 => $"T{tappingTool.Position.ToolNumber()}({tappingTool.Name} M{tappingTool.Diameter}x{tappingTool.Pitch})".Replace(',', '.'),
                    ToolDescriptionOption.GoodwayLeft => $"T{tappingTool.Position.ToolNumber()}G54M58({tappingTool.Name} M{tappingTool.Diameter}x{tappingTool.Pitch})".Replace(',', '.'),
                    ToolDescriptionOption.GoodwayRight => $"T{tappingTool.Position.ToolNumber()}G55M58({tappingTool.Name} M{tappingTool.Diameter}x{tappingTool.Pitch})".Replace(',', '.'),
                    _ => string.Empty,
                },
                ThreadingExternalTool threadingTool => option switch
                {
                    ToolDescriptionOption.General => $"T{threadingTool.Position.ToolNumber()} ({threadingTool.Name} {threadingTool.Pitch} {threadingTool.Angle})".Replace(',', '.'),
                    ToolDescriptionOption.L230 => $"T{threadingTool.Position.ToolNumber()}({threadingTool.Name} {threadingTool.Pitch} {threadingTool.Angle})".Replace(',', '.'),
                    ToolDescriptionOption.GoodwayLeft => $"T{threadingTool.Position.ToolNumber()}G54M58({threadingTool.Name} {threadingTool.Pitch} {threadingTool.Angle})".Replace(',', '.'),
                    ToolDescriptionOption.GoodwayRight => $"T{threadingTool.Position.ToolNumber()}G55M58({threadingTool.Name} {threadingTool.Pitch} {threadingTool.Angle})".Replace(',', '.'),
                    _ => string.Empty,
                },
                GroovingExternalTool groovingTool => option switch
                {
                    ToolDescriptionOption.General => $"T{groovingTool.Position.ToolNumber()} ({groovingTool.Name} {groovingTool.Width}MM {(groovingTool.ZeroPoint == GroovingExternalTool.Point.Left ? "KAK PROHOD" : "KAK OTR")})"
                    .Replace(',', '.'),
                    ToolDescriptionOption.L230 => $"T{groovingTool.Position.ToolNumber()}({groovingTool.Name} {groovingTool.Width}MM {(groovingTool.ZeroPoint == GroovingExternalTool.Point.Left ? "KAK PROHOD" : "KAK OTR")})"
                    .Replace(',', '.'),
                    ToolDescriptionOption.GoodwayLeft => $"T{groovingTool.Position.ToolNumber()}G54M58({groovingTool.Name} {groovingTool.Width}MM {(groovingTool.ZeroPoint == GroovingExternalTool.Point.Left ? "KAK PROHOD" : "KAK OTR")})"
                    .Replace(',', '.'),
                    ToolDescriptionOption.GoodwayRight => $"T{groovingTool.Position.ToolNumber()}G55M58({groovingTool.Name} {groovingTool.Width}MM {(groovingTool.ZeroPoint == GroovingExternalTool.Point.Left ? "KAK PROHOD" : "KAK OTR")})"
                    .Replace(',', '.'),
                    _ => string.Empty,
                },
                _ => string.Empty,
            };
            ;
        }

        /// <summary>
        /// Форматирует число в такую строку, какую хочу я
        /// </summary>
        /// <param name="value">Число</param>
        /// <param name="precision">Точность</param>
        /// <returns>Строку содержащую число</returns>
        public static string ToPrettyString(this double value, int precision = 3, PrettyStringOption stringOption = PrettyStringOption.ZeroToEmpty)
        {
            if (value == 0 && stringOption == PrettyStringOption.ZeroToEmpty) return string.Empty;
            string result = value.ToString($"F{precision}").Replace(",", ".");
            if (result.Contains('.')) return result.TrimEnd('0').TrimEnd('.');
            return result;
        }

        
    }


}
