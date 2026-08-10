using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Sequences.Milling;
using Sunduk.PWA.Infrastructure.Sequences.Turning;
using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Sequences.Factories
{
    public static class GeneralSequenceFactory
    {
        public static LimiterSequence CreateLimiter(Machine machine, CoordinateSystem coordinateSystem, Tool tool, double externalDiameter)
            => new(machine, coordinateSystem, tool, externalDiameter);

        public static TurningCustomSequence CreateTurningCustom(Machine machine, CoordinateSystem coordinateSystem, Tool tool, string customOperation, Coolant coolant = Coolant.General)
            => new(machine, coordinateSystem, tool, customOperation) { Coolant = coolant };

        public static MillingCustomSequence CreateMillingCustom(Machine machine, CoordinateSystem coordinateSystem, Tool tool, string customOperation, Coolant coolant, bool polar, double safePlane)
            => new(machine, coordinateSystem, tool, customOperation, coolant, polar, safePlane);

        public static StopSequence CreateStop(bool optional, string comment)
            => new(optional, comment);

        public static TailstockOnSequence CreateTailstockOn(Machine machine)
            => new(machine);

        public static TailstockOffSequence CreateTailstockOff(Machine machine)
            => new(machine);
    }
}
