using Sunduk.PWA.Infrastructure.CAM;
using Sunduk.PWA.Infrastructure.Sequences;
using Sunduk.PWA.Infrastructure.Sequences.ContourElements.Base;
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

        public static string TailstockOn(Machine machine) => machine.TailstockOnCode;

        public static string TailstockOff(Machine machine) => machine.TailstockOffCode;

        public static string SpindleUnclamp(Machine machine) => machine.SpindleUnclampCode;

        public static string SpindleClamp(Machine machine) => machine.SpindleClampCode;

        public static string CoolantOn(Machine machine, Coolant type = Coolant.General) => type switch
        {
            Coolant.None => string.Empty,
            Coolant.Through => machine.CoolantThroughOnCode ?? machine.CoolantOnCode,
            Coolant.Full => machine.CoolantThroughOnCode ?? machine.CoolantOnCode,
            Coolant.Blow => machine.CoolantBlowOnCode ?? machine.CoolantOnCode,
            _ => machine.CoolantOnCode,
        };

        public static string CoolantOff(Machine machine, Coolant type = Coolant.General) => type switch
        {
            Coolant.None => string.Empty,
            Coolant.Through => machine.CoolantThroughOffCode ?? machine.CoolantOffCode,
            Coolant.Full => machine.CoolantFullOffCode ?? machine.CoolantOffCode,
            Coolant.Blow => machine.CoolantBlowOffCode ?? machine.CoolantOffCode,
            _ => machine.CoolantOffCode,
        };

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
            return machine.MachineType switch
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
        public static string Header(Machine machine, string number, string name, string author, string drawVersion, TimeSpan timeSpan)
        {
            var idBlock = machine.HeaderStyle switch
            {
                HeaderStyle.AngleBracketName =>
                    $"<{number}>({name})\n" +
                    $"({drawVersion.Replace(',', '.')})\n",

                HeaderStyle.ONumber =>
                    $"O0001 ({number})\n" +
                    $"({name})({drawVersion.Replace(',', '.')})\n",

                _ => string.Empty,
            };
            var authorText = string.IsNullOrWhiteSpace(author) ? string.Empty : $"({author})";
            var dateText = $"({DateTime.Now:dd.MM.yy})";
            var trailer = machine.HeaderTrailerTemplate
                .Replace("{AUTHOR}", authorText)
                .Replace("{DATE}", dateText)
                .TrimStart(' ');
            return new GCodeBuilder()
                .Line("%")
                .Raw(idBlock)
                .RawIf(!string.IsNullOrWhiteSpace(machine.HeaderExtraLines), machine.HeaderExtraLines.TrimEnd('\n') + "\n")
                .LineIf(!string.IsNullOrWhiteSpace(trailer), trailer)
                .Line($"({timeSpan.Minutes}M{timeSpan.Seconds}S)")
                .ToString();
        }

        /// <summary>
        /// Строка безопасности — свободный шаблон станка (Machine.SafetyStringTemplate) с
        /// подстановкой {CS}/{S}.
        /// </summary>
        public static string SafetyString(Machine machine, int? speedLimit, CoordinateSystem cs)
        {
            if (string.IsNullOrWhiteSpace(machine.SafetyStringTemplate)) return string.Empty;
            var speed = (speedLimit ?? 0) > machine.SafetySpeedCap ? machine.SafetySpeedCap : speedLimit ?? machine.SafetyDefaultSpeed;
            return machine.SafetyStringTemplate.TrimEnd('\n')
                .Replace("{CS}", cs.ToString())
                .Replace("{S}", speed.ToString())
                + "\n";
        }

        /// <summary>
        /// Упор
        /// </summary>
        public static string BurnishingOperation(Machine machine, CoordinateSystem coordinateSystem, TurningBurnishingTool tool, double diameter, double startZ, double endZ, Coolant coolant = Coolant.General)
        {
            if (tool is null || diameter is 0) return string.Empty;
            var exit = tool is TurningExternalBurnishingTool
                ? $"U1. {CoolantOff(machine, coolant)}\n"
                : $"U-1. {CoolantOff(machine, coolant)}\nG0 Z{startZ.NC()}\n";
            return new GCodeBuilder()
                .ReferentPoint(machine, leading: true)
                .ToolCall(tool, machine, coordinateSystem, coolant)
                .CoordinateSystemFallback(machine, coordinateSystem)
                .CoolantOn(machine, coolant)
                .Line($"G0 X{diameter.NC()} Z{startZ.NC()} S{BurnishingSpeed(tool)} {Direction(tool)}")
                .Line($"G1 Z{endZ.NC()} F{BurnishingFeed(tool).NC()}")
                .Raw(exit)
                .ReferentPoint(machine, leading: false)
                .ToString();
        }

        /// <summary>
        /// Упор
        /// </summary>
        public static string Limiter(Machine machine, CoordinateSystem coordinateSystem, Tool tool, double externalDiameter)
        {
            if (tool is null || externalDiameter == 0) return string.Empty;
            return new GCodeBuilder()
                .ReferentPoint(machine, leading: true)
                .Line($"T{tool.Position.ToolNumber()} ({tool.Name})")
                .LineIf(machine.CoordinateSystems.Count > 1, coordinateSystem.ToString())
                .Line($"G0 X{externalDiameter.NC(0)} Z0.5")
                .Line(SpindleUnclamp(machine))
                .Line(AbsStop)
                .Line("W1.")
                .ReferentPoint(machine, leading: false)
                .ToString();
        }

        /// <summary>
        /// Токарный вызов инструмента
        /// </summary>
        public static string TurningCustomOperation(Machine machine, CoordinateSystem coordinateSystem, Tool tool, string customOperation, Coolant coolant = Coolant.General)
        {
            if (tool is null) return string.Empty;
            return new GCodeBuilder()
                .ReferentPoint(machine, leading: true)
                .ToolCall(tool, machine, coordinateSystem, coolant)
                .CoordinateSystemFallback(machine, coordinateSystem)
                .CoolantOn(machine, coolant)
                .Raw(string.IsNullOrEmpty(customOperation) ? ProcessingSnippet : customOperation + '\n')
                .CoolantOff(machine, coolant)
                .ReferentPoint(machine, leading: false)
                .ToString();
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

            if (machine.MachineType != MachineType.Milling) return string.Empty;

            return new GCodeBuilder()
                .ToolCall(tool, machine, coordinateSystem, coolant)
                .Line($"{coordinateSystem}{(polar ? " G16" : string.Empty)} G0 X0 Y0 S3000 {direction}")
                .Line($"G43 Z{safePlane.NC(option: NcDecimalPointOption.Without)} H{tool.Position} {(coolant is Coolant.General or Coolant.None || machine.ToolCallTemplateHasCoolant() ? string.Empty : CoolantOn(machine, coolant))}")
                .Raw(string.IsNullOrEmpty(customOperation) ? ProcessingSnippet : customOperation + '\n')
                .CoolantOff(machine, coolant)
                .LineIf(polar, "G15")
                .Line(SpindleStop)
                .Raw(MillingReferentPoint)
                .ToString();
        }

        /// <summary>
        /// Точение по контуру — один проход по узлам контура (Point/Line/Arc), без деления на
        /// черновые проходы со съёмом припуска (см. <see cref="GCodeBuilder.Contour"/>). И
        /// черновой, и чистовой переход сейчас трассируют один и тот же контур одним проходом,
        /// отличаясь только скоростью/подачей — многопроходная черновая обработка с реальным
        /// съёмом припуска (что-то вроде G71) не реализована. Координаты контура перед рендером
        /// пересчитываются на радиус пластины инструмента — см. <see cref="Geometry.ToolTipCompensation"/>.
        /// </summary>
        public static string ContourTurning(Machine machine, CoordinateSystem coordinateSystem, TurningTool tool, List<Element> contour, int speed, double feed, Coolant coolant)
        {
            if (tool is null || contour is null || contour.Count < 2) return string.Empty;
            if (machine.MachineType != MachineType.Turning) return string.Empty;
            var compensated = Geometry.ToolTipCompensation.Compensate(contour, tool.Radius, external: tool is TurningExternalTool);
            return new GCodeBuilder()
                .ReferentPoint(machine, leading: true)
                .ToolCall(tool, machine, coordinateSystem, coolant)
                .CoordinateSystemFallback(machine, coordinateSystem)
                .CoolantOn(machine, coolant, also: !machine.LeadingReferentPoint)
                .Contour(compensated, speedOnFirstMove: true, speed, feed, tool.Radius)
                .CoolantOff(machine, coolant)
                .ReferentPoint(machine, leading: false)
                .ToString();
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
