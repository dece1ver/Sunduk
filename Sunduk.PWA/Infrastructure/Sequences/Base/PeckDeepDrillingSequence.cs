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
    public abstract class PeckDeepDrillingSequence : Sequence
    {
        public Machine Machine { get; set; }
        public Material Material { get; set; }
        public DrillingTool Tool { get; set; }
        public double Depth { get; set; }
        public double StartZ { get; set; }
        public double EndZ { get; set; }
        public int Speed { get; set; }
        public double Feed { get; set; }
        public override string Name => $"Глубокое сверление Ø{Tool.Diameter.NC(option: Util.NcDecimalPointOption.Without)} L-{Math.Abs(EndZ).NC(option: Util.NcDecimalPointOption.Without)} ({Tool.Type.Description()})";
        public override OperationTime MachineTime
        {
            get
            {
                double cuttingTime = 0;
                double rapidTime = 5;
                var fullLength = (Math.Abs(EndZ) + Math.Abs(StartZ));
                var steps = (int)Math.Round(fullLength / Depth, MidpointRounding.ToPositiveInfinity);
                var stepLength = Depth + Templates.Operation.Escaping();
                if (stepLength > fullLength)
                {
                    stepLength = fullLength;
                    steps = 1;
                }

                if (steps > 2) steps -= 1;

                var lastStep = fullLength - steps * Depth + Templates.Operation.Escaping();
                var feed = Feed;
                var speed = Speed;
                var spins = (speed * 1000) / (Math.PI * Tool.Diameter);
                if (spins > 3000) spins = 3000;
                double currentLength = 0;
                // время резания
                cuttingTime += steps * stepLength.AxialTurningTime(spins, feed) +
                              lastStep.AxialTurningTime(spins, feed);
                // время ввода/вывода сверла
                if (steps > 1) steps++;
                for (var i = 0; i < steps; i++)
                {
                    currentLength += stepLength;
                    rapidTime += 2 * currentLength.AxialRapidTime();
                }

                return new OperationTime(cuttingTime, rapidTime);
            }
        }
        public PeckDeepDrillingSequence(Machine machine, Material material, DrillingTool tool, double depth, double startZ, double endZ, int speed, double feed)
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
