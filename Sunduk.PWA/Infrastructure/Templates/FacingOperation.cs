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
            double stepOver, (int, int) seqNo)
        {
            if (tool is null ||
                externalDiameter == 0 ||
                externalDiameter < internalDiameter ||
                roughStockAllow < profStockAllow ||
                stepOver == 0) return string.Empty;
            return machine switch
            {
                Machine.GS1500 =>
                TURNING_REFERENT_POINT +
                tool.Description(ToolDescriptionOption.GoodwayLeft) + "\n" +
                $"G0 X{(externalDiameter + 5).NC(1)} Z{roughStockAllow.NC()} S{CuttingSpeedRough(material)} {Direction(tool)}\n" +
                $"G72 W{stepOver.NC()} R0.1\n" +
                $"G72 P{seqNo.Item1} Q{seqNo.Item2}{(profStockAllow > 0 ? " W" + profStockAllow.NC() : string.Empty)} F{FeedRough(tool.Radius).NC()}\n" +
                $"N{seqNo.Item1} G0 Z0.\n" +
                $"N{seqNo.Item2} G1 X{internalDiameter.NC()}\n" +
                $"{CoolantOff(machine)}\n" +
                TURNING_REFERENT_POINT,

                Machine.L230A =>
                tool.Description(ToolDescriptionOption.L230) + "\n" +
                $"{CoolantOn(machine)}\n" +
                $"G0 X{(externalDiameter + 5).NC(1)} Z{roughStockAllow.NC()} S{CuttingSpeedRough(material)} {Direction(tool)}\n" +
                $"G72 W{stepOver.NC()} R0.1\n" +
                $"G72 P{seqNo.Item1} Q{seqNo.Item2}{(profStockAllow > 0 ? " W" + profStockAllow.NC() : string.Empty)} F{FeedRough(tool.Radius).NC()}\n" +
                $"N{seqNo.Item1} G0 Z0.\n" +
                $"N{seqNo.Item2} G1 X{internalDiameter.NC()}\n" +
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
            double stepOver, (int, int) seqNo)
        {
            if (tool is null ||
                externalDiameter == 0 ||
                externalDiameter < internalDiameter ||
                roughStockAllow < profStockAllow ||
                stepOver == 0) return string.Empty;
            return machine switch
            {
                Machine.GS1500 =>
                TURNING_REFERENT_POINT +
                tool.Description(ToolDescriptionOption.GoodwayLeft) + "\n" +
                $"G0 X{(externalDiameter + 5).NC(1)} Z{roughStockAllow.NC()} S{CuttingSpeedRough(material)} {Direction(tool)}\n" +
                $"G72 W{stepOver.NC()} R0.1\n" +
                $"G72 P{seqNo.Item1} Q{seqNo.Item2}{(profStockAllow > 0 ? " W" + profStockAllow.NC() : string.Empty)} F{FeedRough(tool.Radius).NC()}\n" +
                $"N{seqNo.Item1}G0 Z0.\n" +
                $"N{seqNo.Item2}G1 X{internalDiameter.NC()}\n" +
                $"{(profStockAllow > 0 ? $"G70 P{seqNo.Item1} Q{seqNo.Item2} S{CuttingSpeedFinish(material)} F{FeedFinish(tool.Radius).NC()}\n" : string.Empty)}" +
                $"{CoolantOff(machine)}\n" +
                TURNING_REFERENT_POINT,

                Machine.L230A =>
                tool.Description(ToolDescriptionOption.L230) + "\n" +
                $"{CoolantOn(machine)}\n" +
                $"G0 X{(externalDiameter + 5).NC(1)}Z{roughStockAllow.NC()} S{CuttingSpeedRough(material)} {Direction(tool)}\n" +
                $"G72 W{stepOver.NC()} R0.1\n" +
                $"G72 P{seqNo.Item1} Q{seqNo.Item2}{(profStockAllow > 0 ? " W" + profStockAllow.NC() : string.Empty)} F{FeedRough(tool.Radius).NC()}\n" +
                $"N{seqNo.Item1} G0 Z0.\n" +
                $"N{seqNo.Item2} G1 X{internalDiameter.NC()}\n" +
                $"{(profStockAllow > 0 ? $"G70 P{seqNo.Item1} Q{seqNo.Item2} S{CuttingSpeedFinish(material)} F{FeedFinish(tool.Radius).NC()}\n" : string.Empty)}" +
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
            double stepOver, (int, int) seqNo)
        {
            if (tool is null ||
                externalDiameter == 0 ||
                externalDiameter < internalDiameter ||
                roughStockAllow < profStockAllow ||
                stepOver == 0) return string.Empty;
            return machine switch
            {
                Machine.GS1500 =>
                TURNING_REFERENT_POINT +
                tool.Description(ToolDescriptionOption.GoodwayLeft) + "\n" +
                $"G0 X{(externalDiameter + 5).NC(1)}Z{roughStockAllow.NC()} S{CuttingSpeedRough(material)} {Direction(tool)}\n" +
                $"G72 W{stepOver.NC()} R0.1\n" +
                $"G72 P{seqNo.Item1} Q{seqNo.Item2} F{FeedRough(tool.Radius).NC()}\n" +
                $"N{seqNo.Item1} G0 Z{profStockAllow.NC()}\n" +
                $"N{seqNo.Item2} G1 X{internalDiameter.NC()}\n" +
                "M59\n" +
                TURNING_REFERENT_POINT,

                Machine.L230A =>
                tool.Description(ToolDescriptionOption.L230) + "\n" +
                $"{CoolantOn(machine)}\n" +
                $"G0 X{(externalDiameter + 5).NC(1)} Z{roughStockAllow.NC()} S{CuttingSpeedRough(material)} {Direction(tool)}\n" +
                $"G72 W{stepOver.NC()} R0.1\n" +
                $"G72 P{seqNo.Item1} Q{seqNo.Item2} F{FeedRough(tool.Radius).NC()}\n" +
                $"N{seqNo.Item1} G0 Z{profStockAllow.NC()}\n" +
                $"N{seqNo.Item2} G1 X{internalDiameter.NC()}\n" +
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
                $"G70 P{seqNo.Item1}Q{seqNo.Item2} S{CuttingSpeedFinish(material)} F{FeedFinish(tool.Radius).NC()}\n" +
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
        public static string FinishFacing(Machine machine, Material material, TurningExternalTool tool, double externalDiameter, double internalDiameter, double profStockAllow)
        {
            if (tool is null ||
                externalDiameter == 0 ||
                externalDiameter < internalDiameter) return string.Empty;
            return machine switch
            {
                Machine.GS1500 =>
                TURNING_REFERENT_POINT +
                tool.Description(ToolDescriptionOption.GoodwayLeft) + "\n" +
                $"G0 X{(externalDiameter + 5).NC(1)} Z{profStockAllow.NC()} S{CuttingSpeedFinish(material)} {Direction(tool)}\n" +
                $"G1 X{internalDiameter.NC()} F{FeedFinish(tool.Radius).NC()}\n" +
                $"{CoolantOff(machine)}\n" +
                TURNING_REFERENT_POINT,

                Machine.L230A =>
                tool.Description(ToolDescriptionOption.L230) + "\n" +
                $"{CoolantOn(machine)}\n" +
                $"G0 X{(externalDiameter + 5).NC(1)} Z{profStockAllow.NC()} S{CuttingSpeedFinish(material)} {Direction(tool)}\n" +
                $"G1 X{internalDiameter.NC()} F{FeedFinish(tool.Radius).NC()}\n" +
                $"{CoolantOff(machine)}\n" +
                TURNING_REFERENT_POINT,

                _ => string.Empty,
            };
        }
    }
}
