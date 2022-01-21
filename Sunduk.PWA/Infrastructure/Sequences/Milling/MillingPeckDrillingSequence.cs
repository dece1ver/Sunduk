using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Tools.Base;
using Sunduk.PWA.Infrastructure.Tools.Milling;
using System.Collections.Generic;

namespace Sunduk.PWA.Infrastructure.Sequences.Milling
{
    public class MillingPeckDrillingSequence : PeckDrillingSequence
    {
        public List<Hole> Holes { get; set; }
        public override MachineType MachineType => MachineType.Milling;
        public MillingPeckDrillingSequence(Machine machine, Material material, MillingDrillingTool tool, double depth, double startZ, double endZ, List<Hole> holes) 
            : base(machine, material, tool, depth, startZ, endZ)
        {
            Holes = holes;
        }
    }
}
