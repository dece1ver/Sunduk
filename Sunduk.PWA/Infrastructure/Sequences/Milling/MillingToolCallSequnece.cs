using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Sequences.Milling
{
    public class MillingToolCallSequence : ToolCallSequence
    {
        public Coolant Coolant { get; set; }
        public bool Polar { get; set; }
        public override string Operation => Templates.Operation.MillingToolCall(Machine, Tool, Coolant, Polar);
        public override MachineType MachineType => MachineType.Milling;

        public MillingToolCallSequence(Machine machine, Tool tool, Coolant coolant, bool polar) 
            :base(machine, tool)
        {
            Coolant = coolant;
            Polar = polar;
        }
    }
}
