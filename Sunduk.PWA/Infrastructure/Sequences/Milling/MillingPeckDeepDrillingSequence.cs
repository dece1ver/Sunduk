using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Sequences.Milling
{
    public class MillingPeckDeepDrillingSequence : PeckDeepDrillingSequence
    {
        public override MachineType MachineType => MachineType.Milling;
        public MillingPeckDeepDrillingSequence(Machine machine, Material material, DrillingTool tool, double depth, double startZ, double endZ) 
            : base(machine, material, tool, depth, startZ, endZ)
        {
        }
    }
}
