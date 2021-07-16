using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.NC
{
    public class Operation
    {
        public enum Machines { GS1500, L230A }
        public Machines Machine { get; set; } = Machines.GS1500;

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
                    "(0M0S)\n",
                    Machines.L230A =>
                    "%\n" +
                    "o0001(AR00-00-000)\n" +
                    "(NAME)(VER)\n" +
                    "(AUTHOR)\n" +
                    "(0M0S)\n",
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
                    $"G0X{WorkPieceExternalDiameter}.Z0.5\n" +
                    "M10\n" +
                    "M0\n" +
                    "W1.\n" +
                    "G30U0W0\n",
                    Machines.L230A =>
                    "G30U0W0\n" +
                    "T0000(UPOR)\n" +
                    $"G0X{WorkPieceExternalDiameter}.Z0.5\n" +
                    "M69\n" +
                    "M0\n" +
                    "W1.\n" +
                    "G30U0W0\n",
                    _ => string.Empty,
                };
            }
        }

        public int MaxSpindleSpeed { get; set; }

        public double WorkPieceExternalDiameter { get; set; }
        public double WorkPieceInternalDiameter { get; set; }
        public double WorkPieceLength { get; set; }
    }
}
