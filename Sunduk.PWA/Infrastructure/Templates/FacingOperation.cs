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
        /// Торцовка черновая под G70
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
        public static string RoughFacingCycle(Machine machine, Material material, TurningExternalTool tool,
            double externalDiameter, double internalDiameter, double roughStockAllow, double profStockAllow,
            double stepOver, (int, int) seqNo, Blunt bluntType, double bluntCustomAngle = 0, double bluntCustomRadius = 0, double cornerBlunt = 0)
        {
            if (tool is null ||
                externalDiameter == 0 ||
                externalDiameter < internalDiameter ||
                roughStockAllow < profStockAllow ||
                stepOver == 0) return string.Empty;
            var fullChamferSize = cornerBlunt + tool.Radius / 2;
            var fullChamferRadius = bluntType == Blunt.CustomChamfer ? tool.Radius + bluntCustomRadius : tool.Radius + cornerBlunt;
            var blunt = "G0 Z0.";
            if (cornerBlunt > 0)
            {
                switch (bluntType)
                {
                    case Blunt.SimpleChamfer:
                        blunt = $"G0 Z-{fullChamferSize.NC()}\n" +
                            $"G1 X{externalDiameter.NC()}\n" +
                            $"Z0. C{fullChamferSize.NC()}";
                        break;
                    case Blunt.Radius:
                        blunt = $"G0 Z-{fullChamferRadius.NC()}\n" +
                            $"G1 X{externalDiameter.NC()}\n" +
                            $"Z0. R{fullChamferRadius.NC()}";
                        break;
                    case Blunt.CustomChamfer:
                        if (bluntCustomRadius > 0 && bluntCustomAngle > 0 && bluntCustomAngle < 90)
                        {
                            blunt = $"G0 Z{(profStockAllow - fullChamferSize - Calc.ChamferRadiusLengths(bluntCustomAngle, fullChamferRadius).Z).NC()}\n" +
                            $"G1 X{externalDiameter.NC()}\n" +
                            $"Z{(profStockAllow - fullChamferSize).NC()} R{fullChamferRadius.NC()}\n" +
                            $"Z0. A-{bluntCustomAngle.NC()} R{fullChamferRadius.NC()}";
                        }
                        else if (bluntCustomRadius <= 0 && bluntCustomAngle > 0 && bluntCustomAngle < 90)
                        {
                            blunt = $"G0 Z{(profStockAllow - fullChamferSize).NC()}\n" +
                            $"G1 X{externalDiameter.NC()}\n" +
                            $"Z0. A-{bluntCustomAngle.NC()}";
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
                $"G72 P{seqNo.Item1} Q{seqNo.Item2}{(profStockAllow > 0 ? " W" + profStockAllow.NC() : string.Empty)} F{FeedRough(tool.Radius).NC()}\n" +
                $"N{seqNo.Item1} {blunt}\n" +
                $"N{seqNo.Item2} {(cornerBlunt <= 0 ? "G1 " : string.Empty)}X{internalDiameter.NC()}\n" +
                $"{CoolantOff(machine)}\n" +
                TURNING_REFERENT_POINT,

                Machine.L230A =>
                tool.Description(ToolDescriptionOption.L230) + "\n" +
                $"{CoolantOn(machine)}\n" +
                $"G0 X{(externalDiameter + 5).NC(1)} Z{roughStockAllow.NC()} S{CuttingSpeedRough(material)} {Direction(tool)}\n" +
                $"G72 W{stepOver.NC()} R0.1\n" +
                $"G72 P{seqNo.Item1} Q{seqNo.Item2}{(profStockAllow > 0 ? " W" + profStockAllow.NC() : string.Empty)} F{FeedRough(tool.Radius).NC()}\n" +
                $"N{seqNo.Item1} {blunt}\n" +
                $"N{seqNo.Item2} {(cornerBlunt <= 0 ? "G1 " : string.Empty)}X{internalDiameter.NC()}\n" +
                $"{CoolantOff(machine)}\n" +
                TURNING_REFERENT_POINT,

                _ => string.Empty,
            };
        }

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
            double stepOver, (int, int) seqNo, Blunt bluntType = default, double bluntCustomAngle = 0, double bluntCustomRadius = 0, double cornerBlunt = 0)
        {
            if (tool is null ||
                externalDiameter == 0 ||
                externalDiameter < internalDiameter ||
                roughStockAllow < profStockAllow ||
                stepOver == 0) return string.Empty;

            var fullChamferSize = cornerBlunt + tool.Radius / 2;
            var fullChamferRadius = bluntType == Blunt.CustomChamfer ? tool.Radius + bluntCustomRadius : tool.Radius + cornerBlunt;
            var blunt = "G0 Z0.";
            if (cornerBlunt > 0)
            {
                switch (bluntType)
                {
                    case Blunt.SimpleChamfer:
                        blunt = $"G0 Z-{fullChamferSize.NC()}\n" +
                            $"G1 X{externalDiameter.NC()}\n" +
                            $"Z0. C{fullChamferSize.NC()}";
                        break;
                    case Blunt.Radius:
                        blunt = $"G0 Z-{fullChamferRadius.NC()}\n" +
                            $"G1 X{externalDiameter.NC()}\n" +
                            $"Z0. R{fullChamferRadius.NC()}";
                        break;
                    case Blunt.CustomChamfer:
                        if (bluntCustomRadius > 0 && bluntCustomAngle > 0 && bluntCustomAngle < 90)
                        {
                            blunt = $"G0 Z{(profStockAllow - fullChamferSize - Calc.ChamferRadiusLengths(bluntCustomAngle, fullChamferRadius).Z).NC()}\n" +
                            $"G1 X{externalDiameter.NC()}\n" +
                            $"Z{(profStockAllow - fullChamferSize).NC()} R{fullChamferRadius.NC()}\n" +
                            $"Z0. A-{bluntCustomAngle.NC()} R{fullChamferRadius.NC()}";
                        }
                        else if (bluntCustomRadius <= 0 && bluntCustomAngle > 0 && bluntCustomAngle < 90)
                        {
                            blunt = $"G0 Z{(profStockAllow - fullChamferSize).NC()}\n" +
                            $"G1 X{externalDiameter.NC()}\n" +
                            $"Z0. A-{bluntCustomAngle.NC()}";
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
                $"G72 P{seqNo.Item1} Q{seqNo.Item2}{(profStockAllow > 0 ? " W" + profStockAllow.NC() : string.Empty)} F{FeedRough(tool.Radius).NC()}\n" +
                $"N{seqNo.Item1} {blunt}\n" +
                $"N{seqNo.Item2} {(cornerBlunt <= 0 ? "G1 " : string.Empty)}X{internalDiameter.NC()}\n" +
                $"G70 P{seqNo.Item1} Q{seqNo.Item2} S{CuttingSpeedFinish(material)} F{FeedFinish(tool.Radius).NC()}\n" +
                $"{CoolantOff(machine)}\n" +
                TURNING_REFERENT_POINT,

                Machine.L230A =>
                tool.Description(ToolDescriptionOption.L230) + "\n" +
                $"{CoolantOn(machine)}\n" +
                $"G0 X{(externalDiameter + 5).NC(1)} Z{roughStockAllow.NC()} S{CuttingSpeedRough(material)} {Direction(tool)}\n" +
                $"G72 W{stepOver.NC()} R0.1\n" +
                $"G72 P{seqNo.Item1} Q{seqNo.Item2}{(profStockAllow > 0 ? " W" + profStockAllow.NC() : string.Empty)} F{FeedRough(tool.Radius).NC()}\n" +
                $"N{seqNo.Item1} {blunt}\n" +
                $"N{seqNo.Item2} {(cornerBlunt <= 0 ? "G1 " : string.Empty)}X{internalDiameter.NC()}\n" +
                $"G70 P{seqNo.Item1} Q{seqNo.Item2} S{CuttingSpeedFinish(material)} F{FeedFinish(tool.Radius).NC()}\n" +
                $"{CoolantOff(machine)}\n" +
                TURNING_REFERENT_POINT,

                _ => string.Empty,
            };
        }

        /// <summary>
        /// Торцовка черновая
        /// </summary>
        public static string RoughFacing(Machine machine, Material material, TurningExternalTool tool,
            double externalDiameter, double internalDiameter, double roughStockAllow, double profStockAllow,
            double stepOver, (int, int) seqNo, Blunt bluntType = default, double bluntCustomAngle = 0, double bluntCustomRadius = 0, double cornerBlunt = 0)
        {
            if (tool is null ||
                externalDiameter == 0 ||
                externalDiameter < internalDiameter ||
                roughStockAllow < profStockAllow ||
                stepOver == 0) return string.Empty;
            var fullChamferSize = cornerBlunt + tool.Radius / 2;
            var fullChamferRadius = bluntType == Blunt.CustomChamfer ? tool.Radius + bluntCustomRadius : tool.Radius + cornerBlunt;
            var blunt = $"G0 Z{profStockAllow.NC()}";
            if (cornerBlunt > 0)
            {
                switch (bluntType)
                {
                    case Blunt.SimpleChamfer:
                        blunt = $"G0 Z{(profStockAllow - fullChamferSize).NC()}\n" +
                            $"G1 X{externalDiameter.NC()}\n" +
                            $"Z{profStockAllow.NC()} C{fullChamferSize.NC()}";
                        break;
                    case Blunt.Radius:
                        blunt = $"G0 Z{(profStockAllow - fullChamferRadius).NC()}\n" +
                            $"G1 X{externalDiameter.NC()}\n" +
                            $"Z{profStockAllow.NC()} R{fullChamferRadius.NC()}";
                        break;
                    case Blunt.CustomChamfer:
                        if (bluntCustomRadius > 0 && bluntCustomAngle > 0 && bluntCustomAngle < 90)
                        {
                            blunt = $"G0 Z{(profStockAllow - fullChamferSize - Calc.ChamferRadiusLengths(bluntCustomAngle, fullChamferRadius).Z).NC()}\n" +
                            $"G1 X{externalDiameter.NC()}\n" +
                            $"Z{(profStockAllow - fullChamferSize).NC()} R{fullChamferRadius.NC()}\n" +
                            $"Z{profStockAllow.NC()} A-{bluntCustomAngle.NC()} R{fullChamferRadius.NC()}";
                        }
                        else if (bluntCustomRadius <= 0 && bluntCustomAngle > 0 && bluntCustomAngle < 90)
                        {
                            blunt = $"G0 Z{(profStockAllow - fullChamferSize).NC()}\n" +
                            $"G1 X{externalDiameter.NC()}\n" +
                            $"Z{profStockAllow.NC()} A-{bluntCustomAngle.NC()}";
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
                $"G72 P{seqNo.Item1} Q{seqNo.Item2} F{FeedRough(tool.Radius).NC()}\n" +
                $"N{seqNo.Item1} G0 Z{profStockAllow.NC()}\n" +
                $"N{seqNo.Item2} {(cornerBlunt <= 0 ? "G1 " : string.Empty)}X{internalDiameter.NC()}\n" +
                "M59\n" +
                TURNING_REFERENT_POINT,

                Machine.L230A =>
                tool.Description(ToolDescriptionOption.L230) + "\n" +
                $"{CoolantOn(machine)}\n" +
                $"G0 X{(externalDiameter + 5).NC(1)} Z{roughStockAllow.NC()} S{CuttingSpeedRough(material)} {Direction(tool)}\n" +
                $"G72 W{stepOver.NC()} R0.1\n" +
                $"G72 P{seqNo.Item1} Q{seqNo.Item2} F{FeedRough(tool.Radius).NC()}\n" +
                $"N{seqNo.Item1} {blunt}\n" +
                $"N{seqNo.Item2} {(cornerBlunt <= 0 ? "G1 " : string.Empty)}X{internalDiameter.NC()}\n" +
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
        public static string FinishFacingCycleFromFacing(TurningExternalTool tool, FacingSequence roughFacingSequence)
        {
            Machine machine = roughFacingSequence.Machine;
            Material material = roughFacingSequence.Material;
            double externalDiameter = roughFacingSequence.ExternalDiameter;
            double internalDiameter = roughFacingSequence.InternalDiameter;
            double profStockAllow = roughFacingSequence.RoughStockAllow;
            (int, int) seqNo = roughFacingSequence.SeqNumbers;

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
        /// Чистовая торцовка по G70 после торцовки под G70
        /// </summary>
        /// <param name="tool">Инструмент</param>
        /// <param name="roughFacingSequence">Черновой переход</param>
        /// <returns></returns>
        public static string FinishFacingCycleFromRoughCycleFacing(TurningExternalTool tool, RoughFacingCycleSequence roughFacingSequence)
        {
            Machine machine = roughFacingSequence.Machine;
            Material material = roughFacingSequence.Material;
            double externalDiameter = roughFacingSequence.ExternalDiameter;
            double internalDiameter = roughFacingSequence.InternalDiameter;
            double profStockAllow = roughFacingSequence.RoughStockAllow;
            (int, int) seqNo = roughFacingSequence.SeqNumbers;

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
        /// Чистовая торцовка по G70 после черновой торцовки
        /// </summary>
        /// <param name="tool">Инструмент</param>
        /// <param name="roughFacingSequence">Черновой переход</param>
        /// <returns></returns>
        public static string FinishFacingCycleFromRoughFacing(TurningExternalTool tool, RoughFacingSequence roughFacingSequence)
        {
            Machine machine = roughFacingSequence.Machine;
            Material material = roughFacingSequence.Material;
            double externalDiameter = roughFacingSequence.ExternalDiameter;
            double internalDiameter = roughFacingSequence.InternalDiameter;
            double profStockAllow = roughFacingSequence.RoughStockAllow;
            (int, int) seqNo = roughFacingSequence.SeqNumbers;

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
            var fullChamferSize = cornerBlunt + tool.Radius / 2;
            var fullChamferRadius = bluntType == Blunt.CustomChamfer ? tool.Radius + bluntCustomRadius : tool.Radius + cornerBlunt;
            var blunt = $"G0 X{(externalDiameter + 5).NC(1)} Z{profStockAllow.NC()} S{CuttingSpeedFinish(material)} {Direction(tool)}\n" +
                $"G1 ";
            if (cornerBlunt > 0)
            {
                switch (bluntType)
                {
                    case Blunt.SimpleChamfer:
                        blunt = $"G0 X{(externalDiameter + 5).NC(1)} Z{(profStockAllow - fullChamferSize).NC()} S{CuttingSpeedFinish(material)} {Direction(tool)}\n" +
                            $"G1 X{externalDiameter.NC()} F{FeedFinish(tool.Radius).NC()}\n" +
                            $"Z{profStockAllow.NC()} C{fullChamferSize.NC()}\n";
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
                $"{CoolantOff(machine)}\n" +
                TURNING_REFERENT_POINT,

                Machine.L230A =>
                tool.Description(ToolDescriptionOption.L230) + "\n" +
                $"{CoolantOn(machine)}\n" +
                $"{blunt}X{internalDiameter.NC()}{feed}\n" +
                $"{CoolantOff(machine)}\n" +
                TURNING_REFERENT_POINT,

                _ => string.Empty,
            };
        }
    }
}
