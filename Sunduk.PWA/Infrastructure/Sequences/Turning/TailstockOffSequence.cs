using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Sequences.Turning
{
    public class TailstockOffSequence : Sequence
    {
        public override string Operation => Templates.Operation.TailstockOff(Machine);
        public override MachineType MachineType => MachineType.Turning;
        public override double MachineTime => this.OperationTime();
        public Machine Machine { get; set; }

        public TailstockOffSequence(Machine machine)
        {
            Machine = machine;
        }
    }
}
