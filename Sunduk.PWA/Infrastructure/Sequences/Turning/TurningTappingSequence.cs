using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Tools.Turning;

namespace Sunduk.PWA.Infrastructure.Sequences.Turning
{
    public class TurningTappingSequence : TappingSequence
    {
        public CoordinateSystem CoordinateSystem { get; set; }
        public override MachineType MachineType => MachineType.Turning;
        public override string Operation => Templates.ThreadOperation.TurningTapping(Machine, CoordinateSystem, Tool as TurningTappingTool, CutSpeed, StartZ, EndZ, Coolant);
        public TurningTappingSequence(Machine machine, CoordinateSystem coordinateSystem, TurningTappingTool tool, double cutSpeed, double startZ, double endZ)
            :base(machine, tool, cutSpeed, startZ, endZ)
        {
            CoordinateSystem = coordinateSystem;
        }
    }
}
