using Sunduk.PWA.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.NC
{
    public abstract class Operation
    {
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
        /// Скорость резания при сверлении
        /// </summary>
        private static int DrillCuttingSpeed(Materials material)
        {
            return material switch
            {
                Materials.Steel => 200,
                Materials.Stainless => 180,
                Materials.Brass => 350,
                _ => 0
            };
        }


        /// <summary>
        /// шапка
        /// </summary>
        public static string Header(Machines machine)
        {
            return machine switch
            {
                Machines.GS1500 =>
                "%\n" +
                "<AR00-00-000>(NAME)\n" +
                "(VER)\n" +
                "G10L2P1Z-100.B300.(G54)\n" +
                "G10L2P2Z400.(G55)\n" +
                "(AUTHOR)(DATE)\n" +
                "(0M0S)\n" +
                "\n" +
                "<TOOL TABLE>",
                Machines.L230A =>
                "%\n" +
                "o0001(AR00-00-000)\n" +
                "(NAME)(VER)\n" +
                "(AUTHOR)\n" +
                "(0M0S)\n" +
                "\n" +
                "<TOOL TABLE>",
                _ => string.Empty,
            };
        }

        /// <summary>
        /// Строка безопасности
        /// </summary>
        public static string SafetyString(Machines machine, int speedLimit)
        {
            return machine switch
            {
                Machines.GS1500 =>
                "G30U0\n" +
                "G55G30B0\n" +
                "G30W0\n" +
                "G40G80\n" +
                $"G50S{speedLimit}\n" +
                "G96\n",
                Machines.L230A =>
                "G30U0\n" +
                "G30W0\n" +
                "G23\n" +
                "G40G80G55\n" +
                $"G50S{speedLimit}\n" +
                "G96\n",
                _ => string.Empty,
            };
        }

        /// <summary>
        /// Упор
        /// </summary>
        public static string Limiter(Machines machine, double externalDiameter)
        {
            return machine switch
            {
                Machines.GS1500 =>
                "G30U0W0\n" +
                "T0000G54(UPOR)\n" +
                $"G0X{externalDiameter.NC(0)}.Z0.5\n" +
                "M10\n" +
                "M0\n" +
                "W1.\n" +
                "G30U0W0\n" +
                "\n",
                Machines.L230A =>
                "G30U0W0\n" +
                "T0000(UPOR)\n" +
                $"G0X{externalDiameter.NC(0)}.Z0.5\n" +
                "M69\n" +
                "M0\n" +
                "W1.\n" +
                "G30U0W0\n" +
                "\n",
                _ => string.Empty,
            };
        }

        /// <summary>
        /// Торцовка черновая
        /// </summary>
        public static string RoughFacing(Machines machine, Materials material, Tool tool, double externalDiameter, double internalDiameter, double roughStockAllow, double profStockAllow, double stepOver)
        {
            return machine switch
            {
                Machines.GS1500 =>
                "G30U0W0\n" +
                tool.Description(Util.Util.ToolDescriptionOption.GoodwayLeft) + "\n" +
                $"G0X{(externalDiameter + 5).NC(1)}Z{roughStockAllow.NC()}S{CuttingSpeedRough(material)}\n" +
                $"G72W{stepOver.NC()}R0.1\n" +
                "G72P1Q2F0.2\n" +
                $"N1G0Z{profStockAllow.NC()}\n" +
                $"N2G1X{(internalDiameter - 1.6).NC()}\n" +
                "M59\n" +
                "G30U0W0\n" +
                "\n",
                Machines.L230A =>
                "G30U0W0\n" +
                tool.Description(Util.Util.ToolDescriptionOption.L230) + "\n" +
                "M8\n" +
                $"G0X{(externalDiameter + 5).NC(1)}Z{roughStockAllow.NC()}S{CuttingSpeedRough(material)}\n" +
                $"G72W{stepOver.NC()}R0.1\n" +
                "G72P1Q2F0.2\n" +
                $"N1G0Z{profStockAllow.NC()}\n" +
                $"N2G1X{(internalDiameter - 1.6).NC()}\n" +
                "M9\n" +
                "G30U0W0\n" +
                "\n",
                _ => string.Empty,
            };
        }

        /// <summary>
        /// Торцовка 
        /// </summary>
        public static string Facing(Machines machine, Materials material, Tool tool, double externalDiameter, double internalDiameter, double roughStockAllow, double profStockAllow, double stepOver)
        {
            return machine switch
            {
                Machines.GS1500 =>
                "G30U0W0\n" +
                tool.Description(Util.Util.ToolDescriptionOption.GoodwayLeft) + "\n" +
                $"G0X{(externalDiameter + 5).NC(1)}Z{roughStockAllow.NC()}S{CuttingSpeedRough(material)}\n" +
                $"G72W{stepOver.NC()}R0.1\n" +
                $"G72P1Q2W{profStockAllow.NC()}F0.2\n" +
                "N1G0Z0.\n" +
                $"N2G1X{(internalDiameter - 1.6).NC()}.\n" +
                $"G70P1Q2{CuttingSpeedFinish(material)}F0.15\n" +
                "M59\n" +
                "G30U0W0\n" +
                "\n",
                Machines.L230A =>
                "G30U0W0\n" +
                tool.Description(Util.Util.ToolDescriptionOption.L230) + "\n" +
                "M8\n" +
                $"G0X{(externalDiameter + 5).NC(1)}Z{roughStockAllow.NC()}S{CuttingSpeedRough(material)}\n" +
                $"G72W{stepOver.NC()}R0.1\n" +
                $"G72P1Q2W{profStockAllow.NC()}F0.2\n" +
                "N1G0Z0.\n" +
                $"N2G1X{(internalDiameter - 1.6).NC()}.\n" +
                $"G70P1Q2{CuttingSpeedFinish(material)}F0.15\n" +
                "M9\n" +
                "G30U0W0\n" +
                "\n",
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
        public static string DrillingHighSpeed(Machines machine, Materials material)
        {
            return machine switch
            {
                Machines.GS1500 =>
                "G30U0W0\n" +
                "T0000G54M58(SVERLO)\n" +
                $"G0X{-20}Z2.S{DrillCuttingSpeed(material)}\n" +
                $"G1Z{-20}.\n" +
                $"G0Z{2}.\n" +
                "M59\n" +
                "G30U0W0\n" +
                "\n",
                Machines.L230A =>
                "G30U0W0\n" +
                "T0000(SVERLO)\n" +
                "M8\n" +
                $"G0X{-20}Z2.S{DrillCuttingSpeed(material)}\n" +
                $"G1Z{-20}.\n" +
                $"G0Z{2}.\n" +
                "M9\n" +
                "G30U0W0\n" +
                "\n",
                _ => string.Empty
            };
        }

        /// <summary>
        /// Нарезание резьбы
        /// </summary>
        public static string ThreadCutting(Machines machine, Materials material, double threadDiameter)
        {
            return machine switch
            {
                Machines.GS1500 =>
                "G30U0W0\n" +
                "T0000G54M58(REZBA)\n" +
                $"G0X{(threadDiameter + 5).NC(1)}Z2.S{120 * 1000 / (threadDiameter * Math.PI)}\nG97" +
                "G76P010160Q80R0.1\n" +
                $"G76X{threadDiameter.NC(3)}Z-10.P900Q250F1.5\n" +
                "G96M59\n" +
                "G30U0W0\n" +
                "\n",
                Machines.L230A =>
                "G30U0W0\n" +
                "T0000(REZBA)\n" +
                "M8\n" +
                $"G0X{(threadDiameter + 5).NC(1)}Z2.S{120 * 1000 / (threadDiameter * Math.PI)}\nG97" +
                "G76P010160Q80R0.1\n" +
                $"G76X{threadDiameter.NC(3)}Z-10.P900Q250F1.5\n" +
                "G96M59\n" +
                "G30U0W0\n" +
                "\n",
                _ => string.Empty,
            };
        }
    }
}
