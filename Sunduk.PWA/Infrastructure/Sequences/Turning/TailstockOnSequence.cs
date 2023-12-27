using Microsoft.Extensions.Options;
using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Time;
using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Sequences.Turning
{
    public class TailstockOnSequence : Sequence
    {
        public override string Operation => Templates.Operation.TailstockOn(Machine);
        public override string Name => "Подвод задней бабки";
        public override MachineType MachineType => MachineType.Turning;
        public override OperationTime MachineTime => this.OperationTime();
        public Machine Machine { get; set; }

        public TailstockOnSequence(Machine machine)
        {
            Machine = machine;
        }
    }
}