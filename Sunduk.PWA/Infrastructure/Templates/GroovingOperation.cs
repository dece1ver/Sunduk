using Sunduk.PWA.Infrastructure.Sequences.Turning;
using Sunduk.PWA.Infrastructure.Tools.Base;
using Sunduk.PWA.Infrastructure.Tools.Turning;
using Sunduk.PWA.Infrastructure.Tools.Turning.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Sunduk.PWA.Util.Util;

namespace Sunduk.PWA.Infrastructure.Templates
{
    public class GroovingOperation : Operation
    {
        public static string CutOffSequence(Machine machine, Material material, GroovingExternalTool tool, double cuttingPoint, 
            double externalDiameter, double internalDiameter, double cornerBlunt, double stepOver, Blunt bluntType, double bluntCustomAngle = 0, double bluntCustomRadius = 0)
        {
            if (tool is null) return string.Empty;
            var zPoint = tool.ZeroPoint == GroovingExternalTool.Point.Right ? cuttingPoint : cuttingPoint - tool.Width;
            var simpleChamferSize = cornerBlunt + tool.CornerRadius / 2;
            var (fullChamferSizeX, fullChamferSizeZ) = Calc.ChamferShifts(bluntCustomAngle, tool.CornerRadius);
            fullChamferSizeX += cornerBlunt;
            fullChamferSizeZ += cornerBlunt;
            var fullChamferRadius = bluntType == Blunt.CustomChamfer ? tool.CornerRadius + bluntCustomRadius : tool.CornerRadius + cornerBlunt;
            var blunt = string.Empty;
            if (cornerBlunt > 0)
            {
                switch (bluntType)
                {
                    case Blunt.SimpleChamfer:
                        blunt = $"G1 X{(externalDiameter - 2 * cornerBlunt - tool.CornerRadius - 1).NC(0)} F{GroovingFeedRough().NC()}\n" +
                            $"G0 X{(externalDiameter + 1).NC()}\n" +
                            $"Z{(zPoint + simpleChamferSize).NC()}\n" +
                            $"G1 X{externalDiameter.NC()} \n" +
                            $"Z{zPoint.NC()} C{(simpleChamferSize).NC()}\n";
                        break;
                    case Blunt.Radius:
                        blunt = $"G1 X{(externalDiameter - 2 * (cornerBlunt - tool.CornerRadius) - 1).NC(0)} F{GroovingFeedRough().NC()}\n" +
                            $"G0 X{(externalDiameter + 1).NC()}\n" +
                            $"Z{(zPoint + fullChamferRadius).NC()}\n" +
                            $"G1 X{externalDiameter.NC()}\n" +
                            $"Z{zPoint.NC()} R{fullChamferRadius.NC()}\n";
                        break;
                    case Blunt.CustomChamfer:
                        if (bluntCustomRadius > 0 && bluntCustomAngle > 0 && bluntCustomAngle < 90)
                        {
                            blunt = $"G1 X{(externalDiameter - 2 * (fullChamferSizeX * Math.Tan(bluntCustomAngle.Radians()) + Calc.ChamferRadiusLengths(bluntCustomAngle, fullChamferRadius).X)).NC()} F{GroovingFeedRough().NC()}\n" +
                            $"G0 X{(externalDiameter + 1).NC()}\n" +
                            $"Z{(zPoint + fullChamferSizeZ + Calc.ChamferRadiusLengths(bluntCustomAngle, fullChamferRadius).Z).NC()}\n" +
                            $"G1 X{externalDiameter.NC()}\n" +
                            $"Z{(zPoint + fullChamferSizeZ).NC()} R{(fullChamferRadius).NC()}\n" +
                            $"Z{zPoint.NC()} A{bluntCustomAngle.NC()} R{(fullChamferRadius).NC()}\n" +
                            $"{(stepOver > 0 ? $"U-{(2 * Calc.ChamferRadiusLengths(bluntCustomAngle, fullChamferRadius).X).NC()}\n" : string.Empty)}";
                        }
                        else if (bluntCustomRadius <= 0 && bluntCustomAngle > 0 && bluntCustomAngle < 90)
                        {
                            blunt = $"G1 X{(externalDiameter - 2 * (fullChamferSizeX * Math.Tan(bluntCustomAngle.Radians())) + Calc.ChamferRadiusLengths(bluntCustomAngle, fullChamferRadius).X).NC()} F{GroovingFeedRough().NC()}\n" +
                            $"G0 X{(externalDiameter + 1).NC()}\n" +
                            $"Z{(zPoint + fullChamferSizeZ).NC()}\n" +
                            $"G1 X{externalDiameter.NC()}\n" +
                            $"Z{zPoint.NC()} A{bluntCustomAngle.NC()}\n";
                        }
                        else
                        {
                            blunt = string.Empty;
                        }
                        break;
                    default:
                        break;
                }
            }
            var feed = string.IsNullOrEmpty(blunt) ? $" F{GroovingFeedRough().NC()}" : string.Empty;
            var cutting = (stepOver == 0 || stepOver >= (externalDiameter - internalDiameter / 2))
                ? $"G1 X{internalDiameter.NC()}{feed}\n"
                : $"G75 R0.5\n" +
                $"G75 X{internalDiameter.NC()} P{stepOver.Microns()} F{GroovingFeedRough().NC()}\n";
            return machine switch
            {
                Machine.GS1500 =>
                TURNING_REFERENT_POINT +
                tool.Description(ToolDescriptionOption.GoodwayLeft) + "\n" +
                $"{CoolantOn(machine)}\n" +
                $"G0 X{(externalDiameter + 2).NC(0)} Z{zPoint.NC()} S{GroovingSpeedRough(material)} {Direction(tool)}\n" +
                blunt +
                cutting +
                $"G0 X{(externalDiameter + 2).NC(0)}\n" +
                $"{CoolantOff(machine)}\n" +
                TURNING_REFERENT_POINT,

                Machine.L230A =>
                tool.Description(ToolDescriptionOption.L230) + "\n" +
                $"{CoolantOn(machine)}\n" +
                $"G0 X{(externalDiameter + 2).NC(0)} Z{zPoint.NC()} S{GroovingSpeedRough(material)} {Direction(tool)}\n" +
                blunt +
                cutting +
                $"G0 X{(externalDiameter + 2).NC(0)}\n" +
                $"{CoolantOff(machine)}\n" +
                TURNING_REFERENT_POINT,

                _ => string.Empty,
            };
        }

        public static string GroovingSequence(Machine machine,
            Material material,
            TurningGroovingTool tool,
            double cuttingPoint,
            double externalDiameter,
            double internalDiameter,
            double width,
            double stepOver,
            double profStockAllow,
            double outerCornerBlunt,
            double innerCornerBlunt,
            Blunt outerBluntType,
            Blunt innerBluntType,
            bool doFinish)
        {
            var external = tool is GroovingExternalTool;
            var clearance = external ? 1 : -1;

            if (innerCornerBlunt < tool.CornerRadius) innerCornerBlunt = tool.CornerRadius;

            var innerBluntSize = innerBluntType is Blunt.SimpleChamfer ? innerCornerBlunt - tool.CornerRadius / 2 : innerCornerBlunt - tool.CornerRadius;

            if (tool is null 
                || (tool is GroovingExternalTool && externalDiameter < internalDiameter) 
                || (tool is GroovingInternalTool && externalDiameter > internalDiameter)
                || width < tool.Width + 2 * (innerBluntSize + profStockAllow)) return string.Empty;

            var bottomStockAllow = external ? profStockAllow : -profStockAllow;

            var startPoint = tool.ZeroPoint == TurningGroovingTool.Point.Right ? cuttingPoint : cuttingPoint - tool.Width;

            var endPoint = startPoint - (width - tool.Width);

            var centerPoint = startPoint - (width - tool.Width) / 2;

            var outerBluntSize = outerBluntType is Blunt.SimpleChamfer ? outerCornerBlunt + tool.CornerRadius / 2 : outerCornerBlunt + tool.CornerRadius;

            var useCycles = width > (tool.Width - 0.5) * 3;

            var useSteps = !(stepOver == 0 || stepOver >= (externalDiameter - internalDiameter) / 2);

            var bluntLetter = outerBluntType is Blunt.SimpleChamfer ? "C" : "R";
            var bluntRight = $"Z{startPoint.NC()}\n";
            var bluntLeft = $"Z{endPoint.NC()}\n";
            string roughBluntRight;
            string roughBluntLeft;
            if (outerCornerBlunt > 0)
            {
                bluntRight = 
                $"Z{(startPoint + outerBluntSize).NC()}\n" +
                $"G1 X{externalDiameter.NC()}\n" +
                $"Z{startPoint.NC()} {bluntLetter}{outerBluntSize.NC()}\n";
                bluntLeft =
                $"Z{(endPoint - outerBluntSize).NC()}\n" +
                $"G1 X{externalDiameter.NC()}\n" +
                $"Z{endPoint.NC()} {bluntLetter}{outerBluntSize.NC()}\n";
            }
            if (profStockAllow > 0)
            {
                roughBluntRight = $"G0 Z{(startPoint - profStockAllow).NC()}\n";
                roughBluntLeft = $"G0 Z{(endPoint + profStockAllow).NC()}\n";
                if (outerCornerBlunt > 0)
                {
                    roughBluntRight =
                    $"Z{(startPoint + outerBluntSize - profStockAllow).NC()}\n" +
                    $"G1 X{(externalDiameter + bottomStockAllow).NC()}\n" +
                    $"Z{(startPoint - profStockAllow).NC()} {bluntLetter}{outerBluntSize.NC()}\n";
                    roughBluntLeft =
                    $"Z{(endPoint - outerBluntSize + profStockAllow).NC()}\n" +
                    $"G1 X{(externalDiameter + bottomStockAllow).NC()}\n" +
                    $"Z{(endPoint + profStockAllow).NC()} {bluntLetter}{outerBluntSize.NC()}\n";
                }
            }
            else
            {
                roughBluntRight =  bluntRight;
                roughBluntLeft = bluntLeft;
            }
            string roughCutting;

            if (useCycles)
            {
                //var rightStepOver = (Math.Abs(centerPoint) - Math.Abs((startPoint - profStockAllow))) / (Math.Floor((Math.Abs(centerPoint) - Math.Abs((startPoint - profStockAllow))) / tool.Width - 0.5));
                //var leftStepOver = (Math.Abs(centerPoint - (tool.Width - 0.5)) - Math.Abs((endPoint + profStockAllow))) / (Math.Floor((Math.Abs(centerPoint) - Math.Abs((endPoint + profStockAllow))) / tool.Width - 0.5));
                roughCutting = 
                    $"G75 R0.5\n" +
                    $"G75 X{(internalDiameter + bottomStockAllow).NC()} Z{(startPoint - profStockAllow).NC()} P{(!useSteps ? (externalDiameter - internalDiameter).Microns() : stepOver.Microns())} Q{(tool.Width - 0.5).Microns()} F{GroovingFeedRough().NC()}\n" +
                    $"G0 Z{(centerPoint - (tool.Width - 0.5)).NC()}\n" +
                    $"G75 X{(internalDiameter + bottomStockAllow).NC()} Z{(endPoint + profStockAllow).NC()} P{(!useSteps ? (externalDiameter - internalDiameter).Microns() : stepOver.Microns())} Q{(tool.Width - 0.5).Microns()} F{GroovingFeedRough().NC()}\n";
            }
            else
            {
                roughCutting = !useSteps
                ? $"G1 X{(internalDiameter + bottomStockAllow).NC()} F{GroovingFeedRough().NC()}\n"
                : $"G75 R0.5\n" +
                $"G75 X{(internalDiameter + bottomStockAllow).NC()} P{stepOver.Microns()} F{GroovingFeedRough().NC()}\n";
            }

            string cutting = string.Empty;

            if ((profStockAllow > 0 || !doFinish) && !useCycles)
            {
                if (tool.Width == width && outerCornerBlunt > 0)
                {
                    cutting += 
                        $"G0 X{(externalDiameter + clearance).NC()}\n" +
                        roughBluntRight +
                        $"X{(internalDiameter + bottomStockAllow).NC()}{(innerBluntSize > 0 ? $" {bluntLetter}{innerBluntSize.NC()}" : string.Empty)}\n" +
                        $"G0 X{(externalDiameter + clearance).NC()}\n" +
                        roughBluntLeft +
                        $"X{(internalDiameter + bottomStockAllow).NC()}{(innerBluntSize > 0 ? $" {bluntLetter}{innerBluntSize.NC()}" : string.Empty)}\n";
                }
                else if (tool.Width == width - profStockAllow * 2 && outerCornerBlunt <= 0)
                {
                    //cutting += $"G0 X{(externalDiameter + clearance).NC()}\n";
                }
                else
                {
                    if (!useCycles ||
                        (useCycles && (profStockAllow > 0 || outerCornerBlunt > 0)))
                    {
                        cutting +=
                        $"G0 X{(externalDiameter + clearance).NC()}\n" +
                        roughBluntRight +
                        $"X{(internalDiameter + bottomStockAllow).NC()}{(innerBluntSize > 0 ? $" {bluntLetter}{innerBluntSize.NC()}" : string.Empty)}\n" +
                        $"Z{centerPoint.NC()}\n" +
                        $"G0 X{(externalDiameter + clearance).NC()}\n" +
                        roughBluntLeft +
                        $"X{(internalDiameter + bottomStockAllow).NC()}{(innerBluntSize > 0 ? $" {bluntLetter}{innerBluntSize.NC()}" : string.Empty)}\n" +
                        $"Z{centerPoint.NC()}\n";
                    }
                }
            }
            if (doFinish)
            {
                if (tool.Width == width && outerCornerBlunt > 0)
                {
                    cutting += 
                        $"G0 X{(externalDiameter + clearance).NC()}\n" +
                        bluntRight +
                        $"X{internalDiameter.NC()}{(innerBluntSize > 0 ? $" {bluntLetter}{innerBluntSize.NC()}" : string.Empty)}\n" +
                        $"G0 X{(externalDiameter + clearance).NC()}\n" +
                        bluntLeft +
                        $"X{internalDiameter.NC()}{(innerBluntSize > 0 ? $" {bluntLetter}{innerBluntSize.NC()}" : string.Empty)}\n";
                }
                else if (tool.Width == width && outerCornerBlunt <= 0)
                {
                    //cutting += $"G0 X{(externalDiameter + clearance).NC()}\n";
                }
                else
                {
                    if (!useCycles || 
                        (useCycles && (profStockAllow > 0 || outerCornerBlunt > 0)))
                    {
                        cutting += 
                        $"{(!useCycles ? $"G0 X{(externalDiameter + clearance).NC()}\n" : string.Empty)}" +
                        bluntRight +
                        $"X{internalDiameter.NC()}{(innerBluntSize > 0 ? $" {bluntLetter}{innerBluntSize.NC()}" : string.Empty)}\n" +
                        $"Z{centerPoint.NC()}\n" +
                        $"G0 X{(externalDiameter + clearance).NC()}\n" +
                        bluntLeft +
                        $"X{internalDiameter.NC()}{(innerBluntSize > 0 ? $" {bluntLetter}{innerBluntSize.NC()}" : string.Empty)}\n" +
                        $"Z{centerPoint.NC()}\n";
                    }
                }
            }

            return machine switch
            {
                Machine.GS1500 =>
                TURNING_REFERENT_POINT +
                tool.Description(ToolDescriptionOption.GoodwayLeft) + "\n" +
                $"{CoolantOn(machine)}\n" +
                (external
                ? $"G0 X{(externalDiameter + clearance * 2).NC(0)} Z{centerPoint.NC()} S{GroovingSpeedRough(material)} {Direction(tool)}\n"
                : $"G0 X{(externalDiameter + clearance * 2).NC(0)} Z2. S{GroovingSpeedRough(material)} {Direction(tool)}\n" +
                $"Z{centerPoint.NC()}\n") +
                roughCutting +
                cutting +
                $"G0 X{(externalDiameter + clearance * 2).NC(0)}\n" +
                (external ? string.Empty : "Z2.\n") +
                $"{CoolantOff(machine)}\n" +
                TURNING_REFERENT_POINT,

                Machine.L230A =>
                tool.Description(ToolDescriptionOption.L230) + "\n" +
                $"{CoolantOn(machine)}\n" +
                (external 
                ? $"G0 X{(externalDiameter + clearance * 2).NC(0)} Z{centerPoint.NC()} S{GroovingSpeedRough(material)} {Direction(tool)}\n"
                : $"G0 X{(externalDiameter + clearance * 2).NC(0)} Z2. S{GroovingSpeedRough(material)} {Direction(tool)}\n" +
                $"Z{centerPoint.NC()}\n") +
                roughCutting +
                cutting +
                $"G0 X{(externalDiameter + clearance * 2).NC(0)}\n" +
                (external ? string.Empty : "Z2.\n") + 
                $"{CoolantOff(machine)}\n" +
                TURNING_REFERENT_POINT,

                _ => string.Empty,
            };
        }
    }
}
