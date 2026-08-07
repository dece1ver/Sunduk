using Sunduk.PWA.Infrastructure.Time;
using Sunduk.PWA.Infrastructure.Tools.Base;
using System;

namespace Sunduk.PWA.Infrastructure.Sequences.Base
{
    public abstract class HighSpeedDrillingSequence : Sequence
    {
        public Machine Machine { get; set; }
        public Material Material { get; set; }
        public DrillingTool Tool { get; set; }
        public double StartZ { get; set; }
        public double EndZ { get; set; }
        public int Speed { get; set; }
        public double Feed { get; set; }
        public override string Name => $"Высокоскоростное сверление Ø{Tool.Diameter.NC(option: Util.NcDecimalPointOption.Without)} L-{Math.Abs(EndZ).NC(option: Util.NcDecimalPointOption.Without)} ({Tool.Type.Description()})";
        public override OperationTime MachineTime
        {
            get
            {
                double cuttingTime = 0;
                double rapidTime = 5;
                var fullLength = (Math.Abs(EndZ) + Math.Abs(StartZ));
                var feed = Feed;
                var speed = Speed;
                var spins = (speed * 1000) / (Math.PI * Tool.Diameter);
                if (spins > 3000) spins = 3000;
                cuttingTime += fullLength.AxialTurningTime(spins, feed);
                rapidTime += fullLength.AxialRapidTime();
                return new OperationTime(cuttingTime, rapidTime);
            }
        }
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
