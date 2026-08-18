using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure
{
    /// <summary>
    /// Профиль станка: набор G/M-кодов и структурных настроек, которыми параметризуются
    /// методы генерации УП в Infrastructure/Templates/*.cs. Раньше это был фиксированный enum
    /// на 3 значения со сплошными switch(machine) в каждом методе — теперь произвольный станок
    /// можно завести через редактор в приложении, без правки кода.
    /// </summary>
    public class Machine
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string Name { get; set; } = string.Empty;
        public MachineType MachineType { get; set; }

        /// <summary>
        /// Встроенный станок (L230A/GS1500/A110) — нельзя удалить или сменить Id, можно только
        /// дублировать в новый редактируемый профиль.
        /// </summary>
        public bool IsBuiltIn { get; set; }

        /// <summary>
        /// Системы координат, которыми пользуется станок (обычно одна; больше одной — например
        /// у станков с противошпинделем, G54/G55 на разные шпиндели). При больше чем одной СК
        /// в редакторе перехода появляется выбор, какая из них используется в этом переходе.
        /// </summary>
        public List<CoordinateSystem> CoordinateSystems { get; set; } = new() { CoordinateSystem.G54 };

        /// <summary>
        /// Приёмник старого поля DefaultCoordinateSystem при десериализации станков, сохранённых
        /// до появления CoordinateSystems — используется только для миграции в MachineRegistry.
        /// </summary>
        [JsonPropertyName("DefaultCoordinateSystem")]
        public CoordinateSystem? LegacyDefaultCoordinateSystem { get; set; }

        public string TailstockOnCode { get; set; } = string.Empty;
        public string TailstockOffCode { get; set; } = string.Empty;
        public string SpindleClampCode { get; set; } = string.Empty;
        public string SpindleUnclampCode { get; set; } = string.Empty;

        public string CoolantOnCode { get; set; } = string.Empty;
        public string CoolantOffCode { get; set; } = string.Empty;
        public string? CoolantThroughOnCode { get; set; }
        public string? CoolantThroughOffCode { get; set; }
        public string? CoolantFullOffCode { get; set; }
        public string? CoolantBlowOnCode { get; set; }
        public string? CoolantBlowOffCode { get; set; }

        /// <summary>
        /// Возврат в референтную точку (<see cref="Templates.Operation.TurningReferentPoint"/>)
        /// в начале токарного перехода, перед вызовом инструмента.
        /// </summary>
        public bool LeadingReferentPoint { get; set; }

        /// <summary>
        /// Возврат в референтную точку в конце токарного перехода.
        /// </summary>
        public bool TrailingReferentPoint { get; set; } = true;

        public HeaderStyle HeaderStyle { get; set; }

        /// <summary>
        /// Необязательные дополнительные строки шапки программы — например автоматическая
        /// установка привязки детали (G10 L2 ...) или другие параметры ЧПУ, специфичные для
        /// станка. Вставляются как есть, построчно, после строки(строк) с номером программы.
        /// </summary>
        public string HeaderExtraLines { get; set; } = string.Empty;

        /// <summary>
        /// Только для одноразовой миграции в <see cref="HeaderTemplate"/> (см.
        /// MachineRegistry.MigrateHeaderTemplate) — больше нигде не читается и не редактируется.
        /// </summary>
        public string HeaderTrailerTemplate { get; set; } = string.Empty;

        /// <summary>
        /// Единый шаблон "хвоста" шапки программы — построчно, как есть. Плейсхолдеры: {AUTHOR}
        /// (поле "Автор" в наладке — если пусто, схлопывается вместе со своими скобками, не
        /// оставляя "()"), {DATE} (сегодняшняя дата), {MACHINE_TIME} (машинное время всей
        /// программы, "(nMmS)" — раньше была безусловной жёстко зашитой строкой, теперь обычный
        /// плейсхолдер, как и остальные). Плейсхолдер, которого нет в шаблоне — не выводится; если
        /// из-за пустой подстановки строка целиком схлопывается — эта строка пропускается (см.
        /// Util.RenderTemplate). HeaderStyle (формат номера программы) — отдельное поле, этот
        /// шаблон его не затрагивает.
        /// </summary>
        public string HeaderTemplate { get; set; } = string.Empty;

        /// <summary>
        /// Необязательный G-код, выполняемый перед первым переходом на станке — построчно, как
        /// есть (та же механика, что и у <see cref="HeaderExtraLines"/>). Поддерживает плейсхолдеры
        /// {CS} (система координат, см. <see cref="CoordinateSystems"/>) и {S} (ограничение оборотов
        /// с учётом <see cref="SafetySpeedCap"/>/<see cref="SafetyDefaultSpeed"/>). Пусто — строка
        /// безопасности не добавляется.
        /// </summary>
        public string SafetyStringTemplate { get; set; } = string.Empty;
        public int SafetySpeedCap { get; set; }
        public int SafetyDefaultSpeed { get; set; }

        /// <summary>
        /// Только для одноразовой миграции в <see cref="TransitionTemplate"/> (см.
        /// MachineRegistry.MigrateTransitionTemplate) — больше нигде не читается и не
        /// редактируется.
        /// </summary>
        public string ToolCallTemplate { get; set; } = string.Empty;

        /// <summary>
        /// Шаблон перехода — тело вызова инструмента, построчно, как есть. Плейсхолдеры: {TOOL}
        /// (название и параметры инструмента), {T} (номер инструмента без дополнения нулями), {TN}
        /// — номер инструмента с N-значным дополнением нулями (N — любое число, например {T2},
        /// {T4}), {CS} (система координат перехода, см. <see cref="CoordinateSystems"/> — пусто,
        /// если у станка только одна СК), {COOLANT} (код включения СОЖ перехода — пусто при типе
        /// СОЖ «Без СОЖ»). Дополнительно, ТОЛЬКО для переходов со свободным телом (произвольное
        /// точение/фрезерование, точение по контуру): {PROCESSING} — тело обработки (пусто ⇒
        /// стандартная метка "(---OBRABOTKA---)"), {MACHINE_TIME} — машинное время перехода
        /// ("(nMmS)"), {COOLANT_OFF} — код выключения СОЖ перехода. У циклов с фиксированным телом
        /// (подрезка, сверление, канавки, резьба, обкатывание) эти три плейсхолдера, если
        /// присутствуют в шаблоне, просто не выводятся — тело/время/выключение СОЖ таких переходов
        /// формирует сам цикл, не шаблон. Плейсхолдер, которого нет в шаблоне — не выводится; если
        /// строка из-за пустой подстановки схлопывается целиком — эта строка пропускается (см.
        /// Util.RenderTemplate) — отдельного "запасного" вывода {CS}/{COOLANT} по станку больше
        /// нет, гарантия их появления обеспечивается тем, что миграция/сид всегда прописывают эти
        /// плейсхолдеры в самом шаблоне. Пусто — выводится просто номер инструмента ({T}).
        /// </summary>
        public string TransitionTemplate { get; set; } = string.Empty;

        /// <summary>
        /// Инструменты этого станка — своя оснастка на каждый станок, а не общая на всё
        /// приложение.
        /// </summary>
        public List<Tool> Tools { get; set; } = new();

        public override bool Equals(object? obj) => obj is Machine other && other.Id == Id;
        public override int GetHashCode() => Id.GetHashCode();
    }
}
