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
                return Templates.FacingOperation.FinishFacingCycle(Tool, RoughSequence);
            }
            set { }
        }

        public override MachineType MachineType => MachineType.Turning;
        public override string Name => $"Торцовка чистовая (G70)";

        public FinishFacingCycleSequence(TurningExternalTool tool, Sequence roughSequence)
        {
            RoughSequence = roughSequence;
            Tool = tool;
        }
    }
}
