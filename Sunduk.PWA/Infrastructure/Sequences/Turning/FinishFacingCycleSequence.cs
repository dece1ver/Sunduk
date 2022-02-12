using Sunduk.PWA.Infrastructure.CAM;
using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Tools.Turning;

namespace Sunduk.PWA.Infrastructure.Sequences.Turning
{
    public class FinishFacingCycleSequence : Sequence
    {
        public Sequence RoughSequence { get; set; }
        public TurningExternalTool Tool { get; set; }

        public override string Operation 
        {
            get
            {
                return RoughSequence switch
                {
                    RoughFacingSequence => Templates.FacingOperation.FinishFacingCycleFromRoughFacing(Tool, RoughSequence as RoughFacingSequence),
                    RoughFacingCycleSequence => Templates.FacingOperation.FinishFacingCycleFromRoughCycleFacing(Tool, RoughSequence as RoughFacingCycleSequence),
                    FacingSequence => Templates.FacingOperation.FinishFacingCycleFromFacing(Tool, RoughSequence as FacingSequence),
                    _ => null,
                };
            }
            set { }
        }

        public override MachineType MachineType => MachineType.Turning;

        public FinishFacingCycleSequence(TurningExternalTool tool, Sequence roughSequence)
        {
            RoughSequence = roughSequence;
            Tool = tool;
        }
    }
}
