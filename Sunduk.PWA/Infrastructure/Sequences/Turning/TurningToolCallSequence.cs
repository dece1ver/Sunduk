using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Sequences.Turning
{
    public class TurningToolCallSequence : ToolCallSequence
    {
        public override string Operation => Templates.Operation.TurningToolCall(Machine, Tool);
        public override MachineType MachineType => MachineType.Turning;

        public TurningToolCallSequence(Machine machine, Tool tool)
            : base(machine, tool)
        {

        }
    }
}
