using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Tools.Turning;

namespace Sunduk.PWA.Infrastructure.Sequences.Turning
{
    public class TurningTappingSequence : TappingSequence
    {
        public override MachineType MachineType => MachineType.Turning;
        public override string Operation => Templates.ThreadOperation.TurningTapping(Machine, Tool as TurningTappingTool, CutSpeed, StartZ, EndZ);
        public TurningTappingSequence(Machine machine, TurningTappingTool tool, double cutSpeed, double startZ, double endZ)
            :base(machine, tool, cutSpeed, startZ, endZ)
        {
        }
    }
}
