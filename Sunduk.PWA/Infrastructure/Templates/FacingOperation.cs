using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Sequences.Turning;
using Sunduk.PWA.Infrastructure.Tools.Turning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Sunduk.PWA.Util.Util;

namespace Sunduk.PWA.Infrastructure.Templates
{
    public class FacingOperation : Operation
    {
        
        /// <summary>
        /// Торцовка черновая + чистовая
        /// </summary>
        /// <param name="machine">Станок</param>
        /// <param name="material">Материал</param>
        /// <param name="tool">Инструмент</param>
        /// <param name="externalDiameter">Наружный диаметр</param>
        /// <param name="internalDiameter">Внутренний диаметр</param>
        /// <param name="roughStockAllow">Общий припуск</param>
        /// <param name="profStockAllow">Припуск под чистовую</param>
        /// <param name="stepOver">Съем за проход</param>
        /// <param name="seqNo">Номер перехода с циклом</param>
        /// <returns></returns>
        public static string Facing(Machine machine, Material material, TurningExternalTool tool,
            double externalDiameter, double internalDiameter, double roughStockAllow, double profStockAllow,
            double stepOver, (int, int) seqNo, Blunt bluntType, double bluntCustomAngle, double bluntCustomRadius, double cornerBlunt, bool cycleProfStockAllow, bool doFinish)
        {
            if (tool is null ||
                externalDiameter == 0 ||
                externalDiameter < internalDiameter ||
                roughStockAllow < profStockAllow ||
                stepOver == 0) return string.Empty;

            var simpleChamferSize = cornerBlunt + tool.Radius / 2;
            var fullChamferSize = Calc.ChamferShifts(bluntCustomAngle, tool.Radius).Z + cornerBlunt;
            var fullChamferRadius = bluntType == Blunt.CustomChamfer ? tool.Radius + bluntCustomRadius : tool.Radius + cornerBlunt;
            var endZ = cycleProfStockAllow ? 0 : profStockAllow;
            var blunt = $"G0 Z{endZ.NC()}";
            if (cornerBlunt > 0)
            {
                switch (bluntType)
                {
                    case Blunt.SimpleChamfer:
                        blunt = $"G0 Z-{simpleChamferSize.NC()}\n" +
                            $"G1 X{externalDiameter.NC()}\n" +
                            $"Z{endZ.NC()} C{simpleChamferSize.NC()}";
                        break;
                    case Blunt.Radius:
                        blunt = $"G0 Z-{fullChamferRadius.NC()}\n" +
                            $"G1 X{externalDiameter.NC()}\n" +
                            $"Z{endZ.NC()} R{fullChamferRadius.NC()}";
                        break;
                    case Blunt.CustomChamfer:
                        if (bluntCustomRadius > 0 && bluntCustomAngle > 0 && bluntCustomAngle < 90)
                        {
                            blunt = $"G0 Z{(profStockAllow - fullChamferSize - Calc.ChamferRadiusLengths(bluntCustomAngle, fullChamferRadius).Z).NC()}\n" +
                            $"G1 X{externalDiameter.NC()}\n" +
                            $"Z{(profStockAllow - fullChamferSize).NC()} R{fullChamferRadius.NC()}\n" +
                            $"Z{endZ.NC()} A-{bluntCustomAngle.NC()} R{fullChamferRadius.NC()}";
                        }
                        else if (bluntCustomRadius <= 0 && bluntCustomAngle > 0 && bluntCustomAngle < 90)
                        {
                            blunt = $"G0 Z{(profStockAllow - fullChamferSize).NC()}\n" +
                            $"G1 X{externalDiameter.NC()}\n" +
                            $"Z{endZ.NC()} A-{bluntCustomAngle.NC()}";
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
                $"G0 X{(externalDiameter + 5).NC(1)} Z{roughStockAllow.NC()} S{CuttingSpeedRough(material)} {Direction(tool)}\n" +
                $"G72 W{stepOver.NC()} R0.1\n" +
                $"G72 P{seqNo.Item1} Q{seqNo.Item2}{((profStockAllow > 0 && cycleProfStockAllow) ? " W" + profStockAllow.NC() : string.Empty)} F{FeedRough(tool.Radius).NC()}\n" +
                $"N{seqNo.Item1} {blunt}\n" +
                $"N{seqNo.Item2} {(cornerBlunt <= 0 ? "G1 " : string.Empty)}X{internalDiameter.NC()}\n" +
                $"{(doFinish ? $"G70 P{seqNo.Item1} Q{seqNo.Item2} S{CuttingSpeedFinish(material)} F{FeedFinish(tool.Radius).NC()}\n" : string.Empty)}" +
                $"{CoolantOff(machine)}\n" +
                TURNING_REFERENT_POINT,

                Machine.L230A =>
                tool.Description(ToolDescriptionOption.L230) + "\n" +
                $"{CoolantOn(machine)}\n" +
                $"G0 X{(externalDiameter + 5).NC(1)} Z{roughStockAllow.NC()} S{CuttingSpeedRough(material)} {Direction(tool)}\n" +
                $"G72 W{stepOver.NC()} R0.1\n" +
                $"G72 P{seqNo.Item1} Q{seqNo.Item2}{((profStockAllow > 0 && cycleProfStockAllow) ? " W" + profStockAllow.NC() : string.Empty)} F{FeedRough(tool.Radius).NC()}\n" +
                $"N{seqNo.Item1} {blunt}\n" +
                $"N{seqNo.Item2} {(cornerBlunt <= 0 ? "G1 " : string.Empty)}X{internalDiameter.NC()}\n" +
                $"{(doFinish ? $"G70 P{seqNo.Item1} Q{seqNo.Item2} S{CuttingSpeedFinish(material)} F{FeedFinish(tool.Radius).NC()}\n" : string.Empty)}" +
                $"{CoolantOff(machine)}\n" +
                TURNING_REFERENT_POINT,

                _ => string.Empty,
            };
        }

        /// <summary>
        /// Чистовая торцовка по G70 после торцовки
        /// </summary>
        /// <param name="tool">Инструмент</param>
        /// <param name="roughFacingSequence">Черновой переход</param>
        /// <returns></returns>
        public static string FinishFacingCycle(TurningExternalTool tool, Sequence roughFacingSequence)
        {
            Machine machine;
            Material material;
            double externalDiameter;
            double internalDiameter;
            double profStockAllow;
            (int, int) seqNo;
            switch (roughFacingSequence)
            {
                case RoughFacingSequence:
                    machine = (roughFacingSequence as RoughFacingSequence).Machine;
                    material = (roughFacingSequence as RoughFacingSequence).Material;
                    externalDiameter = (roughFacingSequence as RoughFacingSequence).ExternalDiameter;
                    internalDiameter = (roughFacingSequence as RoughFacingSequence).InternalDiameter;
                    profStockAllow = (roughFacingSequence as RoughFacingSequence).RoughStockAllow;
                    seqNo = (roughFacingSequence as RoughFacingSequence).SeqNumbers;
                    break;
                case RoughFacingCycleSequence:
                    machine = (roughFacingSequence as RoughFacingCycleSequence).Machine;
                    material = (roughFacingSequence as RoughFacingCycleSequence).Material;
                    externalDiameter = (roughFacingSequence as RoughFacingCycleSequence).ExternalDiameter;
                    internalDiameter = (roughFacingSequence as RoughFacingCycleSequence).InternalDiameter;
                    profStockAllow = (roughFacingSequence as RoughFacingCycleSequence).RoughStockAllow;
                    seqNo = (roughFacingSequence as RoughFacingCycleSequence).SeqNumbers;
                    break;
                case FacingSequence:
                    machine = (roughFacingSequence as FacingSequence).Machine;
                    material = (roughFacingSequence as FacingSequence).Material;
                    externalDiameter = (roughFacingSequence as FacingSequence).ExternalDiameter;
                    internalDiameter = (roughFacingSequence as FacingSequence).InternalDiameter;
                    profStockAllow = (roughFacingSequence as FacingSequence).RoughStockAllow;
                    seqNo = (roughFacingSequence as FacingSequence).SeqNumbers;
                    break;
                default:
                    return string.Empty;
            }
            

            if (tool is null ||
                externalDiameter == 0 ||
                externalDiameter < internalDiameter) return string.Empty;
            return machine switch
            {
                Machine.GS1500 =>
                TURNING_REFERENT_POINT +
                tool.Description(ToolDescriptionOption.GoodwayLeft) + "\n" +
                $"G0 X{(externalDiameter + 5).NC(1)} Z{profStockAllow.NC()} S{CuttingSpeedFinish(material)} {Direction(tool)}\n" +
                $"G70 P{seqNo.Item1} Q{seqNo.Item2} S{CuttingSpeedFinish(material)} F{FeedFinish(tool.Radius).NC()}\n" +
                $"{CoolantOff(machine)}\n" +
                TURNING_REFERENT_POINT,

                Machine.L230A =>
                tool.Description(ToolDescriptionOption.L230) + "\n" +
                $"{CoolantOn(machine)}\n" +
                $"G0 X{(externalDiameter + 5).NC(1)} Z{profStockAllow.NC()} S{CuttingSpeedFinish(material)} {Direction(tool)}\n" +
                $"G70 P{seqNo.Item1} Q{seqNo.Item2} S{CuttingSpeedFinish(material)} F{FeedFinish(tool.Radius).NC()}\n" +
                $"{CoolantOff(machine)}\n" +
                TURNING_REFERENT_POINT,

                _ => string.Empty,
            };
        }

        /// <summary>
        /// Торцовка чистовая
        /// </summary>
        public static string FinishFacing(Machine machine, Material material, TurningExternalTool tool, double externalDiameter, double internalDiameter, double profStockAllow, Blunt bluntType = default, double bluntCustomAngle = 0, double bluntCustomRadius = 0, double cornerBlunt = 0)
        {
            if (tool is null ||
                externalDiameter == 0 ||
                externalDiameter < internalDiameter) return string.Empty;
            var simpleChamferSize = cornerBlunt + tool.Radius / 2;
            var fullChamferSize = Calc.ChamferShifts(bluntCustomAngle, tool.Radius).Z + cornerBlunt;
            var fullChamferRadius = bluntType == Blunt.CustomChamfer ? tool.Radius + bluntCustomRadius : tool.Radius + cornerBlunt;
            var blunt = $"G0 X{(externalDiameter + 5).NC(1)} Z{profStockAllow.NC()} S{CuttingSpeedFinish(material)} {Direction(tool)}\n" +
                $"G1 ";
            if (cornerBlunt > 0)
            {
                switch (bluntType)
                {
                    case Blunt.SimpleChamfer:
                        blunt = $"G0 X{(externalDiameter + 5).NC(1)} Z{(profStockAllow - simpleChamferSize).NC()} S{CuttingSpeedFinish(material)} {Direction(tool)}\n" +
                            $"G1 X{externalDiameter.NC()} F{FeedFinish(tool.Radius).NC()}\n" +
                            $"Z{profStockAllow.NC()} C{simpleChamferSize.NC()}\n";
                        break;
                    case Blunt.Radius:
                        blunt = $"G0 X{(externalDiameter + 5).NC(1)} Z{(profStockAllow - fullChamferRadius).NC()} S{CuttingSpeedFinish(material)} {Direction(tool)}\n" +
                            $"G1 X{externalDiameter.NC()} F{FeedFinish(tool.Radius).NC()}\n" +
                            $"Z{profStockAllow.NC()} R{fullChamferRadius.NC()}\n";
                        break;
                    case Blunt.CustomChamfer:
                        if (bluntCustomRadius > 0 && bluntCustomAngle > 0 && bluntCustomAngle < 90)
                        {
                            blunt = $"G0 X{(externalDiameter + 5).NC(1)} Z{(profStockAllow - fullChamferSize - Calc.ChamferRadiusLengths(bluntCustomAngle, fullChamferRadius).Z).NC()} S{CuttingSpeedFinish(material)} {Direction(tool)}\n" +
                            $"G1 X{externalDiameter.NC()} F{FeedFinish(tool.Radius).NC()}\n" +
                            $"Z{(profStockAllow - fullChamferSize).NC()} R{fullChamferRadius.NC()}\n" +
                            $"Z{profStockAllow.NC()} A-{bluntCustomAngle.NC()} R{fullChamferRadius.NC()}\n";
                        }
                        else if (bluntCustomRadius <= 0 && bluntCustomAngle > 0 && bluntCustomAngle < 90)
                        {
                            blunt = $"G0 X{(externalDiameter + 5).NC(1)} Z{(profStockAllow - fullChamferSize).NC()} S{CuttingSpeedFinish(material)} {Direction(tool)}\n" +
                            $"G1 X{externalDiameter.NC()} F{FeedFinish(tool.Radius).NC()}\n" +
                            $"Z{profStockAllow.NC()} A-{bluntCustomAngle.NC()}\n";
                        }
                        break;
                    default:
                        break;
                }
            }
            
            var feed = cornerBlunt <= 0 ? $" F{FeedFinish(tool.Radius).NC()}" : string.Empty;
            return machine switch
            {
                Machine.GS1500 =>
                TURNING_REFERENT_POINT +
                tool.Description(ToolDescriptionOption.GoodwayLeft) + "\n" +
                $"{blunt}X{internalDiameter.NC()}{feed}\n" +
                $"W0.5\n" +
                $"{CoolantOff(machine)}\n" +
                TURNING_REFERENT_POINT,

                Machine.L230A =>
                tool.Description(ToolDescriptionOption.L230) + "\n" +
                $"{CoolantOn(machine)}\n" +
                $"{blunt}X{internalDiameter.NC()}{feed}\n" +
                $"W0.5\n" +
                $"{CoolantOff(machine)}\n" +
                TURNING_REFERENT_POINT,

                _ => string.Empty,
            };
        }
    }
}
