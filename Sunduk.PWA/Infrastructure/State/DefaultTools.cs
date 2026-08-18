using System.Collections.Generic;
using Sunduk.PWA.Infrastructure.Sequences;
using Sunduk.PWA.Infrastructure.Tools.Base;
using Sunduk.PWA.Infrastructure.Tools.Milling;
using Sunduk.PWA.Infrastructure.Tools.Turning;
using Sunduk.PWA.Infrastructure.Tools.Turning.Base;

namespace Sunduk.PWA.Infrastructure.State
{
    /// <summary>
    /// Наборы инструментов по умолчанию для встроенных станков (MachineRegistry.SeedBuiltInMachines) —
    /// вынесены в отдельный файл, чтобы можно было легко поднастроить состав "из коробки" отдельно
    /// от остальной логики реестра станков.
    /// </summary>
    public static class DefaultTools
    {
        public static List<Tool> Turning() => new()
        {
            new GroovingExternalTool(1212, GroovingExternalTool.Types.Grooving, 1.5, TurningGroovingTool.Point.Left),
            new GroovingExternalTool(1212, GroovingExternalTool.Types.Cutting, 3, TurningGroovingTool.Point.Right),
            new GroovingFaceTool(1212, 4, TurningGroovingTool.Point.Bottom),
            new GroovingInternalTool(1212, 20, 3, TurningGroovingTool.Point.Right),
            new GroovingInternalTool(1212, 40, 4, TurningGroovingTool.Point.Right),
            new TurningSpecialTool(0101, "UPOR"),
            new ThreadingExternalTool(1111, 1.5, 1),
            new ThreadingInternalTool(1111, 16, 1.5, 1),
            new TurningDrillingTool(0505, DrillingTool.Types.Rapid, 5, 120),
            new TurningDrillingTool(0707, DrillingTool.Types.Solid, 10, 140),
            new TurningDrillingTool(0606, DrillingTool.Types.Insert, 25, 180),
            new TurningExternalTool(0202, TurningExternalTool.Types.Bar, 55, 0.8),
            new TurningExternalTool(0202, TurningExternalTool.Types.Face, 100, 0.8),
            new TurningExternalTool(0303, TurningExternalTool.Types.Bar, 35, 0.4),
            new TurningInternalTool(0202, 25, 55, 0.8),
            new TurningInternalTool(0202, 32, 80, 0.8),
            new TurningInternalTool(0303, 10, 55, 0.4),
            new TurningTappingTool(0909, TappingTool.Types.Cutting, 12, 1.75, ThreadStandard.Metric),
            new TurningTappingTool(0808, TappingTool.Types.Forming, 10, 1.5, ThreadStandard.Metric),
        };

        public static List<Tool> Milling() => new()
        {
            new MillingBoreTool(05, 16, 50, 0.2),
            new MillingChamferTool(05, 12, 45, 1),
            new MillingChamferTool(11, 16, 45, 1),
            new MillingChamferTool(16, 6, 45),
            new MillingDrillingTool(13, DrillingTool.Types.Solid, 11.2, 140),
            new MillingSpecialTool(24, "BLUM"),
            new MillingThreadCuttingTool(14, 9.5, ThreadStandard.NPT, 1.411),
            new MillingTool(04, MillingTool.Types.Insert, 63, 5),
            new MillingTool(12, MillingTool.Types.Insert, 32, 3),
            new MillingTool(20, MillingTool.Types.Insert, 80, 6),
        };
    }
}
