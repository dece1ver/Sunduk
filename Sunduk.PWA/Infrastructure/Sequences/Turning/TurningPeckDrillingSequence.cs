using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Tools.Base;
using Sunduk.PWA.Infrastructure.Tools.Turning;

namespace Sunduk.PWA.Infrastructure.Sequences.Turning
{
    public class TurningPeckDrillingSequence : PeckDrillingSequence
    {
        public override MachineType MachineType => MachineType.Turning;
        public override string Operation => Templates.Operation.TurningPeckDrilling(Machine, Material, Tool, Depth, StartZ, EndZ);
        public TurningPeckDrillingSequence(Machine machine, Material material, TurningDrillingTool tool, double depth, double startZ, double endZ) 
            : base(machine, material, tool, depth, startZ, endZ)
        {
        }
    }
}
