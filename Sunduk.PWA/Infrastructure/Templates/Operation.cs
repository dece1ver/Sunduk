using Sunduk.PWA.Infrastructure.CAM;
using Sunduk.PWA.Infrastructure.Sequences;
using Sunduk.PWA.Infrastructure.Sequences.Turning;
using Sunduk.PWA.Infrastructure.Tools;
using Sunduk.PWA.Infrastructure.Tools.Base;
using Sunduk.PWA.Infrastructure.Tools.Milling;
using Sunduk.PWA.Infrastructure.Tools.Turning;
using Sunduk.PWA.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using static Sunduk.PWA.Util.Util;

namespace Sunduk.PWA.Infrastructure.Templates
{
    public abstract class Operation
    {
        public const double SAFE_APPROACH_DISTANCE = 2;
        public const string TURNING_REFERENT_POINT = "G30 U0 W0\n";
        public const string TURNING_REFERENT_POINT_CONSISTENTLY = "G30 U0\nG30 W0\n";
        public const string MILLING_REFERENT_POINT = "/G91 G30 Z0\n" +
            "/G91 G53 X-800 Y0\n" +
            "M1\n";
        public const string MILLING_SAFETY_STRING = "G90 G17 G54\n";
        public const string GOODWAY_RETURN_B = "G55 G30 B0\n";
        public const string STOP = "M0";
        public const string OP_STOP = "M1";
        public const string PROCESSING_SNIPPET = "(---OBRABOTKA---)\n";

        

        private static string SpindleUnclamp(Machine machine)
        {
            return machine switch
            {
                Machine.GS1500 => "M10",
                Machine.L230A => "M69",
                _ => string.Empty,
            };
        }

        private static string SpindleClamp(Machine machine)
        {
            return machine switch
            {
                Machine.GS1500 => "M11",
                Machine.L230A => "M68", //????????
                _ => string.Empty,
            };
        }

        private static string CoolantOn(Machine machine, CoolantType type = CoolantType.General)
        {
            return machine switch
            {
                Machine.GS1500 => "M58",
                Machine.L230A => "M8",
                Machine.A110 => type switch 
                {
                    CoolantType.General => "M8",
                    CoolantType.Through => "M50",
                    CoolantType.Blow => "M57",
                    _ => string.Empty,
                },
                _ => string.Empty,
            };
        }

        private static string CoolantOff(Machine machine, CoolantType type = CoolantType.General)
        {
            return machine switch
            {
                Machine.GS1500 => "M59",
                Machine.L230A => "M9",
                Machine.A110 => type switch
                {
                    CoolantType.General => "M9",
                    CoolantType.Through => "M51",
                    CoolantType.Blow => "M59",
                    _ => string.Empty,
                },
                _ => string.Empty,
            };
        }

        private static string Direction(Tool tool) => tool.Hand == Tool.ToolHand.Rigth ? "M3" : "M4";


        #region Режимы
        /// <summary>
        /// Скорость резания на черновом точении
        /// </summary>
        public static int CuttingSpeedRough(Material material)
        {
            return material switch
            {
                Material.Steel => 280,
                Material.Stainless => 180,
                Material.Brass => 350,
                _ => 0,
            };
        }

        /// <summary>
        /// Скорость резания на чистовом точении
        /// </summary>
        public static int CuttingSpeedFinish(Material material)
        {
            return material switch
            {
                Material.Steel => 350,
                Material.Stainless => 230,
                Material.Brass => 450,
                _ => 0,
            };
        }

        /// <summary>
        /// Подача на черновом точении
        /// </summary>
        public static double FeedRough(double toolRadius)
        {
            return toolRadius switch
            {
                < 0.2 => 0.07,
                < 0.4 => 0.11,
                < 0.8 => 0.15,
                < 1.2 => 0.25,
                < 1.6 => 0.3,
                < 2 => 0.4,
                _ => 0,
            };
        }

        /// <summary>
        /// Подача на чистовом точении
        /// </summary>
        public static double FeedFinish(double toolRadius)
        {
            return toolRadius switch
            {
                < 0.2 => 0.03,
                < 0.4 => 0.05,
                < 0.8 => 0.1,
                < 1.2 => 0.15,
                < 1.6 => 0.2,
                < 2 => 0.25,
                _ => 0,
            };
        }

        
        /// <summary>
        /// Скорость резания при сверлении
        /// </summary>
        private static int DrillCuttingSpeed(Material material, DrillingTool drillingTool)
        {
            return material switch
            {
                Material.Steel => drillingTool.Type switch
                {
                    TurningDrillingTool.Types.Insert => 200,
                    TurningDrillingTool.Types.Solid => 100,
                    TurningDrillingTool.Types.Tip => 100,
                    TurningDrillingTool.Types.Rapid => 20,
                    TurningDrillingTool.Types.Center => 20,
                    _ => 0,
                },
                Material.Stainless => drillingTool.Type switch
                {
                    TurningDrillingTool.Types.Insert => 150,
                    TurningDrillingTool.Types.Solid => 60,
                    TurningDrillingTool.Types.Tip => 80,
                    TurningDrillingTool.Types.Rapid => 12,
                    TurningDrillingTool.Types.Center => 12,
                    _ => 0,
                },
                Material.Brass => drillingTool.Type switch
                {
                    TurningDrillingTool.Types.Insert => 200,
                    TurningDrillingTool.Types.Solid => 120,
                    TurningDrillingTool.Types.Tip => 120,
                    TurningDrillingTool.Types.Rapid => 20,
                    TurningDrillingTool.Types.Center => 20,
                    _ => 0,
                },
                _ => 0
            };
        }

        private static double DrillFeed(Machine machine, Material material, DrillingTool drillingTool)
        {
            return machine.GetMachineType() switch
            {
                MachineType.Turning =>
                material switch
                {
                    Material.Steel => drillingTool.Type switch
                    {
                        TurningDrillingTool.Types.Insert => (drillingTool.Diameter * 0.0028),
                        TurningDrillingTool.Types.Solid => (drillingTool.Diameter * 0.01),
                        TurningDrillingTool.Types.Tip => (drillingTool.Diameter * 0.01),
                        TurningDrillingTool.Types.Rapid => (drillingTool.Diameter * 0.015),
                        TurningDrillingTool.Types.Center => (drillingTool.Diameter * 0.02),
                        _ => 0,
                    },
                    Material.Stainless => drillingTool.Type switch
                    {
                        TurningDrillingTool.Types.Insert => (drillingTool.Diameter * 0.0028),
                        TurningDrillingTool.Types.Solid => (drillingTool.Diameter * 0.01),
                        TurningDrillingTool.Types.Tip => (drillingTool.Diameter * 0.01),
                        TurningDrillingTool.Types.Rapid => (drillingTool.Diameter * 0.015),
                        TurningDrillingTool.Types.Center => (drillingTool.Diameter * 0.02),
                        _ => 0,
                    },
                    Material.Brass => drillingTool.Type switch
                    {
                        TurningDrillingTool.Types.Insert => (drillingTool.Diameter * 0.0028),
                        TurningDrillingTool.Types.Solid => (drillingTool.Diameter * 0.01),
                        TurningDrillingTool.Types.Tip => (drillingTool.Diameter * 0.01),
                        TurningDrillingTool.Types.Rapid => (drillingTool.Diameter * 0.015),
                        TurningDrillingTool.Types.Center => (drillingTool.Diameter * 0.02),
                        _ => 0,
                    },
                    _ => 0
                },
                MachineType.Milling => 0.15,
                _ => 0,
            };
        }

        #endregion

        /// <summary>
        /// шапка
        /// </summary>
        public static string Header(Machine machine, string number, string name, string author, double drawVersion) => machine switch
        {
            Machine.GS1500 =>
            "%\n" +
            $"<{number}>({name})\n" +
            $"({drawVersion.ToString(null, CultureInfo.InvariantCulture)})\n" +
            "G10 L2 P1 Z-100. B300. (G54)\n" +
            "G10 L2 P2 Z400. (G55)\n" +
            $"({author}) ({DateTime.Now:dd.MM.yy})\n" +
            "(0M0S)\n",

            Machine.L230A =>
            "%\n" +
            $"O0001 ({number})\n" +
            $"({name})({drawVersion.ToString(null, CultureInfo.InvariantCulture)})\n" +
            $"({author})({DateTime.Now:dd.MM.yy})\n" +
            "(0M0S)\n",

            Machine.A110 =>
            "%\n" +
            $"O0001 ({number})\n" +
            $"({name})({drawVersion.ToString(null, CultureInfo.InvariantCulture)})\n" +
            $"({author})({DateTime.Now:dd.MM.yy})\n" +
            "(0M0S)\n",

            _ => string.Empty,
        };

        /// <summary>
        /// Строка безопасности
        /// </summary>
        public static string SafetyString(Machine machine, int? speedLimit) => machine switch
        {
            Machine.GS1500 =>
            TURNING_REFERENT_POINT_CONSISTENTLY +
            GOODWAY_RETURN_B +
            "G40 G80\n" +
            $"G50 S{((speedLimit ?? 0) > 4000 ? 4000 : speedLimit ?? 3500)}\n" +
            "G96\n",

            Machine.L230A =>
            TURNING_REFERENT_POINT_CONSISTENTLY +
            "G40 G80 G55\n" +
            $"G50 S{((speedLimit ?? 0) > 5000 ? 5000 : speedLimit ?? 3000)}\n" +
            "G96 G23\n",

            Machine.A110 => string.Empty,

            _ => string.Empty,
        };

        /// <summary>
        /// Упор
        /// </summary>
        public static string Limiter(Machine machine, Tool tool, double externalDiameter)
        {
            if (tool is null || externalDiameter == 0) return string.Empty;
            return machine switch
            {
                Machine.GS1500 =>
                TURNING_REFERENT_POINT +
                $"T{tool.Position.ToolNumber()} G54 ({tool.Name})\n" +
                $"G0 X{externalDiameter.NC(0)} Z0.5\n" +
                SpindleUnclamp(machine) + "\n" +
                STOP + "\n" +
                "W1.\n" +
                TURNING_REFERENT_POINT,

                Machine.L230A =>
                $"T{tool.Position.ToolNumber()} ({tool.Name})\n" +
                $"G0 X{externalDiameter.NC(0)} Z0.5\n" +
                SpindleUnclamp(machine) + "\n" +
                STOP + "\n" +
                "W1.\n" +
                TURNING_REFERENT_POINT,

                _ => string.Empty,
            };
        }

        /// <summary>
        /// Токарный вызов инструмента
        /// </summary>
        public static string TurningToolCall(Machine machine, Tool tool, CoolantType coolant = CoolantType.General, bool polar = false)
        {
            return machine switch
            {
                Machine.GS1500 =>
                TURNING_REFERENT_POINT +
                tool.Description(ToolDescriptionOption.GoodwayLeft) + "\n" +
                $"{CoolantOff(machine)}\n" +
                PROCESSING_SNIPPET +
                TURNING_REFERENT_POINT,

                Machine.L230A =>
                tool.Description(ToolDescriptionOption.L230) + "\n" +
                $"{CoolantOff(machine)}\n" +
                PROCESSING_SNIPPET +
                TURNING_REFERENT_POINT,

                _ => string.Empty,
            };
        }

        /// <summary>
        /// Фрезерный вызов инструмента
        /// </summary>
        public static string MillingToolCall(Machine machine, Tool tool, CoolantType coolant = CoolantType.General, bool polar = false)
        {
            if (tool is null) return string.Empty;
            string direction = string.Empty;
            if (coolant != CoolantType.General)
            {
                direction = Direction(tool);
            }
            else if (coolant == CoolantType.General || tool.Hand == Tool.ToolHand.Rigth)
            {
                direction = "M13";
            }
            else if (coolant == CoolantType.General || tool.Hand == Tool.ToolHand.Left)
            {
                direction = "M14";
            }
            return machine switch
            {
                Machine.A110 =>
                tool.Description(ToolDescriptionOption.General) + "\n" +
                $"T00\n" +
                $"{(polar ? "G16 " : string.Empty)}" +
                $"G57 G0 X0 Y0 S3000 {direction}\n" +
                $"G43 Z100 H{tool.Position} {((direction == "M13" || direction == "M14") ? string.Empty : CoolantOn(machine, coolant))}\n" +
                PROCESSING_SNIPPET +
                $"{CoolantOff(machine, coolant)}\n" +
                $"{(polar ? "G15\n" : string.Empty)}" +
                MILLING_REFERENT_POINT,

                _ => string.Empty,
            };
        }

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

        /// <summary>
        /// наружная фаска в пределах цикла
        /// </summary>
        /// <param name="chamferDiameter">Конечный диаметр</param>
        /// <param name="angle">Угол фаски</param>
        /// <param name="chamferSize">Размер фаски</param>
        /// <param name="roundCorners">Выполнять ли притупления R0.3 на краях фаски</param>
        /// <returns></returns>
        private static string CycleChamferExternal(double chamferDiameter, double angle, double chamferSize, bool roundCorners = true, bool startProfile = true)
        {
            double result = ((chamferDiameter - (2 * chamferSize * Math.Tan(angle.Radians()))) - 0.8 * Math.Tan(angle.Radians()));
            if (chamferDiameter > 0 && angle > 0 && chamferSize > 0 && !roundCorners)
            {
                return
                    $"X{result.NC()}\n" +
                    $"{(startProfile ? "G1 Z0.\n" : string.Empty)}" +
                    $"X{chamferDiameter.NC()} A-{angle.NC()}";
            }
            else if (chamferDiameter > 0 && angle > 0 && chamferSize > 0 && roundCorners)
            {
                return
                    $"G0 X{result - 2 * (0.8 + 0.3)}\n" +
                    $"G1 Z0.\n" +
                    $"X{result.NC()} R{(0.8 + 0.3).NC()}\n" +
                    $"X{chamferDiameter.NC()} A-{angle.NC()} R{(0.8 + 0.3).NC()}\n" +
                    $"W-{(0.8 + 0.3).NC()}";
            }
            return string.Empty;
        }

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
                $"G43 Z{startZ.NC(option: NcDecimalPointOption.Without)} H{tool.Position} {CoolantOn(machine, CoolantType.Through)}\n" +
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
                $"G43 Z{startZ.NC(option: NcDecimalPointOption.Without)} H{tool.Position} {CoolantOn(machine, CoolantType.Through)}\n" +
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
                $"G43 Z{startZ.NC(option: NcDecimalPointOption.Without)} H{tool.Position} {CoolantOn(machine, CoolantType.Through)}\n" +
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


        /// <summary>
        /// Нарезание резьбы метчиком
        /// </summary>
        public static string TurningTapping(Machine machine, TappingTool tool, double cutSpeed, double startZ, double endZ)
        {
            if (tool is null ||
                startZ <= endZ) return string.Empty;
            string approach = startZ > 0
                ? $"G0 X0. Z{startZ.NC()} S{cutSpeed.ToSpindleSpeed(tool.Diameter, 10)} G97\n"
                : $"G0 X0. Z{SAFE_APPROACH_DISTANCE.NC()} S{((int)cutSpeed).ToSpindleSpeed(tool.Diameter, 10)} G97\nZ{startZ.NC()}\n";
            string exit = startZ > 0
                ? string.Empty
                : $"G0 Z{SAFE_APPROACH_DISTANCE.NC()}\n";
            return machine switch
            {
                Machine.GS1500 =>
                TURNING_REFERENT_POINT +
                tool.Description(ToolDescriptionOption.GoodwayLeft) + "\n" +
                approach +
                $"G84 Z{endZ.NC()} P1000 F{tool.Pitch.NC()}\n" +
                $"G80\n" +
                exit +
                $"G96 {CoolantOff(machine)}\n" +
                TURNING_REFERENT_POINT,

                Machine.L230A =>
                tool.Description(ToolDescriptionOption.L230) + "\n" +
                $"{CoolantOn(machine)}\n" +
                approach +
                $"G84 Z{endZ.NC()} P1000 F{tool.Pitch.NC()}\n" +
                $"G80\n" +
                exit +
                $"G96 {CoolantOff(machine)}\n" +
                TURNING_REFERENT_POINT,
                _ => string.Empty
            };
        }

        public static string MillingTapping(Machine machine, MillingTappingTool tool, double cutSpeed, double startZ, double endZ, List<Hole> holes, bool polar = false)
        {
            if (tool is null || startZ <= endZ) return string.Empty;
            var spindleSpeed = cutSpeed.ToSpindleSpeed(tool.Diameter, 10);
            string result = machine switch
            {

                Machine.A110 =>
                tool.Description(ToolDescriptionOption.General) + "\n" +
                $"T00\n" +
                $"{(polar ? "G16 " : string.Empty)}" +
                $"G57 G0 X{holes[0].X.NC(option: NcDecimalPointOption.Without)} Y{holes[0].Y.NC(option: NcDecimalPointOption.Without)} S{spindleSpeed} {Direction(tool)}\n" +
                $"G43 Z{startZ.NC(option: NcDecimalPointOption.Without)} H{tool.Position} {CoolantOn(machine, CoolantType.Through)}\n" +
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
                    $"{(polar ? "G15" : string.Empty)}" +
                    MILLING_REFERENT_POINT,
                    _ => string.Empty
                };
            }
            return result;
        }

        /// <summary>
        /// Нарезание резьбы
        /// </summary>
        public static string ThreadCutting(Machine machine, Tool tool, ThreadStandart threadStandart, CuttingType type, double threadDiameter, double threadPitch, double startZ, double endZ, double threadNPTPlane)
        {
            if (tool is null ||
                threadDiameter <= 0 ||
                threadPitch <= 0 ||
                startZ < endZ) return string.Empty;
            string approachDiameter = Thread.ApproachDiameter(threadStandart, type, threadDiameter, threadPitch, endZ, startZ, threadNPTPlane).NC(1);
            string endDiameter = Thread.EndDiameter(threadStandart, type, threadDiameter, threadPitch, endZ, startZ, threadNPTPlane).NC(2);
            int minStep = Thread.Passes(threadStandart, type, threadPitch)[^2].Microns();
            double lastPass = Thread.Passes(threadStandart, type, threadPitch)[^1];
            int firstPass = Thread.Passes(threadStandart, type, threadPitch)[0].Microns();
            int profile = Thread.ProfileHeight(threadStandart, type, threadPitch).Microns();
            string threadShift = string.Empty;
            if (threadStandart == ThreadStandart.NPT)
            {
                threadShift = type switch
                {
                    CuttingType.External => $" R-{Thread.IntNPTThreadShift(endZ, startZ).NC(2)}",
                    CuttingType.Internal => $" R{Thread.IntNPTThreadShift(endZ, startZ).NC(2)}",
                    _ => string.Empty,
                };
            }

            return machine switch
            {
                Machine.GS1500 =>
                TURNING_REFERENT_POINT +
                tool.Description(ToolDescriptionOption.GoodwayLeft) + "\n" +
                $"G0 X{approachDiameter} Z{startZ.NC()} S{120.ToSpindleSpeed(threadDiameter, 100)} G97\n" +
                $"G76 P0201{Thread.Profile(threadStandart)} Q{minStep} R{lastPass.NC()}\n" +
                $"G76 X{endDiameter} Z{endZ.NC()} P{profile} Q{firstPass}{threadShift} F{threadPitch.NC()}\n" +
                $"G96 {CoolantOff(machine)}\n" +
                TURNING_REFERENT_POINT,

                Machine.L230A =>
                tool.Description(ToolDescriptionOption.L230) + "\n" +
                $"{CoolantOn(machine)}\n" +
                $"G0 X{approachDiameter}Z{startZ.NC()} S{120.ToSpindleSpeed(threadDiameter, 100)} G97\n" +
                $"G76 P0201{Thread.Profile(threadStandart)} Q{minStep} R{lastPass.NC()}\n" +
                $"G76 X{endDiameter}Z{endZ.NC()}P{profile} Q{firstPass}{threadShift} F{threadPitch.NC()}\n" +
                $"G96{CoolantOff(machine)}\n" +
                TURNING_REFERENT_POINT,

                _ => string.Empty,
            };
        }
    }
}
