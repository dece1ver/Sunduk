using Sunduk.PWA.Infrastructure.Sequences;
using Sunduk.PWA.Infrastructure.Tools.Base;
using Sunduk.PWA.Infrastructure.Tools.Milling;
using Sunduk.PWA.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Sunduk.PWA.Util.Util;

namespace Sunduk.PWA.Infrastructure.Templates
{
    public class DrillingOperation : Operation
    {
        /// <summary>
        /// Высокоскоростное сверление
        /// </summary>
        public static string TurningHighSpeedDrilling(Machine machine, Material material, DrillingTool tool, double startZ, double endZ)
        {
            if (tool is null || startZ <= endZ) return string.Empty;
            string approach = startZ > 0
                ? $"G0 X-{tool.Diameter.NC()} Z{startZ.NC()} S{DrillCuttingSpeed(material, tool)} {Direction(tool)}\n"
                : $"G0 X-{tool.Diameter.NC()} Z{SAFE_APPROACH_DISTANCE.NC()} S{DrillCuttingSpeed(material, tool)} {Direction(tool)}\n Z{startZ.NC()}\n";
            string exit = startZ > 0
                ? $"G0 Z{startZ.NC()}\n"
                : $"G0 Z{SAFE_APPROACH_DISTANCE.NC()}\n";
            return machine switch
            {
                Machine.GS1500 =>
                TURNING_REFERENT_POINT +
                tool.Description(ToolDescriptionOption.GoodwayLeft) + "\n" +
                approach +
                $"G1 Z{(endZ - tool.PointLength()).NC()} F{DrillFeed(machine, material, tool).NC(2)}\n" +
                exit +
                $"{CoolantOff(machine)}\n" +
                TURNING_REFERENT_POINT,

                Machine.L230A =>
                tool.Description(ToolDescriptionOption.L230) + "\n" +
                $"{CoolantOn(machine)}\n" +
                approach +
                $"G1 Z{(endZ - tool.PointLength()).NC()} F{DrillFeed(machine, material, tool).NC(2)}\n" +
                exit +
                $"{CoolantOff(machine)}\n" +
                TURNING_REFERENT_POINT,

                _ => string.Empty
            };
        }

        public static string MillingHighSpeedDrilling(Machine machine, Material material, MillingDrillingTool tool, double startZ, double endZ, List<Hole> holes, bool polar = false)
        {
            if (tool is null || startZ <= endZ) return string.Empty;
            var spindleSpeed = DrillCuttingSpeed(material, tool).ToSpindleSpeed(tool.Diameter, 100);
            var feed = DrillFeed(machine, material, tool).ToFeedPerMin(spindleSpeed, 1, 100);
            string result = machine switch
            {

                Machine.A110 =>
                tool.Description(ToolDescriptionOption.General) + "\n" +
                $"T00\n" +
                $"{(polar ? "G16 " : string.Empty)}" +
                $"G57 G0 X{holes[0].X.NC(option: NcDecimalPointOption.Without)} Y{holes[0].Y.NC(option: NcDecimalPointOption.Without)} S{spindleSpeed} {Direction(tool)}\n" +
                $"G43 Z{startZ.NC(option: NcDecimalPointOption.Without)} H{tool.Position} {CoolantOn(machine, Coolant.Through)}\n" +
                $"G81 Z{(endZ - tool.PointLength()).NC(option: NcDecimalPointOption.Without)} R{startZ.NC(option: NcDecimalPointOption.Without)} F{feed}\n",
                _ => string.Empty
            };

            AddPoints(ref result, holes, polar);

            if (!string.IsNullOrEmpty(result))
            {
                result += machine switch
                {
                    Machine.A110 =>
                    $"G80\n" +
                    $"{CoolantOff(machine)}\n" +
                    $"{(polar ? "G15" : string.Empty)}" +
                    MILLING_REFERENT_POINT,
                    _ => string.Empty
                };
            }
            return result;
        }


        /// <summary>
        /// Прерывистое сверление
        /// </summary>
        public static string TurningPeckDrilling(Machine machine, Material material, DrillingTool tool, double depth, double startZ, double endZ)
        {
            if (tool is null ||
                startZ <= endZ ||
                depth <= 0) return string.Empty;
            string approach = startZ > 0
                ? $"G0 X-{tool.Diameter.NC()} Z{startZ.NC()}S{DrillCuttingSpeed(material, tool)} {Direction(tool)}\n"
                : $"G0 X-{tool.Diameter.NC()} Z{SAFE_APPROACH_DISTANCE.NC()}S{DrillCuttingSpeed(material, tool)} {Direction(tool)}\nZ{startZ.NC()}\n";
            string exit = startZ > 0
                ? $"G0 Z{startZ.NC()}\n"
                : $"G0 Z{SAFE_APPROACH_DISTANCE.NC()}\n";
            return machine switch
            {
                Machine.GS1500 =>
                TURNING_REFERENT_POINT +
                tool.Description(ToolDescriptionOption.GoodwayLeft) + "\n" +
                approach +
                "G74 R0.1\n" +
                $"G74 Z{(endZ - tool.PointLength()).NC()} Q{depth.Microns()} F{DrillFeed(machine, material, tool).NC(2)}\n" +
                exit +
                $"{CoolantOff(machine)}\n" +
                TURNING_REFERENT_POINT,

                Machine.L230A =>
                tool.Description(ToolDescriptionOption.L230) + "\n" +
                $"{CoolantOn(machine)}\n" +
                approach +
                "G74 R0.1\n" +
                $"G74 Z{(endZ - tool.PointLength()).NC()} Q{depth.Microns()} F{DrillFeed(machine, material, tool).NC(2)}\n" +
                exit +
                $"{CoolantOff(machine)}\n" +
                TURNING_REFERENT_POINT,

                _ => string.Empty
            };
        }

        public static string MillingPeckDrilling(Machine machine, Material material, MillingDrillingTool tool, double depth, double startZ, double endZ, List<Hole> holes, bool polar = false)
        {
            if (tool is null || startZ <= endZ) return string.Empty;
            var spindleSpeed = DrillCuttingSpeed(material, tool).ToSpindleSpeed(tool.Diameter, 100);
            var feed = DrillFeed(machine, material, tool).ToFeedPerMin(spindleSpeed, 1, 100);
            string result = machine switch
            {

                Machine.A110 =>
                tool.Description(ToolDescriptionOption.General) + "\n" +
                $"T00\n" +
                $"{(polar ? "G16 " : string.Empty)}" +
                $"G57 G0 X{holes[0].X.NC(option: NcDecimalPointOption.Without)} Y{holes[0].Y.NC(option: NcDecimalPointOption.Without)} S{spindleSpeed} {Direction(tool)}\n" +
                $"G43 Z{startZ.NC(option: NcDecimalPointOption.Without)} H{tool.Position} {CoolantOn(machine, Coolant.Through)}\n" +
                $"G82 Z{(endZ - tool.PointLength()).NC(option: NcDecimalPointOption.Without)} Q{depth.Microns()} R{startZ.NC(option: NcDecimalPointOption.Without)} F{feed}\n",
                _ => string.Empty
            };

            AddPoints(ref result, holes, polar);

            if (!string.IsNullOrEmpty(result))
            {
                result += machine switch
                {
                    Machine.A110 =>
                    $"G80\n" +
                    $"{CoolantOff(machine)}\n" +
                    $"{(polar ? "G15" : string.Empty)}" +
                    MILLING_REFERENT_POINT,
                    _ => string.Empty
                };
            }
            return result;
        }

        /// <summary>
        /// Глубокое сверление
        /// </summary>
        public static string TurningPeckDeepDrilling(Machine machine, Material material, DrillingTool tool, double depth, double startZ, double endZ)
        {
            if (tool is null ||
                startZ <= endZ ||
                depth <= 0) return string.Empty;
            string approach = startZ > 0
                ? $"G0 X-{tool.Diameter.NC()} Z{startZ.NC()} S{DrillCuttingSpeed(material, tool)} {Direction(tool)}\n"
                : $"G0 X-{tool.Diameter.NC()} Z{SAFE_APPROACH_DISTANCE.NC()} S{DrillCuttingSpeed(material, tool)} {Direction(tool)}\nZ{startZ.NC()}\n";
            string exit = startZ > 0
                ? $"G0 Z{startZ.NC()}\n"
                : $"G0 Z{SAFE_APPROACH_DISTANCE.NC()}\n";
            return machine switch
            {
                Machine.GS1500 =>
                TURNING_REFERENT_POINT +
                tool.Description(ToolDescriptionOption.GoodwayLeft) + "\n" +
                approach +
                $"G83 Z{(endZ - tool.PointLength()).NC()} Q{depth.Microns()} F{DrillFeed(machine, material, tool).NC(2)}\n" +
                "G80\n" +
                exit +
                $"{CoolantOff(machine)}\n" +
                TURNING_REFERENT_POINT,

                Machine.L230A =>
                tool.Description(ToolDescriptionOption.L230) + "\n" +
                $"{CoolantOn(machine)}\n" +
                approach +
                $"G83 Z{(endZ - tool.PointLength()).NC()} Q{depth.Microns()} F{DrillFeed(machine, material, tool).NC(2)}\n" +
                "G80\n" +
                exit +
                $"{CoolantOff(machine)}\n" +
                TURNING_REFERENT_POINT,
                _ => string.Empty
            };
        }

        public static string MillingPeckDeepDrilling(Machine machine, Material material, MillingDrillingTool tool, double depth, double startZ, double endZ, List<Hole> holes, bool polar = false)
        {
            if (tool is null || startZ <= endZ) return string.Empty;
            var spindleSpeed = DrillCuttingSpeed(material, tool).ToSpindleSpeed(tool.Diameter, 100);
            var feed = DrillFeed(machine, material, tool).ToFeedPerMin(spindleSpeed, 1, 100);
            string result = machine switch
            {

                Machine.A110 =>
                tool.Description(ToolDescriptionOption.General) + "\n" +
                $"T00\n" +
                $"{(polar ? "G16 " : string.Empty)}" +
                $"G57 G0 X{holes[0].X.NC(option: NcDecimalPointOption.Without)} Y{holes[0].Y.NC(option: NcDecimalPointOption.Without)} S{spindleSpeed} {Direction(tool)}\n" +
                $"G43 Z{startZ.NC(option: NcDecimalPointOption.Without)} H{tool.Position} {CoolantOn(machine, Coolant.Through)}\n" +
                $"G83 Z{(endZ - tool.PointLength()).NC(option: NcDecimalPointOption.Without)} Q{depth.Microns()} R{startZ.NC(option: NcDecimalPointOption.Without)} F{feed}\n",
                _ => string.Empty
            };

            AddPoints(ref result, holes, polar);

            if (!string.IsNullOrEmpty(result))
            {
                result += machine switch
                {
                    Machine.A110 =>
                    $"G80\n" +
                    $"{CoolantOff(machine)}\n" +
                    $"{(polar ? "G15" : string.Empty)}" +
                    MILLING_REFERENT_POINT,
                    _ => string.Empty
                };
            }
            return result;
        }
    }
}
