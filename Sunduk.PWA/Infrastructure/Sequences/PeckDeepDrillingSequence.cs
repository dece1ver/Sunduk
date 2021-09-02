using Sunduk.PWA.Infrastructure.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.Infrastructure.Sequences
{
    public class PeckDeepDrillingSequence : Sequence
    {
        public Machines Machine { get; set; }
        public Materials Material { get; set; }
        public DrillingTool Tool { get; set; }
        public double Depth { get; set; }
        public double StartZ { get; set; }
        public double EndZ { get; set; }
        public override string Operation => Templates.Operation.PeckDeepDrilling(Machine, Material, Tool, Depth, StartZ, EndZ);

        public PeckDeepDrillingSequence(Machines machine, Materials material, DrillingTool tool, double depth, double startZ, double endZ)
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
