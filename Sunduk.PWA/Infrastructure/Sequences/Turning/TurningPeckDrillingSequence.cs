using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Tools.Base;
using Sunduk.PWA.Infrastructure.Tools.Turning;

namespace Sunduk.PWA.Infrastructure.Sequences.Turning
{
    public class TurningPeckDrillingSequence : PeckDrillingSequence
    {
        public override MachineType MachineType => MachineType.Turning;
        public override string Operation => Templates.DrillingOperation.TurningPeckDrilling(Machine, Tool, Depth, StartZ, EndZ, Speed, Feed);
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        public TurningPeckDrillingSequence(Machine machine, Material material, TurningDrillingTool tool, double depth, double startZ, double endZ, int speed, double feed) 
            : base(machine, material, tool, depth, startZ, endZ, speed, feed) { }
    }
}
