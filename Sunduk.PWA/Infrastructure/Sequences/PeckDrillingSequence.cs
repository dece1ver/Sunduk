using Sunduk.PWA.Infrastructure.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.Infrastructure.Sequences
{
    public class PeckDrillingSequence : Sequence
    {
        public Machines Machine { get; set; }
        public Materials Material { get; set; }
        public DrillingTool Tool { get; set; }
        public double Depth { get; set; }
        public double StartZ { get; set; }
        public double EndZ { get; set; }
        public override string Operation => Templates.Operation.PeckDrilling(Machine, Material, Tool, Depth, StartZ, EndZ);

        public PeckDrillingSequence(Machines machine, Materials material, DrillingTool tool, double depth, double startZ, double endZ)
        {
            Machine = machine;
            Material = material;
            Tool = tool;
            Depth = depth;
            StartZ = startZ;
            EndZ = endZ;
        }
    }
}
