using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using MudBlazor;
using Sunduk.PWA.Infrastructure.Sequences;
using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Sequences.ContourElements;
using Sunduk.PWA.Infrastructure.Sequences.ContourElements.Base;
using Sunduk.PWA.Infrastructure.Sequences.Milling;
using Sunduk.PWA.Infrastructure.Sequences.Turning;
using Sunduk.PWA.Infrastructure.Sequences.Turning.Base;
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
        /// Равномерно или вручную распределённые отверстия по окружности (для сверления/резьбофрезерования по цилиндру)
        /// </summary>
        public static List<Hole> PolarHoles(int holesCount, double radius, double startAngle, bool evenly, List<Hole> manualHoles)
        {
            List<Hole> result = new();
            var angleStep = 360.0 / holesCount;
            while (startAngle >= 360) startAngle -= 360;
            while (startAngle <= -360) startAngle += 360;
            for (var i = 0; i < holesCount; i++)
            {
                result.Add(evenly ? new Hole(radius, angleStep * i + startAngle) : new Hole(radius, manualHoles[i].Y));
            }
            return result;
        }

        /// <summary>
        /// Подпись поля притупления/фаски в зависимости от выбранного типа
        /// </summary>
        public static string BluntLabel(Blunt bluntType)
        {
            return bluntType == Blunt.CustomChamfer ? "Размер фаски" : "Величина притупления";
        }


        /// <summary>
        /// Описание инструмента в УП
        /// </summary>
        /// <param name="tool">Инструмент</param>
        /// <param name="option">Тип описания: общий, под конкретный станок</param>
        /// <returns></returns>
        public static string Description(this Tool tool, ToolDescriptionOption option = ToolDescriptionOption.General)
        {
            return tool.Description(option);
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
                ThreadStandard.NPT => "Коническая 60° (К)",
                ThreadStandard.BSPT => "Коническая 55° (R/Rc)",
                ThreadStandard.UNC => "UNC 60°",
                ThreadStandard.UNF => "UNF 60°",
                ThreadStandard.UNEF => "UNEF 60°",
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
                        TurningExternalGroovingSequence turningGroovingSequence => $"({turningGroovingSequence.Tool.Description(ToolDescriptionOption.ToolTable)})",
                        TurningInternalGroovingSequence turningInternalGroovingSequence => $"({turningInternalGroovingSequence.Tool.Description(ToolDescriptionOption.ToolTable)})",
                        TurningExternalRoughGroovingSequence turningExternalRoughGroovingSequence => $"({turningExternalRoughGroovingSequence.Tool.Description(ToolDescriptionOption.ToolTable)})",
                        TurningInternalRoughGroovingSequence turningInternalRoughGroovingSequence => $"({turningInternalRoughGroovingSequence.Tool.Description(ToolDescriptionOption.ToolTable)})",
                        TurningFaceGroovingSequence turningFaceGroovingSequence => $"({turningFaceGroovingSequence.Tool.Description(ToolDescriptionOption.ToolTable)})",
                        TurningFaceRoughGroovingSequence turningFaceRoughGroovingSequence => $"({turningFaceRoughGroovingSequence.Tool.Description(ToolDescriptionOption.ToolTable)})",
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
                        TurningExternalGroovingSequence turningGroovingSequence => $"({turningGroovingSequence.Tool.Description(ToolDescriptionOption.ToolTable)})",
                        TurningInternalGroovingSequence turningInternalGroovingSequence => $"({turningInternalGroovingSequence.Tool.Description(ToolDescriptionOption.ToolTable)})",
                        TurningExternalRoughGroovingSequence turningExternalRoughGroovingSequence => $"({turningExternalRoughGroovingSequence.Tool.Description(ToolDescriptionOption.ToolTable)})",
                        TurningInternalRoughGroovingSequence turningInternalRoughGroovingSequence => $"({turningInternalRoughGroovingSequence.Tool.Description(ToolDescriptionOption.ToolTable)})",
                        TurningFaceGroovingSequence turningFaceGroovingSequence => $"({turningFaceGroovingSequence.Tool.Description(ToolDescriptionOption.ToolTable)})",
                        TurningFaceRoughGroovingSequence turningFaceRoughGroovingSequence => $"({turningFaceRoughGroovingSequence.Tool.Description(ToolDescriptionOption.ToolTable)})",
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
        /// Конвертер Int
        /// </summary>
        public static IConverter<int, string?> IntConverter = Conversions
            .From(
                (int value) => value.ToString(),
                text => text.GetInt(0, GetNumberOption.OnlyPositive)
            );

        /// <summary>
        /// Конвертер Int с дефолтным значением 1
        /// </summary>
        public static IConverter<int, string> IntConverterFromOne = Conversions
            .From(
                (int value) => value.ToString(),
                text => text.GetInt(1, GetNumberOption.OnlyPositive)
        );

        /// <summary>
        /// Конвертер Double
        /// </summary>
        public static IConverter<double, string?> DoubleConverter = Conversions.From(
            (double value) => value.ToPrettyString(),
            text => text.GetDouble(0, GetNumberOption.Any));

        /// <summary>
        /// Конвертер углов 0-180 Double
        /// </summary>
        public static IConverter<double, string?> HalfAngleDoubleConverter = Conversions.From(
            (double value) => value is > 0 and <= 180 ? value.ToPrettyString() : "0",
            text => text.GetDouble(0, GetNumberOption.Any));

        /// <summary>
        /// Конвертер Double с нулем
        /// </summary>
        public static IConverter<double?, string?> NullableDoubleConverterWithZero = Conversions.From(
            (double? value) => value?.ToPrettyString(stringOption: PrettyStringOption.AsIs),
            text => string.IsNullOrEmpty(text) || text is "-" ? null : (double?)text.GetDouble(0, GetNumberOption.Any));

        /// <summary>
        /// Конвертер отверстий для фрезерной сверловки
        /// </summary>
        public static IConverter<int, string?> HolesConverter = Conversions.From(
            (int value) => value.ToString(),
            text => text.GetInt(1, GetNumberOption.OnlyPositive));

        /// <summary>
        /// Конвертер Int
        /// </summary>
        public static IConverter<int, string?> EdgesConverter = Conversions.From(
            (int value) => value.ToString(),
            text => text.GetInt(1, GetNumberOption.OnlyPositive));

        /// <summary>
        /// Получает номера строк для циклов УП в зависимости от количества таких переходов
        /// </summary>
        /// <param name="count">Количество переходов</param>
        /// <returns></returns>
        public static (int seqNo1, int seqNo2) GetCycleRange(this int count)
        {
            return (count * 2 - 1, count * 2);
        }

        public static string PathFromContour(List<Element> contour)
        {
            bool ValidArc(int index)
            {
                if (index >= contour.Count || index <= 0) return false;
                double radius = ((contour[index] as Arc)!).Radius;
                if (radius <= 0) return false; 
                double? xDifference = (contour[index].X - contour[index - 1].X) / 2;
                double? zDifference = contour[index].Z - contour[index - 1].Z;
                double length = Math.Sqrt(Math.Pow(xDifference ?? 0, 2) + Math.Pow(zDifference ?? 0, 2));
                return !(radius * 2 < length);
            }

            if (contour[0].X is null && contour[0].Z is null) return string.Empty;
            string path = string.Empty;
            foreach (var element in contour)
            {
                switch (element)
                {
                    case Point point:
                        path += $"M {(point.Z * 4)?.ToString().Replace(",", ".")},{(-point.X / 2 * 4)?.ToString().Replace(",", ".")} ";
                        if (point.Blunt > 0 && contour.Count > contour.IndexOf(point) + 1)
                        {
                            path += $"A{(point.Blunt * 4).ToString(CultureInfo.InvariantCulture).Replace(",", ".")},{(point.Blunt * 4).ToString(CultureInfo.InvariantCulture).Replace(",", ".")},0,0{(point.Z > (contour[contour.IndexOf(point) + 1].Z) ? 0 : 1)},{(point.Z > (contour[contour.IndexOf(point) + 1].Z) ? -point.Blunt * 4 : point.Blunt * 4).ToString(CultureInfo.InvariantCulture).Replace(",", ".")},{(-contour[contour.IndexOf(point) + 1].X / 2 * 4)?.ToString().Replace(",", ".")} ";
                        }
                        break;
                    case Line line:
                        double? tempLineX = line.X ?? contour[contour.IndexOf(line) - 1].X;
                        double? tempLineZ = line.Z ?? contour[contour.IndexOf(line) - 1].Z;
                        path += $"L {(tempLineZ * 4)?.ToString().Replace(",", ".")},{(-tempLineX / 2 * 4)?.ToString().Replace(",", ".")} ";
                        break;
                    case Arc arc:
                        double? tempArcX = arc.X ?? contour[contour.IndexOf(arc) - 1].X;
                        double? tempArcZ = arc.Z ?? contour[contour.IndexOf(arc) - 1].Z;
                        if (ValidArc(contour.IndexOf(arc)))
                        {
                            path += $"A{(arc.Radius * 4).ToString(CultureInfo.InvariantCulture).Replace(",", ".")},{(arc.Radius * 4).ToString(CultureInfo.InvariantCulture).Replace(",", ".")},0,0{(arc.Direction is Infrastructure.Direction.CCW ? 0 : 1)},{(tempArcZ * 4)?.ToString().Replace(",", ".")},{(-tempArcX / 2 * 4)?.ToString().Replace(",", ".")} ";
                        }
                        else
                        {
                            path += $"L {(tempArcZ * 4)?.ToString().Replace(",", ".")},{(-tempArcX / 2 * 4)?.ToString().Replace(",", ".")} ";
                        }
                        break;
                }
            }
            return path;
        }
        /// <summary>
        /// Преобразует переданное значение в число с плавающей запятой типа <see cref="double"/>.
        /// </summary>
        /// <param name="raw">
        /// Объект для преобразования. Поддерживаются типы:
        /// <see cref="int"/>, <see cref="long"/>, <see cref="double"/>.
        /// </param>
        /// <returns>
        /// Значение, приведённое к типу <see cref="double"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Выбрасывается, если тип аргумента не поддерживается.
        /// </exception>
        public static double ToDouble(object raw)
        {
            return raw switch
            {
                int i => i,
                long l => l,
                double d => d,
                _ => throw new ArgumentException($"Неподдерживаемый тип аргумента: {raw.GetType()}")
            };
        }

        /// <summary>
        /// Добавляет сообщение об ошибке в указанный список и выбрасывает исключение заданного типа.
        /// </summary>
        /// <typeparam name="T">
        /// Тип исключения, который необходимо выбросить. Должен иметь конструктор, принимающий один параметр <see cref="string"/>.
        /// </typeparam>
        /// <param name="message">
        /// Сообщение об ошибке, которое будет добавлено в список и передано в исключение.
        /// </param>
        /// <param name="errors">
        /// Список сообщений об ошибках, в который будет добавлено <paramref name="message"/>.
        /// </param>
        /// <returns>
        /// Данный метод никогда не возвращает значение, так как всегда выбрасывает исключение.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Выбрасывается, если у указанного типа исключения отсутствует конструктор с параметром <see cref="string"/>.
        /// </exception>
        /// <exception cref="T">
        /// Всегда выбрасывается после добавления сообщения в список ошибок.
        /// </exception>
        public static double ThrowLogged<T>(string message, List<string> errors) where T : Exception
        {
            errors.Add(message);

            var ctor = typeof(T).GetConstructor([typeof(string)])
                ?? throw new InvalidOperationException($"У исключения {typeof(T).Name} отсутствует конструктор с параметром string.");
            throw (T)ctor.Invoke([message]);
        }

    }
}
