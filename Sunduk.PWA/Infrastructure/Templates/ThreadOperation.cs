using Sunduk.PWA.Infrastructure.CAM;
using Sunduk.PWA.Infrastructure.Sequences;
using Sunduk.PWA.Infrastructure.Tools.Base;
using Sunduk.PWA.Infrastructure.Tools.Milling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Sunduk.PWA.Infrastructure.Util;

namespace Sunduk.PWA.Infrastructure.Templates
{
    public class ThreadOperation : Operation
    {
        /// <summary>
        /// Нарезание резьбы метчиком
        /// </summary>
        public static string TurningTapping(Machine machine, CoordinateSystem coordinateSystem, TappingTool tool, double cutSpeed, double startZ, double endZ, Coolant coolant = Coolant.General)
        {
            if (tool is null ||
                startZ <= endZ) return string.Empty;
            string approach = startZ > 0
                ? $"G0 X0. Z{startZ.NC()} S{cutSpeed.ToSpindleSpeed(tool.Diameter, 10)} {Direction(tool)} G97\n"
                : $"G0 X0. Z{SafeApproachDistance.NC()} S{((int)cutSpeed).ToSpindleSpeed(tool.Diameter, 10)} {Direction(tool)} G97\nZ{startZ.NC()}\n";
            string exit = startZ > 0
                ? string.Empty
                : $"G0 Z{SafeApproachDistance.NC()}\n";
            if (machine.MachineType != MachineType.Turning) return string.Empty;
            return new GCodeBuilder()
                .ReferentPoint(machine, leading: true)
                .ToolCall(tool, machine, coordinateSystem, coolant, suppressCoolant: machine.LeadingReferentPoint)
                .Raw(approach)
                .Line($"G84 Z{endZ.NC()} P1000 F{tool.Pitch.NC()}")
                .Line("G80")
                .Raw(exit)
                .Line($"G96 {CoolantOff(machine, coolant)}")
                .ReferentPoint(machine, leading: false)
                .ToString();
        }

        public static string MillingTapping(Machine machine, CoordinateSystem coordinateSystem, MillingTappingTool tool, double cutSpeed, double startZ, double endZ, List<Hole> holes, bool polar, double safePlane)
        {
            if (tool is null || startZ <= endZ) return string.Empty;
            if (machine.MachineType != MachineType.Milling) return string.Empty;
            var spindleSpeed = cutSpeed.ToSpindleSpeed(tool.Diameter, 10);
            return new GCodeBuilder()
                .ToolCall(tool, machine, coordinateSystem, Coolant.General)
                .Line($"{coordinateSystem}{(polar ? " G16" : string.Empty)} G0 X{holes[0].X.NC(option: NcDecimalPointOption.Without)} Y{holes[0].Y.NC(option: NcDecimalPointOption.Without)} S{spindleSpeed} {Direction(tool)}")
                .Line($"G43 Z{safePlane.NC(option: NcDecimalPointOption.Without)} H{tool.Position} {CoolantOn(machine, Coolant.Through)}")
                .Line($"G0 Z{startZ.NC(option: NcDecimalPointOption.Without)}")
                .HolePattern(holes, polar, hole => $"G95 G84 Z{endZ.NC(option: NcDecimalPointOption.Without)} R{startZ.NC(option: NcDecimalPointOption.Without)} P500 F{tool.Pitch.NC()}")
                .Line("G80")
                .Line($"G94 {CoolantOff(machine)}")
                .LineIf(polar, "G15")
                .Line(SpindleStop)
                .Raw(MillingReferentPoint)
                .ToString();
        }

        public static string ThreadMilling(Machine machine, CoordinateSystem coordinateSystem, MillingThreadCuttingTool tool, double diameter, double cutSpeed, double startZ, double endZ, List<Hole> holes, bool polar, double safePlane)
        {
            if (tool is null || startZ <= endZ) return string.Empty;
            if (machine.MachineType != MachineType.Milling) return string.Empty;
            var spindleSpeed = cutSpeed.ToSpindleSpeed(tool.Diameter, 10);
            return new GCodeBuilder()
                .ToolCall(tool, machine, coordinateSystem, Coolant.General)
                .Line($"{coordinateSystem}{(polar ? "G16 " : string.Empty)} G0 X{holes[0].X.NC(option: NcDecimalPointOption.Without)} Y{holes[0].Y.NC(option: NcDecimalPointOption.Without)} S{spindleSpeed} {Direction(tool)}")
                .Line($"G43 Z{safePlane.NC(option: NcDecimalPointOption.Without)} H{tool.Position} {CoolantOn(machine, Coolant.Through)}")
                .Line($"G0 Z{startZ.NC(option: NcDecimalPointOption.Without)}")
                .HolePattern(holes, polar, hole => $"G95 G84 Z{endZ.NC(option: NcDecimalPointOption.Without)} R{startZ.NC(option: NcDecimalPointOption.Without)} P500 F{tool.Pitch.NC()}")
                .Line("G80")
                .Line($"{CoolantOff(machine)}")
                .LineIf(polar, "G15")
                .Line(SpindleStop)
                .Raw(MillingReferentPoint)
                .ToString();
        }

        public static string CustomThreadMilling(
            Machine machine,
            CoordinateSystem coordinateSystem,
            MillingThreadCuttingTool tool,
            double diameter,
            double cutSpeed,
            double cutFeed,
            double startZ,
            double endZ,
            int roughPasses,
            double roughStepOver,
            double profStockAllow,
            double exitPlane,
            bool fullCut,
            List<Hole> holes,
            bool polar,
            double safePlane)
        {
            if (tool is null || startZ <= endZ || diameter <= 0 || cutSpeed <= 0 || cutFeed <= 0 || roughPasses < 1 || roughStepOver <= 0 || profStockAllow <= 0) return string.Empty;
            if (machine.MachineType != MachineType.Milling) return string.Empty;
            var spindleSpeed = cutSpeed.ToSpindleSpeed(tool.Diameter, 10);
            return new GCodeBuilder()
                .ToolCall(tool, machine, coordinateSystem, Coolant.General)
                .Line($"{coordinateSystem}{(polar ? "G16 " : string.Empty)} G0 X{holes[0].X.NC(option: NcDecimalPointOption.Without)} Y{holes[0].Y.NC(option: NcDecimalPointOption.Without)} S{spindleSpeed} {Direction(tool)}")
                .Line($"G43 Z{safePlane.NC(option: NcDecimalPointOption.Without)} H{tool.Position} {CoolantOn(machine, Coolant.Through)}")
                .Line($"G0 Z{startZ.NC(option: NcDecimalPointOption.Without)}")
                .HolePattern(holes, polar, hole =>
                    $"G166 X{hole.X.NC(option: NcDecimalPointOption.Without)} Y{hole.Y.NC(option: NcDecimalPointOption.Without)} " +
                    $"T{tool.Diameter.NC(option: NcDecimalPointOption.Without)} D{diameter.NC(option: NcDecimalPointOption.Without)} H{tool.Pitch.NC(option: NcDecimalPointOption.Without)} Z{endZ.NC(option: NcDecimalPointOption.Without)} E{roughPasses} " +
                    $"W{roughStepOver.NC(option: NcDecimalPointOption.Without)} R{profStockAllow.NC(option: NcDecimalPointOption.Without)} U{exitPlane.NC(option: NcDecimalPointOption.Without)} " +
                    $"A{(fullCut ? 1 : 0)} S{spindleSpeed} F{cutFeed.ToFeedPerMin(spindleSpeed, 10)}")
                .Line("G67")
                .Line($"{CoolantOff(machine)}")
                .LineIf(polar, "G15")
                .Line(SpindleStop)
                .Raw(MillingReferentPoint)
                .ToString();
        }

        /// <summary>
        /// Нарезание резьбы
        /// </summary>
        public static string ThreadCutting(Machine machine, CoordinateSystem coordinateSystem, Tool tool, ThreadStandard threadStandard, CuttingType type, double threadDiameter, double threadPitch, double startZ, double endZ, double threadNptPlane, int speed, Coolant coolant = Coolant.General)
        {
            if (tool is null ||
                threadDiameter <= 0 ||
                threadPitch <= 0 ||
                startZ < endZ) return string.Empty;
            var approachDiameter = Thread.ApproachDiameter(threadStandard, type, threadDiameter, threadPitch, endZ, startZ, threadNptPlane).NC(1);
            var endDiameter = Thread.EndDiameter(threadStandard, type, threadDiameter, threadPitch, endZ, startZ, threadNptPlane).NC(2);
            var minStep = Thread.Passes(threadStandard, type, threadPitch)[^2].Microns();
            var lastPass = Thread.Passes(threadStandard, type, threadPitch)[^1];
            var firstPass = Thread.Passes(threadStandard, type, threadPitch)[0].Microns();
            var profile = Thread.ProfileHeight(threadStandard, type, threadPitch).Microns();
            var threadShift = string.Empty;
            if (threadStandard is ThreadStandard.NPT or ThreadStandard.BSPT)
            {
                threadShift = type switch
                {
                    CuttingType.External => $" R-{Thread.IntNptThreadShift(endZ, startZ).NC(2)}",
                    CuttingType.Internal => $" R{Thread.IntNptThreadShift(endZ, startZ).NC(2)}",
                    _ => string.Empty,
                };
            }

            if (machine.MachineType != MachineType.Turning) return string.Empty;
            return new GCodeBuilder()
                .ReferentPoint(machine, leading: true)
                .ToolCall(tool, machine, coordinateSystem, coolant, suppressCoolant: machine.LeadingReferentPoint)
                .Line($"G0 X{approachDiameter} Z{startZ.NC()} S{speed.ToSpindleSpeed(threadDiameter, 100)} {Direction(tool)} G97")
                .Line($"G76 P0201{threadStandard.Profile()} Q{minStep} R{lastPass.NC()}")
                .Line($"G76 X{endDiameter} Z{endZ.NC()} P{profile} Q{firstPass}{threadShift} F{threadPitch.NC()}")
                .Line($"G96 {CoolantOff(machine, coolant)}")
                .ReferentPoint(machine, leading: false)
                .ToString();
        }
    }
}
