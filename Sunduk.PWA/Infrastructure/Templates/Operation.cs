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
                Machine.L230A => "M68", //????????
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
                    Coolant.Blow => "M59",
                    _ => string.Empty,
                },
                _ => string.Empty,
            };
        }

        public static string Direction(Tool tool) => tool.Hand == Tool.ToolHand.Rigth ? "M3" : "M4";


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
        public static int DrillCuttingSpeed(Material material, DrillingTool drillingTool)
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
            return 0.1;
        }

        /// <summary>
        /// Подача на чистовом точении
        /// </summary>
        public static double GroovingFeedFinish()
        {
            return 0.07;
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
        public static string TurningCustomOperation(Machine machine, Tool tool, string customOperation)
        {
            if (tool is null) return string.Empty;
            return machine switch
            {
                Machine.GS1500 =>
                TURNING_REFERENT_POINT +
                tool.Description(ToolDescriptionOption.GoodwayLeft) + "\n" +
                $"{CoolantOn(machine)}\n" +
                (string.IsNullOrEmpty(customOperation) ? PROCESSING_SNIPPET : customOperation + '\n') +
                $"{CoolantOff(machine)}\n" +
                TURNING_REFERENT_POINT,

                Machine.L230A =>
                tool.Description(ToolDescriptionOption.L230) + "\n" +
                $"{CoolantOn(machine)}\n" +
                (string.IsNullOrEmpty(customOperation) ? PROCESSING_SNIPPET : customOperation + '\n') +
                $"{CoolantOff(machine)}\n" +
                TURNING_REFERENT_POINT,

                _ => string.Empty,
            };
        }

        /// <summary>
        /// Фрезерный вызов инструмента
        /// </summary>
        public static string MillingCustomOperation(Machine machine, Tool tool, string customOperation, Coolant coolant = Coolant.General, bool polar = false)
        {
            if (tool is null) return string.Empty;
            string direction = string.Empty;
            if (coolant != Coolant.General)
            {
                direction = Direction(tool);
            }
            else if (coolant == Coolant.General || tool.Hand == Tool.ToolHand.Rigth)
            {
                direction = "M13";
            }
            else if (coolant == Coolant.General || tool.Hand == Tool.ToolHand.Left)
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
                (string.IsNullOrEmpty(customOperation) ? PROCESSING_SNIPPET : customOperation + '\n') +
                $"{CoolantOff(machine, coolant)}\n" +
                $"{(polar ? "G15\n" : string.Empty)}" +
                MILLING_REFERENT_POINT,

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
    }
}
