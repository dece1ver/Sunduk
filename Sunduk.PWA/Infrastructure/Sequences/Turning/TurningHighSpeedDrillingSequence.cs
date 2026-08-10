using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Tools.Base;
using Sunduk.PWA.Infrastructure.Tools.Turning;

namespace Sunduk.PWA.Infrastructure.Sequences.Turning
{
    public class TurningHighSpeedDrillingSequence : HighSpeedDrillingSequence
    {
        public CoordinateSystem CoordinateSystem { get; set; }
        public override MachineType MachineType => MachineType.Turning;
        public override string Operation => Templates.DrillingOperation.TurningHighSpeedDrilling(Machine, CoordinateSystem, Tool, StartZ, EndZ, Speed, Feed, Coolant);
        public TurningHighSpeedDrillingSequence(Machine machine, CoordinateSystem coordinateSystem, Material material, TurningDrillingTool tool, double startZ, double endZ, int speed, double feed)
            : base(machine, material, tool, startZ, endZ, speed, feed)
        {
            CoordinateSystem = coordinateSystem;
        }
    }
}
