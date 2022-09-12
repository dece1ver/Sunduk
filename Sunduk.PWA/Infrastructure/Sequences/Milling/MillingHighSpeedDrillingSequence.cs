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
        public double SafePlane { get; set; }
        public CoordinateSystem CoordinateSystem { get; set; }
        public override MachineType MachineType => MachineType.Milling;
        public override string Operation => Templates.DrillingOperation.MillingHighSpeedDrilling(
            Machine, 
            CoordinateSystem, 
            Material, 
            Tool as MillingDrillingTool, 
            StartZ, 
            EndZ, 
            Holes, 
            Polar, 
            SafePlane);
        public MillingHighSpeedDrillingSequence(Machine machine, CoordinateSystem coordinateSystem, Material material, MillingDrillingTool tool, double startZ, double endZ, List<Hole> holes, bool polar, double safePlane) 
            : base(machine, material, tool, startZ, endZ)
        {
            Holes = holes;
            Polar = polar;
            SafePlane = safePlane;
            CoordinateSystem = coordinateSystem;
        }
    }
}
