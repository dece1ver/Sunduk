using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Tools.Base;
using Sunduk.PWA.Infrastructure.Tools.Milling;
using System.Collections.Generic;

namespace Sunduk.PWA.Infrastructure.Sequences.Milling
{
    public class MillingHighSpeedDrillingSequence : HighSpeedDrillingSequence
    {
        public List<Hole> Holes { get; set; }
        public override MachineType MachineType => MachineType.Milling;
        public override string Operation => Templates.Operation.MillingHighSpeedDrilling(Machine, Material, Tool, StartZ, EndZ, Holes);
        public MillingHighSpeedDrillingSequence(Machine machine, Material material, MillingDrillingTool tool, double startZ, double endZ, List<Hole> holes) 
            : base(machine, material, tool, startZ, endZ)
        {
            Holes = holes;
        }
    }
}
