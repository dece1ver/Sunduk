using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Sequences.Turning
{
    public class TurningPeckDeepDrillingSequence : PeckDeepDrillingSequence
    {
        public override MachineType MachineType => MachineType.Turning;
        public TurningPeckDeepDrillingSequence(Machine machine, Material material, DrillingTool tool, double depth, double startZ, double endZ) 
            : base(machine, material, tool, depth, startZ, endZ)
        {
        }
    }
}
