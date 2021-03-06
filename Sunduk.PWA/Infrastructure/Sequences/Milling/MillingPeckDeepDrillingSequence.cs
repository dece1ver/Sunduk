using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Tools.Base;
using Sunduk.PWA.Infrastructure.Tools.Milling;
using System.Collections.Generic;

namespace Sunduk.PWA.Infrastructure.Sequences.Milling
{
    public class MillingPeckDeepDrillingSequence : PeckDeepDrillingSequence
    {
        public List<Hole> Holes { get; set; }
        public bool Polar { get; set; }
        public override MachineType MachineType => MachineType.Milling;
        public override string Operation => Templates.DrillingOperation.MillingPeckDeepDrilling(Machine, Material, Tool as MillingDrillingTool, Depth, StartZ, EndZ, Holes, Polar);
        public MillingPeckDeepDrillingSequence(Machine machine, Material material, MillingDrillingTool tool, double depth, double startZ, double endZ, List<Hole> holes, bool polar) 
            : base(machine, material, tool, depth, startZ, endZ)
        {
            Holes = holes;
            Polar = polar;
        }
    }
}
