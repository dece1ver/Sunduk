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
            double externalDiameter, double internalDiameter, double cornerBlunt, double stepOver, Blunt bluntType, double bluntCustonAngle = 0, double bluntCustomRadius = 0)
        {
            if (tool is null) return string.Empty;
            var zPoint = tool.ZeroPoint == GroovingExternalTool.Point.Right ? cuttingPoint : cuttingPoint - tool.Width;
            var cutting = (stepOver == 0 || stepOver >= (externalDiameter - internalDiameter / 2))
                ? $"G1 X{internalDiameter.NC()} F{GroovingFeedRough().NC()}\n"
                : $"G74 R0.5\n" +
                $"G74 X{internalDiameter.NC()} P{stepOver.Microns()} F{GroovingFeedRough().NC()}\n";
            var blunt = string.Empty;
            if (cornerBlunt > 0)
            {
                switch (bluntType)
                {
                    case Blunt.SimpleChamfer:
                        blunt = $"G1 X{(externalDiameter - 2 * cornerBlunt - tool.CornerRadius - 1).NC(0)}\n" +
                            $"G0 X{(externalDiameter + 1).NC()}\n" +
                            $"Z{(cuttingPoint + cornerBlunt + tool.CornerRadius / 2).NC()}\n" +
                            $"G1 X{externalDiameter.NC()}\n" +
                            $"Z{cuttingPoint.NC()} C{(cornerBlunt + tool.CornerRadius / 2).NC()}\n" +
                            $"U-1.\n";
                        break;
                    case Blunt.Radius:
                        blunt = $"G1 X{(externalDiameter - 2 * (cornerBlunt - tool.CornerRadius) - 1).NC(0)}\n" +
                            $"G0 X{(externalDiameter + 1).NC()}\n" +
                            $"Z{(cuttingPoint + cornerBlunt + tool.CornerRadius).NC()}\n" +
                            $"G1 X{externalDiameter.NC()}\n" +
                            $"Z{cuttingPoint.NC()} R{(cornerBlunt + tool.CornerRadius).NC()}\n" +
                            $"U-1.\n";
                        break;
                    case Blunt.CustomChamfer:
                        if (bluntCustomRadius > 0 && bluntCustonAngle > 0 && bluntCustonAngle < 90)
                        {
                            blunt = $"G1 X{(externalDiameter - 2 * (cornerBlunt * Math.Tan(bluntCustonAngle.Radians())) - tool.CornerRadius - 1).NC(0)}\n" +
                            $"G0 X{(externalDiameter + 1).NC()}\n" +
                            $"Z{(cuttingPoint + cornerBlunt + tool.CornerRadius / 2 + bluntCustomRadius + tool.CornerRadius).NC()}\n" +
                            $"G1 X{externalDiameter.NC()}\n" +
                            $"Z{(cuttingPoint + cornerBlunt + tool.CornerRadius / 2).NC()} R{(bluntCustomRadius + tool.CornerRadius).NC()}\n" +
                            $"Z{cuttingPoint.NC()} A{bluntCustonAngle.NC()} R{(bluntCustomRadius + tool.CornerRadius).NC()}\n" +
                            $"U-1.\n";
                        }
                        else if (bluntCustomRadius <= 0 && bluntCustonAngle > 0 && bluntCustonAngle < 90)
                        {
                            blunt = $"G1 X{(externalDiameter - 2 * (cornerBlunt * Math.Tan(bluntCustonAngle.Radians())) - tool.CornerRadius - 1).NC(0)}\n" +
                            $"G0 X{(externalDiameter + 1).NC()}\n" +
                            $"Z{(cuttingPoint + cornerBlunt + tool.CornerRadius / 2).NC()}\n" +
                            $"G1 X{externalDiameter.NC()}\n" +
                            $"Z{cuttingPoint.NC()} A{bluntCustonAngle.NC()}\n" +
                            $"U-1.\n";
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
