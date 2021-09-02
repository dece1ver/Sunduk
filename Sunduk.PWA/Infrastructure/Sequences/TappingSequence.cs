using Sunduk.PWA.Infrastructure.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.Infrastructure.Sequences
{
    public class TappingSequence : Sequence
    {
        public Machines Machine { get; set; }
        public TappingTool Tool { get; set; }
        public double CutSpeed { get; set; }
        public double StartZ { get; set; }
        public double EndZ { get; set; }
        public override string Operation => Templates.Operation.Tapping(Machine, Tool, CutSpeed, StartZ, EndZ);

        public TappingSequence(Machines machine, TappingTool tool, double cutSpeed, double startZ, double endZ)
        {
            Machine = machine;
            Tool = tool;
            CutSpeed = cutSpeed;
            StartZ = startZ;
            EndZ = endZ;
        }
    }
}
