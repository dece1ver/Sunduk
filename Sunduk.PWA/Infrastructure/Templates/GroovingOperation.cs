using Sunduk.PWA.Infrastructure.Sequences.Turning;
using Sunduk.PWA.Infrastructure.Tools.Turning;
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
                : $"G74 R0.5\n" +
                $"G74 X{internalDiameter.NC()} P{stepOver.Microns()} F{GroovingFeedRough().NC()}\n";
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
    }
}
