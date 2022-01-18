using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Sequences.Milling
{
    public class MillingHighSpeedDrillingSequence : HighSpeedDrillingSequence
    {
        public override MachineType MachineType => MachineType.Milling;
        public override string Operation => Templates.Operation.MillingHighSpeedDrilling(Machine, Material, Tool, StartZ, EndZ);
        public MillingHighSpeedDrillingSequence(Machine machine, Material material, DrillingTool tool, double startZ, double endZ) 
            : base(machine, material, tool, startZ, endZ)
        {
        }
    }
}
