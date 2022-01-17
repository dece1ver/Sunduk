using Sunduk.PWA.Infrastructure.Tools;
using Sunduk.PWA.Infrastructure.Tools.Turning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.Infrastructure.Sequences
{
    public class HighSpeedDrillingSequence : Sequence
    {
        public Machine Machine { get; set; }
        public Material Material { get; set; }
        public TurningDrillingTool Tool { get; set; }
        public double StartZ { get; set; }
        public double EndZ { get; set; }
        public override string Operation => Templates.Operation.HighSpeedDrilling(Machine, Material, Tool, StartZ, EndZ);
        public override MachineType MachineType { get => MachineType.General; }

        public HighSpeedDrillingSequence(Machine machine, Material material, TurningDrillingTool tool, double startZ, double endZ)
        {
            Machine = machine;
            Material = material;
            Tool = tool;
            StartZ = startZ;
            EndZ = endZ;
        }
    }
}
