using Sunduk.PWA.Infrastructure.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.Infrastructure.Sequences
{
    public class HighSpeedDrillingSequence : Sequence
    {
        public Machines Machine { get; set; }
        public Materials Material { get; set; }
        public DrillingTool Tool { get; set; }
        public double StartZ { get; set; }
        public double EndZ { get; set; }
        public override string Operation => Templates.Operation.HighSpeedDrilling(Machine, Material, Tool, StartZ, EndZ);

        public HighSpeedDrillingSequence(Machines machine, Materials material, DrillingTool tool, double startZ, double endZ)
        {
            Machine = machine;
            Material = material;
            Tool = tool;
            StartZ = startZ;
            EndZ = endZ;
        }
    }
}
