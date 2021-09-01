using Sunduk.PWA.Infrastructure.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.Infrastructure.Sequences
{
    public class LimiterSequence : Sequence
    {
        public Machines Machine { get; set; }
        public Tool Tool { get; set; }
        public double ExternalDiameter { get; set; }
        public override string Operation => Templates.Operation.Limiter(Machine, Tool, ExternalDiameter);

        public LimiterSequence(Machines machine, Tool tool, double externalDiameter)
        {
            Machine = machine;
            Tool = tool;
            ExternalDiameter = externalDiameter;
        }
    }
}
