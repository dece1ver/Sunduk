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
        public static string TurningTapping(Machine machine, TappingTool tool, double cutSpeed, double startZ, double endZ)
        {
            if (tool is null ||
                startZ <= endZ) return string.Empty;
            string approach = startZ > 0
                ? $"G0 X0. Z{startZ.NC()} S{cutSpeed.ToSpindleSpeed(tool.Diameter, 10)} {Direction(tool)} G97\n"
                : $"G0 X0. Z{SafeApproachDistance.NC()} S{((int)cutSpeed).ToSpindleSpeed(tool.Diameter, 10)} {Direction(tool)} G97\nZ{startZ.NC()}\n";
            string exit = startZ > 0
                ? string.Empty
                : $"G0 Z{SafeApproachDistance.NC()}\n";
            return machine switch
            {
                Machine.GS1500 =>
                TurningReferentPoint +
                tool.Description(ToolDescriptionOption.GoodwayLeft) + "\n" +
                approach +
                $"G84 Z{endZ.NC()} P1000 F{tool.Pitch.NC()}\n" +
                $"G80\n" +
                exit +
                $"G96 {CoolantOff(machine)}\n" +
                TurningReferentPoint,

                Machine.L230A =>
                tool.Description(ToolDescriptionOption.L230) + "\n" +
                $"{CoolantOn(machine)}\n" +
                approach +
                $"G84 Z{endZ.NC()} P1000 F{tool.Pitch.NC()}\n" +
                $"G80\n" +
                exit +
                $"G96 {CoolantOff(machine)}\n" +
                TurningReferentPoint,
                _ => string.Empty
            };
        }

        public static string MillingTapping(Machine machine, CoordinateSystem coordinateSystem, MillingTappingTool tool, double cutSpeed, double startZ, double endZ, List<Hole> holes, bool polar = false)
        {
            if (tool is null || startZ <= endZ) return string.Empty;
            var spindleSpeed = cutSpeed.ToSpindleSpeed(tool.Diameter, 10);
            string result = machine switch
            {

                Machine.A110 =>
                tool.Description(ToolDescriptionOption.MillingToolChange) + "\n" +
                $"{coordinateSystem}{(polar ? "G16 " : string.Empty)} G0 X{holes[0].X.NC(option: NcDecimalPointOption.Without)} Y{holes[0].Y.NC(option: NcDecimalPointOption.Without)} S{spindleSpeed} {Direction(tool)}\n" +
                $"G43 Z{startZ.NC(option: NcDecimalPointOption.Without)} H{tool.Position} {CoolantOn(machine, Coolant.Through)}\n" +
                $"G95 G84 Z{endZ.NC(option: NcDecimalPointOption.Without)} R{startZ.NC(option: NcDecimalPointOption.Without)} P500 F{tool.Pitch.NC()}\n",
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
                    $"{(polar ? "G15\n" : string.Empty)}" +
                    SpindleStop + "\n" +
                    MillingReferentPoint,
                    _ => string.Empty
                };
            }
            return result;
        }

        /// <summary>
        /// Нарезание резьбы
        /// </summary>
        public static string ThreadCutting(Machine machine, Tool tool, ThreadStandard threadStandard, CuttingType type, double threadDiameter, double threadPitch, double startZ, double endZ, double threadNPTPlane)
        {
            if (tool is null ||
                threadDiameter <= 0 ||
                threadPitch <= 0 ||
                startZ < endZ) return string.Empty;
            string approachDiameter = Thread.ApproachDiameter(threadStandard, type, threadDiameter, threadPitch, endZ, startZ, threadNPTPlane).NC(1);
            string endDiameter = Thread.EndDiameter(threadStandard, type, threadDiameter, threadPitch, endZ, startZ, threadNPTPlane).NC(2);
            int minStep = Thread.Passes(threadStandard, type, threadPitch)[^2].Microns();
            double lastPass = Thread.Passes(threadStandard, type, threadPitch)[^1];
            int firstPass = Thread.Passes(threadStandard, type, threadPitch)[0].Microns();
            int profile = Thread.ProfileHeight(threadStandard, type, threadPitch).Microns();
            string threadShift = string.Empty;
            if (threadStandard == ThreadStandard.NPT)
            {
                threadShift = type switch
                {
                    CuttingType.External => $" R-{Thread.IntNptThreadShift(endZ, startZ).NC(2)}",
                    CuttingType.Internal => $" R{Thread.IntNptThreadShift(endZ, startZ).NC(2)}",
                    _ => string.Empty,
                };
            }

            return machine switch
            {
                Machine.GS1500 =>
                TurningReferentPoint +
                tool.Description(ToolDescriptionOption.GoodwayLeft) + "\n" +
                $"G0 X{approachDiameter} Z{startZ.NC()} S{120.ToSpindleSpeed(threadDiameter, 100)} {Direction(tool)} G97\n" +
                $"G76 P0201{Thread.Profile(threadStandard)} Q{minStep} R{lastPass.NC()}\n" +
                $"G76 X{endDiameter} Z{endZ.NC()} P{profile} Q{firstPass}{threadShift} F{threadPitch.NC()}\n" +
                $"G96 {CoolantOff(machine)}\n" +
                TurningReferentPoint,

                Machine.L230A =>
                tool.Description(ToolDescriptionOption.L230) + "\n" +
                $"{CoolantOn(machine)}\n" +
                $"G0 X{approachDiameter} Z{startZ.NC()} S{120.ToSpindleSpeed(threadDiameter, 100)} {Direction(tool)} G97\n" +
                $"G76 P0201{Thread.Profile(threadStandard)} Q{minStep} R{lastPass.NC()}\n" +
                $"G76 X{endDiameter} Z{endZ.NC()} P{profile} Q{firstPass}{threadShift} F{threadPitch.NC()}\n" +
                $"G96 {CoolantOff(machine)}\n" +
                TurningReferentPoint,

                _ => string.Empty,
            };
        }
    }
}
