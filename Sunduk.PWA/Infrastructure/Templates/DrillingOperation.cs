using Sunduk.PWA.Infrastructure.Sequences;
using Sunduk.PWA.Infrastructure.Tools.Base;
using Sunduk.PWA.Infrastructure.Tools.Milling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sunduk.PWA.Infrastructure.Tools.Turning;
using static Sunduk.PWA.Infrastructure.Util;

namespace Sunduk.PWA.Infrastructure.Templates
{
    public class DrillingOperation : Operation
    {
        /// <summary>
        /// Высокоскоростное сверление
        /// </summary>
        public static string TurningHighSpeedDrilling(Machine machine, CoordinateSystem coordinateSystem, DrillingTool tool, double startZ, double endZ, int speed, double feed, Coolant coolant = Coolant.General)
        {
            if (tool is null || startZ <= endZ) return string.Empty;
            string approach = startZ > 0
                ? $"G0 X-{tool.Diameter.NC()} Z{startZ.NC()} {tool.SpindleOn(speed)}\n"
                : $"G0 X-{tool.Diameter.NC()} Z{SafeApproachDistance.NC()} {tool.SpindleOn(speed)}\n Z{startZ.NC()}\n";
            string exit = startZ > 0
                ? $"G0 Z{startZ.NC()}\n"
                : $"G0 Z{SafeApproachDistance.NC()}\n";
            if (machine.MachineType != MachineType.Turning) return string.Empty;
            return new GCodeBuilder()
                .ReferentPoint(machine, leading: true)
                .ToolCall(tool, machine, coordinateSystem, coolant, suppressCoolant: machine.LeadingReferentPoint)
                .Raw(approach)
                .Line($"G1 Z{(endZ - tool.PointLength()).NC()} F{feed.NC(2)}")
                .Raw(exit)
                .CoolantOff(machine, coolant)
                .ReferentPoint(machine, leading: false)
                .ToString();
        }

        public static string MillingHighSpeedDrilling(Machine machine, CoordinateSystem coordinateSystem, MillingDrillingTool tool, double startZ, double endZ, int speed, double feed, List<Hole> holes, bool polar, double safePlane)
        {
            if (tool is null || startZ <= endZ) return string.Empty;
            if (machine.MachineType != MachineType.Milling) return string.Empty;
            var spins = speed.ToSpindleSpeed(tool.Diameter, 100);
            var feedPerMin = feed.ToFeedPerMin(spins, 1, 100);
            return new GCodeBuilder()
                .ToolCall(tool, machine, coordinateSystem, Coolant.General)
                .Line($"{coordinateSystem} {(polar ? "G16 " : string.Empty)}G0 X{holes[0].X.NC(option: NcDecimalPointOption.Without)} Y{holes[0].Y.NC(option: NcDecimalPointOption.Without)} S{spins} {Direction(tool)}")
                .Line($"G43 Z{safePlane.NC(option: NcDecimalPointOption.Without)} H{tool.Position} {CoolantOn(machine, Coolant.Through)}")
                .Line($"G0 Z{startZ.NC(option: NcDecimalPointOption.Without)}")
                .HolePattern(holes, polar, hole =>
                    $"G81 X{hole.X.NC(option: NcDecimalPointOption.Without)} Y{hole.Y.NC(option: NcDecimalPointOption.Without)} Z{(endZ - tool.PointLength()).NC(option: NcDecimalPointOption.Without)} R{startZ.NC(option: NcDecimalPointOption.Without)} F{feedPerMin}")
                .Line("G80")
                .Line($"{CoolantOff(machine)}")
                .LineIf(polar, "G15")
                .Line(SpindleStop)
                .Raw(MillingReferentPoint)
                .ToString();
        }


        /// <summary>
        /// Прерывистое сверление
        /// </summary>
        public static string TurningPeckDrilling(Machine machine, CoordinateSystem coordinateSystem, DrillingTool tool, double depth, double startZ, double endZ, int speed, double feed, Coolant coolant = Coolant.General)
        {
            if (tool is null ||
                startZ <= endZ ||
                depth <= 0) return string.Empty;
            var approach = startZ > 0
                ? $"G0 X-{tool.Diameter.NC()} Z{startZ.NC()}{tool.SpindleOn(speed)}\n"
                : $"G0 X-{tool.Diameter.NC()} Z{SafeApproachDistance.NC()}{tool.SpindleOn(speed)}\nZ{startZ.NC()}\n";
            var exit = startZ > 0
                ? $"G0 Z{startZ.NC()}\n"
                : $"G0 Z{SafeApproachDistance.NC()}\n";
            if (machine.MachineType != MachineType.Turning) return string.Empty;
            return new GCodeBuilder()
                .ReferentPoint(machine, leading: true)
                .ToolCall(tool, machine, coordinateSystem, coolant, suppressCoolant: machine.LeadingReferentPoint)
                .Raw(approach)
                .Line("G74 R0.1")
                .Line($"G74 Z{(endZ - tool.PointLength()).NC()} Q{depth.Microns()} F{feed.NC(2)}")
                .Raw(exit)
                .CoolantOff(machine, coolant)
                .ReferentPoint(machine, leading: false)
                .ToString();
        }

        public static string MillingPeckDrilling(Machine machine, CoordinateSystem coordinateSystem, MillingDrillingTool tool, double depth, double startZ, double endZ, int speed, double feed, List<Hole> holes, bool polar, double safePlane)
        {
            if (tool is null || startZ <= endZ) return string.Empty;
            if (machine.MachineType != MachineType.Milling) return string.Empty;
            var spins = speed.ToSpindleSpeed(tool.Diameter, 100);
            var feedPerMin = feed.ToFeedPerMin(spins, 1, 100);
            return new GCodeBuilder()
                .ToolCall(tool, machine, coordinateSystem, Coolant.General)
                .Line($"{coordinateSystem} {(polar ? "G16 " : string.Empty)}G0 X{holes[0].X.NC(option: NcDecimalPointOption.Without)} Y{holes[0].Y.NC(option: NcDecimalPointOption.Without)} S{spins} {Direction(tool)}")
                .Line($"G43 Z{safePlane.NC(option: NcDecimalPointOption.Without)} H{tool.Position} {CoolantOn(machine, Coolant.Through)}")
                .Line($"G0 Z{startZ.NC(option: NcDecimalPointOption.Without)}")
                .HolePattern(holes, polar, hole =>
                    $"G73 X{hole.X.NC(option: NcDecimalPointOption.Without)} Y{hole.Y.NC(option: NcDecimalPointOption.Without)} Z{(endZ - tool.PointLength()).NC(option: NcDecimalPointOption.Without)} Q{depth.Microns()} R{startZ.NC(option: NcDecimalPointOption.Without)} F{feedPerMin}")
                .Line("G80")
                .Line($"{CoolantOff(machine)}")
                .LineIf(polar, "G15")
                .Line(SpindleStop)
                .Raw(MillingReferentPoint)
                .ToString();
        }

        /// <summary>
        /// Глубокое сверление
        /// </summary>
        public static string TurningPeckDeepDrilling(Machine machine, CoordinateSystem coordinateSystem, TurningDrillingTool tool, double depth, double startZ, double endZ, int speed, double feed, Coolant coolant = Coolant.General)
        {
            if (tool is null ||
                startZ <= endZ ||
                depth <= 0) return string.Empty;
            var approach = startZ > 0
                ? $"G0 X-{tool.Diameter.NC()} Z{startZ.NC()} {tool.SpindleOn(speed)}\n"
                : $"G0 X-{tool.Diameter.NC()} Z{SafeApproachDistance.NC()} {tool.SpindleOn(speed)}\nZ{startZ.NC()}\n";
            var exit = startZ > 0
                ? $"G0 Z{startZ.NC()}\n"
                : $"G0 Z{SafeApproachDistance.NC()}\n";
            if (machine.MachineType != MachineType.Turning) return string.Empty;
            return new GCodeBuilder()
                .ReferentPoint(machine, leading: true)
                .ToolCall(tool, machine, coordinateSystem, coolant, suppressCoolant: machine.LeadingReferentPoint)
                .Raw(approach)
                .Line($"G83 Z{(endZ - tool.PointLength()).NC()} Q{depth.Microns()} F{feed.NC(2)}")
                .Line("G80")
                .Raw(exit)
                .CoolantOff(machine, coolant)
                .ReferentPoint(machine, leading: false)
                .ToString();
        }

        public static string MillingPeckDeepDrilling(Machine machine, CoordinateSystem coordinateSystem, MillingDrillingTool tool, double depth, double startZ, double endZ, int speed, double feed, List<Hole> holes, bool polar, double safePlane)
        {
            if (tool is null || startZ <= endZ) return string.Empty;
            if (machine.MachineType != MachineType.Milling) return string.Empty;
            var spins = speed.ToSpindleSpeed(tool.Diameter, 100);
            var feedPerMin = feed.ToFeedPerMin(spins, 1, 100);
            return new GCodeBuilder()
                .ToolCall(tool, machine, coordinateSystem, Coolant.General)
                .Line($"{coordinateSystem} {(polar ? "G16 " : string.Empty)}G0 X{holes[0].X.NC(option: NcDecimalPointOption.Without)} Y{holes[0].Y.NC(option: NcDecimalPointOption.Without)} S{spins} {Direction(tool)}")
                .Line($"G43 Z{safePlane.NC(option: NcDecimalPointOption.Without)} H{tool.Position} {CoolantOn(machine, Coolant.Through)}")
                .Line($"G0 Z{startZ.NC(option: NcDecimalPointOption.Without)}")
                .HolePattern(holes, polar, hole =>
                    $"G83 X{hole.X.NC(option: NcDecimalPointOption.Without)} Y{hole.Y.NC(option: NcDecimalPointOption.Without)} Z{(endZ - tool.PointLength()).NC(option: NcDecimalPointOption.Without)} Q{depth.Microns()} R{startZ.NC(option: NcDecimalPointOption.Without)} F{feedPerMin}")
                .Line("G80")
                .Line($"{CoolantOff(machine)}")
                .LineIf(polar, "G15")
                .Line(SpindleStop)
                .Raw(MillingReferentPoint)
                .ToString();
        }
    }
}
