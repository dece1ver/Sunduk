using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Sequences.Base
{
    public class CustomSequence : Sequence
    {
        public Machine Machine { get; set; }
        public Tool Tool { get; set; }
        public override string Name { get => $"Вызов инструмента"; }
        public string CustomOperation { get; set; } 
        public CustomSequence(Machine machine, Tool tool, string customOperation)
        {
            Machine = machine;
            Tool = tool;
            CustomOperation = customOperation;
        }
    }
}
