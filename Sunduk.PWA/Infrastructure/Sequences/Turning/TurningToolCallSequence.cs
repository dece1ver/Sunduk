using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Sequences.Turning
{
    public class MillingToolCallSequence : ToolCallSequence
    {
        public override string Operation => Templates.Operation.ToolCall(Machine, Tool);
        public override MachineType MachineType => MachineType.Turning;

        public MillingToolCallSequence(Machine machine, Tool tool)
            : base(machine, tool)
        {

        }
    }
}
