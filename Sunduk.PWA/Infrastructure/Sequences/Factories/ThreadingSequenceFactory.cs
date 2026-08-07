using System.Collections.Generic;
using Sunduk.PWA.Infrastructure.CAM;
using Sunduk.PWA.Infrastructure.Sequences.Milling;
using Sunduk.PWA.Infrastructure.Sequences.Turning;
using Sunduk.PWA.Infrastructure.Tools.Milling;
using Sunduk.PWA.Infrastructure.Tools.Turning;
using Sunduk.PWA.Infrastructure.Tools.Turning.Base;

namespace Sunduk.PWA.Infrastructure.Sequences.Factories
{
    public static class ThreadingSequenceFactory
    {
        public static ThreadCuttingSequence CreateThreadCutting(Machine machine, ThreadingTool tool, ThreadStandard threadStandard, CuttingType type, double threadDiameter, double threadPitch, double startZ, double endZ, double threadNptPlane, int speed)
            => new(machine, tool, threadStandard, type, threadDiameter, threadPitch, startZ, endZ, threadNptPlane, speed);

        public static TurningTappingSequence CreateTurningTapping(Machine machine, TurningTappingTool tool, double cutSpeed, double startZ, double endZ)
            => new(machine, tool, cutSpeed, startZ, endZ);

        public static MillingTappingSequence CreateMillingTapping(Machine machine, CoordinateSystem coordinateSystem, MillingTappingTool tool, double cutSpeed, double startZ, double endZ, List<Hole> holes, bool polar, double safePlane)
            => new(machine, coordinateSystem, tool, cutSpeed, startZ, endZ, holes, polar, safePlane);

        public static ThreadMillingNormalSequence CreateThreadMilling(Machine machine, CoordinateSystem coordinateSystem, MillingThreadCuttingTool tool, double threadDiameter, double cutSpeed, double cutFeed, double startZ, double endZ, int roughPasses, double roughStepOver, double profStockAllow, double exitPlane, bool fullCut, List<Hole> holes, bool polar, double safePlane)
            => new(machine, coordinateSystem, tool, threadDiameter, cutSpeed, cutFeed, startZ, endZ, roughPasses, roughStepOver, profStockAllow, exitPlane, fullCut, holes, polar, safePlane);

        public static ThreadMillingCustomSequence CreateCustomThreadMilling(Machine machine, CoordinateSystem coordinateSystem, MillingThreadCuttingTool tool, double threadDiameter, double cutSpeed, double cutFeed, double startZ, double endZ, int roughPasses, double roughStepOver, double profStockAllow, double exitPlane, bool fullCut, List<Hole> holes, bool polar, double safePlane)
            => new(machine, coordinateSystem, tool, threadDiameter, cutSpeed, cutFeed, startZ, endZ, roughPasses, roughStepOver, profStockAllow, exitPlane, fullCut, holes, polar, safePlane);
    }
}
