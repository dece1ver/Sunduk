using Sunduk.PWA.Infrastructure.Tools;
using Sunduk.PWA.Infrastructure.Tools.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.Infrastructure.Sequences
{
    public class LimiterSequence : Sequence
    {
        public Machine Machine { get; set; }
        public Tool Tool { get; set; }
        public double ExternalDiameter { get; set; }
        public override string Operation => Templates.Operation.Limiter(Machine, Tool, ExternalDiameter);
        public override MachineType MachineType { get => MachineType.Turning; }

        public LimiterSequence(Machine machine, Tool tool, double externalDiameter)
        {
            Machine = machine;
            Tool = tool;
            ExternalDiameter = externalDiameter;
        }
    }
}
