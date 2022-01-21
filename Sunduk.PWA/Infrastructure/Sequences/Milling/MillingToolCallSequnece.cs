using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Sequences.Milling
{
    public class MillingToolCallSequence : ToolCallSequence
    {
        public CoolantType Coolant { get; set; }
        public bool Polar { get; set; }
        public override string Operation => Templates.Operation.ToolCall(Machine, Tool, Coolant, Polar);
        public override MachineType MachineType => MachineType.Milling;

        public MillingToolCallSequence(Machine machine, Tool tool, CoolantType coolant, bool polar) 
            :base(machine, tool)
        {
            Coolant = coolant;
            Polar = polar;
        }
    }
}
