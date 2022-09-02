using Sunduk.PWA.Infrastructure.Time;

namespace Sunduk.PWA.Infrastructure.Sequences.Base
{
    public class StopSequence : Sequence
    {
        public StopSequence(bool optional, string comment)
        {
            Optional = optional;
            Comment = comment.Translate();
        }

        public bool Optional { get; set; }
        public string Comment { get; set; }
        public override string Operation => Templates.Operation.Stop(Optional, Comment);
        public override string Name => Optional ? "Опциональная остановка" : "Остановка";
        public override OperationTime MachineTime => this.OperationTime();
    }
}
