using System.Collections.Generic;
using Sunduk.PWA.Infrastructure.Sequences.Milling;
using Sunduk.PWA.Infrastructure.Sequences.Turning;
using Sunduk.PWA.Infrastructure.Tools.Milling;
using Sunduk.PWA.Infrastructure.Tools.Turning;

namespace Sunduk.PWA.Infrastructure.Sequences.Factories
{
    public static class DrillingSequenceFactory
    {
        public static TurningHighSpeedDrillingSequence CreateTurningHighSpeedDrilling(Machine machine, Material material, TurningDrillingTool tool, double startZ, double endZ, int speed, double feed)
            => new(machine, material, tool, startZ, endZ, speed, feed);

        public static MillingHighSpeedDrillingSequence CreateMillingHighSpeedDrilling(Machine machine, CoordinateSystem coordinateSystem, Material material, MillingDrillingTool tool, double startZ, double endZ, int speed, double feed, List<Hole> holes, bool polar, double safePlane)
            => new(machine, coordinateSystem, material, tool, startZ, endZ, speed, feed, holes, polar, safePlane);

        public static TurningPeckDrillingSequence CreateTurningPeckDrilling(Machine machine, Material material, TurningDrillingTool tool, double depth, double startZ, double endZ, int speed, double feed)
            => new(machine, material, tool, depth, startZ, endZ, speed, feed);

        public static MillingPeckDrillingSequence CreateMillingPeckDrilling(Machine machine, CoordinateSystem coordinateSystem, Material material, MillingDrillingTool tool, double depth, double startZ, double endZ, int speed, double feed, List<Hole> holes, bool polar, double safePlane)
            => new(machine, coordinateSystem, material, tool, depth, startZ, endZ, speed, feed, holes, polar, safePlane);

        public static TurningPeckDeepDrillingSequence CreateTurningPeckDeepDrilling(Machine machine, Material material, TurningDrillingTool tool, double depth, double startZ, double endZ, int speed, double feed)
            => new(machine, material, tool, depth, startZ, endZ, speed, feed);

        public static MillingPeckDeepDrillingSequence CreateMillingPeckDeepDrilling(Machine machine, CoordinateSystem coordinateSystem, Material material, MillingDrillingTool tool, double depth, double startZ, double endZ, int speed, double feed, List<Hole> holes, bool polar, double safePlane)
            => new(machine, coordinateSystem, material, tool, depth, startZ, endZ, speed, feed, holes, polar, safePlane);
    }
}
