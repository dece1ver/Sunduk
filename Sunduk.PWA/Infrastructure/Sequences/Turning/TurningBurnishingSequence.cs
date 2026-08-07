using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Time;
using Sunduk.PWA.Infrastructure.Tools.Turning;
using Sunduk.PWA.Infrastructure.Tools.Turning.Base;
using System;

namespace Sunduk.PWA.Infrastructure.Sequences.Turning
{
    public class TurningBurnishingSequence : Sequence
    {
        public Machine Machine { get; set; }
        public TurningBurnishingTool Tool { get; set; }
        public double Diameter { get; set; }
        public double StartZ { get; set; }
        public double EndZ { get; set; }
        public int SpeedFinish { get; set; }
        public double FeedFinish { get; set; }
        public override string Name {
            get
            {
                var name = string.Empty;
                switch (Tool)
                {
                    case TurningExternalBurnishingTool:
                        name += "Наружнее накатывание ";
                        break;
                    case TurningInternalBurnishingTool:
                        name += "Внутреннее накатывание ";
                        break;
                    default:
                        name += "Накатывание ";
                        break;
                }
                name += Tool.Type is TurningBurnishingTool.Types.Diamond ? " алмазом" : " роликом";
                return name;
            }
        }
        public override string Operation => Templates.Operation.BurnishingOperation(Machine, Tool, Diameter, StartZ, EndZ);
        public override OperationTime MachineTime
        {
            get
            {
                double cuttingTime = 0;
                double rapidTime = 5;
                var fullLength = (Math.Abs(StartZ) + Math.Abs(EndZ));
                var additionalRapidMove = Tool is TurningExternalBurnishingTool ? 1 : 1 + fullLength;
                var speed = SpeedFinish;
                var feed = FeedFinish;
                var spins = (speed * 1000) / (Math.PI * Diameter);
                if (spins > 1000) spins = 1000;
                cuttingTime += (fullLength + 1).AxialTurningTime(spins, feed);
                cuttingTime += Templates.Operation.Escaping().AxialTurningTime(spins, feed);
                rapidTime += fullLength.AxialRapidTime();
                rapidTime += additionalRapidMove.AxialRapidTime();
                return new OperationTime(cuttingTime, rapidTime);
            }
        }
        public override MachineType MachineType => MachineType.Turning;

        public TurningBurnishingSequence(Machine machine, TurningBurnishingTool tool, double diameter, double startZ, double endZ, int speedFinish, double feedFinish)
        {
            Machine = machine;
            Tool = tool;
            Diameter = diameter;
            StartZ = startZ;
            EndZ = endZ;
            SpeedFinish = speedFinish;
            FeedFinish = feedFinish;
        }
    }
}
