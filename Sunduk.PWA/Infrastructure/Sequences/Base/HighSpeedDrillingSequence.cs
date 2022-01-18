using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Sequences.Base
{
    public class HighSpeedDrillingSequence : Sequence
    {
        public Machine Machine { get; set; }
        public Material Material { get; set; }
        public DrillingTool Tool { get; set; }
        public double StartZ { get; set; }
        public double EndZ { get; set; }

        public HighSpeedDrillingSequence(Machine machine, Material material, DrillingTool tool, double startZ, double endZ)
        {
            Machine = machine;
            Material = material;
            Tool = tool;
            StartZ = startZ;
            EndZ = endZ;
        }
    }
}
