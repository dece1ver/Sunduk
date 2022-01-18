using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Sequences.Turning
{
    public class TurningHighSpeedDrillingSequence : HighSpeedDrillingSequence
    {
        public override MachineType MachineType => MachineType.Turning;
        public override string Operation => Templates.Operation.TurningHighSpeedDrilling(Machine, Material, Tool, StartZ, EndZ);
        public TurningHighSpeedDrillingSequence(Machine machine, Material material, DrillingTool tool, double startZ, double endZ) 
            : base(machine, material, tool, startZ, endZ)
        {
        }
    }
}
