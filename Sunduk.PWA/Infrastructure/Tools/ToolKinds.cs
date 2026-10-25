using System;
using System.Collections.Generic;
using System.Linq;
using Sunduk.PWA.Infrastructure.Sequences;
using Sunduk.PWA.Infrastructure.Templates;
using Sunduk.PWA.Infrastructure.Tools.Base;
using Sunduk.PWA.Infrastructure.Tools.Milling;
using Sunduk.PWA.Infrastructure.Tools.Turning;
using Sunduk.PWA.Infrastructure.Tools.Turning.Base;

namespace Sunduk.PWA.Infrastructure.Tools
{
    public enum NumberKind { Plain, HalfAngle }

    public sealed record SelectOption(object Value, string Text);

    /// <summary> Поле формы редактирования инструмента. Видимость и доступность могут зависеть от самого инструмента. </summary>
    public abstract record ToolField(string Label)
    {
        public Func<Tool, bool> Visible { get; init; } = _ => true;
    }

    public sealed record NumberField(string Label, Func<Tool, double> Get, Action<Tool, double> Set) : ToolField(Label)
    {
        public string? Prefix { get; init; }
        public string? Suffix { get; init; }
        public NumberKind Kind { get; init; }
        public Func<Tool, bool> ReadOnly { get; init; } = _ => false;
    }

    public sealed record IntField(string Label, Func<Tool, int> Get, Action<Tool, int> Set) : ToolField(Label)
    {
        public string? Prefix { get; init; }

        /// <summary> Сколько знаков показывать (с нулями слева): номера инструмента вида T0101. 0 — без дополнения. </summary>
        public int Digits { get; init; }
    }

    public sealed record TextField(string Label, Func<Tool, string> Get, Action<Tool, string> Set) : ToolField(Label);

    public sealed record SelectField(string Label, Func<Tool, IReadOnlyList<SelectOption>> Options, Func<Tool, object?> Get, Action<Tool, object?> Set) : ToolField(Label);

    /// <summary>
    /// Описание одного вида инструмента: как называется, как создаётся и какие у него поля.
    /// Заменяет отдельную таблицу на каждый вид: список и форма редактирования строятся по этим описаниям.
    /// Порядок полей важен: значения копируются на исходный инструмент по очереди (см. <see cref="CopyValues"/>),
    /// а некоторые сеттеры зависят друг от друга (тип сверла задаёт угол, стандарт резьбы сбрасывает шаблон).
    /// </summary>
    public sealed record ToolKind(string Title, string Singular, MachineType MachineType, Type ToolType, Func<Tool> Create, IReadOnlyList<ToolField> Fields)
    {
        /// <summary> Переносит значения всех полей с <paramref name="source"/> на <paramref name="target"/>. </summary>
        public void CopyValues(Tool source, Tool target)
        {
            foreach (var field in Fields)
            {
                switch (field)
                {
                    case NumberField f: f.Set(target, f.Get(source)); break;
                    case IntField f: f.Set(target, f.Get(source)); break;
                    case TextField f: f.Set(target, f.Get(source)); break;
                    case SelectField f: f.Set(target, f.Get(source)); break;
                }
            }
        }
    }

    public static class ToolKinds
    {
        public static ToolKind? Find(Tool tool) => All.FirstOrDefault(k => k.ToolType == tool.GetType());

        public static IEnumerable<ToolKind> For(MachineType machineType) => All.Where(k => k.MachineType == machineType);

        #region Конструкторы полей

        private static ToolField Position(MachineType machineType) =>
            new IntField("Позиция", t => t.Position, (t, v) => t.Position = v)
            { Prefix = "T", Digits = machineType == MachineType.Turning ? 4 : 2 };

        private static ToolField Hand() => new SelectField("Исполнение",
            _ => new SelectOption[] { new(Tool.ToolHand.Right, "Правое"), new(Tool.ToolHand.Left, "Левое") },
            t => t.Hand, (t, v) => t.Hand = (Tool.ToolHand)v!);

        private static NumberField Num<T>(string label, Func<T, double> get, Action<T, double> set,
            string? prefix = null, string? suffix = null, NumberKind kind = NumberKind.Plain) where T : Tool =>
            new(label, t => get((T)t), (t, v) => set((T)t, v)) { Prefix = prefix, Suffix = suffix, Kind = kind };

        private static NumberField Diameter<T>(Func<T, double> get, Action<T, double> set) where T : Tool =>
            Num("Диаметр", get, set, prefix: "Ø");

        private static NumberField Angle<T>(string label, Func<T, double> get, Action<T, double> set, NumberKind kind = NumberKind.Plain) where T : Tool =>
            Num(label, get, set, suffix: "°", kind: kind);

        private static NumberField CornerRadius<T>(string label, Func<T, double> get, Action<T, double> set) where T : Tool =>
            Num(label, get, set, prefix: "R");

        private static SelectField Select<T, TValue>(string label, (TValue Value, string Text)[] options, Func<T, TValue> get, Action<T, TValue> set)
            where T : Tool where TValue : notnull =>
            new(label,
                _ => options.Select(o => new SelectOption(o.Value, o.Text)).ToList(),
                t => get((T)t), (t, v) => set((T)t, (TValue)v!));

        private static ToolKind Of<T>(string title, string singular, MachineType machineType, Func<T> create, params ToolField[] fields) where T : Tool =>
            new(title, singular, machineType, typeof(T), () => create(), fields);

        #endregion

        #region Общие поля нескольких видов

        private static readonly (DrillingTool.Types, string)[] DrillTypes =
        {
            (DrillingTool.Types.Solid, "Твердосплавное"),
            (DrillingTool.Types.Insert, "С пластинами"),
            (DrillingTool.Types.Tip, "С головкой"),
            (DrillingTool.Types.Rapid, "Быстрорез"),
            (DrillingTool.Types.Center, "Центровочное"),
        };

        // Только стандарты, которые умеют обрабатывать классы инструментов (остальные дают пустое название)
        private static readonly (ThreadStandard, string)[] ThreadStandards =
            new[] { ThreadStandard.Metric, ThreadStandard.BSPP, ThreadStandard.Trapezoidal, ThreadStandard.NPT }
                .Select(s => (s, s.Description())).ToArray();

        private static bool IsTemplateStandard(ThreadStandard s) => s is ThreadStandard.BSPP or ThreadStandard.NPT;

        private static IReadOnlyList<SelectOption> TemplateOptions(ThreadStandard standard) =>
            (standard == ThreadStandard.BSPP ? Thread.BsppTemplates : Thread.NptTemplates)
                .Select(x => new SelectOption(x, x)).ToList();

        /// <summary> Шаг трубной и конической резьбы однозначно определяется выбранным шаблоном. </summary>
        private static double PitchFromTemplate(ThreadStandard standard, string template)
        {
            if (standard == ThreadStandard.BSPP)
            {
                Thread.GetBsppValues(template, out _, out var pitch);
                return pitch.GetDouble();
            }
            Thread.GetNptValues(template, out _, out _, out var nptPitch, out _, out _);
            return nptPitch.GetDouble();
        }

        private static ToolField[] TappingFields<T>(MachineType machineType) where T : TappingTool => new ToolField[]
        {
            Position(machineType),
            Select<T, TappingTool.Types>("Тип", new[] { (TappingTool.Types.Cutting, "Нарезной"), (TappingTool.Types.Forming, "Раскатник") }, t => t.Type, (t, v) => t.Type = v),
            Select<T, ThreadStandard>("Стандарт", ThreadStandards, t => t.ThreadStandard, (t, v) =>
            {
                if (t.ThreadStandard == v) return;
                t.ThreadStandard = v;
                t.StandardTemplate = string.Empty;
            }),
            new SelectField("Резьба", t => TemplateOptions(((TappingTool)t).ThreadStandard),
                t => ((TappingTool)t).StandardTemplate,
                (t, v) =>
                {
                    var tool = (TappingTool)t;
                    var template = (string?)v ?? string.Empty;
                    tool.StandardTemplate = template;
                    if (template.Length > 0) tool.Pitch = PitchFromTemplate(tool.ThreadStandard, template);
                }) { Visible = t => IsTemplateStandard(((TappingTool)t).ThreadStandard) },
            new NumberField("Диаметр", t => ((TappingTool)t).Diameter, (t, v) => ((TappingTool)t).Diameter = v)
                { Prefix = "Ø", Visible = t => !IsTemplateStandard(((TappingTool)t).ThreadStandard) },
            new NumberField("Шаг", t => ((TappingTool)t).Pitch, (t, v) => ((TappingTool)t).Pitch = v)
                { Suffix = "мм", ReadOnly = t => IsTemplateStandard(((TappingTool)t).ThreadStandard) },
            Hand(),
        };

        #endregion

        public static IReadOnlyList<ToolKind> All { get; } = new List<ToolKind>
        {
            // ── Фрезерные ──
            Of(
                "Фрезы", "Фреза", MachineType.Milling, () => new MillingTool(1, MillingTool.Types.Solid, 12),
                Position(MachineType.Milling),
                Select<MillingTool, MillingTool.Types>("Тип", new[] { (MillingTool.Types.Solid, "Твердосплавная"), (MillingTool.Types.Insert, "С пластинами") }, t => t.Type, (t, v) => t.Type = v),
                Diameter<MillingTool>(t => t.Diameter, (t, v) => t.Diameter = v),
                new IntField("Зубья", t => ((MillingTool)t).Edges, (t, v) => ((MillingTool)t).Edges = Math.Max(1, v)),
                Num<MillingTool>("Рабочая глубина", t => t.CuttingLength, (t, v) => t.CuttingLength = v, prefix: "L"),
                CornerRadius<MillingTool>("Радиус при вершине", t => t.CornerRadius, (t, v) => t.CornerRadius = v),
                Hand()),
            Of(
                "Сверла", "Сверло", MachineType.Milling, () => new MillingDrillingTool(1, DrillingTool.Types.Solid, 10, 140),
                Position(MachineType.Milling),
                Select<MillingDrillingTool, DrillingTool.Types>("Тип", DrillTypes, t => t.Type, (t, v) => t.Type = v),
                Diameter<MillingDrillingTool>(t => t.Diameter, (t, v) => t.Diameter = v),
                // Угол идёт после типа: смена типа подставляет типовой угол, который можно поправить вручную
                Angle<MillingDrillingTool>("Угол", t => t.Angle, (t, v) => t.Angle = v),
                Hand()),
            Of(
                "Расточки", "Расточная головка", MachineType.Milling, () => new MillingBoreTool(1, 20),
                Position(MachineType.Milling),
                Diameter<MillingBoreTool>(t => t.Diameter, (t, v) => t.Diameter = v),
                Num<MillingBoreTool>("Рабочая глубина", t => t.CuttingLength, (t, v) => t.CuttingLength = v, prefix: "L"),
                CornerRadius<MillingBoreTool>("Радиус при вершине", t => t.Radius, (t, v) => t.Radius = v),
                Hand()),
            Of(
                "Фасочники", "Фасочная фреза", MachineType.Milling, () => new MillingChamferTool(1, 6, 45),
                Position(MachineType.Milling),
                Diameter<MillingChamferTool>(t => t.Diameter, (t, v) => t.Diameter = v),
                Angle<MillingChamferTool>("Угол", t => t.Angle, (t, v) => t.Angle = v, NumberKind.HalfAngle),
                Num<MillingChamferTool>("Компенсация кончика", t => t.TipCompensation, (t, v) => t.TipCompensation = v, suffix: "мм"),
                Hand()),
            Of(
                "Метчики", "Метчик", MachineType.Milling, () => new MillingTappingTool(1, TappingTool.Types.Cutting, 8, 1.25, ThreadStandard.Metric),
                TappingFields<MillingTappingTool>(MachineType.Milling)),
            Of(
                "Резьбофрезы", "Резьбофреза", MachineType.Milling, () => new MillingThreadCuttingTool(1, 9.5, ThreadStandard.Metric, 1.5),
                Position(MachineType.Milling),
                Select<MillingThreadCuttingTool, ThreadStandard>("Стандарт", ThreadStandards, t => t.ThreadStandard, (t, v) =>
                {
                    if (t.ThreadStandard == v) return;
                    t.ThreadStandard = v;
                    t.StandardTemplate = string.Empty;
                }),
                Diameter<MillingThreadCuttingTool>(t => t.Diameter, (t, v) => t.Diameter = v),
                new SelectField("Резьба", t => TemplateOptions(((MillingThreadCuttingTool)t).ThreadStandard),
                    t => ((MillingThreadCuttingTool)t).StandardTemplate,
                    (t, v) =>
                    {
                        var tool = (MillingThreadCuttingTool)t;
                        var template = (string?)v ?? string.Empty;
                        tool.StandardTemplate = template;
                        if (template.Length > 0) tool.Pitch = PitchFromTemplate(tool.ThreadStandard, template);
                    }) { Visible = t => IsTemplateStandard(((MillingThreadCuttingTool)t).ThreadStandard) },
                new NumberField("Шаг", t => ((MillingThreadCuttingTool)t).Pitch, (t, v) => ((MillingThreadCuttingTool)t).Pitch = v)
                    { Suffix = "мм", Visible = t => !IsTemplateStandard(((MillingThreadCuttingTool)t).ThreadStandard) },
                Hand()),
            Of(
                "Специальные", "Специальный инструмент", MachineType.Milling, () => new MillingSpecialTool(1, string.Empty),
                Position(MachineType.Milling),
                new TextField("Описание", t => t.Name ?? string.Empty, (t, v) => t.Name = v)),

            // ── Токарные ──
            Of(
                "Точение наружное", "Резец наружный", MachineType.Turning, () => new TurningExternalTool(0101, TurningExternalTool.Types.Bar, 80, 0.8),
                Position(MachineType.Turning),
                Select<TurningExternalTool, TurningExternalTool.Types>("Тип", new[] { (TurningExternalTool.Types.Bar, "Проходной"), (TurningExternalTool.Types.Face, "Подрезной") }, t => t.Type, (t, v) => t.Type = v),
                Angle<TurningExternalTool>("Угол при вершине", t => t.Angle, (t, v) => t.Angle = v),
                CornerRadius<TurningExternalTool>("Радиус при вершине", t => t.Radius, (t, v) => t.Radius = v),
                Hand()),
            Of(
                "Точение внутреннее", "Резец расточной", MachineType.Turning, () => new TurningInternalTool(0101, 25, 80, 0.8),
                Position(MachineType.Turning),
                Diameter<TurningInternalTool>(t => t.Diameter, (t, v) => t.Diameter = v),
                Angle<TurningInternalTool>("Угол при вершине", t => t.Angle, (t, v) => t.Angle = v),
                CornerRadius<TurningInternalTool>("Радиус при вершине", t => t.Radius, (t, v) => t.Radius = v),
                Hand()),
            Of(
                "Канавки наружные", "Канавочный резец наружный", MachineType.Turning, () => new GroovingExternalTool(0101, GroovingExternalTool.Types.Cutting, 3, TurningGroovingTool.Point.Left),
                Position(MachineType.Turning),
                Select<GroovingExternalTool, GroovingExternalTool.Types>("Тип", new[] { (GroovingExternalTool.Types.Grooving, "Канавочный"), (GroovingExternalTool.Types.Cutting, "Отрезной"), (GroovingExternalTool.Types.Blade, "Лезвие") }, t => t.Type, (t, v) => t.Type = v),
                Num<GroovingExternalTool>("Ширина", t => t.Width, (t, v) => t.Width = v, suffix: "мм"),
                CornerRadius<GroovingExternalTool>("Радиус", t => t.CornerRadius, (t, v) => t.CornerRadius = v),
                Select<GroovingExternalTool, TurningGroovingTool.Point>("Привязка", new[] { (TurningGroovingTool.Point.Left, "Как проходной"), (TurningGroovingTool.Point.Right, "Как отрезной") }, t => t.ZeroPoint, (t, v) => t.ZeroPoint = v),
                Hand()),
            Of(
                "Канавки торцевые", "Канавочный резец торцевой", MachineType.Turning, () => new GroovingFaceTool(0101, 3, TurningGroovingTool.Point.Top),
                Position(MachineType.Turning),
                Num<GroovingFaceTool>("Ширина", t => t.Width, (t, v) => t.Width = v, suffix: "мм"),
                CornerRadius<GroovingFaceTool>("Радиус", t => t.CornerRadius, (t, v) => t.CornerRadius = v),
                Select<GroovingFaceTool, TurningGroovingTool.Point>("Привязка", new[] { (TurningGroovingTool.Point.Bottom, "Как проходной"), (TurningGroovingTool.Point.Top, "Как расточной") }, t => t.ZeroPoint, (t, v) => t.ZeroPoint = v),
                Hand()),
            Of(
                "Канавки внутренние", "Канавочный резец внутренний", MachineType.Turning, () => new GroovingInternalTool(0101, 20, 3, TurningGroovingTool.Point.Left),
                Position(MachineType.Turning),
                Diameter<GroovingInternalTool>(t => t.Diameter, (t, v) => t.Diameter = v),
                Num<GroovingInternalTool>("Ширина", t => t.Width, (t, v) => t.Width = v, suffix: "мм"),
                CornerRadius<GroovingInternalTool>("Радиус", t => t.CornerRadius, (t, v) => t.CornerRadius = v),
                Select<GroovingInternalTool, TurningGroovingTool.Point>("Привязка", new[] { (TurningGroovingTool.Point.Left, "Как расточной"), (TurningGroovingTool.Point.Right, "Как отрезной") }, t => t.ZeroPoint, (t, v) => t.ZeroPoint = v),
                Hand()),
            Of(
                "Резьбовые наружные", "Резьбовой резец наружный", MachineType.Turning, () => new ThreadingExternalTool(0101, 1.5, 1),
                Position(MachineType.Turning),
                Num<ThreadingExternalTool>("Шаг", t => t.Pitch, (t, v) => t.Pitch = v, suffix: "мм"),
                Angle<ThreadingExternalTool>("Угол опорной пластины", t => t.Angle, (t, v) => t.Angle = v),
                Hand()),
            Of(
                "Резьбовые внутренние", "Резьбовой резец внутренний", MachineType.Turning, () => new ThreadingInternalTool(0101, 20, 1.5, 1),
                Position(MachineType.Turning),
                Diameter<ThreadingInternalTool>(t => t.Diameter, (t, v) => t.Diameter = v),
                Num<ThreadingInternalTool>("Шаг", t => t.Pitch, (t, v) => t.Pitch = v, suffix: "мм"),
                Angle<ThreadingInternalTool>("Угол опорной пластины", t => t.Angle, (t, v) => t.Angle = v),
                Hand()),
            Of(
                "Накатки наружные", "Накатка наружная", MachineType.Turning, () => new TurningExternalBurnishingTool(0101, TurningBurnishingTool.Types.Diamond),
                Position(MachineType.Turning),
                Select<TurningExternalBurnishingTool, TurningBurnishingTool.Types>("Тип", new[] { (TurningBurnishingTool.Types.Diamond, "Алмаз"), (TurningBurnishingTool.Types.Roller, "Ролик") }, t => t.Type, (t, v) => t.Type = v)),
            Of(
                "Накатки внутренние", "Накатка внутренняя", MachineType.Turning, () => new TurningInternalBurnishingTool(0101, TurningInternalBurnishingTool.Types.Diamond, 32),
                Position(MachineType.Turning),
                Select<TurningInternalBurnishingTool, TurningBurnishingTool.Types>("Тип", new[] { (TurningBurnishingTool.Types.Diamond, "Алмаз"), (TurningBurnishingTool.Types.Roller, "Ролик") }, t => t.Type, (t, v) => t.Type = v),
                Diameter<TurningInternalBurnishingTool>(t => t.Diameter, (t, v) => t.Diameter = v)),
            Of(
                "Сверла", "Сверло", MachineType.Turning, () => new TurningDrillingTool(0101, DrillingTool.Types.Solid, 10, 140),
                Position(MachineType.Turning),
                Select<TurningDrillingTool, DrillingTool.Types>("Тип", DrillTypes, t => t.Type, (t, v) => t.Type = v),
                Diameter<TurningDrillingTool>(t => t.Diameter, (t, v) => t.Diameter = v),
                Angle<TurningDrillingTool>("Угол", t => t.Angle, (t, v) => t.Angle = v, NumberKind.HalfAngle),
                Hand()),
            Of(
                "Метчики", "Метчик", MachineType.Turning, () => new TurningTappingTool(0101, TappingTool.Types.Cutting, 8, 1.25, ThreadStandard.Metric),
                TappingFields<TurningTappingTool>(MachineType.Turning)),
            Of(
                "Специальные", "Специальный инструмент", MachineType.Turning, () => new TurningSpecialTool(0101, string.Empty),
                Position(MachineType.Turning),
                new TextField("Описание", t => t.Name ?? string.Empty, (t, v) => t.Name = v)),
        };
    }
}
