using Sunduk.PWA.Infrastructure.Time;
using Sunduk.PWA.Infrastructure.Tools;
using Sunduk.PWA.Infrastructure.Tools.Base;
using Sunduk.PWA.Infrastructure.Tools.Turning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.Infrastructure.Sequences.Base
{
    public class PeckDrillingSequence : Sequence
    {
        public Machine Machine { get; set; }
        public Material Material { get; set; }
        public DrillingTool Tool { get; set; }
        public double Depth { get; set; }
        public double StartZ { get; set; }
        public double EndZ { get; set; }
        public int Speed { get; set; }
        public double Feed { get; set; }
        public override string Name => $"Прерывистое сверление Ø{Tool.Diameter.NC(option: Util.NcDecimalPointOption.Without)} L-{Math.Abs(EndZ).NC(option: Util.NcDecimalPointOption.Without)} ({Tool.Type.Description()})";
        public override OperationTime MachineTime => this.OperationTime();
        public PeckDrillingSequence(Machine machine, Material material, DrillingTool tool, double depth, double startZ, double endZ, int speed, double feed)
        {
            Machine = machine;
            Material = material;
            Tool = tool;
            Depth = depth;
            StartZ = startZ;
            EndZ = endZ;
            Speed = speed;
            Feed = feed;
        }
    }
}
