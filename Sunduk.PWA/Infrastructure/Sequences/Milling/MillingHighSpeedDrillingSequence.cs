using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Tools.Base;
using Sunduk.PWA.Infrastructure.Tools.Milling;
using System.Collections.Generic;

namespace Sunduk.PWA.Infrastructure.Sequences.Milling
{
    public class MillingHighSpeedDrillingSequence : HighSpeedDrillingSequence
    {
        public List<Hole> Holes { get; set; }
        public bool Polar { get; set; }
        public override MachineType MachineType => MachineType.Milling;
        public override string Operation => Templates.DrillingOperation.MillingHighSpeedDrilling(Machine, Material, Tool as MillingDrillingTool, StartZ, EndZ, Holes);
        public MillingHighSpeedDrillingSequence(Machine machine, Material material, MillingDrillingTool tool, double startZ, double endZ, List<Hole> holes, bool polar) 
            : base(machine, material, tool, startZ, endZ)
        {
            Holes = holes;
            Polar = polar;
        }
    }
}
