using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Sequences.Milling
{
    public class MillingCustomSequence : CustomSequence
    {
        public Coolant Coolant { get; set; }
        public bool Polar { get; set; }
        public override string Operation => Templates.Operation.MillingCustomOperation(Machine, Tool, CustomOperation, Coolant, Polar);
        public override MachineType MachineType => MachineType.Milling;

        public MillingCustomSequence(Machine machine, Tool tool, string customOperation, Coolant coolant, bool polar)
            : base(machine, tool, customOperation)
        {
            Coolant = coolant;
            Polar = polar;
        }
    }
}
