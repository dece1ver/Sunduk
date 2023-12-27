using Sunduk.PWA.Infrastructure.CAM;
using Sunduk.PWA.Infrastructure.Sequences;
using Sunduk.PWA.Infrastructure.Sequences.Turning;
using Sunduk.PWA.Infrastructure.Tools;
using Sunduk.PWA.Infrastructure.Tools.Base;
using Sunduk.PWA.Infrastructure.Tools.Milling;
using Sunduk.PWA.Infrastructure.Tools.Turning.Base;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sunduk.PWA.Infrastructure.Tools.Turning;
using static Sunduk.PWA.Infrastructure.Util;

namespace Sunduk.PWA.Infrastructure.Templates
{
    public abstract class Operation
    {
        public const double SafeApproachDistance = 2;
        public const string TurningReferentPoint = "G30 U0 W0\n";
        public const string TurningReferentPointConsistently = "G30 U0\nG30 W0\n";
        public const string MillingReferentPoint = 
            "/G91 G30 Z0\n" +
            "/G90 G53 X-800 Y0\n" +
            "M1\n";
        public const string MillingSafetyString = "G90 G17 G54\n";
        public const string GoodwayReturnB = "G55 G30 B0\n";
        public const string SpindleStop = "M5";
        public const string AbsStop = "M0";
        public const string OptionalStop = "M1";
        public const string ProcessingSnippet = "(---OBRABOTKA---)\n";

        public static double RapidSpeed() => 7500;
        public static double Escaping() => 0.2;

        public static string Stop(bool optional, string comment)
        {
            string result = string.Empty;
            result += optional switch
            {
                true => OptionalStop,
                false => AbsStop,
            };
            if (!string.IsNullOrEmpty(comment)) result += $" ({comment.Translate()})";
            return result;
        }

        public static string TailstockOn(Machine machine) => machine switch
        {
            Machine.L230A => "M25",
            Machine.GS1500 => "M225",
            _ => string.Empty,
        };

        public static string TailstockOff(Machine machine) => machine switch
        {
            Machine.L230A => "M28",
            Machine.GS1500 => "M226",
            _ => string.Empty,
        };

        public static string SpindleUnclamp(Machine machine)
        {
            return machine switch
            {
                Machine.GS1500 => "M10",
                Machine.L230A => "M69",
                _ => string.Empty,
            };
        }

        public static string SpindleClamp(Machine machine)
        {
            return machine switch
            {
                Machine.GS1500 => "M11",
                Machine.L230A => "M68",
                _ => string.Empty,
            };
        }

        public static string CoolantOn(Machine machine, Coolant type = Coolant.General)
        {
            return machine switch
            {
                Machine.GS1500 => "M58",
                Machine.L230A => "M8",
                Machine.A110 => type switch 
                {
                    Coolant.General => "M8",
                    Coolant.Through => "M50",
                    Coolant.Full => "M50",
                    Coolant.Blow => "M57",
                    _ => string.Empty,
                },
                _ => string.Empty,
            };
        }

        public static string CoolantOff(Machine machine, Coolant type = Coolant.General)
        {
            return machine switch
            {
                Machine.GS1500 => "M59",
                Machine.L230A => "M9",
                Machine.A110 => type switch
                {
                    Coolant.General => "M9",
                    Coolant.Through => "M51",
                    Coolant.Full => "M9 M51",
                    Coolant.Blow => "M59",
                    _ => string.Empty,
                },
                _ => string.Empty,
            };
        }

        public static string Direction(Tool tool) => tool.Hand == Tool.ToolHand.Right ? "M3" : "M4";


        #region Режимы
        /// <summary>
        /// Скорость резания на черновом точении
        /// </summary>
        public static int CuttingSpeedRough(Material material)
        {
            return material switch
            {
                Material.Steel => 230,
                Material.Stainless => 160,
                Material.Brass => 300,
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
                Material.Stainless => 220,
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
                < 1.2 => 0.22,
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
                < 0.4 => 0.04,
                < 0.8 => 0.08,
                < 1.2 => 0.13,
                < 1.6 => 0.2,
                < 2 => 0.25,
                _ => 0,
            };
        }

        /// <summary>
        /// Скорость резания на накатке
        /// </summary>
        public static int BurnishingSpeed(TurningBurnishingTool tool)
        {
            return tool.Type switch
            {
                TurningBurnishingTool.Types.Diamond => 80,
                TurningBurnishingTool.Types.Roller => 120,
                _ => 0,
            };
        }

        /// <summary>
        /// Подача на накатке
        /// </summary>
        public static double BurnishingFeed(TurningBurnishingTool tool)
        {
            return tool.Type switch
            {
                TurningBurnishingTool.Types.Diamond => 0.03,
                TurningBurnishingTool.Types.Roller => 0.2,
                _ => 0,
            };
        }

        /// <summary>
        /// Скорость резания при сверлении
        /// </summary>
        public static int DrillCuttingSpeed(Material material, DrillingTool drillingTool)
        {
            return material switch
            {
                Material.Steel => drillingTool.Type switch
                {
                    DrillingTool.Types.Insert => 180,
                    DrillingTool.Types.Solid => 100,
                    DrillingTool.Types.Tip => 100,
                    DrillingTool.Types.Rapid => 15,
                    DrillingTool.Types.Center => 15,
                    _ => 0,
                },
                Material.Stainless => drillingTool.Type switch
                {
                    DrillingTool.Types.Insert => 150,
                    DrillingTool.Types.Solid => 60,
                    DrillingTool.Types.Tip => 80,
                    DrillingTool.Types.Rapid => 12,
                    DrillingTool.Types.Center => 8,
                    _ => 0,
                },
                Material.Brass => drillingTool.Type switch
                {
                    DrillingTool.Types.Insert => 200,
                    DrillingTool.Types.Solid => 120,
                    DrillingTool.Types.Tip => 120,
                    DrillingTool.Types.Rapid => 30,
                    DrillingTool.Types.Center => 30,
                    _ => 0,
                },
                _ => 0
            };
        }

        /// <summary>
        /// Скорость резания на канавках черновая
        /// </summary>
        public static int GroovingSpeedRough(Material material)
        {
            return material switch
            {
                Material.Steel => 100,
                Material.Stainless => 90,
                Material.Brass => 120,
                _ => 0,
            };
        }

        /// <summary>
        /// Скорость резания на канавках чистовая
        /// </summary>
        public static int GroovingSpeedFinish(Material material)
        {
            return material switch
            {
                Material.Steel => 120,
                Material.Stainless => 120,
                Material.Brass => 140,
                _ => 0,
            };
        }

        /// <summary>
        /// Подача на чистовом точении
        /// </summary>
        public static double GroovingFeedRough()
        {
            return 0.08;
        }

        /// <summary>
        /// Подача на чистовом точении
        /// </summary>
        public static double GroovingFeedFinish()
        {
            return 0.05;
        }

        public static double DrillFeed(Machine machine, Material material, DrillingTool drillingTool)
        {
            return machine.GetMachineType() switch
            {
                MachineType.Turning => 
                material switch
                {
                    Material.Steel => drillingTool.Type switch
                    {
                        DrillingTool.Types.Insert => Math.Round(drillingTool.Diameter * 0.0028, 2),
                        DrillingTool.Types.Solid => (drillingTool.Diameter > 2 ? drillingTool.Diameter * 0.015 : drillingTool.Diameter * 0.01),
                        DrillingTool.Types.Tip => (drillingTool.Diameter > 2 ? drillingTool.Diameter * 0.015 : drillingTool.Diameter * 0.01),
                        DrillingTool.Types.Rapid => (drillingTool.Diameter * 0.01),
                        DrillingTool.Types.Center => (drillingTool.Diameter * 0.02),
                        _ => 0,
                    },
                    Material.Stainless => drillingTool.Type switch
                    {
                        DrillingTool.Types.Insert => Math.Round(drillingTool.Diameter * 0.0028, 2),
                        DrillingTool.Types.Solid => (drillingTool.Diameter > 2 ? drillingTool.Diameter * 0.015 : drillingTool.Diameter * 0.01),
                        DrillingTool.Types.Tip => (drillingTool.Diameter * 0.015),
                        DrillingTool.Types.Rapid => (drillingTool.Diameter * 0.01),
                        DrillingTool.Types.Center => (drillingTool.Diameter * 0.02),
                        _ => 0,
                    },
                    Material.Brass => drillingTool.Type switch
                    {
                        DrillingTool.Types.Insert => Math.Round(drillingTool.Diameter * 0.0028, 2),
                        DrillingTool.Types.Solid => (drillingTool.Diameter > 2 ? drillingTool.Diameter * 0.015 : drillingTool.Diameter * 0.01),
                        DrillingTool.Types.Tip => (drillingTool.Diameter > 2 ? drillingTool.Diameter * 0.015 : drillingTool.Diameter * 0.01),
                        DrillingTool.Types.Rapid => (drillingTool.Diameter * 0.01),
                        DrillingTool.Types.Center => (drillingTool.Diameter * 0.02),
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
        public static string Header(Machine machine, string number, string name, string author, string drawVersion, TimeSpan timeSpan) => machine switch
        {
            Machine.GS1500 =>
            "%\n" +
            $"<{number}>({name})\n" +
            $"({drawVersion.Replace(',', '.')})\n" +
            "G10 L2 P1 Z-100. B300. (G54)\n" +
            "G10 L2 P2 Z400. (G55)\n" +
            $"({author}) ({DateTime.Now:dd.MM.yy})\n" +
            $"({timeSpan.Minutes}M{timeSpan.Seconds}S)\n",

            Machine.L230A =>
            "%\n" +
            $"O0001 ({number})\n" +
            $"({name})({drawVersion.Replace(',', '.')})\n" +
            $"({author})({DateTime.Now:dd.MM.yy})\n" +
            $"({timeSpan.Minutes}M{timeSpan.Seconds}S)\n",

            Machine.A110 =>
            "%\n" +
            $"O0001 ({number})\n" +
            $"({name})({drawVersion.Replace(',', '.')})\n" +
            $"({author})({DateTime.Now:dd.MM.yy})\n" +
            $"({timeSpan.Minutes}M{timeSpan.Seconds}S)\n",

            _ => string.Empty,
        };

        /// <summary>
        /// Строка безопасности
        /// </summary>
        public static string SafetyString(Machine machine, int? speedLimit, CoordinateSystem cs) => machine switch
        {
            Machine.GS1500 =>
            TurningReferentPointConsistently +
            GoodwayReturnB +
            "G40 G80\n" +
            $"G50 S{((speedLimit ?? 0) > 4000 ? 4000 : speedLimit ?? 3500)}\n" +
            "G96\n",

            Machine.L230A =>
            TurningReferentPointConsistently +
            $"G40 G80 {cs}\n" +
            $"G50 S{((speedLimit ?? 0) > 5000 ? 5000 : speedLimit ?? 3000)}\n" +
            "G96 G23\n",

            Machine.A110 => string.Empty,

            _ => string.Empty,
        };

        /// <summary>
        /// Упор
        /// </summary>
        public static string BurnishingOperation(Machine machine, TurningBurnishingTool tool, double diameter, double startZ, double endZ)
        {
            if (tool is null || diameter is 0) return string.Empty;
            var exit = tool is TurningExternalBurnishingTool
                ? $"U1. {CoolantOff(machine)}\n"
                : $"U-1. {CoolantOff(machine)}\nG0 Z{startZ.NC()}\n";
            return machine switch
            {
                Machine.GS1500 =>
                    TurningReferentPoint +
                    tool.Description(ToolDescriptionOption.GoodwayLeft) + "\n" +
                    $"{CoolantOn(machine)}\n" +
                    $"G0 X{diameter.NC()} Z{startZ.NC()} S{BurnishingSpeed(tool)} {Direction(tool)}\n" +
                    $"G1 Z{endZ.NC()} F{BurnishingFeed(tool).NC()}\n" +
                    exit +
                    TurningReferentPoint,

                Machine.L230A =>
                    tool.Description(ToolDescriptionOption.L230) + "\n" +
                    $"{CoolantOn(machine)}\n" +
                    $"G0 X{diameter.NC()} Z{startZ.NC()} S{BurnishingSpeed(tool)} {Direction(tool)}\n" +
                    $"G1 Z{endZ.NC()} F{BurnishingFeed(tool).NC()}\n" +
                    exit +
                    TurningReferentPoint,

                _ => string.Empty,
            };
        }

        /// <summary>
        /// Упор
        /// </summary>
        public static string Limiter(Machine machine, Tool tool, double externalDiameter)
        {
            if (tool is null || externalDiameter == 0) return string.Empty;
            return machine switch
            {
                Machine.GS1500 =>
                    TurningReferentPoint +
                    $"T{tool.Position.ToolNumber()} G54 ({tool.Name})\n" +
                    $"G0 X{externalDiameter.NC(0)} Z0.5\n" +
                    SpindleUnclamp(machine) + "\n" +
                    AbsStop + "\n" +
                    "W1.\n" +
                    TurningReferentPoint,

                Machine.L230A =>
                    $"T{tool.Position.ToolNumber()} ({tool.Name})\n" +
                    $"G0 X{externalDiameter.NC(0)} Z0.5\n" +
                    SpindleUnclamp(machine) + "\n" +
                    AbsStop + "\n" +
                    "W1.\n" +
                    TurningReferentPoint,

                _ => string.Empty,
            };
        }

        /// <summary>
        /// Токарный вызов инструмента
        /// </summary>
        public static string TurningCustomOperation(Machine machine, Tool tool, string customOperation)
        {
            if (tool is null) return string.Empty;
            return machine switch
            {
                Machine.GS1500 =>
                TurningReferentPoint +
                tool.Description(ToolDescriptionOption.GoodwayLeft) + "\n" +
                $"{CoolantOn(machine)}\n" +
                (string.IsNullOrEmpty(customOperation) ? ProcessingSnippet : customOperation + '\n') +
                $"{CoolantOff(machine)}\n" +
                TurningReferentPoint,

                Machine.L230A =>
                tool.Description(ToolDescriptionOption.L230) + "\n" +
                $"{CoolantOn(machine)}\n" +
                (string.IsNullOrEmpty(customOperation) ? ProcessingSnippet : customOperation + '\n') +
                $"{CoolantOff(machine)}\n" +
                TurningReferentPoint,

                _ => string.Empty,
            };
        }

        /// <summary>
        /// Фрезерный вызов инструмента
        /// </summary>
        public static string MillingCustomOperation(Machine machine, CoordinateSystem coordinateSystem, Tool tool, string customOperation, Coolant coolant, bool polar, double safePlane)
        {
            if (tool is null) return string.Empty;
            var direction = string.Empty;
            if (coolant is not Coolant.General and not Coolant.Full)
            {
                direction = Direction(tool);
            }
            else
                direction = coolant switch
                {
                    Coolant.General or Coolant.Full when tool.Hand == Tool.ToolHand.Right => "M13",
                    Coolant.General or Coolant.Full when tool.Hand == Tool.ToolHand.Left => "M14",
                    _ => direction
                };

            return machine switch
            {
                Machine.A110 =>
                tool.Description(ToolDescriptionOption.MillingToolChange) + "\n" +
                $"{coordinateSystem}{(polar ? " G16" : string.Empty)} G0 X0 Y0 S3000 {direction}\n" +
                $"G43 Z{safePlane.NC(option: NcDecimalPointOption.Without)} H{tool.Position} {(coolant is Coolant.General ? string.Empty : CoolantOn(machine, coolant))}\n" +
                (string.IsNullOrEmpty(customOperation) ? ProcessingSnippet : customOperation + '\n') +
                $"{CoolantOff(machine, coolant)}\n" +
                $"{(polar ? "G15\n" : string.Empty)}" +
                SpindleStop + "\n" +
                MillingReferentPoint,

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
        /// <param name="startProfile">Фаска в начале контура?</param>
        /// <returns></returns>
        private static string CycleChamferExternal(double chamferDiameter, double angle, double chamferSize, bool roundCorners = true, bool startProfile = true)
        {
            double result = ((chamferDiameter - (2 * chamferSize * Math.Tan(angle.Radians()))) - 0.8 * Math.Tan(angle.Radians()));
            return chamferDiameter switch
            {
                > 0 when angle > 0 && chamferSize > 0 && !roundCorners => $"X{result.NC()}\n" +
                                                                          $"{(startProfile ? "G1 Z0.\n" : string.Empty)}" +
                                                                          $"X{chamferDiameter.NC()} A-{angle.NC()}",
                > 0 when angle > 0 && chamferSize > 0 && roundCorners => $"G0 X{result - 2 * (0.8 + 0.3)}\n" +
                                                                         $"G1 Z0.\n" +
                                                                         $"X{result.NC()} R{(0.8 + 0.3).NC()}\n" +
                                                                         $"X{chamferDiameter.NC()} A-{angle.NC()} R{(0.8 + 0.3).NC()}\n" +
                                                                         $"W-{(0.8 + 0.3).NC()}",
                _ => string.Empty
            };
        }
    }
}
