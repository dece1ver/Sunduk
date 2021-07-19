using Sunduk.PWA.NC;
using System;
using System.Globalization;

namespace Sunduk.PWA.Util
{
    public static class Util
    {
        public enum PassesOption { FullPasses, Infeed }
        public enum GetNumberOption { Any, OnlyPositive }
        public enum PrettyStringOption { AsIs, ZeroToEmpty }

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
            return value.ToString($"F{precision}").Replace(",", ".").TrimEnd('0');
        }

        public static string ToolNumber(this int value)
        {
            return value.ToString($"D{4}");
        }

        public static string Description(this Tool tool)
        {
            switch (tool)
            {
                case TurningTool turningTool:
                    return $"T{turningTool.Position.ToolNumber()}({turningTool.Name} {turningTool.Angle} R{turningTool.Raduis})";
                case DrillingTool drillingTool:
                    return $"T{drillingTool.Position.ToolNumber()}({drillingTool.Name} D{drillingTool.Diameter})";
                case ThreadingTool threadingTool:
                    return $"T{threadingTool.Position.ToolNumber()}({threadingTool.Name})";
                case GroovingTool groovingTool:
                    return $"T{groovingTool.Position.ToolNumber()}({groovingTool.Name} {groovingTool.Width}MM {(groovingTool.ZeroPoint == GroovingTool.Point.Left ? "KAK PROHOD" : "KAK OTR")})";
                default:
                    return $"T{tool.Position.ToolNumber()}({tool.Name})";
            }
        }

        /// <summary>
        /// Форматирует число в такую строку, какую хочу я
        /// </summary>
        /// <param name="value">Число</param>
        /// <param name="precision">Точность</param>
        /// <returns>Строку содержащую число</returns>
        public static string ToPrettyString(this double value, int precision = 3, PrettyStringOption stringOption = PrettyStringOption.ZeroToEmpty)
        {
            if (value == 0 && stringOption == PrettyStringOption.ZeroToEmpty)
            {
                return string.Empty;
            }
            string result = value.ToString($"F{precision}").Replace(",", ".");
            string _trimmed = '.' + new string('0', precision);
            return result.Replace(_trimmed, string.Empty);
        }

        /// <summary>
        /// Считает количество проходов при нарезании резьбы
        /// </summary>
        /// <param name="threadDepth">Высота профиля резьбы</param>
        /// <param name="passesCount">Количество проходов</param>
        /// <param name="passesOption">Возврат абсолютных или инкрементных значений.</param>
        /// <returns>Массив с глубиной каждого прохода</returns>
        public static double[] CalcPasses(double threadDepth, int passesCount, PassesOption passesOption = PassesOption.FullPasses)
        {
            double[] passes = new double[passesCount];
            for (int pass = 1; pass <= passesCount; pass++)
            {
                passes[pass - 1] = Math.Round(threadDepth / Math.Sqrt(passesCount - 1) * Math.Sqrt(pass > 1 ? pass - 1 : 0.3), 2);
            }
            if (passesOption == PassesOption.FullPasses)
            {
                return passes;
            }
            else
            {
                double[] infeed = new double[passesCount];
                for (int i = 0; i < passes.Length; i++)
                {

                    if (i > 0)
                    {
                        infeed[i] = passes[i] - passes[i - 1];
                    }
                    else
                    {
                        infeed[i] = passes[i];
                    }
                }
                return infeed;
            }

        }
    }


}
