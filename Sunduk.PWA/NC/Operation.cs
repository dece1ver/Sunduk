using Sunduk.PWA.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.NC
{
    public class Operation
    {
        public Operation(Machines machine, Materials material, double workpieceExternalDiameter, double workpieceInternalDiameter, double workpieceLength)
        {
            Machine = machine;
            Material = material;
            WorkpieceExternalDiameter = workpieceExternalDiameter;
            WorkpieceInternalDiameter = workpieceInternalDiameter;
            WorkpieceLength = workpieceLength;
        }
        

        public Machines Machine { get; set; }
        public Materials Material { get; }
        public double WorkpieceExternalDiameter { get; }
        public double WorkpieceInternalDiameter { get; }
        public double WorkpieceLength { get; }

        /// <summary>
        /// Скорость резания на черновом точении
        /// </summary>
        public int CuttingSpeedRough { get
            {
                return Material switch
                {
                    Materials.Steel => 280,
                    Materials.Stainless => 180,
                    Materials.Brass => 350,
                    _ => 0,
                };
            }
        }

        /// <summary>
        /// Скорость резания на чистовом точении
        /// </summary>
        public int CuttingSpeedFinish
        {
            get
            {
                return Material switch
                {
                    Materials.Steel => 350,
                    Materials.Stainless => 230,
                    Materials.Brass => 450,
                    _ => 0,
                };
            }
        }

        /// <summary>
        /// шапка
        /// </summary>
        public string Header
        {
            get
            {
                return Machine switch
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
        }

        /// <summary>
        /// Строка безопасности
        /// </summary>
        public string SafetyString
        {
            get
            {
                return Machine switch
                {
                    Machines.GS1500 => 
                    "G30U0\n" +
                    "G55G30B0\n" +
                    "G30W0\n" +
                    "G40G80\n" +
                    "G50S3000\n" +
                    "G96\n",
                    Machines.L230A => 
                    "G30U0\n" +
                    "G30W0\n" +
                    "G23\n" +
                    "G40G80G55\n" +
                    "G50S3000\n" +
                    "G96\n",
                    _ => string.Empty,
                };
            }
        }

        /// <summary>
        /// Упор
        /// </summary>
        public string Limiter
        {
            get
            {
                return Machine switch
                {
                    Machines.GS1500 =>
                    "G30U0W0\n" +
                    "T0000G54(UPOR)\n" +
                    $"G0X{WorkpieceExternalDiameter.NC(0)}.Z0.5\n" +
                    "M10\n" +
                    "M0\n" +
                    "W1.\n" +
                    "G30U0W0\n" +
                    "\n",
                    Machines.L230A =>
                    "G30U0W0\n" +
                    "T0000(UPOR)\n" +
                    $"G0X{WorkpieceExternalDiameter.NC(0)}.Z0.5\n" +
                    "M69\n" +
                    "M0\n" +
                    "W1.\n" +
                    "G30U0W0\n" +
                    "\n",
                    _ => string.Empty,
                };
            }
        }

        /// <summary>
        /// Торцовка черновая
        /// </summary>
        public string FacingRough
        {
            get
            {
                return Machine switch
                {
                    Machines.GS1500 =>
                    "G30U0W0\n" +
                    "T0000G54M58(PROHOD 80 R0.8)\n" +
                    $"G0X{(WorkpieceExternalDiameter + 5).NC(1)}Z2.S{CuttingSpeedRough}\n" +
                    "G72W1.R0.1\n" +
                    "G72P1Q2F0.2\n" +
                    "N1G0Z0.2\n" +
                    $"N2G1X{(WorkpieceInternalDiameter - 1.6).NC()}.\n" +
                    "M59\n" +
                    "G30U0W0\n" +
                    "\n",
                    Machines.L230A =>
                    "G30U0W0\n" +
                    "T0000(TORC R0.8)\n" +
                    "M8" +
                    $"G0X{(WorkpieceExternalDiameter + 5).NC(1)}Z2.S{CuttingSpeedRough}\n" +
                    "G72W1.R0.1\n" +
                    "G72P1Q2F0.2\n" +
                    "N1G0Z0.2\n" +
                    $"N2G1X{(WorkpieceInternalDiameter - 1.6).NC()}.\n" +
                    "M9\n" +
                    "G30U0W0\n" +
                    "\n",
                    _ => string.Empty,
                };
            }
        }

        /// <summary>
        /// Торцовка 
        /// </summary>
        public string Facing
        {
            get
            {
                return Machine switch
                {
                    Machines.GS1500 =>
                    "G30U0W0\n" +
                    "T0000G54M58(PROHOD 80 R0.8)\n" +
                    $"G0X{(WorkpieceExternalDiameter + 5).NC(1)}Z2.S{CuttingSpeedRough}\n" +
                    "G72W1.R0.1\n" +
                    "G72P1Q2W0.2F0.2\n" +
                    "N1G0Z0.2\n" +
                    $"N2G1X{(WorkpieceInternalDiameter - 1.6).NC()}.\n" +
                    $"G70P1Q2{CuttingSpeedFinish}F0.15\n" +
                    "M59\n" +
                    "G30U0W0\n" +
                    "\n",
                    Machines.L230A =>
                    "G30U0W0\n" +
                    "T0000(TORC R0.8)\n" +
                    "M8" +
                    $"G0X{(WorkpieceExternalDiameter + 5).NC(1)}Z2.S{CuttingSpeedRough}\n" +
                    "G72W1.R0.1\n" +
                    "G72P1Q2W0.2F0.2\n" +
                    "N1G0Z0.2\n" +
                    $"N2G1X{(WorkpieceInternalDiameter - 1.6).NC()}.\n" +
                    $"G70P1Q2{CuttingSpeedFinish}F0.15\n" +
                    "M9\n" +
                    "G30U0W0\n" +
                    "\n",
                    _ => string.Empty,
                };
            }
        }

        public int MaxSpindleSpeed { get; set; }

        /// <summary>
        /// наружная фаска в пределах цикла
        /// </summary>
        /// <param name="chamferDiameter">Конечный диаметр</param>
        /// <param name="angle">Угол фаски</param>
        /// <param name="chamferSize">Размер фаски</param>
        /// <param name="roundCorners">Выполнять ли притупления R0.3 на краях фаски</param>
        /// <returns></returns>
        private string CycleChamferExternal(double chamferDiameter, double angle, double chamferSize, bool roundCorners = true, bool startProfile = true)
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
    }
}
