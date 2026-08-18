using Sunduk.PWA.Infrastructure.CAM;
using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Time;
using Sunduk.PWA.Infrastructure.Tools.Turning;
using System;

namespace Sunduk.PWA.Infrastructure.Sequences.Turning
{
    public class FinishFacingCycleSequence : Sequence
    {
        public Sequence RoughSequence { get; set; }
        public TurningExternalTool Tool { get; set; }
        public override OperationTime MachineTime
        {
            get
            {
                double cuttingTime = 0;
                double rapidTime = 5;

                double startX;
                double endX;

                switch (RoughSequence)
                {
                    case RoughFacingSequence roughFacingSequence:
                        startX = roughFacingSequence.ExternalDiameter;
                        endX = roughFacingSequence.InternalDiameter;
                        break;
                    case RoughFacingCycleSequence roughFacingCycleSequence:
                        startX = roughFacingCycleSequence.ExternalDiameter;
                        endX = roughFacingCycleSequence.InternalDiameter;
                        break;
                    case FacingSequence facingSequence:
                        startX = facingSequence.ExternalDiameter;
                        endX = facingSequence.InternalDiameter;
                        break;
                    default: return new OperationTime(0, 0);
                }
                var feedFinish = FeedFinish;
                var speedFinish = SpeedFinish;
                var spinsFinish = (speedFinish * 1000) / (Math.PI * ((startX - endX) / 2));
                cuttingTime += Calc.CrossTurningTime(startX, endX, spinsFinish, feedFinish);
                rapidTime += Calc.CrossRapidTime(startX, endX);

                return new OperationTime(cuttingTime, rapidTime);
            }
        }
        public int SpeedFinish { get; set; }
        public double FeedFinish { get; set; }
        public Material Material {
            get
            {
                return RoughSequence switch
                {
                    RoughFacingSequence roughFacingSequence => roughFacingSequence.Material,
                    RoughFacingCycleSequence roughFacingCycleSequence => roughFacingCycleSequence.Material,
                    FacingSequence facingSequence => facingSequence.Material,
                    _ => Material.Stainless
                };
            }

        }

        public override string Operation => Templates.FacingOperation.FinishFacingCycle(Tool, RoughSequence, SpeedFinish, FeedFinish);

        public override MachineType MachineType => MachineType.Turning;
        public override string Name => $"Торцовка чистовая (G70)";

        public FinishFacingCycleSequence(
            TurningExternalTool tool, 
            Sequence roughSequence, 
            int speedFinish, 
            double feedFinish)
        {
            RoughSequence = roughSequence;
            Tool = tool;
            SpeedFinish = speedFinish;
            FeedFinish = feedFinish;
        }
    }
}
