using System.Collections.Generic;
using Sunduk.PWA.Infrastructure.Sequences.ContourElements.Base;
using Sunduk.PWA.Infrastructure.Sequences.Turning;
using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Sequences.Factories
{
    public static class TurningSequenceFactory
    {
        public static RoughTurningSequence CreateRoughTurning(Machine machine, Material material, TurningTool tool, List<Element> contour, double stepOver, double roughStockAllow, double profStockAllow, int speedRough, double feedRough)
            => new(machine, material, tool, contour, stepOver, roughStockAllow, profStockAllow, speedRough, feedRough);

        public static FinishTurningSequence CreateFinishTurning(Machine machine, Material material, TurningTool tool, List<Element> contour, double stepOver, double roughStockAllow, double profStockAllow, int speedFinish, double feedFinish)
            => new(machine, material, tool, contour, stepOver, roughStockAllow, profStockAllow, speedFinish, feedFinish);
    }
}
