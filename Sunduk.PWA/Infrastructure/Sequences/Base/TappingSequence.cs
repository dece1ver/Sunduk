using Sunduk.PWA.Infrastructure.Tools.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.Infrastructure.Sequences.Base
{
    public class TappingSequence : Sequence
    {
        public Machine Machine { get; set; }
        public TappingTool Tool { get; set; }
        public double CutSpeed { get; set; }
        public double StartZ { get; set; }
        public double EndZ { get; set; }

        public TappingSequence(Machine machine, TappingTool tool, double cutSpeed, double startZ, double endZ)
        {
            Machine = machine;
            Tool = tool;
            CutSpeed = cutSpeed;
            StartZ = startZ;
            EndZ = endZ;
        }
    }
}
