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
    public abstract class PeckDrillingSequence : Sequence
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
        public override OperationTime MachineTime
        {
            get
            {
                double cuttingTime = 0;
                double rapidTime = 5;
                var fullLength = (Math.Abs(EndZ) + Math.Abs(StartZ));
                var steps = (int)Math.Round(fullLength / Depth, MidpointRounding.ToPositiveInfinity);
                var feed = Feed;
                var speed = Speed;
                var spins = (speed * 1000) / (Math.PI * Tool.Diameter);
                if (spins > 3000) spins = 3000;
                var stepLength = Depth + Templates.Operation.Escaping();
                if (stepLength > fullLength)
                {
                    stepLength = fullLength;
                    steps = 1;
                }
                if (steps > 2) steps -= 1;

                var lastStep = fullLength - steps * Depth + Templates.Operation.Escaping();

                // время резания
                cuttingTime += steps * stepLength.AxialTurningTime(spins, feed) +
                               lastStep.AxialTurningTime(spins, feed);
                // время ввода/вывода сверла
                if (steps > 1) steps++;
                rapidTime += steps * Templates.Operation.Escaping().AxialRapidTime();
                rapidTime += fullLength.AxialRapidTime();
                return new OperationTime(cuttingTime, rapidTime);
            }
        }
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
