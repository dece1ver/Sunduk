using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using MudBlazor;
using Sunduk.PWA.Infrastructure.Sequences;
using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Sequences.Milling;
using Sunduk.PWA.Infrastructure.Sequences.Turning;
using Sunduk.PWA.Infrastructure.Tools.Base;
using Sunduk.PWA.Infrastructure.Tools.Milling;
using Sunduk.PWA.Infrastructure.Tools.Turning;
using Sunduk.PWA.Infrastructure.Tools.Turning.Base;

namespace Sunduk.PWA.Infrastructure
{
    public static class Util
    {

        public enum GetNumberOption { Any, OnlyPositive }
        public enum PrettyStringOption { AsIs, ZeroToEmpty }
        public enum ToolDescriptionOption { General, L230, GoodwayLeft, GoodwayRight, ToolTable, MillingToolChange }
        public enum NcDecimalPointOption { With, Without }
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
            //if (stringNumber is "-") return double.NegativeInfinity;
            NumberFormatInfo numberFormat = new() { NumberDecimalSeparator = "," };
            if (!double.TryParse(stringNumber, NumberStyles.Any, numberFormat, out double result)) return defaultValue;
            return numberOption switch
            {
                GetNumberOption.OnlyPositive when result >= 0 => result,
                GetNumberOption.Any => result,
                _ => defaultValue
            };
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
            NumberFormatInfo numberFormat = new() { NumberDecimalSeparator = "," };
            if (!int.TryParse(stringNumber, NumberStyles.Any, numberFormat, out int result)) return defaultValue;
            if (numberOption == GetNumberOption.OnlyPositive && result > 0)
            {
                return result;
            }
            else
            {
                return defaultValue;
            }
        }

        

        /// <summary>
        /// Возвращает шаг в мм для шага в нитках на дюйм (TPI), либо шаг в нитках на дюйм для шага в мм
        /// </summary>
        /// <param name="pitch">Шаг в нитках на дюйм / Шаг в мм</param>
        public static double ThreadConvert(this double pitch) => 25.4 / pitch;

        /// <summary>
        /// Форматирует число в строку подходящую для СЧПУ
        /// </summary>
        /// <param name="value">Число</param>
        /// <param name="precision">Точность</param>
        /// <param name="option">Опция плавающей точки</param>
        /// <returns>Отформатированную строку</returns>
        public static string NC(this double value, int precision = 3, NcDecimalPointOption option = NcDecimalPointOption.With)
        {
            string result = value.ToString($"F{precision}", CultureInfo.InvariantCulture).Contains('.')
                ? value.ToString($"F{precision}", CultureInfo.InvariantCulture).TrimEnd('0')
                : value.ToString($"F{precision}");
            return option == NcDecimalPointOption.With 
                ? result.Contains('.') ? result : result + '.' 
                : result.TrimEnd('.');
        }

        /// <summary>
        /// Транслитерация строки 
        /// </summary>
        /// <param name="value">Число</param>
        /// <param name="option">Опция удаления символов</param>
        /// <returns>Отформатированную строку</returns>
        public static string Translate(this string value, TranslateOption option = TranslateOption.RemoveBadSymbols)
        {
            if (value is null) return string.Empty;
            value = value.ToUpper()
                .Replace("ИЙ", "IY")
                .Replace("ОЙ", "OY")
                .Replace("ЕЙ", "EY")
                .Replace("ЫЙ", "IY")
                .Replace("ЬЕ", "YE")
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
            return option != TranslateOption.RemoveBadSymbols ? value : Path.GetInvalidPathChars().Union(Path.GetInvalidFileNameChars()).Aggregate(value, (current, item) => current.Replace(item, '-'));
        }

        public static MachineType GetMachineType(this Machine machine)
        {
            return machine switch
            {
                Machine.L230A => MachineType.Turning,
                Machine.GS1500 => MachineType.Turning,
                Machine.A110 => MachineType.Milling,
                _ => MachineType.Turning
            };
        }

        /// <summary>
        /// Добавляет отверстия к переданному куску перехода сверления
        /// </summary>
        /// <param name="operation">Заполняемая строка</param>
        /// <param name="holes">Список отверстий</param>
        /// <param name="polar">Используется ли программирование в полярной системе координат</param>
        /// <returns></returns>
        public static string AddPoints(ref string operation, List<Hole> holes, bool polar = false)
        {
            if (holes.Count <= 1 || string.IsNullOrEmpty(operation)) return operation;
            foreach (var hole in holes.Skip(1))
            {
                if (polar)
                {
                    while (hole.Y >= 360)
                    {
                        hole.Y -= 360;
                    }
                }
                if (Math.Abs(hole.X - holes[holes.IndexOf(hole) - 1].X) > 0.001)
                {
                    operation += $"X{hole.X.NC(option: NcDecimalPointOption.Without)} ";
                };
                if (Math.Abs(hole.Y - holes[holes.IndexOf(hole) - 1].Y) > 0.001)
                {
                    operation += $"Y{hole.Y.NC(option: NcDecimalPointOption.Without)} ";
                };
                if (Math.Abs(hole.Y - holes[holes.IndexOf(hole) - 1].Y) < 0.001 && Math.Abs(hole.X - holes[holes.IndexOf(hole) - 1].X) < 0.001)
                {
                    operation += $"X{hole.X.NC(option: NcDecimalPointOption.Without)} Y{hole.Y.NC(option: NcDecimalPointOption.Without)}";
                };
                operation += "\n";
            }
            return operation;
        }

        /// <summary>
        /// Форматирует число в номер инструмента
        /// </summary>
        /// <param name="value">Число</param>
        /// <returns>Отформатированную строку</returns>
        public static string ToolNumber(this int value)
        {
            return value.ToString($"D4");
        }


        /// <summary>
        /// Описание инструмента в УП
        /// </summary>
        /// <param name="tool">Инструмент</param>
        /// <param name="option">Тип описания: общий, под конкретный станок</param>
        /// <returns></returns>
        public static string Description(this Tool tool, ToolDescriptionOption option = ToolDescriptionOption.General)
        {
            return tool switch
            {
                MillingBoreTool millingBoreTool => option switch
                {
                    ToolDescriptionOption.General => $"T{millingBoreTool.Position} ({millingBoreTool.Name} D{millingBoreTool.Diameter.NC(option: NcDecimalPointOption.Without)})",
                    ToolDescriptionOption.MillingToolChange => $"T{millingBoreTool.Position} M6 ({millingBoreTool.Name} D{millingBoreTool.Diameter.NC(option: NcDecimalPointOption.Without)})",
                    ToolDescriptionOption.ToolTable => millingBoreTool.Description().Split('(')[1].TrimEnd(')'),
                    _ => string.Empty,
                },
                MillingChamferTool millingChamferTool => option switch
                {
                    ToolDescriptionOption.General => $"T{millingChamferTool.Position} ({millingChamferTool.Name} D{millingChamferTool.Diameter.NC(option: NcDecimalPointOption.Without)}x{millingChamferTool.Angle.NC(option: NcDecimalPointOption.Without)})",
                    ToolDescriptionOption.MillingToolChange => $"T{millingChamferTool.Position} M6 ({millingChamferTool.Name} D{millingChamferTool.Diameter.NC(option: NcDecimalPointOption.Without)}x{millingChamferTool.Angle.NC(option: NcDecimalPointOption.Without)})",
                    ToolDescriptionOption.ToolTable => millingChamferTool.Description().Split('(')[1].TrimEnd(')'),
                    _ => string.Empty,
                },
                MillingDrillingTool millingDrillingTool => option switch
                {
                    ToolDescriptionOption.General => $"T{millingDrillingTool.Position} ({millingDrillingTool.Name} D{millingDrillingTool.Diameter.NC(option: NcDecimalPointOption.Without)})",
                    ToolDescriptionOption.MillingToolChange => $"T{millingDrillingTool.Position} M6 ({millingDrillingTool.Name} D{millingDrillingTool.Diameter.NC(option: NcDecimalPointOption.Without)})",
                    ToolDescriptionOption.ToolTable => millingDrillingTool.Description().Split('(')[1].TrimEnd(')'),
                    _ => string.Empty,
                },
                MillingSpecialTool millingSpecialTool => option switch
                {
                    ToolDescriptionOption.General => $"T{millingSpecialTool.Position} ({millingSpecialTool.Name})",
                    ToolDescriptionOption.MillingToolChange => $"T{millingSpecialTool.Position} M6 ({millingSpecialTool.Name})",
                    ToolDescriptionOption.ToolTable => millingSpecialTool.Description().Split('(')[1].TrimEnd(')'),
                    _ => string.Empty,
                },
                MillingTappingTool millingTappingTool => option switch
                {
                    ToolDescriptionOption.General => $"T{millingTappingTool.Position} ({millingTappingTool.Name})",
                    ToolDescriptionOption.MillingToolChange => $"T{millingTappingTool.Position} M6 ({millingTappingTool.Name})",
                    ToolDescriptionOption.ToolTable => millingTappingTool.Description().Split('(')[1].TrimEnd(')'),
                    _ => string.Empty,
                },
                MillingThreadCuttingTool millingThreadCuttingTool => option switch
                {
                    ToolDescriptionOption.General => $"T{millingThreadCuttingTool.Position} ({millingThreadCuttingTool.Name})",
                    ToolDescriptionOption.MillingToolChange => $"T{millingThreadCuttingTool.Position} M6 ({millingThreadCuttingTool.Name})",
                    ToolDescriptionOption.ToolTable => millingThreadCuttingTool.Description().Split('(')[1].TrimEnd(')'),
                    _ => string.Empty,
                },
                MillingTool millingTool => option switch
                {
                    ToolDescriptionOption.General => $"T{millingTool.Position} ({millingTool.Name} D{millingTool.Diameter.NC(option: NcDecimalPointOption.Without)} L{millingTool.CuttingLength} Z{millingTool.Edges})",
                    ToolDescriptionOption.MillingToolChange => $"T{millingTool.Position} M6 ({millingTool.Name} D{millingTool.Diameter.NC(option: NcDecimalPointOption.Without)} L{millingTool.CuttingLength} Z{millingTool.Edges})",
                    ToolDescriptionOption.ToolTable => millingTool.Description().Split('(')[1].TrimEnd(')'),
                    _ => string.Empty,
                },

                GroovingExternalTool groovingExternalTool => option switch
                {
                    ToolDescriptionOption.General => $"T{groovingExternalTool.Position.ToolNumber()} ({groovingExternalTool.Name} {groovingExternalTool.Width}MM {(groovingExternalTool.ZeroPoint == TurningGroovingTool.Point.Left ? "KAK PROHOD" : "KAK OTR")})".Replace(',', '.'),
                    ToolDescriptionOption.L230 => $"T{groovingExternalTool.Position.ToolNumber()} ({groovingExternalTool.Name} {groovingExternalTool.Width}MM {(groovingExternalTool.ZeroPoint == TurningGroovingTool.Point.Left ? "KAK PROHOD" : "KAK OTR")})".Replace(',', '.'),
                    ToolDescriptionOption.GoodwayLeft => $"T{groovingExternalTool.Position.ToolNumber()} G54 M58 ({groovingExternalTool.Name} {groovingExternalTool.Width}MM {(groovingExternalTool.ZeroPoint == TurningGroovingTool.Point.Left ? "KAK PROHOD" : "KAK OTR")})".Replace(',', '.'),
                    ToolDescriptionOption.GoodwayRight => $"T{groovingExternalTool.Position.ToolNumber()} G55 M58 ({groovingExternalTool.Name} {groovingExternalTool.Width}MM {(groovingExternalTool.ZeroPoint == TurningGroovingTool.Point.Left ? "KAK PROHOD" : "KAK OTR")})".Replace(',', '.'),
                    ToolDescriptionOption.ToolTable => groovingExternalTool.Description().Split('(')[1].TrimEnd(')'),
                    _ => string.Empty,
                },
                GroovingFaceTool groovingFaceTool => option switch
                {
                    ToolDescriptionOption.General => $"T{groovingFaceTool.Position.ToolNumber()} ({groovingFaceTool.Name} {groovingFaceTool.Width}MM {(groovingFaceTool.ZeroPoint == TurningGroovingTool.Point.Bottom ? "KAK PROHOD" : "KAK RAST")})".Replace(',', '.'),
                    ToolDescriptionOption.L230 => $"T{groovingFaceTool.Position.ToolNumber()} ({groovingFaceTool.Name} {groovingFaceTool.Width}MM {(groovingFaceTool.ZeroPoint == TurningGroovingTool.Point.Bottom ? "KAK PROHOD" : "KAK RAST")})".Replace(',', '.'),
                    ToolDescriptionOption.GoodwayLeft => $"T{groovingFaceTool.Position.ToolNumber()} G54 M58 ({groovingFaceTool.Name} {groovingFaceTool.Width}MM {(groovingFaceTool.ZeroPoint == TurningGroovingTool.Point.Bottom ? "KAK PROHOD" : "KAK RAST")})".Replace(',', '.'),
                    ToolDescriptionOption.GoodwayRight => $"T{groovingFaceTool.Position.ToolNumber()} G55 M58 ({groovingFaceTool.Name} {groovingFaceTool.Width}MM {(groovingFaceTool.ZeroPoint == TurningGroovingTool.Point.Bottom ? "KAK PROHOD" : "KAK RAST")})".Replace(',', '.'),
                    ToolDescriptionOption.ToolTable => groovingFaceTool.Description().Split('(')[1].TrimEnd(')'),
                    _ => string.Empty,
                },
                GroovingInternalTool groovingInternalTool => option switch
                {
                    ToolDescriptionOption.General => $"T{groovingInternalTool.Position.ToolNumber()} ({groovingInternalTool.Name} D{groovingInternalTool.Diameter.ToPrettyString()} {groovingInternalTool.Width}MM {(groovingInternalTool.ZeroPoint == TurningGroovingTool.Point.Left ? "KAK PROHOD" : "KAK OTR")})".Replace(',', '.'),
                    ToolDescriptionOption.L230 => $"T{groovingInternalTool.Position.ToolNumber()} ({groovingInternalTool.Name} D{groovingInternalTool.Diameter.ToPrettyString()} {groovingInternalTool.Width}MM {(groovingInternalTool.ZeroPoint == TurningGroovingTool.Point.Left ? "KAK PROHOD" : "KAK OTR")})".Replace(',', '.'),
                    ToolDescriptionOption.GoodwayLeft => $"T{groovingInternalTool.Position.ToolNumber()} G54 M58 ({groovingInternalTool.Name} D{groovingInternalTool.Diameter.ToPrettyString()} {groovingInternalTool.Width}MM {(groovingInternalTool.ZeroPoint == TurningGroovingTool.Point.Left ? "KAK PROHOD" : "KAK OTR")})".Replace(',', '.'),
                    ToolDescriptionOption.GoodwayRight => $"T{groovingInternalTool.Position.ToolNumber()} G55 M58 ({groovingInternalTool.Name} D{groovingInternalTool.Diameter.ToPrettyString()} {groovingInternalTool.Width}MM {(groovingInternalTool.ZeroPoint == TurningGroovingTool.Point.Left ? "KAK PROHOD" : "KAK OTR")})".Replace(',', '.'),
                    ToolDescriptionOption.ToolTable => groovingInternalTool.Description().Split('(')[1].TrimEnd(')'),
                    _ => string.Empty,
                },
                TurningSpecialTool specialTool => option switch
                {
                    ToolDescriptionOption.General => $"T{specialTool.Position.ToolNumber()} ({specialTool.Name})".Replace(',', '.'),
                    ToolDescriptionOption.L230 => $"T{specialTool.Position.ToolNumber()} ({specialTool.Name})".Replace(',', '.'),
                    ToolDescriptionOption.GoodwayLeft => $"T{specialTool.Position.ToolNumber()} G54 M58 ({specialTool.Name})".Replace(',', '.'),
                    ToolDescriptionOption.GoodwayRight => $"T{specialTool.Position.ToolNumber()} G55 M58 ({specialTool.Name})".Replace(',', '.'),
                    ToolDescriptionOption.ToolTable => specialTool.Description().Split('(')[1].TrimEnd(')'),
                    _ => string.Empty,
                },
                ThreadingExternalTool threadingExternalTool => option switch
                {
                    ToolDescriptionOption.General => $"T{threadingExternalTool.Position.ToolNumber()} ({threadingExternalTool.Name} {threadingExternalTool.Pitch} {threadingExternalTool.Angle})".Replace(',', '.'),
                    ToolDescriptionOption.L230 => $"T{threadingExternalTool.Position.ToolNumber()} ({threadingExternalTool.Name} {threadingExternalTool.Pitch} {threadingExternalTool.Angle})".Replace(',', '.'),
                    ToolDescriptionOption.GoodwayLeft => $"T{threadingExternalTool.Position.ToolNumber()} G54 M58 ({threadingExternalTool.Name} {threadingExternalTool.Pitch} {threadingExternalTool.Angle})".Replace(',', '.'),
                    ToolDescriptionOption.GoodwayRight => $"T{threadingExternalTool.Position.ToolNumber()} G55 M58 ({threadingExternalTool.Name} {threadingExternalTool.Pitch} {threadingExternalTool.Angle})".Replace(',', '.'),
                    ToolDescriptionOption.ToolTable => threadingExternalTool.Description().Split('(')[1].TrimEnd(')'),
                    _ => string.Empty,
                },
                ThreadingInternalTool threadingInternalTool => option switch
                {
                    ToolDescriptionOption.General => $"T{threadingInternalTool.Position.ToolNumber()} ({threadingInternalTool.Name} D{threadingInternalTool.Diameter} {threadingInternalTool.Pitch} {threadingInternalTool.Angle})".Replace(',', '.'),
                    ToolDescriptionOption.L230 => $"T{threadingInternalTool.Position.ToolNumber()}({threadingInternalTool.Name} D{threadingInternalTool.Diameter} {threadingInternalTool.Pitch} {threadingInternalTool.Angle})".Replace(',', '.'),
                    ToolDescriptionOption.GoodwayLeft => $"T{threadingInternalTool.Position.ToolNumber()} G54 M58 ({threadingInternalTool.Name} D{threadingInternalTool.Diameter} {threadingInternalTool.Pitch} {threadingInternalTool.Angle})".Replace(',', '.'),
                    ToolDescriptionOption.GoodwayRight => $"T{threadingInternalTool.Position.ToolNumber()} G55 M58 ({threadingInternalTool.Name} D{threadingInternalTool.Diameter} {threadingInternalTool.Pitch} {threadingInternalTool.Angle})".Replace(',', '.'),
                    ToolDescriptionOption.ToolTable => threadingInternalTool.Description().Split('(')[1].TrimEnd(')'),
                    _ => string.Empty,
                },
                TurningDrillingTool turningDrillingTool => option switch
                {
                    ToolDescriptionOption.General => $"T{turningDrillingTool.Position.ToolNumber()} ({turningDrillingTool.Name} D{turningDrillingTool.Diameter})".Replace(',', '.'),
                    ToolDescriptionOption.L230 => $"T{turningDrillingTool.Position.ToolNumber()} ({turningDrillingTool.Name} D{turningDrillingTool.Diameter})".Replace(',', '.'),
                    ToolDescriptionOption.GoodwayLeft => $"T{turningDrillingTool.Position.ToolNumber()} G54 M58 ({turningDrillingTool.Name} D{turningDrillingTool.Diameter})".Replace(',', '.'),
                    ToolDescriptionOption.GoodwayRight => $"T{turningDrillingTool.Position.ToolNumber()} G55 M58 ({turningDrillingTool.Name} D{turningDrillingTool.Diameter})".Replace(',', '.'),
                    ToolDescriptionOption.ToolTable => turningDrillingTool.Description().Split('(')[1].TrimEnd(')'),
                    _ => string.Empty,
                },
                TurningExternalTool turningExternalTool => option switch
                {
                    ToolDescriptionOption.General => $"T{turningExternalTool.Position.ToolNumber()} ({turningExternalTool.Name} {turningExternalTool.Angle} R{turningExternalTool.Radius})".Replace(',', '.'),
                    ToolDescriptionOption.L230 => $"T{turningExternalTool.Position.ToolNumber()} ({turningExternalTool.Name} {turningExternalTool.Angle} R{turningExternalTool.Radius})".Replace(',', '.'),
                    ToolDescriptionOption.GoodwayLeft => $"T{turningExternalTool.Position.ToolNumber()} G54 M58 ({turningExternalTool.Name} {turningExternalTool.Angle} R{turningExternalTool.Radius})".Replace(',', '.'),
                    ToolDescriptionOption.GoodwayRight => $"T{turningExternalTool.Position.ToolNumber()} G55 M58 ({turningExternalTool.Name} {turningExternalTool.Angle} R{turningExternalTool.Radius})".Replace(',', '.'),
                    ToolDescriptionOption.ToolTable => turningExternalTool.Description().Split('(')[1].TrimEnd(')'),
                    _ => string.Empty,
                },
                TurningInternalTool turningInternalTool => option switch
                {
                    ToolDescriptionOption.General => $"T{turningInternalTool.Position.ToolNumber()} ({turningInternalTool.Name} D{turningInternalTool.Diameter} {turningInternalTool.Angle} R{turningInternalTool.Radius})".Replace(',', '.'),
                    ToolDescriptionOption.L230 => $"T{turningInternalTool.Position.ToolNumber()} ({turningInternalTool.Name} D{turningInternalTool.Diameter} {turningInternalTool.Angle} R{turningInternalTool.Radius})".Replace(',', '.'),
                    ToolDescriptionOption.GoodwayLeft => $"T{turningInternalTool.Position.ToolNumber()} G54 M58 ({turningInternalTool.Name} D{turningInternalTool.Diameter} {turningInternalTool.Angle} R{turningInternalTool.Radius})".Replace(',', '.'),
                    ToolDescriptionOption.GoodwayRight => $"T{turningInternalTool.Position.ToolNumber()} G55 M58 ({turningInternalTool.Name} D{turningInternalTool.Diameter} {turningInternalTool.Angle} R{turningInternalTool.Radius})".Replace(',', '.'),
                    ToolDescriptionOption.ToolTable => turningInternalTool.Description().Split('(')[1].TrimEnd(')'),
                    _ => string.Empty,
                },
                TurningTappingTool turningTappingTool => option switch
                {
                    ToolDescriptionOption.General => $"T{turningTappingTool.Position.ToolNumber()} ({turningTappingTool.Name})",
                    ToolDescriptionOption.L230 => $"T{turningTappingTool.Position.ToolNumber()} ({turningTappingTool.Name})",
                    ToolDescriptionOption.GoodwayLeft => $"T{turningTappingTool.Position.ToolNumber()} G54 M58 ({turningTappingTool.Name})",
                    ToolDescriptionOption.GoodwayRight => $"T{turningTappingTool.Position.ToolNumber()} G55 M58 ({turningTappingTool.Name})",
                    ToolDescriptionOption.ToolTable => turningTappingTool.Description().Split('(')[1].TrimEnd(')'),
                    _ => string.Empty,
                },
                _ => string.Empty,
            };
        }

        /// <summary>
        /// Описание перехода
        /// </summary>
        public static string Description(this Sequence sequence)
        {
            return sequence switch
            {
                FacingSequence facingSequence => $"{facingSequence.Name} [ T{facingSequence.Tool.Position:D4} Z{facingSequence.RoughStockAllow.NC()} => Z{facingSequence.ProfStockAllow.NC()} | W = {facingSequence.StepOver}]",
                RoughFacingSequence roughFacingSequence => $"{roughFacingSequence.Name} [ T{roughFacingSequence.Tool.Position:D4} Z{roughFacingSequence.RoughStockAllow.NC()} => Z{roughFacingSequence.ProfStockAllow.NC()} | W = {roughFacingSequence.StepOver}]",
                RoughFacingCycleSequence roughFacingCycleSequence => $"{roughFacingCycleSequence.Name} [ T{roughFacingCycleSequence.Tool.Position:D4} Z{roughFacingCycleSequence.RoughStockAllow.NC()} => Z{roughFacingCycleSequence.ProfStockAllow.NC()} | W = {roughFacingCycleSequence.StepOver}]",
                _ => sequence.Name,
            };
        }

        /// <summary>
        /// Описание типа сверла
        /// </summary>
        public static string Description(this DrillingTool.Types drillType)
        {
            return drillType switch
            {
                DrillingTool.Types.Insert => "Корпусное с пластинами",
                DrillingTool.Types.Solid => "Твёрдосплавное",
                DrillingTool.Types.Tip => "Корпусное с головкой",
                DrillingTool.Types.Center => "Центровочное",
                DrillingTool.Types.Rapid => "Быстрорежущее",
                _ => string.Empty,
            };
        }

        /// <summary>
        /// Описание типа резьбы
        /// </summary>
        public static string Description(this ThreadStandard threadStandard)
        {
            return threadStandard switch
            {
                ThreadStandard.Metric => "Метрическая 60° (М)",
                ThreadStandard.BSPP => "Трубная цилиндрическая 55° (G)",
                ThreadStandard.Trapezoidal => "Трапециедальная 30° (Tr)",
                ThreadStandard.NPT => "Коническая 60° (K)",
                _ => string.Empty,
            };
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


        /// <summary>
        /// Получает таблицу инструмента из текста УП
        /// </summary>
        /// <param name="machine">Станок</param>
        /// <param name="program">Программа в виде списка переходов</param>
        /// <returns></returns>
        public static string GetToolTable(Machine machine, List<Sequence> program) // переписать без регулярок, через инструмент в переходах
        {
            List<string> tools = new();
            foreach (var seq in program.Skip(2))
            {
                string tool = machine switch
                {
                    Machine.L230A => seq switch 
                    {
                        FacingSequence facingSequence => $"({facingSequence.Tool.Description(ToolDescriptionOption.ToolTable)})",
                        FinishFacingCycleSequence finishFacingCycleSequence => $"({finishFacingCycleSequence.Tool.Description(ToolDescriptionOption.ToolTable)})",
                        FinishFacingSequence finishFacingSequence => $"({finishFacingSequence.Tool.Description(ToolDescriptionOption.ToolTable)})",
                        LimiterSequence limiterSequence => $"({limiterSequence.Tool.Description(ToolDescriptionOption.ToolTable)})",
                        RoughFacingCycleSequence roughFacingCycleSequence => $"({roughFacingCycleSequence.Tool.Description(ToolDescriptionOption.ToolTable)})",
                        RoughFacingSequence roughFacingSequence => $"({roughFacingSequence.Tool.Description(ToolDescriptionOption.ToolTable)})",
                        ThreadCuttingSequence threadCuttingSequence => $"({threadCuttingSequence.Tool.Description(ToolDescriptionOption.ToolTable)})",
                        TurningCutOffSequence turningCutOffSequence => $"({turningCutOffSequence.Tool.Description(ToolDescriptionOption.ToolTable)})",
                        TurningHighSpeedDrillingSequence turningHighSpeedDrillingSequence => $"({turningHighSpeedDrillingSequence.Tool.Description(ToolDescriptionOption.ToolTable)})",
                        TurningPeckDeepDrillingSequence turningPeckDeepDrillingSequence => $"({turningPeckDeepDrillingSequence.Tool.Description(ToolDescriptionOption.ToolTable)})",
                        TurningPeckDrillingSequence turningPeckDrillingSequence => $"({turningPeckDrillingSequence.Tool.Description(ToolDescriptionOption.ToolTable)})",
                        TurningTappingSequence turningTappingSequence => $"({turningTappingSequence.Tool.Description(ToolDescriptionOption.ToolTable)})",
                        TurningCustomSequence turningCustomSequence => $"({turningCustomSequence.Tool.Description(ToolDescriptionOption.ToolTable)})",
                        _ => string.Empty,
                    },
                    Machine.GS1500 => seq switch
                    {
                        FacingSequence facingSequence => $"({facingSequence.Tool.Description(ToolDescriptionOption.ToolTable)})",
                        FinishFacingCycleSequence finishFacingCycleSequence => $"({finishFacingCycleSequence.Tool.Description(ToolDescriptionOption.ToolTable)})",
                        FinishFacingSequence finishFacingSequence => $"({finishFacingSequence.Tool.Description(ToolDescriptionOption.ToolTable)})",
                        LimiterSequence limiterSequence => $"({limiterSequence.Tool.Description(ToolDescriptionOption.ToolTable)})",
                        RoughFacingCycleSequence roughFacingCycleSequence => $"({roughFacingCycleSequence.Tool.Description(ToolDescriptionOption.ToolTable)})",
                        RoughFacingSequence roughFacingSequence => $"({roughFacingSequence.Tool.Description(ToolDescriptionOption.ToolTable)})",
                        ThreadCuttingSequence threadCuttingSequence => $"({threadCuttingSequence.Tool.Description(ToolDescriptionOption.ToolTable)})",
                        TurningHighSpeedDrillingSequence turningHighSpeedDrillingSequence => $"({turningHighSpeedDrillingSequence.Tool.Description(ToolDescriptionOption.ToolTable)})",
                        TurningPeckDeepDrillingSequence turningPeckDeepDrillingSequence => $"({turningPeckDeepDrillingSequence.Tool.Description(ToolDescriptionOption.ToolTable)})",
                        TurningPeckDrillingSequence turningPeckDrillingSequence => $"({turningPeckDrillingSequence.Tool.Description(ToolDescriptionOption.ToolTable)})",
                        TurningTappingSequence turningTappingSequence => $"({turningTappingSequence.Tool.Description(ToolDescriptionOption.ToolTable)})",
                        TurningCustomSequence turningCustomSequence => $"({turningCustomSequence.Tool.Description(ToolDescriptionOption.ToolTable)})",
                        _ => string.Empty,
                    },
                    Machine.A110 => seq switch
                    {
                        MillingHighSpeedDrillingSequence millingHighSpeedDrillingSequence => $"(T{(millingHighSpeedDrillingSequence.Tool.Position > 9 ? millingHighSpeedDrillingSequence.Tool.Position : millingHighSpeedDrillingSequence.Tool.Position + " ")} - {millingHighSpeedDrillingSequence.Tool.Description(ToolDescriptionOption.ToolTable)})",
                        MillingPeckDeepDrillingSequence millingPeckDeepDrillingSequence => $"(T{(millingPeckDeepDrillingSequence.Tool.Position > 9 ? millingPeckDeepDrillingSequence.Tool.Position : millingPeckDeepDrillingSequence.Tool.Position + " ")} - {millingPeckDeepDrillingSequence.Tool.Description(ToolDescriptionOption.ToolTable)})",
                        MillingPeckDrillingSequence millingPeckDrillingSequence => $"(T{(millingPeckDrillingSequence.Tool.Position > 9 ? millingPeckDrillingSequence.Tool.Position : millingPeckDrillingSequence.Tool.Position + " ")} - {millingPeckDrillingSequence.Tool.Description(ToolDescriptionOption.ToolTable)})",
                        MillingCustomSequence millingCustomSequence => $"(T{(millingCustomSequence.Tool.Position > 9 ? millingCustomSequence.Tool.Position : millingCustomSequence.Tool.Position + " ")} - {millingCustomSequence.Tool.Description(ToolDescriptionOption.ToolTable)})",
                        _ => string.Empty,
                    }, 
                    _ => string.Empty,
                };
                if (!tools.Contains(tool)) tools.Add(tool);
            }
            return tools.Count <= 1 ? string.Empty : $"\n{string.Join("\n", tools)}\n";
        }


        /// <summary>
        /// Меняет местами 2 элемента списка
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">Список в котором меняем</param>
        /// <param name="index1">Индекс первого элемента</param>
        /// <param name="index2">Индекс второго элемента</param>
        public static void Swap<T>(this List<T> list, int index1, int index2) => (list[index1], list[index2]) = (list[index2], list[index1]);

        /// <summary>
        /// Конвертер Double
        /// </summary>
        public static Converter<int> IntConverterFromOne = new()
        {
            SetFunc = value => value.ToString(),
            GetFunc = text => text.GetInt(1, GetNumberOption.OnlyPositive),
        };

        /// <summary>
        /// Конвертер Double
        /// </summary>
        public static Converter<double> DoubleConverter = new()
        {
            SetFunc = value => value.ToPrettyString(),
            GetFunc = text => text.GetDouble(0, GetNumberOption.Any),
        };

        /// <summary>
        /// Конвертер углов 0-180 Double
        /// </summary>
        public static Converter<double> HalfAngleDoubleConverter = new()
        {
            SetFunc = value => 
            { 
                if (value is > 0 and <= 180) return value.ToPrettyString();
                return "0";
            },
            GetFunc = text => text.GetDouble(0, GetNumberOption.Any),
        };

        /// <summary>
        /// Конвертер Double с нулем
        /// </summary>
        public static Converter<double?> NullableDoubleConverterWithZero = new()
        {
            SetFunc = value => value?.ToPrettyString(stringOption: PrettyStringOption.AsIs),
            GetFunc = text => (string.IsNullOrEmpty(text) || text is "-") ? null : text.GetDouble(0, GetNumberOption.Any),
        };

        /// <summary>
        /// Конвертер отверстий для фрезерной сверловки
        /// </summary>
        public static Converter<int> HolesConverter = new()
        {
            SetFunc = value => value.ToString(),
            GetFunc = text => text.GetInt(1, GetNumberOption.OnlyPositive),
        };

        /// <summary>
        /// Конвертер Double
        /// </summary>
        public static Converter<int> EdgesConverter = new()
        {
            SetFunc = value => value.ToString(),
            GetFunc = text => text.GetInt(1, GetNumberOption.OnlyPositive),
        };

        /// <summary>
        /// Получает номера строк для циклов УП в зависимости от количества таких переходов
        /// </summary>
        /// <param name="count">Количество переходов</param>
        /// <returns></returns>
        public static (int seqNo1, int seqNo2) GetCycleRange(this int count)
        {
            return (count * 2 - 1, count * 2);
        }
    }
}
