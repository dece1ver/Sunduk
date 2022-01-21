using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Sequences.Base
{
    public class ToolCallSequence : Sequence
    {
        public Machine Machine { get; set; }
        public Tool Tool { get; set; }

        public ToolCallSequence(Machine machine, Tool tool)
        {
            Machine = machine;
            Tool = tool;
        }
    }
}
