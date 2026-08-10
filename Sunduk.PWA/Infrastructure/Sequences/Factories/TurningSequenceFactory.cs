using System.Collections.Generic;
using Sunduk.PWA.Infrastructure.Sequences.ContourElements.Base;
using Sunduk.PWA.Infrastructure.Sequences.Turning;
using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Sequences.Factories
{
    public static class TurningSequenceFactory
    {
        public static RoughTurningSequence CreateRoughTurning(Machine machine, CoordinateSystem coordinateSystem, Material material, TurningTool tool, List<Element> contour, double stepOver, double roughStockAllow, double profStockAllow, int speedRough, double feedRough, Coolant coolant = Coolant.General)
            => new(machine, material, tool, contour, stepOver, roughStockAllow, profStockAllow, speedRough, feedRough) { CoordinateSystem = coordinateSystem, Coolant = coolant };

        public static FinishTurningSequence CreateFinishTurning(Machine machine, CoordinateSystem coordinateSystem, Material material, TurningTool tool, List<Element> contour, double stepOver, double roughStockAllow, double profStockAllow, int speedFinish, double feedFinish, Coolant coolant = Coolant.General)
            => new(machine, material, tool, contour, stepOver, roughStockAllow, profStockAllow, speedFinish, feedFinish) { CoordinateSystem = coordinateSystem, Coolant = coolant };
    }
}
