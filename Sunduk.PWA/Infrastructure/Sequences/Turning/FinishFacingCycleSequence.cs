using Sunduk.PWA.Infrastructure.CAM;
using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Tools.Turning;

namespace Sunduk.PWA.Infrastructure.Sequences.Turning
{
    public class FinishFacingCycleSequence : Sequence
    {
        public Sequence RoughSequence { get; set; }
        public TurningExternalTool Tool { get; set; }
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

        public override string Operation 
        {
            get => Templates.FacingOperation.FinishFacingCycle(Tool, RoughSequence, SpeedFinish, FeedFinish);
            set { }
        }

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
