using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Tools.Milling;
using System.Collections.Generic;

namespace Sunduk.PWA.Infrastructure.Sequences.Milling
{
    public class MillingTappingSequence : TappingSequence
    {
        public List<Hole> Holes { get; set; }
        public bool Polar { get; set; }
        public CoordinateSystem CoordinateSystem { get; set; }
        public override MachineType MachineType => MachineType.Milling;
        public override string Operation => Templates.ThreadOperation.MillingTapping(Machine, CoordinateSystem, Tool as MillingTappingTool, CutSpeed, StartZ, EndZ, Holes, Polar);
        public MillingTappingSequence(Machine machine, CoordinateSystem coordinateSystem, MillingTappingTool tool, double cutSpeed, double startZ, double endZ, List<Hole> holes, bool polar)
            : base(machine, tool, cutSpeed, startZ, endZ)
        {
            Holes = holes;
            Polar = polar;
            CoordinateSystem = coordinateSystem;
        }
    }
}
