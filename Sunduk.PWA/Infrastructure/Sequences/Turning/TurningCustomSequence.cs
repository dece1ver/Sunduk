using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Sequences.Turning
{
    public class TurningCustomSequence : CustomSequence
    {
        public override string Operation => Templates.Operation.TurningCustomOperation(Machine, Tool, CustomOperation);
        public override MachineType MachineType => MachineType.Turning;

        public TurningCustomSequence(Machine machine, Tool tool, string customOperation)
            : base(machine, tool, customOperation) { }
    }
}
