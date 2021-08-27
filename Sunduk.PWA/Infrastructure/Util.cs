using MudBlazor;
using Sunduk.PWA.Infrastructure.Tools;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sunduk.PWA.Util
{
    public static class Util
    {

        public enum GetNumberOption { Any, OnlyPositive }
        public enum PrettyStringOption { AsIs, ZeroToEmpty }
        public enum ToolDescriptionOption { General, L230, GoodwayLeft, GoodwayRight }
        public enum TranslateOption { RemoveBadSymbols, OnlyTranslate }

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
            return value.ToString($"F{precision}", CultureInfo.InvariantCulture).Contains('.')
                ? value.ToString($"F{precision}", CultureInfo.InvariantCulture).TrimEnd('0')
                : value.ToString($"F{precision}");
        }

        /// <summary>
        /// Транслитерация строки 
        /// </summary>
        /// <param name="value">Число</param>
        /// <returns>Отформатированную строку</returns>
        public static string Translate(this string value, TranslateOption option = TranslateOption.RemoveBadSymbols)
        {
            if (value is null) return string.Empty;
            value = value.ToUpper()
                .Replace("ИЙ", "IY")
                .Replace("А", "A")
                .Replace("Б", "B")
                .Replace("В", "V")
                .Replace("Г", "G")
                .Replace("Д", "D")
                .Replace("Е", "E")
                .Replace("Ё", "E")
                .Replace("Ж", "J")
                .Replace("З", "Z")
                .Replace("И", "I")
                .Replace("Й", "I")
                .Replace("К", "K")
                .Replace("Л", "L")
                .Replace("М", "M")
                .Replace("Н", "N")
                .Replace("О", "O")
                .Replace("П", "P")
                .Replace("Р", "R")
                .Replace("С", "S")
                .Replace("Т", "T")
                .Replace("У", "U")
                .Replace("Ф", "F")
                .Replace("Х", "H")
                .Replace("Ц", "C")
                .Replace("Ч", "CH")
                .Replace("Ш", "SH")
                .Replace("Щ", "SH")
                .Replace("Ъ", "")
                .Replace("Ы", "Y")
                .Replace("Ь", "")
                .Replace("Э", "E")
                .Replace("Ю", "YU")
                .Replace("Я", "YA");
            if (option == TranslateOption.RemoveBadSymbols)
            {
                foreach (char item in Path.GetInvalidPathChars().Union(Path.GetInvalidFileNameChars()))
                {
                    value = value.Replace(item, '-');
                }
            }

            return value;
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
                TurningExternalTool turningExternalTool => option switch
                {
                    ToolDescriptionOption.General => $"T{turningExternalTool.Position.ToolNumber()} ({turningExternalTool.Name} {turningExternalTool.Angle} R{turningExternalTool.Radius})".Replace(',', '.'),
                    ToolDescriptionOption.L230 => $"T{turningExternalTool.Position.ToolNumber()}({turningExternalTool.Name} {turningExternalTool.Angle} R{turningExternalTool.Radius})".Replace(',', '.'),
                    ToolDescriptionOption.GoodwayLeft => $"T{turningExternalTool.Position.ToolNumber()}G54M58({turningExternalTool.Name} {turningExternalTool.Angle} R{turningExternalTool.Radius})".Replace(',', '.'),
                    ToolDescriptionOption.GoodwayRight => $"T{turningExternalTool.Position.ToolNumber()}G55M58({turningExternalTool.Name} {turningExternalTool.Angle} R{turningExternalTool.Radius})".Replace(',', '.'),
                    _ => string.Empty,
                },
                TurningInternalTool turningInternalTool => option switch
                {
                    ToolDescriptionOption.General => $"T{turningInternalTool.Position.ToolNumber()} ({turningInternalTool.Name} D{turningInternalTool.Diameter} {turningInternalTool.Angle} R{turningInternalTool.Radius})".Replace(',', '.'),
                    ToolDescriptionOption.L230 => $"T{turningInternalTool.Position.ToolNumber()}({turningInternalTool.Name} D{turningInternalTool.Diameter} {turningInternalTool.Angle} R{turningInternalTool.Radius})".Replace(',', '.'),
                    ToolDescriptionOption.GoodwayLeft => $"T{turningInternalTool.Position.ToolNumber()}G54M58({turningInternalTool.Name} D{turningInternalTool.Diameter} {turningInternalTool.Angle} R{turningInternalTool.Radius})".Replace(',', '.'),
                    ToolDescriptionOption.GoodwayRight => $"T{turningInternalTool.Position.ToolNumber()}G55M58({turningInternalTool.Name} D{turningInternalTool.Diameter} {turningInternalTool.Angle} R{turningInternalTool.Radius})".Replace(',', '.'),
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
                ThreadingExternalTool threadingExternalTool => option switch
                {
                    ToolDescriptionOption.General => $"T{threadingExternalTool.Position.ToolNumber()} ({threadingExternalTool.Name} {threadingExternalTool.Pitch} {threadingExternalTool.Angle})".Replace(',', '.'),
                    ToolDescriptionOption.L230 => $"T{threadingExternalTool.Position.ToolNumber()}({threadingExternalTool.Name} {threadingExternalTool.Pitch} {threadingExternalTool.Angle})".Replace(',', '.'),
                    ToolDescriptionOption.GoodwayLeft => $"T{threadingExternalTool.Position.ToolNumber()}G54M58({threadingExternalTool.Name} {threadingExternalTool.Pitch} {threadingExternalTool.Angle})".Replace(',', '.'),
                    ToolDescriptionOption.GoodwayRight => $"T{threadingExternalTool.Position.ToolNumber()}G55M58({threadingExternalTool.Name} {threadingExternalTool.Pitch} {threadingExternalTool.Angle})".Replace(',', '.'),
                    _ => string.Empty,
                },
                ThreadingInternalTool threadingInternalTool => option switch
                {
                    ToolDescriptionOption.General => $"T{threadingInternalTool.Position.ToolNumber()} ({threadingInternalTool.Name} D{threadingInternalTool.Diameter} {threadingInternalTool.Pitch} {threadingInternalTool.Angle})".Replace(',', '.'),
                    ToolDescriptionOption.L230 => $"T{threadingInternalTool.Position.ToolNumber()}({threadingInternalTool.Name} D{threadingInternalTool.Diameter} {threadingInternalTool.Pitch} {threadingInternalTool.Angle})".Replace(',', '.'),
                    ToolDescriptionOption.GoodwayLeft => $"T{threadingInternalTool.Position.ToolNumber()}G54M58({threadingInternalTool.Name} D{threadingInternalTool.Diameter} {threadingInternalTool.Pitch} {threadingInternalTool.Angle})".Replace(',', '.'),
                    ToolDescriptionOption.GoodwayRight => $"T{threadingInternalTool.Position.ToolNumber()}G55M58({threadingInternalTool.Name} D{threadingInternalTool.Diameter} {threadingInternalTool.Pitch} {threadingInternalTool.Angle})".Replace(',', '.'),
                    _ => string.Empty,
                },
                GroovingExternalTool groovingExternalTool => option switch
                {
                    ToolDescriptionOption.General => $"T{groovingExternalTool.Position.ToolNumber()} ({groovingExternalTool.Name} {groovingExternalTool.Width}MM {(groovingExternalTool.ZeroPoint == GroovingExternalTool.Point.Left ? "KAK PROHOD" : "KAK OTR")})"
                    .Replace(',', '.'),
                    ToolDescriptionOption.L230 => $"T{groovingExternalTool.Position.ToolNumber()}({groovingExternalTool.Name} {groovingExternalTool.Width}MM {(groovingExternalTool.ZeroPoint == GroovingExternalTool.Point.Left ? "KAK PROHOD" : "KAK OTR")})"
                    .Replace(',', '.'),
                    ToolDescriptionOption.GoodwayLeft => $"T{groovingExternalTool.Position.ToolNumber()}G54M58({groovingExternalTool.Name} {groovingExternalTool.Width}MM {(groovingExternalTool.ZeroPoint == GroovingExternalTool.Point.Left ? "KAK PROHOD" : "KAK OTR")})"
                    .Replace(',', '.'),
                    ToolDescriptionOption.GoodwayRight => $"T{groovingExternalTool.Position.ToolNumber()}G55M58({groovingExternalTool.Name} {groovingExternalTool.Width}MM {(groovingExternalTool.ZeroPoint == GroovingExternalTool.Point.Left ? "KAK PROHOD" : "KAK OTR")})"
                    .Replace(',', '.'),
                    _ => string.Empty,
                },
                GroovingInternalTool groovingInternalTool => option switch
                {
                    ToolDescriptionOption.General => $"T{groovingInternalTool.Position.ToolNumber()} ({groovingInternalTool.Name} {groovingInternalTool.Width}MM {(groovingInternalTool.ZeroPoint == GroovingInternalTool.Point.Left ? "KAK PROHOD" : "KAK OTR")})"
                    .Replace(',', '.'),
                    ToolDescriptionOption.L230 => $"T{groovingInternalTool.Position.ToolNumber()}({groovingInternalTool.Name} {groovingInternalTool.Width}MM {(groovingInternalTool.ZeroPoint == GroovingInternalTool.Point.Left ? "KAK PROHOD" : "KAK OTR")})"
                    .Replace(',', '.'),
                    ToolDescriptionOption.GoodwayLeft => $"T{groovingInternalTool.Position.ToolNumber()}G54M58({groovingInternalTool.Name} {groovingInternalTool.Width}MM {(groovingInternalTool.ZeroPoint == GroovingInternalTool.Point.Left ? "KAK PROHOD" : "KAK OTR")})"
                    .Replace(',', '.'),
                    ToolDescriptionOption.GoodwayRight => $"T{groovingInternalTool.Position.ToolNumber()}G55M58({groovingInternalTool.Name} {groovingInternalTool.Width}MM {(groovingInternalTool.ZeroPoint == GroovingInternalTool.Point.Left ? "KAK PROHOD" : "KAK OTR")})"
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


        public static string GetToolTable(string program)
        {
            List<string> tools = new();
            foreach (var line in program.Split('\n'))
            {
                if (new Regex(@"T(\d+)", RegexOptions.Compiled).IsMatch(line) && line.Contains("("))
                {
                    var fLine = "(" + line.Split("(")[1].Trim();
                    if (!tools.Contains(fLine)) tools.Add(fLine);
                }
            }
            return string.Join("\n", tools);
        }

        public static Converter<double> DoubleConverter = new()
        {
            SetFunc = value => value.ToPrettyString(),
            GetFunc = text => Util.GetDouble(text),
        };
    }


}
