using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Tools.Base;
using Sunduk.PWA.Infrastructure.Tools.Turning;

namespace Sunduk.PWA.Infrastructure.Sequences.Turning
{
    public class TurningPeckDrillingSequence : PeckDrillingSequence
    {
        public CoordinateSystem CoordinateSystem { get; set; }
        public override MachineType MachineType => MachineType.Turning;
        public override string Operation => Templates.DrillingOperation.TurningPeckDrilling(Machine, CoordinateSystem, Tool, Depth, StartZ, EndZ, Speed, Feed, Coolant);
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        public TurningPeckDrillingSequence(Machine machine, CoordinateSystem coordinateSystem, Material material, TurningDrillingTool tool, double depth, double startZ, double endZ, int speed, double feed)
            : base(machine, material, tool, depth, startZ, endZ, speed, feed)
        {
            CoordinateSystem = coordinateSystem;
        }
    }
}
