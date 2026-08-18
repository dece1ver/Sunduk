using Sunduk.PWA.Infrastructure.Sequences.Turning;
using Sunduk.PWA.Infrastructure.Tools.Turning.Base;

namespace Sunduk.PWA.Infrastructure.Sequences.Factories
{
    public static class BurnishingSequenceFactory
    {
        public static TurningBurnishingSequence CreateTurningBurnishing(Machine machine, CoordinateSystem coordinateSystem, TurningBurnishingTool tool, double diameter, double startZ, double endZ, int speedFinish, double feedFinish, Coolant coolant = Coolant.General)
            => new(machine, coordinateSystem, tool, diameter, startZ, endZ, speedFinish, feedFinish) { Coolant = coolant };
    }
}
