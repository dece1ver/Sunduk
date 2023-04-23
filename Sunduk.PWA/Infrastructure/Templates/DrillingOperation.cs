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
        public static string TurningHighSpeedDrilling(Machine machine, DrillingTool tool, double startZ, double endZ, int speed, double feed)
        {
            if (tool is null || startZ <= endZ) return string.Empty;
            string approach = startZ > 0
                ? $"G0 X-{tool.Diameter.NC()} Z{startZ.NC()} S{speed} {Direction(tool)}\n"
                : $"G0 X-{tool.Diameter.NC()} Z{SafeApproachDistance.NC()} S{speed} {Direction(tool)}\n Z{startZ.NC()}\n";
            string exit = startZ > 0
                ? $"G0 Z{startZ.NC()}\n"
                : $"G0 Z{SafeApproachDistance.NC()}\n";
            return machine switch
            {
                Machine.GS1500 =>
                TurningReferentPoint +
                tool.Description(ToolDescriptionOption.GoodwayLeft) + "\n" +
                approach +
                $"G1 Z{(endZ - tool.PointLength()).NC()} F{feed.NC(2)}\n" +
                exit +
                $"{CoolantOff(machine)}\n" +
                TurningReferentPoint,

                Machine.L230A =>
                tool.Description(ToolDescriptionOption.L230) + "\n" +
                $"{CoolantOn(machine)}\n" +
                approach +
                $"G1 Z{(endZ - tool.PointLength()).NC()} F{feed.NC(2)}\n" +
                exit +
                $"{CoolantOff(machine)}\n" +
                TurningReferentPoint,

                _ => string.Empty
            };
        }

        public static string MillingHighSpeedDrilling(Machine machine, CoordinateSystem coordinateSystem, MillingDrillingTool tool, double startZ, double endZ, int speed, double feed, List<Hole> holes, bool polar, double safePlane)
        {
            if (tool is null || startZ <= endZ) return string.Empty;
            var spins = speed.ToSpindleSpeed(tool.Diameter, 100);
            var feedPerMin = feed.ToFeedPerMin(spins, 1, 100);
            var result = machine switch
            {

                Machine.A110 =>
                tool.Description(ToolDescriptionOption.MillingToolChange) + "\n" +
                $"{coordinateSystem} {(polar ? "G16 " : string.Empty)}G0 X{holes[0].X.NC(option: NcDecimalPointOption.Without)} Y{holes[0].Y.NC(option: NcDecimalPointOption.Without)} S{spins} {Direction(tool)}\n" +
                $"G43 Z{safePlane.NC(option: NcDecimalPointOption.Without)} H{tool.Position} {CoolantOn(machine, Coolant.Through)}\n" +
                $"G0 Z{startZ.NC(option: NcDecimalPointOption.Without)}\n" +
                $"G81 X{holes[0].X.NC(option: NcDecimalPointOption.Without)} Y{holes[0].Y.NC(option: NcDecimalPointOption.Without)} Z{(endZ - tool.PointLength()).NC(option: NcDecimalPointOption.Without)} R{startZ.NC(option: NcDecimalPointOption.Without)} F{feedPerMin}\n",
                _ => string.Empty
            };

            AddPoints(ref result, holes, polar);

            if (!string.IsNullOrEmpty(result))
            {
                result += machine switch
                {
                    Machine.A110 =>
                    "G80\n" +
                    $"{CoolantOff(machine)}\n" +
                    $"{(polar ? "G15\n" : string.Empty)}" +
                    SpindleStop + "\n" +
                    MillingReferentPoint,
                    _ => string.Empty
                };
            }
            return result;
        }


        /// <summary>
        /// Прерывистое сверление
        /// </summary>
        public static string TurningPeckDrilling(Machine machine, DrillingTool tool, double depth, double startZ, double endZ, int speed, double feed)
        {
            if (tool is null ||
                startZ <= endZ ||
                depth <= 0) return string.Empty;
            var approach = startZ > 0
                ? $"G0 X-{tool.Diameter.NC()} Z{startZ.NC()}S{speed} {Direction(tool)}\n"
                : $"G0 X-{tool.Diameter.NC()} Z{SafeApproachDistance.NC()}S{speed} {Direction(tool)}\nZ{startZ.NC()}\n";
            var exit = startZ > 0
                ? $"G0 Z{startZ.NC()}\n"
                : $"G0 Z{SafeApproachDistance.NC()}\n";
            return machine switch
            {
                Machine.GS1500 =>
                TurningReferentPoint +
                tool.Description(ToolDescriptionOption.GoodwayLeft) + "\n" +
                approach +
                "G74 R0.1\n" +
                $"G74 Z{(endZ - tool.PointLength()).NC()} Q{depth.Microns()} F{feed.NC(2)}\n" +
                exit +
                $"{CoolantOff(machine)}\n" +
                TurningReferentPoint,

                Machine.L230A =>
                tool.Description(ToolDescriptionOption.L230) + "\n" +
                $"{CoolantOn(machine)}\n" +
                approach +
                "G74 R0.1\n" +
                $"G74 Z{(endZ - tool.PointLength()).NC()} Q{depth.Microns()} F{feed.NC(2)}\n" +
                exit +
                $"{CoolantOff(machine)}\n" +
                TurningReferentPoint,

                _ => string.Empty
            };
        }

        public static string MillingPeckDrilling(Machine machine, CoordinateSystem coordinateSystem, MillingDrillingTool tool, double depth, double startZ, double endZ, int speed, double feed, List<Hole> holes, bool polar, double safePlane)
        {
            if (tool is null || startZ <= endZ) return string.Empty;
            var spins = speed.ToSpindleSpeed(tool.Diameter, 100);
            var feedPerMin = feed.ToFeedPerMin(spins, 1, 100);
            var result = machine switch
            {

                Machine.A110 =>
                tool.Description(ToolDescriptionOption.MillingToolChange) + "\n" +
                $"{coordinateSystem} {(polar ? "G16 " : string.Empty)}G0 X{holes[0].X.NC(option: NcDecimalPointOption.Without)} Y{holes[0].Y.NC(option: NcDecimalPointOption.Without)} S{spins} {Direction(tool)}\n" +
                $"G43 Z{safePlane.NC(option: NcDecimalPointOption.Without)} H{tool.Position} {CoolantOn(machine, Coolant.Through)}\n" +
                $"G0 Z{startZ.NC(option: NcDecimalPointOption.Without)}\n" +
                $"G73 X{holes[0].X.NC(option: NcDecimalPointOption.Without)} Y{holes[0].Y.NC(option: NcDecimalPointOption.Without)} Z{(endZ - tool.PointLength()).NC(option: NcDecimalPointOption.Without)} Q{depth.Microns()} R{startZ.NC(option: NcDecimalPointOption.Without)} F{feedPerMin}\n",
                _ => string.Empty
            };

            AddPoints(ref result, holes, polar);

            if (!string.IsNullOrEmpty(result))
            {
                result += machine switch
                {
                    Machine.A110 =>
                    "G80\n" +
                    $"{CoolantOff(machine)}\n" +
                    $"{(polar ? "G15\n" : string.Empty)}" +
                    SpindleStop + "\n" +
                    MillingReferentPoint,
                    _ => string.Empty
                };
            }
            return result;
        }

        /// <summary>
        /// Глубокое сверление
        /// </summary>
        public static string TurningPeckDeepDrilling(Machine machine, TurningDrillingTool tool, double depth, double startZ, double endZ, int speed, double feed)
        {
            if (tool is null ||
                startZ <= endZ ||
                depth <= 0) return string.Empty;
            var approach = startZ > 0
                ? $"G0 X-{tool.Diameter.NC()} Z{startZ.NC()} S{speed} {Direction(tool)}\n"
                : $"G0 X-{tool.Diameter.NC()} Z{SafeApproachDistance.NC()} S{speed} {Direction(tool)}\nZ{startZ.NC()}\n";
            var exit = startZ > 0
                ? $"G0 Z{startZ.NC()}\n"
                : $"G0 Z{SafeApproachDistance.NC()}\n";
            return machine switch
            {
                Machine.GS1500 =>
                TurningReferentPoint +
                tool.Description(ToolDescriptionOption.GoodwayLeft) + "\n" +
                approach +
                $"G83 Z{(endZ - tool.PointLength()).NC()} Q{depth.Microns()} F{feed.NC(2)}\n" +
                "G80\n" +
                exit +
                $"{CoolantOff(machine)}\n" +
                TurningReferentPoint,

                Machine.L230A =>
                tool.Description(ToolDescriptionOption.L230) + "\n" +
                $"{CoolantOn(machine)}\n" +
                approach +
                $"G83 Z{(endZ - tool.PointLength()).NC()} Q{depth.Microns()} F{feed.NC(2)}\n" +
                "G80\n" +
                exit +
                $"{CoolantOff(machine)}\n" +
                TurningReferentPoint,
                _ => string.Empty
            };
        }

        public static string MillingPeckDeepDrilling(Machine machine, CoordinateSystem coordinateSystem, MillingDrillingTool tool, double depth, double startZ, double endZ, int speed, double feed, List<Hole> holes, bool polar, double safePlane)
        {
            if (tool is null || startZ <= endZ) return string.Empty;
            var spins = speed.ToSpindleSpeed(tool.Diameter, 100);
            var feedPerMin = feed.ToFeedPerMin(spins, 1, 100);
            string result = machine switch
            {

                Machine.A110 =>
                tool.Description(ToolDescriptionOption.MillingToolChange) + "\n" +
                $"{coordinateSystem} {(polar ? "G16 " : string.Empty)}G0 X{holes[0].X.NC(option: NcDecimalPointOption.Without)} Y{holes[0].Y.NC(option: NcDecimalPointOption.Without)} S{spins} {Direction(tool)}\n" +
                $"G43 Z{safePlane.NC(option: NcDecimalPointOption.Without)} H{tool.Position} {CoolantOn(machine, Coolant.Through)}\n" +
                $"G0 Z{startZ.NC(option: NcDecimalPointOption.Without)}\n" +
                $"G83 X{holes[0].X.NC(option: NcDecimalPointOption.Without)} Y{holes[0].Y.NC(option: NcDecimalPointOption.Without)} Z{(endZ - tool.PointLength()).NC(option: NcDecimalPointOption.Without)} Q{depth.Microns()} R{startZ.NC(option: NcDecimalPointOption.Without)} F{feedPerMin}\n",
                _ => string.Empty
            };

            AddPoints(ref result, holes, polar);

            if (!string.IsNullOrEmpty(result))
            {
                result += machine switch
                {
                    Machine.A110 =>
                    "G80\n" +
                    $"{CoolantOff(machine)}\n" +
                    $"{(polar ? "G15\n" : string.Empty)}" +
                    SpindleStop + "\n" +
                    MillingReferentPoint,
                    _ => string.Empty
                };
            }
            return result;
        }
    }
}
