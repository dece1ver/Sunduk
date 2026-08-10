using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Time;
using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Sequences.Turning
{
    public class TurningCustomSequence : CustomSequence
    {
        public CoordinateSystem CoordinateSystem { get; set; }
        public override string Operation => Templates.Operation.TurningCustomOperation(Machine, CoordinateSystem, Tool, CustomOperation, Coolant);
        public override MachineType MachineType => MachineType.Turning;

        public TurningCustomSequence(Machine machine, CoordinateSystem coordinateSystem, Tool tool, string customOperation)
            : base(machine, tool, customOperation)
        {
            CoordinateSystem = coordinateSystem;
        }
    }
}
