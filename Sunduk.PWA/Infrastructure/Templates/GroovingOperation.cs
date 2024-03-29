﻿using Sunduk.PWA.Infrastructure.Sequences.Turning;
using Sunduk.PWA.Infrastructure.Tools.Base;
using Sunduk.PWA.Infrastructure.Tools.Turning;
using Sunduk.PWA.Infrastructure.Tools.Turning.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using static Sunduk.PWA.Infrastructure.Util;

namespace Sunduk.PWA.Infrastructure.Templates
{
    public class GroovingOperation : Operation
    {
        public static string CutOffSequence(Machine machine, GroovingExternalTool tool, double cuttingPoint, 
            double externalDiameter, double internalDiameter, double cornerBlunt, double stepOver, int speedRough, double feedRough, Blunt bluntType, double bluntCustomAngle = 0, double bluntCustomRadius = 0)
        {
            if (tool is null) return string.Empty;
            var zPoint = tool.ZeroPoint == TurningGroovingTool.Point.Right ? cuttingPoint : cuttingPoint - tool.Width;
            var simpleChamferSize = cornerBlunt + tool.CornerRadius / 2;
            var (fullChamferSizeX, fullChamferSizeZ) = Calc.ChamferShifts(bluntCustomAngle, tool.CornerRadius);
            fullChamferSizeX += cornerBlunt;
            fullChamferSizeZ += cornerBlunt;
            var fullChamferRadius = bluntType == Blunt.CustomChamfer ? tool.CornerRadius + bluntCustomRadius : tool.CornerRadius + cornerBlunt;
            var blunt = string.Empty;
            if (cornerBlunt > 0)
            {
                blunt = bluntType switch
                {
                    Blunt.SimpleChamfer =>
                        $"G1 X{(externalDiameter - 2 * cornerBlunt - tool.CornerRadius - 1).NC(0)} F{feedRough.NC()}\n" +
                        $"G0 X{(externalDiameter + 1).NC()}\n" + $"Z{(zPoint + simpleChamferSize).NC()}\n" +
                        $"G1 X{externalDiameter.NC()} \n" + $"Z{zPoint.NC()} C{(simpleChamferSize).NC()}\n",
                    Blunt.Radius =>
                        $"G1 X{(externalDiameter - 2 * (cornerBlunt - tool.CornerRadius) - 1).NC(0)} F{feedRough.NC()}\n" +
                        $"G0 X{(externalDiameter + 1).NC()}\n" + $"Z{(zPoint + fullChamferRadius).NC()}\n" +
                        $"G1 X{externalDiameter.NC()}\n" + $"Z{zPoint.NC()} R{fullChamferRadius.NC()}\n",
                    Blunt.CustomChamfer => bluntCustomRadius switch
                    {
                        > 0 when bluntCustomAngle is > 0 and < 90 =>
                            $"G1 X{(externalDiameter - 2 * (fullChamferSizeX * Math.Tan(bluntCustomAngle.Radians()) + Calc.ChamferRadiusLengths(bluntCustomAngle, fullChamferRadius).X)).NC()} F{feedRough.NC()}\n" +
                            $"G0 X{(externalDiameter + 1).NC()}\n" +
                            $"Z{(zPoint + fullChamferSizeZ + Calc.ChamferRadiusLengths(bluntCustomAngle, fullChamferRadius).Z).NC()}\n" +
                            $"G1 X{externalDiameter.NC()}\n" +
                            $"Z{(zPoint + fullChamferSizeZ).NC()} R{(fullChamferRadius).NC()}\n" +
                            $"Z{zPoint.NC()} A{bluntCustomAngle.NC()} R{(fullChamferRadius).NC()}\n" +
                            $"{(stepOver > 0 ? $"U-{(2 * Calc.ChamferRadiusLengths(bluntCustomAngle, fullChamferRadius).X).NC()}\n" : string.Empty)}",
                        <= 0 when bluntCustomAngle is > 0 and < 90 =>
                            $"G1 X{(externalDiameter - 2 * (fullChamferSizeX * Math.Tan(bluntCustomAngle.Radians())) + Calc.ChamferRadiusLengths(bluntCustomAngle, fullChamferRadius).X).NC()} F{feedRough.NC()}\n" +
                            $"G0 X{(externalDiameter + 1).NC()}\n" + $"Z{(zPoint + fullChamferSizeZ).NC()}\n" +
                            $"G1 X{externalDiameter.NC()}\n" + $"Z{zPoint.NC()} A{bluntCustomAngle.NC()}\n",
                        _ => string.Empty
                    },
                    _ => blunt
                };
            }
            var feed = string.IsNullOrEmpty(blunt) ? $" F{feedRough.NC()}" : string.Empty;
            var cutting = (stepOver == 0 || stepOver >= (externalDiameter - internalDiameter / 2))
                ? $"G1 X{internalDiameter.NC()}{feed}\n"
                : $"G75 R0.5\n" +
                $"G75 X{internalDiameter.NC()} P{stepOver.Microns()} F{feedRough.NC()}\n";
            if (!string.IsNullOrEmpty(blunt))
            {
                cutting = $"G1 X{(externalDiameter - 2 * (fullChamferSizeX * Math.Tan(bluntCustomAngle.Radians())) + Calc.ChamferRadiusLengths(bluntCustomAngle, fullChamferRadius).X).NC()}\n" +
                    cutting;
            }
            return machine switch
            {
                Machine.GS1500 =>
                TurningReferentPoint +
                tool.Description(ToolDescriptionOption.GoodwayLeft) + "\n" +
                $"{CoolantOn(machine)}\n" +
                $"G0 X{(externalDiameter + 2).NC(0)} Z{zPoint.NC()} S{speedRough} {Direction(tool)}\n" +
                blunt +
                cutting +
                $"G0 X{(externalDiameter + 2).NC(0)}\n" +
                $"{CoolantOff(machine)}\n" +
                TurningReferentPoint,

                Machine.L230A =>
                tool.Description(ToolDescriptionOption.L230) + "\n" +
                $"{CoolantOn(machine)}\n" +
                $"G0 X{(externalDiameter + 2).NC(0)} Z{zPoint.NC()} S{speedRough} {Direction(tool)}\n" +
                blunt +
                cutting +
                $"G0 X{(externalDiameter + 2).NC(0)}\n" +
                $"{CoolantOff(machine)}\n" +
                TurningReferentPoint,

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
            bool doFinish, int speedRough, int speedFinish, double feedRough, double feedFinish)
        {
            if (!Enum.IsDefined(typeof(Material), material))
                throw new InvalidEnumArgumentException(nameof(material), (int)material, typeof(Material));
            var external = tool is GroovingExternalTool;
            var clearance = external ? 1 : -1;

            if (innerCornerBlunt < tool.CornerRadius) innerCornerBlunt = tool.CornerRadius;

            var innerBluntSize = innerBluntType is Blunt.SimpleChamfer ? innerCornerBlunt - tool.CornerRadius / 2 : innerCornerBlunt - tool.CornerRadius;

            if ((tool is GroovingExternalTool && externalDiameter < internalDiameter) 
                || (tool is GroovingInternalTool && externalDiameter > internalDiameter)
                || width < tool.Width + 2 * (innerBluntSize + profStockAllow)) return string.Empty;

            var bottomStockAllow = external ? profStockAllow : -profStockAllow;

            var startPoint = tool.ZeroPoint == TurningGroovingTool.Point.Right ? cuttingPoint : cuttingPoint - tool.Width;

            var endPoint = startPoint - (width - tool.Width);

            var centerPoint = startPoint - (width - tool.Width) / 2;

            var outerBluntSize = outerBluntType is Blunt.SimpleChamfer ? outerCornerBlunt + tool.CornerRadius / 2 : outerCornerBlunt + tool.CornerRadius;

            var useCycles = width > (tool.Width - 0.2) * 3;

            var useSteps = !(stepOver == 0 || stepOver >= (externalDiameter - internalDiameter) / 2);

            var outerBluntLetter = outerBluntType is Blunt.SimpleChamfer ? "C" : "R";
            var innerBluntLetter = innerBluntType is Blunt.SimpleChamfer ? "C" : "R";
            var bluntRight = $"Z{startPoint.NC()}\n";
            var bluntLeft = $"Z{endPoint.NC()}\n";
            string roughBluntRight;
            string roughBluntLeft;
            if (outerCornerBlunt > 0)
            {
                bluntRight = 
                $"Z{(startPoint + outerBluntSize).NC()}\n" +
                $"G1 X{externalDiameter.NC()}{(feedFinish is 0 ? string.Empty : $" F{feedFinish.NC()}")}\n" +
                $"Z{startPoint.NC()} {outerBluntLetter}{outerBluntSize.NC()}\n";
                bluntLeft =
                $"Z{(endPoint - outerBluntSize).NC()}\n" +
                $"G1 X{externalDiameter.NC()}\n" +
                $"Z{endPoint.NC()} {outerBluntLetter}{outerBluntSize.NC()}\n";
            }
            if (profStockAllow > 0)
            {
                roughBluntRight = $"G0 Z{(startPoint - profStockAllow).NC()}\n";
                roughBluntLeft = $"G0 Z{(endPoint + profStockAllow).NC()}\n";
                if (outerCornerBlunt > 0)
                {
                    roughBluntRight =
                    $"Z{(startPoint + outerBluntSize - profStockAllow).NC()}\n" +
                    $"G1 X{(externalDiameter + bottomStockAllow).NC()}{(feedFinish is 0 ? string.Empty : $" F{feedFinish.NC()}")}\n" +
                    $"Z{(startPoint - profStockAllow).NC()} {outerBluntLetter}{outerBluntSize.NC()}\n";
                    roughBluntLeft =
                    $"Z{(endPoint - outerBluntSize + profStockAllow).NC()}\n" +
                    $"G1 X{(externalDiameter + bottomStockAllow).NC()}\n" +
                    $"Z{(endPoint + profStockAllow).NC()} {outerBluntLetter}{outerBluntSize.NC()}\n";
                }
            }
            else
            {
                roughBluntRight = bluntRight;
                roughBluntLeft = bluntLeft;
            }
            string roughCutting;

            if (useCycles)
            {
                var numberCuts = Math.Ceiling((width - 2 * profStockAllow - tool.Width) / (tool.Width - 0.2));
                if (numberCuts % 2 != 0) numberCuts++;
                var widthStepOver = (width - 2 * profStockAllow - tool.Width) / numberCuts;
                roughCutting = 
                    $"G75 R0.5\n" +
                    $"G75 X{(internalDiameter + bottomStockAllow).NC()} Z{(startPoint - profStockAllow).NC()} P{(!useSteps ? (externalDiameter - internalDiameter).Microns() : stepOver.Microns())} Q{widthStepOver.Microns()} F{feedRough.NC()}\n" +
                    $"G0 Z{(centerPoint - widthStepOver).NC()}\n" +
                    $"G75 X{(internalDiameter + bottomStockAllow).NC()} Z{(endPoint + profStockAllow).NC()} P{(!useSteps ? (externalDiameter - internalDiameter).Microns() : stepOver.Microns())} Q{widthStepOver.Microns()} F{feedRough.NC()}\n";
            }
            else
            {
                roughCutting = !useSteps
                ? $"G1 X{(internalDiameter + bottomStockAllow).NC()} F{feedRough.NC()}\n"
                : $"G75 R0.5\n" +
                $"G75 X{(internalDiameter + bottomStockAllow).NC()} P{stepOver.Microns()} F{feedRough.NC()}\n";
            }

            var cutting = string.Empty;

            if ((profStockAllow > 0 || !doFinish) && !useCycles)
            {
                if (Math.Abs(tool.Width - width) < 0.001 && outerCornerBlunt > 0)
                {
                    cutting += 
                        $"G0 X{(externalDiameter + clearance).NC()}\n" +
                        roughBluntRight +
                        $"X{(internalDiameter + bottomStockAllow).NC()}{(innerBluntSize > 0 ? $" {innerBluntLetter}{innerBluntSize.NC()}" : string.Empty)}\n" +
                        $"G0 X{(externalDiameter + clearance).NC()}\n" +
                        roughBluntLeft +
                        $"X{(internalDiameter + bottomStockAllow).NC()}{(innerBluntSize > 0 ? $" {innerBluntLetter}{innerBluntSize.NC()}" : string.Empty)}\n";
                }
                else if (Math.Abs(tool.Width - (width - profStockAllow * 2)) < 0.001 && outerCornerBlunt <= 0)
                {
                    //cutting += $"G0 X{(externalDiameter + clearance).NC()}\n";
                }
                else
                {
                    cutting +=
                        $"G0 X{(externalDiameter + clearance).NC()}\n" +
                        roughBluntRight +
                        $"X{(internalDiameter + bottomStockAllow).NC()}{(innerBluntSize > 0 ? $" {innerBluntLetter}{innerBluntSize.NC()}" : string.Empty)}\n" +
                        $"Z{centerPoint.NC()}\n" +
                        $"G0 X{(externalDiameter + clearance).NC()}\n" +
                        roughBluntLeft +
                        $"X{(internalDiameter + bottomStockAllow).NC()}{(innerBluntSize > 0 ? $" {innerBluntLetter}{innerBluntSize.NC()}" : string.Empty)}\n" +
                        $"Z{centerPoint.NC()}\n";
                }
            }

            if (!doFinish)
                return machine switch
                {
                    Machine.GS1500 =>
                        TurningReferentPoint +
                        tool.Description(ToolDescriptionOption.GoodwayLeft) + "\n" +
                        $"{CoolantOn(machine)}\n" +
                        (external
                            ? $"G0 X{(externalDiameter + clearance * 2).NC(0)} Z{centerPoint.NC()} S{speedRough} {Direction(tool)}\n"
                            : $"G0 X{(externalDiameter + clearance * 2).NC(0)} Z2. S{speedRough} {Direction(tool)}\n" +
                              $"Z{centerPoint.NC()}\n") +
                        roughCutting +
                        cutting +
                        $"G0 X{(externalDiameter + clearance * 2).NC(0)}\n" +
                        (external ? string.Empty : "Z2.\n") +
                        $"{CoolantOff(machine)}\n" +
                        TurningReferentPoint,

                    Machine.L230A =>
                        tool.Description(ToolDescriptionOption.L230) + "\n" +
                        $"{CoolantOn(machine)}\n" +
                        (external
                            ? $"G0 X{(externalDiameter + clearance * 2).NC(0)} Z{centerPoint.NC()} S{speedRough} {Direction(tool)}\n"
                            : $"G0 X{(externalDiameter + clearance * 2).NC(0)} Z2. S{speedRough} {Direction(tool)}\n" +
                              $"Z{centerPoint.NC()}\n") +
                        roughCutting +
                        cutting +
                        $"G0 X{(externalDiameter + clearance * 2).NC(0)}\n" +
                        (external ? string.Empty : "Z2.\n") +
                        $"{CoolantOff(machine)}\n" +
                        TurningReferentPoint,

                    _ => string.Empty,
                };

            switch (Math.Abs(tool.Width - width))
            {
                case < 0.001 when outerCornerBlunt > 0:
                    cutting += 
                        $"G0 X{(externalDiameter + clearance).NC()}{(speedFinish == speedRough ? string.Empty : $" S{speedFinish}")}\n" +
                        bluntRight +
                        $"X{internalDiameter.NC()}{(innerBluntSize > 0 ? $" {innerBluntLetter}{innerBluntSize.NC()}" : string.Empty)}\n" +
                        $"G0 X{(externalDiameter + clearance).NC()}\n" +
                        bluntLeft +
                        $"X{internalDiameter.NC()}{(innerBluntSize > 0 ? $" {innerBluntLetter}{innerBluntSize.NC()}" : string.Empty)}\n";
                    break;
                case < 0.001 when outerCornerBlunt <= 0:
                    //cutting += $"G0 X{(externalDiameter + clearance).NC()}\n";
                    break;
                default:
                {
                    if (!useCycles || profStockAllow > 0 || outerCornerBlunt > 0)
                    {
                        cutting += 
                            $"{(!useCycles ? $"G0 X{(externalDiameter + clearance).NC()}{(speedFinish == speedRough ? string.Empty : $" S{speedFinish}")}\n" : string.Empty)}" +
                            bluntRight +
                            $"{(bluntRight.Contains("G1") ? string.Empty : "G1 ")}X{internalDiameter.NC()}{(innerBluntSize > 0 ? $" {innerBluntLetter}{innerBluntSize.NC()}" : string.Empty)}{(feedFinish == feedRough ? string.Empty : $" F{feedFinish.NC()}")}\n" +
                            $"Z{centerPoint.NC()}\n" +
                            $"G0 X{(externalDiameter + clearance).NC()}\n" +
                            bluntLeft +
                            $"{(bluntLeft.Contains("G1") ? string.Empty : "G1 ")}X{internalDiameter.NC()}{(innerBluntSize > 0 ? $" {innerBluntLetter}{innerBluntSize.NC()}" : string.Empty)}\n" +
                            $"Z{centerPoint.NC()}\n";
                    }

                    break;
                }
            }

            return machine switch
            {
                Machine.GS1500 =>
                TurningReferentPoint +
                tool.Description(ToolDescriptionOption.GoodwayLeft) + "\n" +
                $"{CoolantOn(machine)}\n" +
                (external
                ? $"G0 X{(externalDiameter + clearance * 2).NC(0)} Z{centerPoint.NC()} S{speedRough} {Direction(tool)}\n"
                : $"G0 X{(externalDiameter + clearance * 2).NC(0)} Z2. S{speedRough} {Direction(tool)}\n" +
                $"Z{centerPoint.NC()}\n") +
                roughCutting +
                cutting +
                $"G0 X{(externalDiameter + clearance * 2).NC(0)}\n" +
                (external ? string.Empty : "Z2.\n") +
                $"{CoolantOff(machine)}\n" +
                TurningReferentPoint,

                Machine.L230A =>
                tool.Description(ToolDescriptionOption.L230) + "\n" +
                $"{CoolantOn(machine)}\n" +
                (external 
                ? $"G0 X{(externalDiameter + clearance * 2).NC(0)} Z{centerPoint.NC()} S{speedRough} {Direction(tool)}\n"
                : $"G0 X{(externalDiameter + clearance * 2).NC(0)} Z2. S{speedRough} {Direction(tool)}\n" +
                $"Z{centerPoint.NC()}\n") +
                roughCutting +
                cutting +
                $"G0 X{(externalDiameter + clearance * 2).NC(0)}\n" +
                (external ? string.Empty : "Z2.\n") + 
                $"{CoolantOff(machine)}\n" +
                TurningReferentPoint,

                _ => string.Empty,
            };
        }

        public static string FaceGroovingSequence(Machine machine,
            Material material,
            GroovingFaceTool tool,
            double startPoint,
            double externalDiameter,
            double internalDiameter,
            double endPoint,
            double stepOver,
            double profStockAllow,
            double outerCornerBlunt,
            double innerCornerBlunt,
            Blunt outerBluntType,
            Blunt innerBluntType,
            bool doFinish, 
            int speedRough, 
            int speedFinish, 
            double feedRough, 
            double feedFinish)
        {
            var width = (externalDiameter - internalDiameter) / 2;
            const int clearance = 1;
            if (innerCornerBlunt < tool.CornerRadius) innerCornerBlunt = tool.CornerRadius;

            var innerBluntSize = innerBluntType is Blunt.SimpleChamfer ? innerCornerBlunt - tool.CornerRadius / 2 : innerCornerBlunt - tool.CornerRadius;

            if (width < tool.Width + 2 * (innerBluntSize + profStockAllow)
                || Math.Abs(startPoint - endPoint) < 0.001
                ) return string.Empty;

            var upperPoint = tool.ZeroPoint == TurningGroovingTool.Point.Top ? externalDiameter : externalDiameter - tool.Width * 2;

            var lowerPoint = upperPoint - (width - tool.Width) * 2;

            var centerPoint = upperPoint - (width - tool.Width);

            var outerBluntSize = outerBluntType is Blunt.SimpleChamfer ? outerCornerBlunt + tool.CornerRadius / 2 : outerCornerBlunt + tool.CornerRadius;

            var useCycles = width > (tool.Width - 0.2) * 3;

            var useSteps = !(stepOver == 0 || stepOver >= (externalDiameter - internalDiameter) / 2);

            var outerBluntLetter = outerBluntType is Blunt.SimpleChamfer ? "C" : "R";
            var innerBluntLetter = innerBluntType is Blunt.SimpleChamfer ? "C" : "R";
            var bluntUpper = $"X{upperPoint.NC()}\n";
            var bluntLower = $"X{lowerPoint.NC()}\n";
            string roughBluntUpper;
            string roughBluntLower;
            if (outerCornerBlunt > 0)
            {
                bluntUpper =
                $"X{(upperPoint + outerBluntSize * 2).NC()}\n" +
                $"G1 Z{startPoint.NC()} F{feedFinish.NC()}\n" +
                $"X{upperPoint.NC()} {outerBluntLetter}{outerBluntSize.NC()}\n";
                bluntLower =
                $"X{(lowerPoint - outerBluntSize * 2).NC()}\n" +
                $"G1 Z{startPoint.NC()}\n" +
                $"X{lowerPoint.NC()} {outerBluntLetter}{outerBluntSize.NC()}\n";
            }
            if (profStockAllow > 0)
            {
                roughBluntUpper = $"G0 X{(upperPoint - profStockAllow).NC()}\n";
                roughBluntLower = $"G0 X{(lowerPoint + profStockAllow).NC()}\n";
                if (outerCornerBlunt > 0)
                {
                    roughBluntUpper =
                    $"X{(upperPoint + outerBluntSize * 2 - profStockAllow).NC()}\n" +
                    $"G1 Z{(startPoint + profStockAllow).NC()}\n" +
                    $"X{(upperPoint - profStockAllow).NC()} {outerBluntLetter}{outerBluntSize.NC()}\n";
                    roughBluntLower =
                    $"X{(lowerPoint - outerBluntSize * 2 + profStockAllow).NC()}\n" +
                    $"G1 Z{(startPoint + profStockAllow).NC()}\n" +
                    $"X{(lowerPoint + profStockAllow).NC()} {outerBluntLetter}{outerBluntSize.NC()}\n";
                }
            }
            else
            {
                roughBluntUpper = bluntUpper;
                roughBluntLower = bluntLower;
            }
            string roughCutting;

            if (useCycles)
            {
                var numberCuts = Math.Ceiling((width - 2 * profStockAllow - tool.Width) / (tool.Width - 0.2));
                if (numberCuts % 2 != 0) numberCuts++;
                var widthStepOver = (width - 2 * profStockAllow - tool.Width) / numberCuts;
                roughCutting =
                    $"G74 R0.5\n" +
                    $"G74 X{(upperPoint - profStockAllow).NC()} Z{(endPoint + profStockAllow).NC()} P{(widthStepOver + 0.01).Microns()} Q{(!useSteps ? Math.Abs(endPoint - startPoint).Microns() : stepOver.Microns())} F{feedRough.NC()}\n" +
                    $"G0 X{(centerPoint - widthStepOver * 2).NC()}\n" +
                    $"G74 X{(lowerPoint + profStockAllow).NC()} Z{(endPoint + profStockAllow).NC()} P{(widthStepOver + 0.01).Microns()} Q{(!useSteps ? Math.Abs(endPoint - startPoint).Microns() : stepOver.Microns())} F{feedRough.NC()}\n";
            }
            else
            {
                roughCutting = !useSteps
                ? $"G1 Z{(endPoint + profStockAllow).NC()} F{feedRough.NC()}\n"
                : $"G74 R0.5\n" +
                $"G74 Z{(endPoint + profStockAllow).NC()} Q{stepOver.Microns()} F{feedRough.NC()}\n";
            }

            var cutting = string.Empty;

            if ((profStockAllow > 0 || !doFinish) && !useCycles)
            {
                if (Math.Abs(tool.Width - width) < 0.001 && outerCornerBlunt > 0)
                {
                    cutting +=
                        $"G0 Z{(startPoint + clearance).NC()}\n" +
                        roughBluntUpper +
                        $"Z{(endPoint + profStockAllow).NC()}{(innerBluntSize > 0 ? $" {innerBluntLetter}{innerBluntSize.NC()}" : string.Empty)}\n" +
                        $"G0 Z{(startPoint + clearance).NC()}\n" +
                        roughBluntLower +
                        $"Z{(endPoint + profStockAllow).NC()}{(innerBluntSize > 0 ? $" {innerBluntLetter}{innerBluntSize.NC()}" : string.Empty)}\n";
                }
                else if (Math.Abs(tool.Width - (width - profStockAllow * 2)) < 0.001 && outerCornerBlunt <= 0)
                {
                    //cutting += $"G0 X{(externalDiameter + clearance).NC()}\n";
                }
                else
                {
                    cutting +=
                        $"G0 Z{(startPoint + clearance).NC()}\n" +
                        roughBluntUpper +
                        $"Z{(endPoint + profStockAllow).NC()}{(innerBluntSize > 0 ? $" {innerBluntLetter}{innerBluntSize.NC()}" : string.Empty)}\n" +
                        $"X{centerPoint.NC()}\n" +
                        $"G0 Z{(startPoint + clearance).NC()}\n" +
                        roughBluntLower +
                        $"Z{(endPoint + profStockAllow).NC()}{(innerBluntSize > 0 ? $" {innerBluntLetter}{innerBluntSize.NC()}" : string.Empty)}\n" +
                        $"X{centerPoint.NC()}\n";
                }
            }

            if (!doFinish)
                return machine switch
                {
                    Machine.GS1500 =>
                        TurningReferentPoint +
                        tool.Description(ToolDescriptionOption.GoodwayLeft) + "\n" +
                        $"{CoolantOn(machine)}\n" +
                        $"G0 X{centerPoint.NC()} Z{(startPoint + clearance * 2).NC(0)} S{speedFinish} {Direction(tool)}\n" +
                        roughCutting +
                        cutting +
                        $"G0 Z{(startPoint + clearance * 2).NC(0)}\n" +
                        $"{CoolantOff(machine)}\n" +
                        TurningReferentPoint,

                    Machine.L230A =>
                        tool.Description(ToolDescriptionOption.L230) + "\n" +
                        $"{CoolantOn(machine)}\n" +
                        $"G0 X{centerPoint.NC()} Z{(startPoint + clearance * 2).NC(0)} S{speedFinish} {Direction(tool)}\n" +
                        roughCutting +
                        cutting +
                        $"G0 Z{(startPoint + clearance * 2).NC(0)}\n" +
                        $"{CoolantOff(machine)}\n" +
                        TurningReferentPoint,

                    _ => string.Empty,
                };

            switch (Math.Abs(tool.Width - width))
            {
                case < 0.001 when outerCornerBlunt > 0:
                    cutting +=
                        $"G0 Z{(startPoint + clearance).NC()}\n" +
                        bluntUpper +
                        $"Z{endPoint.NC()}{(innerBluntSize > 0 ? $" {innerBluntLetter}{innerBluntSize.NC()}" : string.Empty)}\n" +
                        $"G0 Z{(startPoint + clearance).NC()}\n" +
                        bluntLower +
                        $"Z{endPoint.NC()}{(innerBluntSize > 0 ? $" {innerBluntLetter}{innerBluntSize.NC()}" : string.Empty)}\n";
                    break;
                case < 0.001 when outerCornerBlunt <= 0:
                    //cutting += $"G0 X{(externalDiameter + clearance).NC()}\n";
                    break;
                default:
                {
                    if (!useCycles ||
                        profStockAllow > 0 || outerCornerBlunt > 0) // (useCycles && (profStockAllow > 0 || outerCornerBlunt > 0))) 
                    {
                        cutting +=
                            $"{(!useCycles ? $"G0 Z{(startPoint + clearance).NC()}\n" : string.Empty)}" +
                            bluntUpper +
                            $"Z{endPoint.NC()}{(innerBluntSize > 0 ? $" {innerBluntLetter}{innerBluntSize.NC()}" : string.Empty)}\n" +
                            $"X{centerPoint.NC()}\n" +
                            $"G0 Z{(startPoint + clearance).NC()}\n" +
                            bluntLower +
                            $"Z{endPoint.NC()}{(innerBluntSize > 0 ? $" {innerBluntLetter}{innerBluntSize.NC()}" : string.Empty)}\n" +
                            $"X{centerPoint.NC()}\n";
                    }

                    break;
                }
            }

            return machine switch
            {
                Machine.GS1500 =>
                TurningReferentPoint +
                tool.Description(ToolDescriptionOption.GoodwayLeft) + "\n" +
                $"{CoolantOn(machine)}\n" +
                $"G0 X{centerPoint.NC()} Z{(startPoint + clearance * 2).NC(0)} S{speedRough} {Direction(tool)}\n" +
                roughCutting +
                cutting +
                $"G0 Z{(startPoint + clearance * 2).NC(0)}\n" +
                $"{CoolantOff(machine)}\n" +
                TurningReferentPoint,

                Machine.L230A =>
                tool.Description(ToolDescriptionOption.L230) + "\n" +
                $"{CoolantOn(machine)}\n" +
                $"G0 X{centerPoint.NC()} Z{(startPoint + clearance * 2).NC(0)} S{speedRough} {Direction(tool)}\n" +
                roughCutting +
                cutting +
                $"G0 Z{(startPoint + clearance * 2).NC(0)}\n" +
                $"{CoolantOff(machine)}\n" +
                TurningReferentPoint,

                _ => string.Empty,
            };
        }
    }
}
