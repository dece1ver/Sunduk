using Sunduk.PWA.Infrastructure.Time;
using Sunduk.PWA.Infrastructure.Tools.Base;
using System;

namespace Sunduk.PWA.Infrastructure.Sequences.Base
{
    public class HighSpeedDrillingSequence : Sequence
    {
        public Machine Machine { get; set; }
        public Material Material { get; set; }
        public DrillingTool Tool { get; set; }
        public double StartZ { get; set; }
        public double EndZ { get; set; }
        public int Speed { get; set; }
        public double Feed { get; set; }
        public override string Name => $"Высокоскоростное сверление Ø{Tool.Diameter.NC(option: Util.NcDecimalPointOption.Without)} L-{Math.Abs(EndZ).NC(option: Util.NcDecimalPointOption.Without)} ({Tool.Type.Description()})";
        public override OperationTime MachineTime => this.OperationTime(Material);
        public HighSpeedDrillingSequence(Machine machine, Material material, DrillingTool tool, double startZ, double endZ, int speed, double feed)
        {
            Machine = machine;
            Material = material;
            Tool = tool;
            StartZ = startZ;
            EndZ = endZ;
            Speed = speed;
            Feed = feed;
        }
    }
}
