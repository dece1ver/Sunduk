﻿using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Tools.Base;
using Sunduk.PWA.Infrastructure.Tools.Milling;
using System.Collections.Generic;

namespace Sunduk.PWA.Infrastructure.Sequences.Milling
{
    public class MillingPeckDrillingSequence : PeckDrillingSequence
    {
        public List<Hole> Holes { get; set; }
        public bool Polar { get; set; }
        public double SafePlane { get; set; }
        public CoordinateSystem CoordinateSystem { get; set; }
        public override MachineType MachineType => MachineType.Milling;
        public override string Operation => Templates.DrillingOperation.MillingPeckDrilling(
            Machine, 
            CoordinateSystem,
            Tool as MillingDrillingTool, 
            Depth, 
            StartZ, 
            EndZ, 
            Speed, 
            Feed, 
            Holes, 
            Polar, 
            SafePlane);
        public MillingPeckDrillingSequence(Machine machine, CoordinateSystem coordinateSystem, Material material, MillingDrillingTool tool, double depth, double startZ, double endZ, int speed, double feed, List<Hole> holes, bool polar, double safePlane) 
            : base(machine, material, tool, depth, startZ, endZ, speed, feed)
        {
            Holes = holes;
            Polar = polar;
            SafePlane = safePlane;
            CoordinateSystem = coordinateSystem;
        }
    }
}
