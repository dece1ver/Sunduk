using Sunduk.PWA.Infrastructure.Sequences;
using Sunduk.PWA.Infrastructure.Tools;
using Sunduk.PWA.Util;
using System;
using System.Globalization;

namespace Sunduk.PWA.Infrastructure.Templates
{
    public abstract class Operation
    {
        public const double SAFE_APPROACH_DISTANCE = 2;
        public const string REFERENT_POINT = "G30U0W0\n";
        public const string REFERENT_POINT_CONSISTENTLY = "G30U0\nG30W0\n";
        public const string GOODWAY_RETURN_B = "G55G30B0\n";
        public const string STOP = "M0";
        public const string OP_STOP = "M1";

        private static string SpindleUnclamp(Machines machine)
        {
            return machine switch
            {
                Machines.GS1500 => "M10",
                Machines.L230A => "M69",
                _ => string.Empty,
            };
        }

        private static string SpindleClamp(Machines machine)
        {
            return machine switch
            {
                Machines.GS1500 => "M11",
                Machines.L230A => "M68", //????????
                _ => string.Empty,
            };
        }

        private static string CoolantOn(Machines machine)
        {
            return machine switch
            {
                Machines.GS1500 => "M58",
                Machines.L230A => "M8",
                _ => string.Empty,
            };
        }

        private static string CoolantOff(Machines machine)
        {
            return machine switch
            {
                Machines.GS1500 => "M59",
                Machines.L230A => "M9",
                _ => string.Empty,
            };
        }

        private static int Direction(Tool tool) => tool.Hand == Tool.ToolHand.Rigth ? 3 : 4;


        #region Режимы
        /// <summary>
        /// Скорость резания на черновом точении
        /// </summary>
        public static int CuttingSpeedRough(Materials material)
        {
            return material switch
            {
                Materials.Steel => 280,
                Materials.Stainless => 180,
                Materials.Brass => 350,
                _ => 0,
            };
        }

        /// <summary>
        /// Скорость резания на чистовом точении
        /// </summary>
        public static int CuttingSpeedFinish(Materials material)
        {
            return material switch
            {
                Materials.Steel => 350,
                Materials.Stainless => 230,
                Materials.Brass => 450,
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
        private static int DrillCuttingSpeed(Materials material, DrillingTool drillingTool)
        {
            return material switch
            {
                Materials.Steel => drillingTool.Type switch
                {
                    DrillingTool.Types.Insert => 200,
                    DrillingTool.Types.Solid => 100,
                    DrillingTool.Types.Tip => 100,
                    DrillingTool.Types.HSS => 20,
                    DrillingTool.Types.Center => 20,
                    _ => 0,
                },
                Materials.Stainless => drillingTool.Type switch
                {
                    DrillingTool.Types.Insert => 150,
                    DrillingTool.Types.Solid => 60,
                    DrillingTool.Types.Tip => 80,
                    DrillingTool.Types.HSS => 12,
                    DrillingTool.Types.Center => 12,
                    _ => 0,
                },
                Materials.Brass => drillingTool.Type switch
                {
                    DrillingTool.Types.Insert => 200,
                    DrillingTool.Types.Solid => 120,
                    DrillingTool.Types.Tip => 120,
                    DrillingTool.Types.HSS => 20,
                    DrillingTool.Types.Center => 20,
                    _ => 0,
                },
                _ => 0
            };
        }

        private static double DrillFeed(Materials material, DrillingTool drillingTool)
        {
            return material switch
            {
                Materials.Steel => drillingTool.Type switch
                {
                    DrillingTool.Types.Insert => (drillingTool.Diameter * 0.0028),
                    DrillingTool.Types.Solid => (drillingTool.Diameter * 0.01),
                    DrillingTool.Types.Tip => (drillingTool.Diameter * 0.01),
                    DrillingTool.Types.HSS => (drillingTool.Diameter * 0.015),
                    DrillingTool.Types.Center => (drillingTool.Diameter * 0.02),
                    _ => 0,
                },
                Materials.Stainless => drillingTool.Type switch
                {
                    DrillingTool.Types.Insert => (drillingTool.Diameter * 0.0028),
                    DrillingTool.Types.Solid => (drillingTool.Diameter * 0.01),
                    DrillingTool.Types.Tip => (drillingTool.Diameter * 0.01),
                    DrillingTool.Types.HSS => (drillingTool.Diameter * 0.015),
                    DrillingTool.Types.Center => (drillingTool.Diameter * 0.02),
                    _ => 0,
                },
                Materials.Brass => drillingTool.Type switch
                {
                    DrillingTool.Types.Insert => (drillingTool.Diameter * 0.0028),
                    DrillingTool.Types.Solid => (drillingTool.Diameter * 0.01),
                    DrillingTool.Types.Tip => (drillingTool.Diameter * 0.01),
                    DrillingTool.Types.HSS => (drillingTool.Diameter * 0.015),
                    DrillingTool.Types.Center => (drillingTool.Diameter * 0.02),
                    _ => 0,
                },
                _ => 0
            };
        }

        #endregion

        /// <summary>
        /// шапка
        /// </summary>
        public static string Header(Machines machine, string number, string name, string author, double drawVersion) => machine switch
        {
            Machines.GS1500 =>
            "%\n" +
            $"<{number}>({name})\n" +
            $"({drawVersion.ToString(null, CultureInfo.InvariantCulture)})\n" +
            "G10L2P1Z-100.B300.(G54)\n" +
            "G10L2P2Z400.(G55)\n" +
            $"({author})({DateTime.Now:dd.MM.yy})\n" +
            "(0M0S)\n",

            Machines.L230A =>
            "%\n" +
            $"O0001({number})\n" +
            $"({name})({drawVersion.ToString(null, CultureInfo.InvariantCulture)})\n" +
            $"({author})({DateTime.Now:dd.MM.yy})\n" +
            "(0M0S)\n",

            _ => string.Empty,
        };

        /// <summary>
        /// Строка безопасности
        /// </summary>
        public static string SafetyString(Machines machine, int? speedLimit) => machine switch
        {
            Machines.GS1500 =>
            REFERENT_POINT_CONSISTENTLY +
            GOODWAY_RETURN_B +
            "G40G80\n" +
            $"G50S{((speedLimit ?? 0) > 4000 ? 4000 : speedLimit ?? 0)}\n" +
            "G96\n",

            Machines.L230A =>
            REFERENT_POINT_CONSISTENTLY +
            "G40G80G55\n" +
            $"G50S{((speedLimit ?? 0) > 5000 ? 5000 : speedLimit ?? 0)}\n" +
            "G96G23\n",

            _ => string.Empty,
        };

        /// <summary>
        /// Упор
        /// </summary>
        public static string Limiter(Machines machine, Tool tool, double externalDiameter)
        {
            if (tool is null ||
                externalDiameter == 0) return string.Empty;
            return machine switch
            {
                Machines.GS1500 =>
                $"T{tool.Position.ToolNumber()}G54({tool.Name})\n" +
                $"G0X{externalDiameter.NC(0)}.Z0.5\n" +
                SpindleUnclamp(machine) + "\n" +
                STOP + "\n" +
                "W1.\n" +
                REFERENT_POINT,

                Machines.L230A =>
                $"T{tool.Position.ToolNumber()}({tool.Name})\n" +
                $"G0X{externalDiameter.NC(0)}.Z0.5\n" +
                SpindleUnclamp(machine) + "\n" +
                STOP + "\n" +
                "W1.\n" +
                REFERENT_POINT,

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
        public static string RoughFacingCycle(Machines machine, Materials material, TurningExternalTool tool,
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
                Machines.GS1500 =>
                tool.Description(Util.Util.ToolDescriptionOption.GoodwayLeft) + "\n" +
                $"G0X{(externalDiameter + 5).NC(1)}Z{roughStockAllow.NC()}S{CuttingSpeedRough(material)}M{Direction(tool)}\n" +
                $"G72W{stepOver.NC()}R0.1\n" +
                $"G72P{seqNo.Item1}Q{seqNo.Item2}{(profStockAllow > 0 ? "W" + profStockAllow.NC() : string.Empty)}F{FeedRough(tool.Radius).NC()}\n" +
                $"N{seqNo.Item1}G0Z0.\n" +
                $"N{seqNo.Item2}G1X{internalDiameter.NC()}\n" +
                $"{CoolantOff(machine)}\n" +
                REFERENT_POINT,

                Machines.L230A =>
                tool.Description(Util.Util.ToolDescriptionOption.L230) + "\n" +
                $"{CoolantOn(machine)}\n" +
                $"G0X{(externalDiameter + 5).NC(1)}Z{roughStockAllow.NC()}S{CuttingSpeedRough(material)}M{Direction(tool)}\n" +
                $"G72W{stepOver.NC()}R0.1\n" +
                $"G72P{seqNo.Item1}Q{seqNo.Item2}{(profStockAllow > 0 ? "W" + profStockAllow.NC() : string.Empty)}F{FeedRough(tool.Radius).NC()}\n" +
                $"N{seqNo.Item1}G0Z0.\n" +
                $"N{seqNo.Item2}G1X{internalDiameter.NC()}\n" +
                $"{CoolantOff(machine)}\n" +
                REFERENT_POINT,

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
        public static string Facing(Machines machine, Materials material, TurningExternalTool tool, 
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
                Machines.GS1500 =>
                tool.Description(Util.Util.ToolDescriptionOption.GoodwayLeft) + "\n" +
                $"G0X{(externalDiameter + 5).NC(1)}Z{roughStockAllow.NC()}S{CuttingSpeedRough(material)}M{Direction(tool)}\n" +
                $"G72W{stepOver.NC()}R0.1\n" +
                $"G72P{seqNo.Item1}Q{seqNo.Item2}{(profStockAllow > 0 ? "W" + profStockAllow.NC() : string.Empty)}F{FeedRough(tool.Radius).NC()}\n" +
                $"N{seqNo.Item1}G0Z0.\n" +
                $"N{seqNo.Item2}G1X{internalDiameter.NC()}\n" +
                $"{(profStockAllow > 0 ? $"G70P{seqNo.Item1}Q{seqNo.Item2}S{CuttingSpeedFinish(material)}F{FeedFinish(tool.Radius).NC()}\n" : string.Empty)}" +
                $"{CoolantOff(machine)}\n" +
                REFERENT_POINT,

                Machines.L230A =>
                tool.Description(Util.Util.ToolDescriptionOption.L230) + "\n" +
                $"{CoolantOn(machine)}\n" +
                $"G0X{(externalDiameter + 5).NC(1)}Z{roughStockAllow.NC()}S{CuttingSpeedRough(material)}M{Direction(tool)}\n" +
                $"G72W{stepOver.NC()}R0.1\n" +
                $"G72P{seqNo.Item1}Q{seqNo.Item2}{(profStockAllow > 0 ? "W" + profStockAllow.NC() : string.Empty)}F{FeedRough(tool.Radius).NC()}\n" +
                $"N{seqNo.Item1}G0Z0.\n" +
                $"N{seqNo.Item2}G1X{internalDiameter.NC()}\n" +
                $"{(profStockAllow > 0 ? $"G70P{seqNo.Item1}Q{seqNo.Item2}S{CuttingSpeedFinish(material)}F{FeedFinish(tool.Radius).NC()}\n" : string.Empty)}" +
                $"{CoolantOff(machine)}\n" +
                REFERENT_POINT,

                _ => string.Empty,
            };
        }

        /// <summary>
        /// Торцовка черновая
        /// </summary>
        public static string RoughFacing(Machines machine, Materials material, TurningExternalTool tool, 
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
                Machines.GS1500 =>
                tool.Description(Util.Util.ToolDescriptionOption.GoodwayLeft) + "\n" +
                $"G0X{(externalDiameter + 5).NC(1)}Z{roughStockAllow.NC()}S{CuttingSpeedRough(material)}M{Direction(tool)}\n" +
                $"G72W{stepOver.NC()}R0.1\n" +
                $"G72P{seqNo.Item1}Q{seqNo.Item2}F{FeedRough(tool.Radius).NC()}\n" +
                $"N{seqNo.Item1}G0Z{profStockAllow.NC()}\n" +
                $"N{seqNo.Item2}G1X{internalDiameter.NC()}\n" +
                "M59\n" +
                REFERENT_POINT,

                Machines.L230A =>
                tool.Description(Util.Util.ToolDescriptionOption.L230) + "\n" +
                $"{CoolantOn(machine)}\n" +
                $"G0X{(externalDiameter + 5).NC(1)}Z{roughStockAllow.NC()}S{CuttingSpeedRough(material)}M{Direction(tool)}\n" +
                $"G72W{stepOver.NC()}R0.1\n" +
                $"G72P{seqNo.Item1}Q{seqNo.Item2}F{FeedRough(tool.Radius).NC()}\n" +
                $"N{seqNo.Item1}G0Z{profStockAllow.NC()}\n" +
                $"N{seqNo.Item2}G1X{internalDiameter.NC()}\n" +
                $"{CoolantOff(machine)}\n" +
                REFERENT_POINT,

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
            Machines machine = roughFacingSequence.Machine;
            Materials material = roughFacingSequence.Material;
            double externalDiameter = roughFacingSequence.ExternalDiameter;
            double internalDiameter = roughFacingSequence.InternalDiameter;
            double profStockAllow = roughFacingSequence.RoughStockAllow;
            (int, int) seqNo = roughFacingSequence.SeqNumbers;

            if (tool is null ||
                externalDiameter == 0 ||
                externalDiameter < internalDiameter) return string.Empty;
            return machine switch
            {
                Machines.GS1500 =>
                tool.Description(Util.Util.ToolDescriptionOption.GoodwayLeft) + "\n" +
                $"G0X{(externalDiameter + 5).NC(1)}Z{profStockAllow.NC()}S{CuttingSpeedFinish(material)}M{Direction(tool)}\n" +
                $"G70P{seqNo.Item1}Q{seqNo.Item2}S{CuttingSpeedFinish(material)}F{FeedFinish(tool.Radius).NC()}\n" +
                $"{CoolantOff(machine)}\n" +
                REFERENT_POINT,

                Machines.L230A =>
                tool.Description(Util.Util.ToolDescriptionOption.L230) + "\n" +
                $"{CoolantOn(machine)}\n" +
                $"G0X{(externalDiameter + 5).NC(1)}Z{profStockAllow.NC()}S{CuttingSpeedFinish(material)}M{Direction(tool)}\n" +
                $"G70P{seqNo.Item1}Q{seqNo.Item2}S{CuttingSpeedFinish(material)}F{FeedFinish(tool.Radius).NC()}\n" +
                $"{CoolantOff(machine)}\n" +
                REFERENT_POINT,

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
            Machines machine = roughFacingSequence.Machine;
            Materials material = roughFacingSequence.Material;
            double externalDiameter = roughFacingSequence.ExternalDiameter;
            double internalDiameter = roughFacingSequence.InternalDiameter;
            double profStockAllow = roughFacingSequence.RoughStockAllow;
            (int, int) seqNo = roughFacingSequence.SeqNumbers;

            if (tool is null ||
                externalDiameter == 0 ||
                externalDiameter < internalDiameter) return string.Empty;
            return machine switch
            {
                Machines.GS1500 =>
                tool.Description(Util.Util.ToolDescriptionOption.GoodwayLeft) + "\n" +
                $"G0X{(externalDiameter + 5).NC(1)}Z{profStockAllow.NC()}S{CuttingSpeedFinish(material)}M{Direction(tool)}\n" +
                $"G70P{seqNo.Item1}Q{seqNo.Item2}S{CuttingSpeedFinish(material)}F{FeedFinish(tool.Radius).NC()}\n" +
                $"{CoolantOff(machine)}\n" +
                REFERENT_POINT,

                Machines.L230A =>
                tool.Description(Util.Util.ToolDescriptionOption.L230) + "\n" +
                $"{CoolantOn(machine)}\n" +
                $"G0X{(externalDiameter + 5).NC(1)}Z{profStockAllow.NC()}S{CuttingSpeedFinish(material)}M{Direction(tool)}\n" +
                $"G70P{seqNo.Item1}Q{seqNo.Item2}S{CuttingSpeedFinish(material)}F{FeedFinish(tool.Radius).NC()}\n" +
                $"{CoolantOff(machine)}\n" +
                REFERENT_POINT,

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
            Machines machine = roughFacingSequence.Machine;
            Materials material = roughFacingSequence.Material;
            double externalDiameter = roughFacingSequence.ExternalDiameter;
            double internalDiameter = roughFacingSequence.InternalDiameter;
            double profStockAllow = roughFacingSequence.RoughStockAllow;
            (int, int) seqNo = roughFacingSequence.SeqNumbers;

            if (tool is null ||
                externalDiameter == 0 ||
                externalDiameter < internalDiameter) return string.Empty;
            return machine switch
            {
                Machines.GS1500 =>
                tool.Description(Util.Util.ToolDescriptionOption.GoodwayLeft) + "\n" +
                $"G0X{(externalDiameter + 5).NC(1)}Z{profStockAllow.NC()}S{CuttingSpeedFinish(material)}M{Direction(tool)}\n" +
                $"G70P{seqNo.Item1}Q{seqNo.Item2}S{CuttingSpeedFinish(material)}F{FeedFinish(tool.Radius).NC()}\n" +
                $"{CoolantOff(machine)}\n" +
                REFERENT_POINT,

                Machines.L230A =>
                tool.Description(Util.Util.ToolDescriptionOption.L230) + "\n" +
                $"{CoolantOn(machine)}\n" +
                $"G0X{(externalDiameter + 5).NC(1)}Z{profStockAllow.NC()}S{CuttingSpeedFinish(material)}M{Direction(tool)}\n" +
                $"G70P{seqNo.Item1}Q{seqNo.Item2}S{CuttingSpeedFinish(material)}F{FeedFinish(tool.Radius).NC()}\n" +
                $"{CoolantOff(machine)}\n" +
                REFERENT_POINT,

                _ => string.Empty,
            };
        }

        /// <summary>
        /// Торцовка чистовая
        /// </summary>
        public static string FinishFacing(Machines machine, Materials material, TurningExternalTool tool, double externalDiameter, double internalDiameter, double profStockAllow)
        {
            if (tool is null ||
                externalDiameter == 0 ||
                externalDiameter < internalDiameter) return string.Empty;
            return machine switch
            {
                Machines.GS1500 =>
                tool.Description(Util.Util.ToolDescriptionOption.GoodwayLeft) + "\n" +
                $"G0X{(externalDiameter + 5).NC(1)}Z{profStockAllow.NC()}S{CuttingSpeedFinish(material)}M{Direction(tool)}\n" +
                $"G1X{internalDiameter.NC()}F{FeedFinish(tool.Radius).NC()}\n" +
                $"{CoolantOff(machine)}\n" +
                REFERENT_POINT,

                Machines.L230A =>
                tool.Description(Util.Util.ToolDescriptionOption.L230) + "\n" +
                $"{CoolantOn(machine)}\n" +
                $"G0X{(externalDiameter + 5).NC(1)}Z{profStockAllow.NC()}S{CuttingSpeedFinish(material)}M{Direction(tool)}\n" +
                $"G1X{internalDiameter.NC()}F{FeedFinish(tool.Radius).NC()}\n" +
                $"{CoolantOff(machine)}\n" +
                REFERENT_POINT,

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
                    $"{(startProfile ? "G1Z0.\n" : string.Empty)}" +
                    $"X{chamferDiameter.NC()} A-{angle.NC()}";
            }
            else if (chamferDiameter > 0 && angle > 0 && chamferSize > 0 && roundCorners)
            {
                return
                    $"G0 X{result - 2 * (0.8 + 0.3)}\n" +
                    $"G1Z0.\n" +
                    $"X{result.NC()} R{(0.8 + 0.3).NC()}\n" +
                    $"X{chamferDiameter.NC()} A-{angle.NC()} R{(0.8 + 0.3).NC()}\n" +
                    $"W-{(0.8 + 0.3).NC()}";
            }
            return string.Empty;
        }

        /// <summary>
        /// Высокоскоростное сверление
        /// </summary>
        public static string HighSpeedDrilling(Machines machine, Materials material, DrillingTool tool, double startZ, double endZ)
        {
            if (tool is null ||
                startZ <= endZ) return string.Empty;
            string approach = startZ > 0
                ? $"G0X-{tool.Diameter.NC()}Z{startZ.NC()}S{DrillCuttingSpeed(material, tool)}M{Direction(tool)}\n"
                : $"G0X-{tool.Diameter.NC()}Z{SAFE_APPROACH_DISTANCE.NC()}S{DrillCuttingSpeed(material, tool)}M{Direction(tool)}\nZ{startZ.NC()}\n";
            string exit = startZ > 0
                ? $"G0Z{startZ.NC()}\n"
                : $"G0Z{SAFE_APPROACH_DISTANCE.NC()}\n";
            return machine switch
            {
                Machines.GS1500 =>
                tool.Description(Util.Util.ToolDescriptionOption.GoodwayLeft) + "\n" +
                approach +
                $"G1Z{(endZ - (tool.Diameter / 2 * Math.Tan((90 - tool.Angle / 2).Radians()))).NC()}F{DrillFeed(material, tool).NC(2)}\n" +
                exit +
                $"{CoolantOff(machine)}\n" +
                REFERENT_POINT,

                Machines.L230A =>
                REFERENT_POINT +
                tool.Description(Util.Util.ToolDescriptionOption.L230) + "\n" +
                $"{CoolantOn(machine)}\n" +
                approach +
                $"G1Z{(endZ - (tool.Diameter / 2 * Math.Tan((90 - tool.Angle / 2).Radians()))).NC()}F{DrillFeed(material, tool).NC(2)}\n" +
                exit +
                $"{CoolantOff(machine)}\n" +
                REFERENT_POINT,

                _ => string.Empty
            };
        }


        /// <summary>
        /// Прерывистое сверление
        /// </summary>
        public static string PeckDrilling(Machines machine, Materials material, DrillingTool tool, double depth, double startZ, double endZ)
        {
            if (tool is null ||
                startZ <= endZ ||
                depth <= 0) return string.Empty;
            string approach = startZ > 0
                ? $"G0X-{tool.Diameter.NC()}Z{startZ.NC()}S{DrillCuttingSpeed(material, tool)}M{Direction(tool)}\n"
                : $"G0X-{tool.Diameter.NC()}Z{SAFE_APPROACH_DISTANCE.NC()}S{DrillCuttingSpeed(material, tool)}M{Direction(tool)}\nZ{startZ.NC()}\n";
            string exit = startZ > 0
                ? $"G0Z{startZ.NC()}\n"
                : $"G0Z{SAFE_APPROACH_DISTANCE.NC()}\n";
            return machine switch
            {
                Machines.GS1500 =>
                REFERENT_POINT +
                tool.Description(Util.Util.ToolDescriptionOption.GoodwayLeft) + "\n" +
                approach +
                "G74R0.1\n" +
                $"G74Z{(endZ - (tool.Diameter / 2 * Math.Tan((90 - tool.Angle / 2).Radians()))).NC()}Q{depth.Microns()}F{DrillFeed(material, tool).NC(2)}\n" +
                exit +
                $"{CoolantOff(machine)}\n" +
                REFERENT_POINT,

                Machines.L230A =>
                REFERENT_POINT +
                tool.Description(Util.Util.ToolDescriptionOption.L230) + "\n" +
                $"{CoolantOn(machine)}\n" +
                approach +
                "G74R0.1\n" +
                $"G74Z{(endZ - (tool.Diameter / 2 * Math.Tan((90 - tool.Angle / 2).Radians()))).NC()}Q{depth.Microns()}F{DrillFeed(material, tool).NC(2)}\n" +
                exit +
                $"{CoolantOff(machine)}\n" +
                REFERENT_POINT,

                _ => string.Empty
            };
        }

        /// <summary>
        /// Глубокое сверление
        /// </summary>
        public static string PeckDeepDrilling(Machines machine, Materials material, DrillingTool tool, double depth, double startZ, double endZ)
        {
            if (tool is null ||
                startZ <= endZ ||
                depth <= 0) return string.Empty;
            string approach = startZ > 0
                ? $"G0X-{tool.Diameter.NC()}Z{startZ.NC()}S{DrillCuttingSpeed(material, tool)}M{Direction(tool)}\n"
                : $"G0X-{tool.Diameter.NC()}Z{SAFE_APPROACH_DISTANCE.NC()}S{DrillCuttingSpeed(material, tool)}M{Direction(tool)}\nZ{startZ.NC()}\n";
            string exit = startZ > 0
                ? $"G0Z{startZ.NC()}\n"
                : $"G0Z{SAFE_APPROACH_DISTANCE.NC()}\n";
            return machine switch
            {
                Machines.GS1500 =>
                REFERENT_POINT +
                tool.Description(Util.Util.ToolDescriptionOption.GoodwayLeft) + "\n" +
                approach +
                $"G83Z{(endZ - (tool.Diameter / 2 * Math.Tan((90 - tool.Angle / 2).Radians()))).NC()}Q{depth.Microns()}F{DrillFeed(material, tool).NC(2)}\n" +
                "G80\n" +
                exit +
                $"{CoolantOff(machine)}\n" +
                REFERENT_POINT,

                Machines.L230A =>
                REFERENT_POINT +
                tool.Description(Util.Util.ToolDescriptionOption.L230) + "\n" +
                $"{CoolantOn(machine)}\n" +
                approach +
                $"G83Z{(endZ - (tool.Diameter / 2 * Math.Tan((90 - tool.Angle / 2).Radians()))).NC()}Q{depth.Microns()}F{DrillFeed(material, tool).NC(2)}\n" +
                "G80\n" +
                exit +
                $"{CoolantOff(machine)}\n" +
                REFERENT_POINT,
                _ => string.Empty
            };
        }

        /// <summary>
        /// Нарезание резьбы метчиком
        /// </summary>
        public static string Tapping(Machines machine, TappingTool tool, double cutSpeed, double startZ, double endZ)
        {
            if (tool is null ||
                startZ <= endZ) return string.Empty;
            string approach = startZ > 0
                ? $"G0X0.Z{startZ.NC()}S{(cutSpeed * 1000 / (tool.Diameter * Math.PI)).Round(10)}G97\n"
                : $"G0X0.Z{SAFE_APPROACH_DISTANCE.NC()}S{(cutSpeed * 1000 / (tool.Diameter * Math.PI)).Round(10)}G97\nZ{startZ.NC()}\n";
            string exit = startZ > 0
                ? string.Empty
                : $"G0Z{SAFE_APPROACH_DISTANCE.NC()}\n";
            return machine switch
            {
                Machines.GS1500 =>
                REFERENT_POINT +
                tool.Description(Util.Util.ToolDescriptionOption.GoodwayLeft) + "\n" +
                approach +
                $"G84Z{endZ.NC()}P1000F{tool.Pitch.NC()}\n" +
                $"G80\n" +
                exit +
                $"{CoolantOff(machine)}\n" +
                REFERENT_POINT,

                Machines.L230A =>
                REFERENT_POINT +
                tool.Description(Util.Util.ToolDescriptionOption.L230) + "\n" +
                $"{CoolantOn(machine)}\n" +
                approach +
                $"G84Z{endZ.NC()}P1000F{tool.Pitch.NC()}\n" +
                $"G80\n" +
                exit +
                $"{CoolantOff(machine)}\n" +
                REFERENT_POINT,

                _ => string.Empty
            };
        }

        /// <summary>
        /// Нарезание резьбы
        /// </summary>
        public static string ThreadCutting(Machines machine, Tool tool, ThreadStandart threadStandart, CuttingType type, double threadDiameter, double threadPitch, double startZ, double endZ, double threadNPTPlane)
        {
            if (tool is null ||
                threadDiameter <= 0 ||
                threadPitch <= 0 ||
                startZ < endZ) return string.Empty;
            string approachDiameter = Thread.ApproachDiameter(threadStandart, type, threadDiameter, threadPitch, endZ, startZ, threadNPTPlane).NC(3);
            string endDiameter = Thread.EndDiameter(threadStandart, type, threadDiameter, threadPitch, endZ, startZ, threadNPTPlane).NC(3);
            int minStep = Thread.Passes(threadStandart, type, threadPitch)[^2].Microns();
            double lastPass = Thread.Passes(threadStandart, type, threadPitch)[^1];
            int firstPass = Thread.Passes(threadStandart, type, threadPitch)[0].Microns();
            int profile = Thread.ProfileHeight(threadStandart, type, threadPitch).Microns();
            string threadShift = string.Empty;
            if (threadStandart == ThreadStandart.NPT)
            {
                threadShift = type switch
                {
                    CuttingType.External => $"R-{Thread.IntNPTThreadShift(endZ, startZ).NC(2)} ",
                    CuttingType.Internal => $"R{Thread.IntNPTThreadShift(endZ, startZ).NC(2)} ",
                    _ => string.Empty,
                };
            }
            
            

            return machine switch
            {
                Machines.GS1500 =>
                REFERENT_POINT +
                tool.Description(Util.Util.ToolDescriptionOption.GoodwayLeft) + "\n" +
                $"G0X{approachDiameter}Z{startZ.NC()}S{(120 * 1000 / (threadDiameter * Math.PI)).Round(100)}G97\n" +
                $"G76P0201{Thread.Profile(threadStandart)}Q{minStep}R{lastPass.NC()}\n" +
                $"G76X{endDiameter}Z{endZ.NC()}P{profile}Q{firstPass}{threadShift}F{threadPitch.NC()}\n" +
                $"G96{CoolantOff(machine)}\n" +
                REFERENT_POINT,

                Machines.L230A =>
                REFERENT_POINT +
                tool.Description(Util.Util.ToolDescriptionOption.L230) + "\n" +
                $"{CoolantOn(machine)}\n" +
                $"G0X{approachDiameter}Z{startZ.NC()}S{(120 * 1000 / (threadDiameter * Math.PI)).Round(100)}G97\n" +
                $"G76P0201{Thread.Profile(threadStandart)}Q{minStep}R{lastPass.NC()}\n" +
                $"G76X{endDiameter}Z{endZ.NC()}P{profile}Q{firstPass}{threadShift}F{threadPitch.NC()}\n" +
                $"G96{CoolantOff(machine)}\n" +
                REFERENT_POINT,

                _ => string.Empty,
            };
        }
    }
}
