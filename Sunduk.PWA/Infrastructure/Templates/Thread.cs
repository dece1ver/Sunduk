using Sunduk.PWA.Infrastructure.CAM;
using Sunduk.PWA.Infrastructure.Sequences;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Sunduk.PWA.Infrastructure.Templates
{
    public static class Thread
    {
        public enum PassesOption { FullPasses, Infeed }


        #region Шаблоны
        public static readonly HashSet<string> MetricTemplates = new()
        {
            "M2x0.4 (Стандартный)", "M2x0.25 (Мелкий)",
            "M2.5x0.45 (Стандартный)", "M2.5x0.35 (Мелкий)",
            "M3x0.5 (Стандартный)", "M3x0.35 (Мелкий)",
            "M3.5x0.6 (Стандартный)", "M3.5x0.35 (Мелкий)",
            "M4x0.7 (Стандартный)", "M4x0.5 (Мелкий)", "M4.5x0.75", "M4.5x0.5 (Мелкий)",
            "M5x0.8 (Стандартный)", "M5x0.5 (Мелкий)", "M5.5x0.5 (Стандартный)",
            "M6x1 (Стандартный)",
            "M6x0.75 (Мелкий)",
            "M6x0.5 (Мелкий)",
            "M7x1 (Стандартный)", "M7x0.75 (Мелкий)", "M7x0.5 (Мелкий)",
            "M8x1.25 (Стандартный)",
            "M8x1 (Мелкий)",
            "M8x0.75 (Мелкий)",
            "M8x0.5 (Мелкий)",
            "M9x1.25 (Стандартный)", "M9x1 (Мелкий)", "M9x0.75 (Мелкий)", "M9x0.5 (Мелкий)",
            "M10x1.5 (Стандартный)",
            "M10x1.25 (Мелкий)",
            "M10x1 (Мелкий)",
            "M10x0.75 (Мелкий)",
            "M10x0.5 (Мелкий)",
            "M11x1.5 (Стандартный)", "M11x1 (Мелкий)", "M11x0.75 (Мелкий)", "M11x0.5 (Мелкий)",
            "M12x1.75 (Стандартный)",
            "M12x1.5 (Мелкий)",
            "M12x1.25 (Мелкий)",
            "M12x1 (Мелкий)",
            "M12x0.75 (Мелкий)",
            "M12x0.5 (Мелкий)",
            "M14x2 (Стандартный)",
            "M14x1.5 (Мелкий)",
            "M14x1.25 (Мелкий)",
            "M14x1 (Мелкий)",
            "M14x0.75 (Мелкий)",
            "M14x0.5 (Мелкий)",
            "M15x1.5 (Стандартный)", "M15x1 (Мелкий)",
            "M16x2 (Стандартный)",
            "M16x1.5 (Мелкий)",
            "M16x1 (Мелкий)",
            "M16x0.75 (Мелкий)",
            "M16x0.5 (Мелкий)",
            "M17x1.5 (Стандартный)", "M17x1 (Мелкий)",
            "M18x2.5 (Стандартный)",
            "M18x2 (Мелкий)",
            "M18x1.5 (Мелкий)",
            "M18x1 (Мелкий)",
            "M18x0.75 (Мелкий)",
            "M18x0.5 (Мелкий)",
            "M20x2.5 (Стандартный)",
            "M20x2 (Мелкий)",
            "M20x1.5 (Мелкий)",
            "M20x1 (Мелкий)",
            "M20x0.75 (Мелкий)",
            "M20x0.5 (Мелкий)",
            "M22x2.5 (Стандартный)", "M22x2 (Мелкий)", "M22x1.5 (Мелкий)", "M22x1 (Мелкий)", "M22x0.75 (Мелкий)", "M22x0.5 (Мелкий)",
            "M24x3 (Стандартный)",
            "M24x2 (Мелкий)",
            "M24x1.5 (Мелкий)",
            "M24x1 (Мелкий)",
            "M24x0.75 (Мелкий)",
            "M25x2 (Стандартный)", "M25x1.5 (Мелкий)", "M25x1 (Мелкий)",
            "M26x1.5 (Стандартный)",
            "M27x3 (Стандартный)", "M27x2 (Мелкий)", "M27x1.5 (Мелкий)", "M27x1 (Мелкий)", "M27x0.75 (Мелкий)",
            "M27x2 (Стандартный)", "M27x1.5 (Мелкий)", "M27x1 (Мелкий)",
            "M30x3.5 (Стандартный)",
            "M30x3 (Мелкий)",
            "M30x2 (Мелкий)",
            "M30x1.5 (Мелкий)",
            "M30x1 (Мелкий)",
            "M30x0.75 (Мелкий)",
            "M32x2 (Стандартный)", "M32x1.5 (Мелкий)",
            "M33x3.5 (Стандартный)",
            "M33x3 (Мелкий)",
            "M33x2 (Мелкий)",
            "M33x1.5 (Мелкий)",
            "M33x1 (Мелкий)",
            "M33x0.75 (Мелкий)",
            "M35x1.5 (Стандартный)",
            "M36x4 (Стандартный)",
            "M36x3 (Мелкий)",
            "M36x2 (Мелкий)",
            "M36x1.5 (Мелкий)",
            "M36x1 (Мелкий)",
            "M38x1.5 (Стандартный)",
            "M39x4 (Стандартный)", "M39x3 (Мелкий)", "M39x2 (Мелкий)", "M39x1.5 (Мелкий)", "M39x1 (Мелкий)",
            "M40x3 (Стандартный)", "M40x2 (Мелкий)", "M40x1.5 (Мелкий)",
            "M42x4.5 (Стандартный)", "M42x4 (Мелкий)", "M42x3 (Мелкий)", "M42x2 (Мелкий)", "M42x1.5 (Мелкий)", "M42x1 (Мелкий)",
            "M45x4.5 (Стандартный)", "M45x4 (Мелкий)", "M45x3 (Мелкий)", "M45x2 (Мелкий)", "M45x1.5 (Мелкий)", "M45x1 (Мелкий)",
            "M48x5 (Стандартный)", "M48x4 (Мелкий)", "M48x3 (Мелкий)", "M48x2 (Мелкий)", "M48x1.5 (Мелкий)", "M48x1 (Мелкий)",
            "M50x3 (Стандартный)", "M50x2 (Мелкий)", "M50x1.5 (Мелкий)",
            "M52x5 (Стандартный)", "M52x4 (Мелкий)", "M52x3 (Мелкий)", "M52x2 (Мелкий)", "M52x1.5 (Мелкий)", "M52x1 (Мелкий)",
            "M55x4 (Стандартный)", "M55x3 (Мелкий)", "M55x2 (Мелкий)", "M55x1.5 (Мелкий)",
            "M56x5.5 (Стандартный)", "M56x4 (Мелкий)", "M56x3 (Мелкий)", "M56x2 (Мелкий)", "M56x1.5 (Мелкий)", "M56x1 (Мелкий)",
            "M58x4 (Стандартный)", "M58x3 (Мелкий)", "M58x2 (Мелкий)", "M58x1.5 (Мелкий)",
            "M60x5.5 (Стандартный)", "M60x4 (Мелкий)", "M60x3 (Мелкий)", "M60x2 (Мелкий)", "M60x1.5 (Мелкий)", "M60x1 (Мелкий)",
            "M62x4 (Стандартный)", "M62x3 (Мелкий)", "M62x2 (Мелкий)", "M62x1.5 (Мелкий)",
            "M64x6 (Стандартный)", "M64x4 (Мелкий)", "M64x3 (Мелкий)", "M64x2 (Мелкий)", "M64x1.5 (Мелкий)", "M64x1 (Мелкий)",
            "M65x4 (Стандартный)", "M65x3 (Мелкий)", "M65x2 (Мелкий)", "M65x1.5 (Мелкий)",
            "M68x6 (Стандартный)", "M68x4 (Мелкий)", "M68x3 (Мелкий)", "M68x2 (Мелкий)", "M68x1.5 (Мелкий)", "M68x1 (Мелкий)",
            "M70x6 (Стандартный)", "M70x4 (Мелкий)", "M70x3 (Мелкий)", "M70x2 (Мелкий)", "M70x1.5 (Мелкий)",
            "M72x6 (Стандартный)", "M72x4 (Мелкий)", "M72x3 (Мелкий)", "M72x2 (Мелкий)", "M72x1.5 (Мелкий)", "M72x1 (Мелкий)",
            "M75x4 (Стандартный)", "M75x3 (Мелкий)", "M75x2 (Мелкий)", "M75x1.5 (Мелкий)",
            "M76x6 (Стандартный)", "M76x4 (Мелкий)", "M76x3 (Мелкий)", "M76x2 (Мелкий)", "M76x1.5 (Мелкий)", "M76x1 (Мелкий)",
            "M78x2 (Стандартный)", 
            "M80x6 (Стандартный)", "M80x4 (Мелкий)", "M80x3 (Мелкий)", "M80x2 (Мелкий)", "M80x1.5 (Мелкий)", "M80x1 (Мелкий)",
            "M82x2 (Стандартный)",
            "M85x6 (Стандартный)", "M85x4 (Мелкий)", "M85x3 (Мелкий)", "M85x2 (Мелкий)", "M85x1.5 (Мелкий)",
            "M90x6 (Стандартный)", "M90x4 (Мелкий)", "M90x3 (Мелкий)", "M90x2 (Мелкий)", "M90x1.5 (Мелкий)",
            "M95x6 (Стандартный)", "M95x4 (Мелкий)", "M95x3 (Мелкий)", "M95x2 (Мелкий)", "M95x1.5 (Мелкий)",
            "M100x6 (Стандартный)", "M100x4 (Мелкий)", "M100x3 (Мелкий)", "M100x2 (Мелкий)", "M100x1.5 (Мелкий)",
            "M105x6 (Стандартный)", "M105x4 (Мелкий)", "M105x3 (Мелкий)", "M105x2 (Мелкий)", "M105x1.5 (Мелкий)",
            "M110x6 (Стандартный)", "M110x4 (Мелкий)", "M110x3 (Мелкий)", "M110x2 (Мелкий)", "M110x1.5 (Мелкий)",
            "M115x6 (Стандартный)", "M115x4 (Мелкий)", "M115x3 (Мелкий)", "M115x2 (Мелкий)", "M115x1.5 (Мелкий)",
            "M120x6 (Стандартный)", "M120x4 (Мелкий)", "M120x3 (Мелкий)", "M120x2 (Мелкий)", "M120x1.5 (Мелкий)",
            "M125x8 (Стандартный)", "M125x6 (Мелкий)", "M125x4 (Мелкий)", "M125x3 (Мелкий)", "M125x2 (Мелкий)", "M125x1.5 (Мелкий)",
            "M130x8 (Стандартный)", "M130x6 (Мелкий)", "M130x4 (Мелкий)", "M130x3 (Мелкий)", "M130x2 (Мелкий)", "M130x1.5 (Мелкий)",
            "M135x6 (Стандартный)", "M135x4 (Мелкий)", "M135x3 (Мелкий)", "M135x2 (Мелкий)", "M135x1.5 (Мелкий)",
            "M140x8 (Стандартный)", "M140x6 (Мелкий)", "M140x4 (Мелкий)", "M140x3 (Мелкий)", "M140x2 (Мелкий)", "M140x1.5 (Мелкий)",
            "M145x6 (Стандартный)", "M145x4 (Мелкий)", "M145x3 (Мелкий)", "M145x2 (Мелкий)", "M145x1.5 (Мелкий)",
            "M150x8 (Стандартный)", "M150x6 (Мелкий)", "M150x4 (Мелкий)", "M150x3 (Мелкий)", "M150x2 (Мелкий)", "M150x1.5 (Мелкий)",
            "M155x6 (Стандартный)", "M155x4 (Мелкий)", "M155x3 (Мелкий)", "M155x2 (Мелкий)",
            "M160x8 (Стандартный)", "M160x6 (Мелкий)", "M160x4 (Мелкий)", "M160x3 (Мелкий)", "M160x2 (Мелкий)",
            "M165x6 (Стандартный)", "M165x4 (Мелкий)", "M165x3 (Мелкий)", "M165x2 (Мелкий)",
            "M170x8 (Стандартный)", "M170x6 (Мелкий)", "M170x4 (Мелкий)", "M170x3 (Мелкий)", "M170x2 (Мелкий)",
            "M175x6 (Стандартный)", "M175x4 (Мелкий)", "M175x3 (Мелкий)", "M175x2 (Мелкий)",
            "M180x8 (Стандартный)", "M180x6 (Мелкий)", "M180x4 (Мелкий)", "M180x3 (Мелкий)", "M180x2 (Мелкий)",
            "M185x6 (Стандартный)", "M185x4 (Мелкий)", "M185x3 (Мелкий)", "M185x2 (Мелкий)",
            "M190x8 (Стандартный)", "M190x6 (Мелкий)", "M190x4 (Мелкий)", "M190x3 (Мелкий)", "M190x2 (Мелкий)",
            "M195x6 (Стандартный)", "M195x4 (Мелкий)", "M195x3 (Мелкий)", "M195x2 (Мелкий)",
            "M200x8 (Стандартный)", "M200x6 (Мелкий)", "M200x4 (Мелкий)", "M200x3 (Мелкий)", "M200x2 (Мелкий)",
            "M205x6 (Стандартный)", "M205x4 (Мелкий)", "M205x3 (Мелкий)",
            "M210x8 (Стандартный)", "M210x6 (Мелкий)", "M210x4 (Мелкий)", "M210x3 (Мелкий)",
            "M215x6 (Стандартный)", "M215x4 (Мелкий)", "M215x3 (Мелкий)",
            "M220x8 (Стандартный)", "M220x6 (Мелкий)", "M220x4 (Мелкий)", "M220x3 (Мелкий)",
            "M225x6 (Стандартный)", "M225x4 (Мелкий)", "M225x3 (Мелкий)",
            "M230x8 (Стандартный)", "M230x6 (Мелкий)", "M230x4 (Мелкий)", "M230x3 (Мелкий)",
            "M235x6 (Стандартный)", "M235x4 (Мелкий)", "M235x3 (Мелкий)",
            "M240x8 (Стандартный)", "M240x6 (Мелкий)", "M240x4 (Мелкий)", "M240x3 (Мелкий)",
            "M245x6 (Стандартный)", "M245x4 (Мелкий)", "M245x3 (Мелкий)",
            "M250x8 (Стандартный)", "M250x6 (Мелкий)", "M250x4 (Мелкий)", "M250x3 (Мелкий)",
            "M255x6 (Стандартный)", "M255x4 (Мелкий)", "M255x3 (Мелкий)",
            "M260x8 (Стандартный)", "M260x6 (Мелкий)", "M260x4 (Мелкий)", "M260x3 (Мелкий)",
            "M265x6 (Стандартный)", "M265x4 (Мелкий)", "M265x3 (Мелкий)",
            "M270x8 (Стандартный)", "M270x6 (Мелкий)", "M270x4 (Мелкий)", "M270x3 (Мелкий)",
            "M275x6 (Стандартный)", "M275x4 (Мелкий)", "M275x3 (Мелкий)",
            "M280x8 (Стандартный)", "M280x6 (Мелкий)", "M280x4 (Мелкий)", "M280x3 (Мелкий)",
            "M285x6 (Стандартный)", "M285x4 (Мелкий)", "M285x3 (Мелкий)",
            "M290x8 (Стандартный)", "M290x6 (Мелкий)", "M290x4 (Мелкий)", "M290x3 (Мелкий)",
            "M295x6 (Стандартный)", "M295x4 (Мелкий)", "M295x3 (Мелкий)",
            "M300x8 (Стандартный)", "M300x6 (Мелкий)", "M300x4 (Мелкий)", "M300x3 (Мелкий)",
        };

        public const string Bspp1d16 = "G1/16";
        public const string Bspp1d8 = "G1/8";
        public const string Bspp1d4 = "G1/4";
        public const string Bspp3d8 = "G3/8";
        public const string Bspp1d2 = "G1/2";
        public const string Bspp5d8 = "G5/8";
        public const string Bspp3d4 = "G3/4";
        public const string Bspp7d8 = "G7/8";
        public const string Bspp1 = "G1";
        public const string Bspp1N1d8 = "G1⅛";
        public const string Bspp1N1d4 = "G1¼";
        public const string Bspp1N3d8 = "G1⅜";
        public const string Bspp1N1d2 = "G1½";
        public const string Bspp1N3d4 = "G1¾";
        public const string Bspp2 = "G2";
        public const string Bspp2N1d4 = "G2¼";
        public const string Bspp2N1d2 = "G2½";
        public const string Bspp2N3d4 = "G2¾";
        public const string Bspp3 = "G3";
        public const string Bspp3N1d4 = "G3¼";
        public const string Bspp3N1d2 = "G3½";
        public const string Bspp3N3d4 = "G3¾";
        public const string Bspp4 = "G4";
        public const string Bspp4N1d2 = "G4½";
        public const string Bspp5 = "G5";
        public const string Bspp5N1d2 = "G5½";
        public const string Bspp6 = "G6";

        public static readonly HashSet<string> BsppTemplates = new()
        {
            Bspp1d16,
            Bspp1d8,
            Bspp1d4,
            Bspp3d8,
            Bspp1d2,
            Bspp5d8,
            Bspp3d4,
            Bspp7d8,
            Bspp1,
            Bspp1N1d8,
            Bspp1N1d4,
            Bspp1N3d8,
            Bspp1N1d2,
            Bspp1N3d4,
            Bspp2,
            Bspp2N1d4,
            Bspp2N1d2,
            Bspp2N3d4,
            Bspp3,
            Bspp3N1d4,
            Bspp3N1d2,
            Bspp3N3d4,
            Bspp4,
            Bspp4N1d2,
            Bspp5,
            Bspp5N1d2,
            Bspp6
        };

        public const string Bspt1d16Ex = "R1/16";
        public const string Bspt1d8Ex = "R1/8";
        public const string Bspt1d4Ex = "R1/4";
        public const string Bspt3d8Ex = "R3/8";
        public const string Bspt1d2Ex = "R1/2";
        public const string Bspt3d4Ex = "R3/4";
        public const string Bspt1Ex = "R1";
        public const string Bspt1N1d4Ex = "R1¼";
        public const string Bspt1N1d2Ex = "R1½";
        public const string Bspt2Ex = "R2";
        public const string Bspt2N1d2Ex = "R2½";
        public const string Bspt3Ex = "R3";
        public const string Bspt3N1d2Ex = "R3½";
        public const string Bspt4Ex = "R4";
        public const string Bspt5Ex = "R5";
        public const string Bspt6Ex = "R6";

        public static readonly HashSet<string> BsptTemplatesEx = new()
        {
            Bspt1d16Ex,
            Bspt1d8Ex,
            Bspt1d4Ex,
            Bspt3d8Ex,
            Bspt1d2Ex,
            Bspt3d4Ex,
            Bspt1Ex,
            Bspt1N1d4Ex,
            Bspt1N1d2Ex,
            Bspt2Ex,
            Bspt2N1d2Ex,
            Bspt3Ex,
            Bspt3N1d2Ex,
            Bspt4Ex,
            Bspt5Ex,
            Bspt6Ex
        };

        public const string Bspt1d16In = "Rc1/16";
        public const string Bspt1d8In = "Rc1/8";
        public const string Bspt1d4In = "Rc1/4";
        public const string Bspt3d8In = "Rc3/8";
        public const string Bspt1d2In = "Rc1/2";
        public const string Bspt3d4In = "Rc3/4";
        public const string Bspt1In = "Rc1";
        public const string Bspt1N1d4In = "Rc1¼";
        public const string Bspt1N1d2In = "Rc1½";
        public const string Bspt2In = "Rc2";
        public const string Bspt2N1d2In = "Rc2½";
        public const string Bspt3In = "Rc3";
        public const string Bspt3N1d2In = "Rc3½";
        public const string Bspt4In = "Rc4";
        public const string Bspt5In = "Rc5";
        public const string Bspt6In = "Rc6";

        public static readonly HashSet<string> BsptTemplatesIn = new()
        {
            Bspt1d16In,
            Bspt1d8In,
            Bspt1d4In,
            Bspt3d8In,
            Bspt1d2In,
            Bspt3d4In,
            Bspt1In,
            Bspt1N1d4In,
            Bspt1N1d2In,
            Bspt2In,
            Bspt2N1d2In,
            Bspt3In,
            Bspt3N1d2In,
            Bspt4In,
            Bspt5In,
            Bspt6In
        };

        public const string Unc1 = "UNC #1";
        public const string Unc2 = "UNC #2";
        public const string Unc3 = "UNC #3";
        public const string Unc4 = "UNC #4";
        public const string Unc5 = "UNC #5";
        public const string Unc6 = "UNC #6";
        public const string Unc8 = "UNC #8";
        public const string Unc10 = "UNC #10";
        public const string Unc12 = "UNC #12";
        public const string Unc1d4 = "UNC 1/4";
        public const string Unc5d16 = "UNC 5/16";
        public const string Unc3d8 = "UNC 3/8";
        public const string Unc7d16 = "UNC 7/16";
        public const string Unc1d2 = "UNC 1/2";
        public const string Unc9d16 = "UNC 9/16";
        public const string Unc5d8 = "UNC 5/8";
        public const string Unc3d4 = "UNC 3/4";
        public const string Unc7d8 = "UNC 7/8";
        public const string Unc1N = "UNC 1";
        public const string Unc1N1d8 = "UNC 1⅛";
        public const string Unc1N1d4 = "UNC 1¼";
        public const string Unc1N3d8 = "UNC 1⅜";
        public const string Unc1N1d2 = "UNC 1½";
        public const string Unc1N3d4 = "UNC 1¾";
        public const string Unc2N = "UNC 2";
        public const string Unc2N1d4 = "UNC 2¼";
        public const string Unc2N1d2 = "UNC 2½";
        public const string Unc2N3d4 = "UNC 2¾";
        public const string Unc3N = "UNC 3";
        public const string Unc3N1d4 = "UNC 3¼";
        public const string Unc3N1d2 = "UNC 3½";
        public const string Unc3N3d4 = "UNC 3¾";
        public const string Unc4N = "UNC 4";

        public static readonly HashSet<string> UncTemplates = new()
        {
            Unc1,
            Unc2,
            Unc3,
            Unc4,
            Unc5,
            Unc6,
            Unc8,
            Unc10,
            Unc12,
            Unc1d4,
            Unc5d16,
            Unc3d8,
            Unc7d16,
            Unc1d2,
            Unc9d16,
            Unc5d8,
            Unc3d4,
            Unc7d8,
            Unc1N,
            Unc1N1d8,
            Unc1N1d4,
            Unc1N3d8,
            Unc1N1d2,
            Unc1N3d4,
            Unc2N,
            Unc2N1d4,
            Unc2N1d2,
            Unc2N3d4,
            Unc3N,
            Unc3N1d4,
            Unc3N1d2,
            Unc3N3d4,
            Unc4N
        };


        public const string Unf0 = "UNF #0";
        public const string Unf1 = "UNF #1";
        public const string Unf2 = "UNF #2";
        public const string Unf3 = "UNF #3";
        public const string Unf4 = "UNF #4";
        public const string Unf5 = "UNF #5";
        public const string Unf6 = "UNF #6";
        public const string Unf8 = "UNF #8";
        public const string Unf10 = "UNF #10";
        public const string Unf12 = "UNF #12";
        public const string Unf1d4 = "UNF 1/4";
        public const string Unf5d16 = "UNF 5/16";
        public const string Unf3d8 = "UNF 3/8";
        public const string Unf7d16 = "UNF 7/16";
        public const string Unf1d2 = "UNF 1/2";
        public const string Unf9d16 = "UNF 9/16";
        public const string Unf5d8 = "UNF 5/8";
        public const string Unf3d4 = "UNF 3/4";
        public const string Unf7d8 = "UNF 7/8";
        public const string Unf1N = "UNF 1";
        public const string Unf1N1d8 = "UNF 1⅛";
        public const string Unf1N1d4 = "UNF 1¼";
        public const string Unf1N3d8 = "UNF 1⅜";
        public const string Unf1N1d2 = "UNF 1½";

        public static readonly HashSet<string> UnfTemplates = new()
        {
            Unf0,
            Unf1,
            Unf2,
            Unf3,
            Unf4,
            Unf5,
            Unf6,
            Unf8,
            Unf10,
            Unf12,
            Unf1d4,
            Unf5d16,
            Unf3d8,
            Unf7d16,
            Unf1d2,
            Unf9d16,
            Unf5d8,
            Unf3d4,
            Unf7d8,
            Unf1N,
            Unf1N1d8,
            Unf1N1d4,
            Unf1N3d8,
            Unf1N1d2
        };

        public const string Unef12 = "UNEF #12";
        public const string Unef1d4 = "UNEF 1/4";
        public const string Unef5d16 = "UNEF 5/16";
        public const string Unef3d8 = "UNEF 3/8";
        public const string Unef7d16 = "UNEF 7/16";
        public const string Unef1d2 = "UNEF 1/2";
        public const string Unef9d16 = "UNEF 9/16";
        public const string Unef5d8 = "UNEF 5/8";
        public const string Unef3d4 = "UNEF 3/4";
        public const string Unef7d8 = "UNEF 7/8";
        public const string Unef1N = "UNEF 1";

        public static readonly HashSet<string> UnefTemplates = new()
        {
            Unef12,
            Unef1d4,
            Unef5d16,
            Unef3d8,
            Unef7d16,
            Unef1d2,
            Unef9d16,
            Unef5d8,
            Unef3d4,
            Unef7d8,
            Unef1N,
        };

        public static readonly HashSet<string> TrapezoidalTemplates = new()
        {
            "Tr8x2 (Стандартный)", "Tr8x1.5 (Мелкий)",
            "Tr9x2 (Стандартный)", "Tr9x1.5 (Мелкий)",
            "Tr10x2 (Стандартный)", "Tr10x1.5 (Мелкий)",
            "Tr11x3 (Стандартный)", "Tr11x2 (Мелкий)",
            "Tr12x3 (Стандартный)", "Tr12x2 (Мелкий)",
            "Tr14x3 (Стандартный)", "Tr14x2 (Мелкий)",
            "Tr16x4 (Стандартный)",
            "Tr16x2 (Мелкий)",
            "Tr18x4 (Стандартный)",
            "Tr18x2 (Мелкий)",
            "Tr20x4 (Стандартный)",
            "Tr20x2 (Мелкий)",
            "Tr22x8 (Стандартный)", "Tr22x5 (Мелкий)", "Tr22x3 (Мелкий)", "Tr22x2 (Мелкий)",
            "Tr24x8 (Стандартный)",
            "Tr24x5 (Мелкий)",
            "Tr24x3 (Мелкий)",
            "Tr24x2 (Мелкий)",
            "Tr26x8 (Стандартный)", "Tr26x5 (Мелкий)", "Tr26x3 (Мелкий)", "Tr26x2 (Мелкий)",
            "Tr28x8 (Стандартный)", "Tr28x5 (Мелкий)", "Tr28x3 (Мелкий)", "Tr28x2 (Мелкий)",
            "Tr30x10 (Стандартный)",
            "Tr30x6 (Мелкий)",
            "Tr30x3 (Мелкий)",
            "Tr32x10 (Стандартный)", "Tr32x6 (Мелкий)", "Tr32x3 (Мелкий)",
            "Tr34x10 (Стандартный)", "Tr34x6 (Мелкий)", "Tr34x3 (Мелкий)",
            "Tr36x10 (Стандартный)", "Tr36x6 (Мелкий)", "Tr36x3 (Мелкий)",
            "Tr38x10 (Стандартный)", "Tr38x7 (Мелкий)", "Tr38x6 (Мелкий)", "Tr38x3 (Мелкий)",
            "Tr40x10 (Стандартный)",
            "Tr40x7 (Мелкий)",
            "Tr40x6 (Мелкий)",
            "Tr40x3 (Мелкий)",
            "Tr42x10 (Стандартный)", "Tr42x7 (Мелкий)", "Tr42x6 (Мелкий)", "Tr42x3 (Мелкий)",
            "Tr44x12 (Стандартный)", "Tr44x8 (Мелкий)", "Tr44x7 (Мелкий)", "Tr44x3 (Мелкий)",
            "Tr46x12 (Стандартный)", "Tr46x8 (Мелкий)", "Tr46x3 (Мелкий)",
            "Tr48x12 (Стандартный)", "Tr48x8 (Мелкий)", "Tr48x3 (Мелкий)",
            "Tr50x12 (Стандартный)",
            "Tr50x8 (Мелкий)",
            "Tr50x3 (Мелкий)",
            "Tr52x12 (Стандартный)", "Tr52x8 (Мелкий)", "Tr52x3 (Мелкий)",
            "Tr55x14 (Стандартный)", "Tr55x12 (Мелкий)", "Tr55x9 (Мелкий)", "Tr55x8 (Мелкий)", "Tr55x3 (Мелкий)",
            "Tr60x14 (Стандартный)",
            "Tr60x12 (Мелкий)",
            "Tr60x9 (Мелкий)",
            "Tr60x8 (Мелкий)",
            "Tr60x3 (Мелкий)",
            "Tr65x16 (Стандартный)", "Tr65x10 (Мелкий)", "Tr65x4 (Мелкий)",
            "Tr70x16 (Стандартный)",
            "Tr70x10 (Мелкий)",
            "Tr70x4 (Мелкий)",
            "Tr75x16 (Стандартный)", "Tr75x10 (Мелкий)", "Tr75x4 (Мелкий)",
            "Tr80x16 (Стандартный)",
            "Tr80x10 (Мелкий)",
            "Tr80x4 (Мелкий)",
            "Tr85x20 (Стандартный)", "Tr85x18 (Мелкий)", "Tr85x12 (Мелкий)", "Tr85x5 (Мелкий)", "Tr85x4 (Мелкий)",
            "Tr90x20 (Стандартный)",
            "Tr90x18 (Мелкий)",
            "Tr90x12 (Мелкий)",
            "Tr90x5 (Мелкий)",
            "Tr90x4 (Мелкий)",
            "Tr95x20 (Стандартный)", "Tr95x18 (Мелкий)", "Tr95x12 (Мелкий)", "Tr95x5 (Мелкий)", "Tr95x4 (Мелкий)",
            "Tr100x20 (Стандартный)",
            "Tr100x12 (Мелкий)",
            "Tr100x5 (Мелкий)",
            "Tr100x4 (Мелкий)",
            "Tr110x20 (Стандартный)", "Tr110x12 (Мелкий)", "Tr110x5 (Мелкий)", "Tr110x4 (Мелкий)",
        };

        public const string Npt1d16 = "K1/16";
        public const string Npt1d8 = "K1/8";
        public const string Npt1d4 = "K1/4";
        public const string Npt3d8 = "K3/8";
        public const string Npt1d2 = "K1/2";
        public const string Npt3d4 = "K3/4";
        public const string Npt1 = "K1";
        public const string Npt1N1d4 = "K1¼";
        public const string Npt1N1d2 = "K1½";
        public const string Npt2 = "K2";

        public static readonly HashSet<string> NptTemplates = new()
        {
            Npt1d16,
            Npt1d8,
            Npt1d4,
            Npt3d8,
            Npt1d2,
            Npt3d4,
            Npt1,
            Npt1N1d4,
            Npt1N1d2,
            Npt2
        };

        public static double BsppHoleDiameter(string template)
        {
            return template switch
            {
                "G1/16" => 6.7,
                "G1/8" => 8.7,
                "G1/4" => 11.6,
                "G3/8" => 15.1,
                "G1/2" => 18.7,
                "G5/8" => 20.7,
                "G3/4" => 24.2,
                "G7/8" => 28,
                "G1" => 30.5,
                "G1⅛" => 35,
                "G1¼" => 39,
                "G1⅜" => 41.5,
                "G1½" => 45,
                "G1¾" => 51,
                "G2" => 56.9,
                "G2¼" => 63,
                "G2½" => 72.4,
                "G2¾" => 78.8,
                "G3" => 85.1,
                "G3¼" => 91.2,
                "G3½" => 97.6,
                "G3¾" => 103.9,
                "G4" => 110.3,
                "G4½" => 123,
                "G5" => 135.7,
                "G5½" => 148.4,
                "G6" => 161.1,
                _ => 0
            };
        }

        public static double NptHoleDiameter(string template)
        {
            return template switch
            {
                "K1/16" => 6.4,
                "K1/8" => 8.8,
                "K1/4" => 11.3,
                "K3/8" => 14.8,
                "K1/2" => 18.3,
                "K3/4" => 23.7,
                "K1" => 29.7,
                "K1¼" => 38.6,
                "K1½" => 44.7,
                "K2" => 56.6,
                _ => 0
            };
        }

        public static double NptHoleLength(string template)
        {
            return template switch
            {
                "K1/16" => 13,
                "K1/8" => 14,
                "K1/4" => 20,
                "K3/8" => 21,
                "K1/2" => 26.5,
                "K3/4" => 26.5,
                "K1" => 33.5,
                "K1¼" => 34.5,
                "K1½" => 34.5,
                "K2" => 35,
                _ => 0
            };
        }

        public static string SimpleThreadTemplate(string template)
        {
            return template
                .Replace("½", " 1/2")
                .Replace("¼", " 1/4")
                .Replace("¾", " 3/4")
                .Replace("⅜", " 3/8")
                .Replace("⅛", " 1/8");
        }
        #endregion

        #region Чтение шаблонов
        public static void GetMetricValues(string template, out string diameter, out string pitch)
        {
            if (string.IsNullOrWhiteSpace(template))
            {
                diameter = "";
                pitch = "";
            }
            diameter = template.Split("M")[1].Split('x')[0];
            pitch = template.Split("M")[1].Split('x')[1].Split()[0];
        }

        public static void GetBsppValues(string template, out string diameter, out string pitch)
        {
            switch (template)
            {
                case Bspp1d16:
                    diameter = "7.723";
                    pitch = "0.907";
                    break;
                case Bspp1d8:
                    diameter = "9.728";
                    pitch = "0.907";
                    break;
                case Bspp1d4:
                    diameter = "13.157";
                    pitch = "1.337";
                    break;
                case Bspp3d8:
                    diameter = "16.662";
                    pitch = "1.337";
                    break;
                case Bspp1d2:
                    diameter = "20.995";
                    pitch = "1.814";
                    break;
                case Bspp5d8:
                    diameter = "22.911";
                    pitch = "1.814";
                    break;
                case Bspp3d4:
                    diameter = "26.441";
                    pitch = "1.814";
                    break;
                case Bspp7d8:
                    diameter = "30.201";
                    pitch = "1.814";
                    break;
                case Bspp1:
                    diameter = "33.249";
                    pitch = "2.309";
                    break;
                case Bspp1N1d8:
                    diameter = "37.897";
                    pitch = "2.309";
                    break;
                case Bspp1N1d4:
                    diameter = "41.91";
                    pitch = "2.309";
                    break;
                case Bspp1N3d8:
                    diameter = "44.323";
                    pitch = "2.309";
                    break;
                case Bspp1N1d2:
                    diameter = "47.803";
                    pitch = "2.309";
                    break;
                case Bspp1N3d4:
                    diameter = "53.746";
                    pitch = "2.309";
                    break;
                case Bspp2:
                    diameter = "59.614";
                    pitch = "2.309";
                    break;
                case Bspp2N1d4:
                    diameter = "65.71";
                    pitch = "2.309";
                    break;
                case Bspp2N1d2:
                    diameter = "75.184";
                    pitch = "2.309";
                    break;
                case Bspp2N3d4:
                    diameter = "81.534";
                    pitch = "2.309";
                    break;
                case Bspp3:
                    diameter = "87.884";
                    pitch = "2.309";
                    break;
                case Bspp3N1d4:
                    diameter = "93.98";
                    pitch = "2.309";
                    break;
                case Bspp3N1d2:
                    diameter = "100.33";
                    pitch = "2.309";
                    break;
                case Bspp3N3d4:
                    diameter = "106.68";
                    pitch = "2.309";
                    break;
                case Bspp4:
                    diameter = "113.03";
                    pitch = "2.309";
                    break;
                case Bspp4N1d2:
                    diameter = "125.73";
                    pitch = "2.309";
                    break;
                case Bspp5:
                    diameter = "138.43";
                    pitch = "2.309";
                    break;
                case Bspp5N1d2:
                    diameter = "151.13";
                    pitch = "2.309";
                    break;
                case Bspp6:
                    diameter = "163.83";
                    pitch = "2.309";
                    break;
                default:
                    diameter = string.Empty;
                    pitch = string.Empty;
                    break;
            }
        }

        public static void GetTrapezoidalValues(string template, out string diameter, out string pitch)
        {
            var result = template.Split("Tr")[1].Split('x');
            diameter = result[0];
            pitch = result[1].Split()[0];
        }

        public static void GetNptValues(string template, out string externalDiameter, out string internalDiameter, out string pitch, out double planeLength, out double threadLength)
        {
            switch (template)
            {
                case Npt1d16:
                    externalDiameter = "7.895";
                    internalDiameter = "6.389";
                    pitch = "0.941";
                    planeLength = 4.064;
                    threadLength = 6.5;
                    break;
                case Npt1d8:
                    externalDiameter = "10.272";
                    internalDiameter = "8.766";
                    pitch = "0.941";
                    planeLength = 4.572;
                    threadLength = 7;
                    break;
                case Npt1d4:
                    externalDiameter = "13.572";
                    internalDiameter = "11.314";
                    pitch = "1.411";
                    planeLength = 5.08;
                    threadLength = 9.5;
                    break;
                case Npt3d8:
                    externalDiameter = "17.055";
                    internalDiameter = "14.797";
                    pitch = "1.411";
                    planeLength = 6.096;
                    threadLength = 10.5;
                    break;
                case Npt1d2:
                    externalDiameter = "21.223";
                    internalDiameter = "18.321";
                    pitch = "1.814";
                    planeLength = 8.128;
                    threadLength = 13.5;
                    break;
                case Npt3d4:
                    externalDiameter = "26.568";
                    internalDiameter = "23.666";
                    pitch = "1.814";
                    planeLength = 8.611;
                    threadLength = 14;
                    break;
                case Npt1:
                    externalDiameter = "33.228";
                    internalDiameter = "29.694";
                    pitch = "2.209";
                    planeLength = 10.16;
                    threadLength = 17.5;
                    break;
                case Npt1N1d4:
                    externalDiameter = "41.985";
                    internalDiameter = "38.451";
                    pitch = "2.209";
                    planeLength = 10.668;
                    threadLength = 18;
                    break;
                case Npt1N1d2:
                    externalDiameter = "48.054";
                    internalDiameter = "44.52";
                    pitch = "2.209";
                    planeLength = 10.668;
                    threadLength = 18.5;
                    break;
                case Npt2:
                    externalDiameter = "60.092";
                    internalDiameter = "56.558";
                    pitch = "2.209";
                    planeLength = 11.074;
                    threadLength = 19;
                    break;
                default:
                    externalDiameter = string.Empty;
                    internalDiameter = string.Empty;
                    pitch = string.Empty;
                    planeLength = 0;
                    threadLength = 0;
                    break;
            }
        }

        public static void GetBsptValues(string template, out string externalDiameter, out string internalDiameter, out string pitch, out double planeLength, out double threadLength)
        {
            switch (template)
            {
                case Bspt1d16Ex or Bspt1d16In:
                    externalDiameter = "7.723";
                    internalDiameter = "6.561";
                    pitch = "0.907";
                    planeLength = 4;
                    threadLength = 6.5;
                    break;
                case Bspt1d8Ex or Bspt1d8In:
                    externalDiameter = "9.147";
                    internalDiameter = "8.566";
                    pitch = "0.907";
                    planeLength = 4;
                    threadLength = 6.5;
                    break;
                case Bspt1d4Ex or Bspt1d4In:
                    externalDiameter = "13.157";
                    internalDiameter = "11.445";
                    pitch = "1.337";
                    planeLength = 6;
                    threadLength = 9.7;
                    break;
                case Bspt3d8Ex or Bspt3d8In:
                    externalDiameter = "16.662";
                    internalDiameter = "14.95";
                    pitch = "1.337";
                    planeLength = 6.4;
                    threadLength = 10.1;
                    break;
                case Bspt1d2Ex or Bspt1d2In:
                    externalDiameter = "20.955";
                    internalDiameter = "18.631";
                    pitch = "1.814";
                    planeLength = 8.2;
                    threadLength = 13.2;
                    break;
                case Bspt3d4Ex or Bspt3d4In:
                    externalDiameter = "26.441";
                    internalDiameter = "24.117";
                    pitch = "1.814";
                    planeLength = 9.5;
                    threadLength = 14.5;
                    break;
                case Bspt1Ex or Bspt1In:
                    externalDiameter = "33.249";
                    internalDiameter = "30.291";
                    pitch = "2.309";
                    planeLength = 10.4;
                    threadLength = 16.8;
                    break;
                case Bspt1N1d4Ex or Bspt1N1d4In:
                    externalDiameter = "41.91";
                    internalDiameter = "38.952";
                    pitch = "2.309";
                    planeLength = 12.7;
                    threadLength = 19.1;
                    break;
                case Bspt1N1d2Ex or Bspt1N1d2In:
                    externalDiameter = "47.803";
                    internalDiameter = "44.845";
                    pitch = "2.309";
                    planeLength = 12.7;
                    threadLength = 19.1;
                    break;
                case Bspt2Ex or Bspt2In:
                    externalDiameter = "59.614";
                    internalDiameter = "56.565";
                    pitch = "2.309";
                    planeLength = 15.9;
                    threadLength = 23.4;
                    break;
                case Bspt2N1d2Ex or Bspt2N1d2In:
                    externalDiameter = "75.184";
                    internalDiameter = "72.226";
                    pitch = "2.309";
                    planeLength = 17.5;
                    threadLength = 26.7;
                    break;
                case Bspt3Ex or Bspt3In:
                    externalDiameter = "87.884";
                    internalDiameter = "84.926";
                    pitch = "2.309";
                    planeLength = 20.6;
                    threadLength = 29.8;
                    break;
                case Bspt3N1d2Ex or Bspt3N1d2In:
                    externalDiameter = "100.33";
                    internalDiameter = "97.372";
                    pitch = "2.309";
                    planeLength = 22.2;
                    threadLength = 31.4;
                    break;
                case Bspt4Ex or Bspt4In:
                    externalDiameter = "113.03";
                    internalDiameter = "110.072";
                    pitch = "2.309";
                    planeLength = 25.4;
                    threadLength = 35.8;
                    break;
                case Bspt5Ex or Bspt5In:
                    externalDiameter = "138.43";
                    internalDiameter = "135.472";
                    pitch = "2.309";
                    planeLength = 28.6;
                    threadLength = 40.1;
                    break;
                case Bspt6Ex or Bspt6In:
                    externalDiameter = "163.83";
                    internalDiameter = "160.872";
                    pitch = "2.309";
                    planeLength = 28.6;
                    threadLength = 40.1;
                    break;

                default:
                    externalDiameter = string.Empty;
                    internalDiameter = string.Empty;
                    pitch = string.Empty;
                    planeLength = 0;
                    threadLength = 0;
                    break;
            }
        }

        public static void GetUncValues(string template, out string diameter, out string pitch)
        {
            switch (template)
            {
                case Unc1:
                    diameter = "1.8542";
                    pitch = "0.3969";
                    break;
                case Unc2:
                    diameter = "2.1844";
                    pitch = "0.4536";
                    break;
                case Unc3:
                    diameter = "2.5146";
                    pitch = "0.5292";
                    break;
                case Unc4:
                    diameter = "2.8448";
                    pitch = "0.635";
                    break;
                case Unc5:
                    diameter = "3.175";
                    pitch = "0.635";
                    break;
                case Unc6:
                    diameter = "3.5052";
                    pitch = "0.7938";
                    break;
                case Unc8:
                    diameter = "4.1656";
                    pitch = "0.7938";
                    break;
                case Unc10:
                    diameter = "4.826";
                    pitch = "1.0583";
                    break;
                case Unc12:
                    diameter = "5.4864";
                    pitch = "1.0583";
                    break;
                case Unc1d4:
                    diameter = "6.35";
                    pitch = "1.27";
                    break;
                case Unc5d16:
                    diameter = "7.9375";
                    pitch = "1.4111";
                    break;
                case Unc3d8:
                    diameter = "9.525";
                    pitch = "1.5875";
                    break;
                case Unc7d16:
                    diameter = "11.1125";
                    pitch = "1.8143";
                    break;
                case Unc1d2:
                    diameter = "12.7";
                    pitch = "1.9538";
                    break;
                case Unc9d16:
                    diameter = "14.2875";
                    pitch = "2.1167";
                    break;
                case Unc5d8:
                    diameter = "15.875";
                    pitch = "2.3091";
                    break;
                case Unc3d4:
                    diameter = "19.05";
                    pitch = "2.54";
                    break;
                case Unc7d8:
                    diameter = "22.225";
                    pitch = "2.8222";
                    break;
                case Unc1N:
                    diameter = "25.4";
                    pitch = "3.175";
                    break;
                case Unc1N1d8:
                    diameter = "28.575";
                    pitch = "3.6286";
                    break;
                case Unc1N1d4:
                    diameter = "31.75";
                    pitch = "3.6286";
                    break;
                case Unc1N3d8:
                    diameter = "34.925";
                    pitch = "4.2333";
                    break;
                case Unc1N1d2:
                    diameter = "38.1";
                    pitch = "4.2333";
                    break;
                case Unc1N3d4:
                    diameter = "44.45";
                    pitch = "5.08";
                    break;
                case Unc2N:
                    diameter = "50.8";
                    pitch = "5.6444";
                    break;
                case Unc2N1d4:
                    diameter = "57.15";
                    pitch = "5.6444";
                    break;
                case Unc2N1d2:
                    diameter = "63.5";
                    pitch = "6.35";
                    break;
                case Unc2N3d4:
                    diameter = "69.85";
                    pitch = "6.35";
                    break;
                case Unc3N:
                    diameter = "76.2";
                    pitch = "6.35";
                    break;
                case Unc3N1d4:
                    diameter = "82.55";
                    pitch = "6.35";
                    break;
                case Unc3N1d2:
                    diameter = "88.9";
                    pitch = "6.35";
                    break;
                case Unc3N3d4:
                    diameter = "95.25";
                    pitch = "6.35";
                    break;
                case Unc4N:
                    diameter = "101.6";
                    pitch = "6.35";
                    break;

                default:
                    diameter = string.Empty;
                    pitch = string.Empty;
                    break;
            }
        }

        public static void GetUnfValues(string template, out string diameter, out string pitch)
        {
            switch (template)
            {
                case Unf0:
                    diameter = "1.524";
                    pitch = "0.3175";
                    break;
                case Unf1:
                    diameter = "1.8542";
                    pitch = "0.3528";
                    break;
                case Unf2:
                    diameter = "2.1844";
                    pitch = "0.3969";
                    break;
                case Unf3:
                    diameter = "2.5146";
                    pitch = "0.4536";
                    break;
                case Unf4:
                    diameter = "2.8448";
                    pitch = "0.5292";
                    break;
                case Unf5:
                    diameter = "3.175";
                    pitch = "0.5773";
                    break;
                case Unf6:
                    diameter = "3.5052";
                    pitch = "0.635";
                    break;
                case Unf8:
                    diameter = "4.1656";
                    pitch = "0.7056";
                    break;
                case Unf10:
                    diameter = "4.826";
                    pitch = "0.7938";
                    break;
                case Unf12:
                    diameter = "5.4864";
                    pitch = "0.9071";
                    break;
                case Unf1d4:
                    diameter = "6.35";
                    pitch = "0.9071";
                    break;
                case Unf5d16:
                    diameter = "7.9375";
                    pitch = "1.0583";
                    break;
                case Unf3d8:
                    diameter = "9.525";
                    pitch = "1.0583";
                    break;
                case Unf7d16:
                    diameter = "11.1125";
                    pitch = "1.27";
                    break;
                case Unf1d2:
                    diameter = "12.7";
                    pitch = "1.27";
                    break;
                case Unf9d16:
                    diameter = "14.2875";
                    pitch = "1.4111";
                    break;
                case Unf5d8:
                    diameter = "15.875";
                    pitch = "1.4111";
                    break;
                case Unf3d4:
                    diameter = "19.05";
                    pitch = "1.5875";
                    break;
                case Unf7d8:
                    diameter = "22.225";
                    pitch = "1.8143";
                    break;
                case Unf1N:
                    diameter = "25.4";
                    pitch = "2.1167";
                    break;
                case Unf1N1d8:
                    diameter = "28.575";
                    pitch = "2.1167";
                    break;
                case Unf1N1d4:
                    diameter = "31.75";
                    pitch = "2.1167";
                    break;
                case Unf1N3d8:
                    diameter = "34.925";
                    pitch = "2.1167";
                    break;
                case Unf1N1d2:
                    diameter = "38.1";
                    pitch = "2.1167";
                    break;

                default:
                    diameter = string.Empty;
                    pitch = string.Empty;
                    break;
            }
        }

        public static void GetUnefValues(string template, out string diameter, out string pitch)
        {
            switch (template)
            {
                case Unef12:
                    diameter = "5.4864";
                    pitch = "0.7938";
                    break;
                case Unef1d4:
                    diameter = "6.35";
                    pitch = "0.7938";
                    break;
                case Unef5d16:
                    diameter = "7.9375";
                    pitch = "0.7938";
                    break;
                case Unef3d8:
                    diameter = "9.525";
                    pitch = "0.7938";
                    break;
                case Unef7d16:
                    diameter = "11.1125";
                    pitch = "0.9071";
                    break;
                case Unef1d2:
                    diameter = "12.7";
                    pitch = "0.9071";
                    break;
                case Unef9d16:
                    diameter = "14.2875";
                    pitch = "1.0583";
                    break;
                case Unef5d8:
                    diameter = "15.875";
                    pitch = "1.0583";
                    break;
                case Unef3d4:
                    diameter = "19.05";
                    pitch = "1.27";
                    break;
                case Unef7d8:
                    diameter = "22.225";
                    pitch = "1.27";
                    break;
                case Unef1N:
                    diameter = "25.4";
                    pitch = "1.27";
                    break;

                default:
                    diameter = string.Empty;
                    pitch = string.Empty;
                    break;
            }
        }
        #endregion

        #region Расчеты параметров резьб
        /// <summary>
        /// Зазор на трапецеидальной резьбе в зависимости от шага
        /// </summary>
        /// <param name="threadPitch">Шаг</param>
        public static double TrapezoidalClearance(double threadPitch)
        {
            return threadPitch switch
            {
                <= 1.5 => 0.15,
                <= 5 => 0.25,
                <= 12 => 0.5,
                <= 40 => 1,
                _ => 0,
            };
        }

        /// <summary>
        /// Номинальная высота профиля
        /// </summary>
        /// <param name="threadStandard">Стандарт резьбы</param>
        /// <param name="threadPitch">Шаг резьбы</param>
        /// <returns></returns>
        public static double NominalHeight(ThreadStandard threadStandard, double threadPitch)
        {
            return threadStandard switch
            {
                ThreadStandard.Metric or ThreadStandard.UNC or ThreadStandard.UNF or ThreadStandard.UNEF => Math.Sqrt(3) / 2 * threadPitch,
                ThreadStandard.BSPP => 0.960491 * threadPitch,
                ThreadStandard.Trapezoidal => 1.866 * threadPitch,
                ThreadStandard.NPT => 0.866 * threadPitch,
                ThreadStandard.BSPT => 0.960237 * threadPitch,
                _ => 0,
            };
        }

        /// <summary>
        /// Рабочая высота профиля
        /// </summary>
        /// <param name="threadStandard">Стандарт резьбы</param>
        /// <param name="type">Тип резьбы</param>
        /// <param name="threadPitch">Шаг резьбы</param>
        /// <returns></returns>
        public static double ProfileHeight(ThreadStandard threadStandard, CuttingType type, double threadPitch)
        {
            return threadStandard switch
            {
                ThreadStandard.Metric or ThreadStandard.UNC or ThreadStandard.UNF or ThreadStandard.UNEF => type == CuttingType.External
                ? (17.0 / 24.0 * NominalHeight(threadStandard, threadPitch))
                : (5.0 / 8.0 * NominalHeight(threadStandard, threadPitch)),
                ThreadStandard.BSPP => 0.640327 * threadPitch,
                ThreadStandard.Trapezoidal => 0.5 * threadPitch + TrapezoidalClearance(threadPitch),
                ThreadStandard.NPT => 0.8 * threadPitch,
                ThreadStandard.BSPT => 0.640327 * threadPitch,
                _ => 0,
            };
        }



        /// <summary>
        /// Угол подъема резьбы
        /// </summary>
        /// <param name="threadDiameter">Номинальный диаметр резьбы</param>
        /// <param name="threadPitch">Шаг резьбы</param>
        /// <returns></returns>
        public static double Angle(double threadDiameter, double threadPitch)
        {
            double result = Math.Atan(threadPitch / (threadDiameter * Math.PI));
            return result.Degrees();
        }

        #endregion


        /// <summary>
        /// Смещение диаметра для начальной точки NPT резьбы
        /// </summary>
        /// <param name="endPoint">Конечная точка по Z (уже со смещением на шаг)</param>
        /// <param name="plane">Расстояние от торца до основной плоскости</param>
        /// <returns></returns>
        public static double ExtNptThreadShift(double endPoint, double plane)
        {
            double result = (Math.Abs(endPoint) - Math.Abs(plane)) * Math.Tan(1.79.Radians());
            return result;
        }

        public static double IntNptThreadShift(double endPoint, double startPoint)
        {
            double result = (Math.Abs(endPoint) + Math.Abs(startPoint)) * Math.Tan(1.79.Radians());
            return result;

        }


        /// <summary>
        /// Число проходов в зависимости от шага
        /// </summary>
        /// <param name="threadStandard">Стандарт резьбы</param>
        /// <param name="threadPitch">Шаг резьбы</param>
        /// <returns></returns>
        public static int PassesCount(ThreadStandard threadStandard, double threadPitch)
        {
            if (threadStandard is ThreadStandard.Metric or ThreadStandard.UNC or ThreadStandard.UNF or ThreadStandard.UNEF)
            {
                return threadPitch switch
                {
                    <= 0.75 => 4,
                    <= 1 => 5,
                    <= 1.5 => 6,
                    <= 2 => 8,
                    <= 2.5 => 10,
                    <= 3.5 => 12,
                    <= 5.5 => 14,
                    > 5.5 => 16,
                    _ => 0,
                };
            }
            else if (threadStandard is ThreadStandard.BSPP)
            {
                return Math.Round(threadPitch.ThreadConvert(), 1) switch
                {
                    <= 8 => 12,
                    <= 11 => 9,
                    <= 14 => 8,
                    <= 19 => 6,
                    > 19 => 5,
                    _ => 0,
                };
            }
            else if (threadStandard is ThreadStandard.Trapezoidal)
            {
                return threadPitch switch
                {
                    <= 1.5 => 6,
                    <= 2 => 8,
                    <= 3 => 12,
                    <= 4 => 13,
                    <= 5 => 14,
                    <= 7 => 16,
                    > 7 => 19,
                    _ => 0,
                };
            }
            else if (threadStandard is ThreadStandard.NPT or ThreadStandard.BSPT)
            {
                return Math.Round(threadPitch.ThreadConvert(), 1) switch
                {
                    <= 8 => 15,
                    <= 11.5 => 12,
                    <= 14 => 10,
                    <= 18 => 8,
                    > 18 => 6,
                    _ => 0,
                };
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Размеры фасок по ГОСТ 10549-80
        /// </summary>
        /// <param name="threadStandard"></param>
        /// <param name="threadPitch"></param>
        /// <param name="threadType"></param>
        /// <returns></returns>
        public static double ThreadChamfer(ThreadStandard threadStandard, double threadPitch, CuttingType threadType)
        {
            switch (threadStandard)
            {
                case ThreadStandard.Metric:
                    return threadType switch
                    {
                        CuttingType.External => threadPitch switch
                        {
                            <= 0.3 => 0.2,
                            <= 0.45 => 0.3,
                            <= 0.7 => 0.5,
                            <= 1 => 1,
                            <= 1.75 => 1.6,
                            <= 2 => 2,
                            <= 3 => 2.5,
                            <= 4 => 3,
                            > 4 => 4,
                            _ => 0
                        },
                        CuttingType.Internal => threadPitch switch
                        {
                            <= 0.35 => 0.2,
                            <= 0.45 => 0.3,
                            <= 0.7 => 0.5,
                            <= 1 => 1,
                            <= 1.75 => 1.6,
                            <= 2 => 2,
                            <= 3 => 2.5,
                            <= 4 => 3,
                            > 4 => 4,
                            _ => 0
                        },
                        _ => 0
                    };
                case ThreadStandard.BSPP or ThreadStandard.UNC or ThreadStandard.UNF or ThreadStandard.UNEF:
                    return threadType switch
                    {
                        CuttingType.External => threadPitch switch
                        {
                            <= 0.907 => // 28
                                1,
                            <= 1.337 => // 19
                                1.6,
                            <= 1.814 => // 14
                                2,
                            > 1.814 => //>14
                                2.5,
                            _ => 0
                        },
                        CuttingType.Internal => threadPitch switch
                        {
                            <= 1.814 => 1,
                            > 1.814 => 1.6,
                            _ => 0
                        },
                        _ => 0
                    };
                case ThreadStandard.NPT or ThreadStandard.BSPT:
                    return threadPitch switch
                    {
                        <= 0.941 => // 27
                            1,
                        <= 1.814 => // 14
                            1.6,
                        > 1.814 => // >14
                            2,
                        _ => 0
                    };
                case ThreadStandard.Trapezoidal:
                    return threadPitch switch
                    {
                        <= 1.5 => 1,
                        <= 2 => 1.6,
                        <= 3 => 2,
                        <= 4 => 2.5,
                        <= 5 => 3,
                        <= 6 => 3.5,
                        <= 7 => 4,
                        <= 8 => 4.5,
                        <= 9 => 5,
                        <= 10 => 5.5,
                        <= 12 => 6.5,
                        <= 14 => 8,
                        <= 16 => 9,
                        <= 18 => 10,
                        <= 20 => 11,
                        <= 22 => 12,
                        <= 24 => 13,
                        <= 28 => 16,
                        <= 32 => 17,
                        <= 36 => 20,
                        <= 40 => 21,
                        > 40 => 25,
                        _ => 0
                    };
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Размеры сбегов по ГОСТ 10549-80
        /// </summary>
        /// <param name="threadStandard">Стандарт резьба</param>
        /// <param name="threadPitch">Шаг резьбы</param>
        /// <param name="threadType">Тип резьбы</param>
        /// <returns></returns>
        public static double ThreadRunout(ThreadStandard threadStandard, double threadPitch, CuttingType threadType)
        {
            switch (threadStandard)
            {
                case ThreadStandard.Metric:
                    return Math.Round(1.25 * threadPitch, 2);
                case ThreadStandard.BSPP or ThreadStandard.UNC or ThreadStandard.UNF or ThreadStandard.UNEF:
                    return threadType switch
                    {
                        CuttingType.External => threadPitch switch
                        {
                            <= 0.907 => // 28
                                1,
                            <= 1.337 => // 19
                                1.5,
                            <= 1.814 => // 14
                                2,
                            > 1.814 =>  // >14
                                2.5,
                            _ => 0
                        },
                        CuttingType.Internal => threadPitch switch
                        {
                            <= 0.907 => // 28
                                1.4,
                            <= 1.337 => // 19
                                2,
                            <= 1.814 => // 14
                                3,
                            > 1.814 =>  // >14
                                4,
                            _ => 0
                        },
                        _ => 0
                    };
                case ThreadStandard.NPT:
                    return threadType switch
                    {
                        CuttingType.External => threadPitch switch
                        {
                            <= 0.941 => // 27
                                2.5,
                            <= 1.411 => // 18
                                3.5,
                            <= 1.814 => // 14
                                4.5,
                            > 1.814 =>  // >14
                                5.5,
                            _ => 0
                        },
                        CuttingType.Internal => threadPitch switch
                        {
                            <= 0.941 => // 27
                                3,
                            <= 1.411 => // 18
                                4,
                            <= 1.814 => // 14
                                4.5,
                            > 1.814 =>  // >14
                                6.5,
                            _ => 0
                        },
                        _ => 0,
                    };
                case ThreadStandard.BSPT:
                    return threadType switch
                    {
                        CuttingType.External => threadPitch switch
                        {
                            <= 0.907 => // 28
                                2,
                            <= 1.337 => // 18
                                3,
                            <= 1.814 => // 14
                                3.5,
                            > 1.814 =>  // >14
                                4.5,
                            _ => 0
                        },
                        CuttingType.Internal => threadPitch switch
                        {
                            <= 0.907 => // 27
                                3,
                            <= 1.337 => // 18
                                4,
                            <= 1.814 => // 14
                                5.5,
                            > 1.814 =>  // >14
                                7,
                            _ => 0
                        },
                        _ => 0,
                    };
                case ThreadStandard.Trapezoidal:
                    return 0;
                default:
                    return 0;
            }
        }

        public static string ToTpi(this double thredPitch)
        {
            var tpi = 25.4 / thredPitch;
            return tpi.ToPrettyString(1).Replace(".0", "");
        }

        /// <summary>
        /// Считает проходы при нарезании резьбы
        /// </summary>
        /// <param name="threadDepth">Высота профиля резьбы</param>
        /// <param name="passesCount">Количество проходов</param>
        /// <param name="passesOption">Возврат абсолютных или инкрементных значений.</param>
        /// <returns>Массив с глубиной каждого прохода</returns>
        public static double[] CalcPasses(double threadDepth, int passesCount, PassesOption passesOption = PassesOption.FullPasses)
        {
            double[] passes = new double[passesCount];
            for (int pass = 1; pass <= passesCount; pass++)
            {
                passes[pass - 1] = Math.Round(threadDepth / Math.Sqrt(passesCount - 1) * Math.Sqrt(pass > 1 ? pass - 1 : 0.3), 2);
            }
            if (passesOption == PassesOption.FullPasses)
            {
                return passes;
            }
            else
            {
                double[] infeed = new double[passesCount];
                for (int i = 0; i < passes.Length; i++)
                {

                    if (i > 0)
                    {
                        infeed[i] = passes[i] - passes[i - 1];
                    }
                    else
                    {
                        infeed[i] = passes[i];
                    }
                }
                return infeed;
            }

        }

        public static double[] Passes(ThreadStandard threadStandard, CuttingType type, double threadPitch)
            => CalcPasses(ProfileHeight(threadStandard, type, threadPitch), PassesCount(threadStandard, threadPitch), PassesOption.Infeed);

        public static double[] TotalPasses(ThreadStandard threadStandard, CuttingType type, double threadPitch)
            => CalcPasses(ProfileHeight(threadStandard, type, threadPitch), PassesCount(threadStandard, threadPitch));

        /// <summary>
        /// Профиль резьбы для записи в УП
        /// </summary>
        /// <param name="threadStandard">Стандарт резьбы</param>
        /// <returns></returns>
        public static string Profile(this ThreadStandard threadStandard)
        {
            return threadStandard switch
            {
                ThreadStandard.Metric or ThreadStandard.UNC or ThreadStandard.UNF or ThreadStandard.UNEF => "60",
                ThreadStandard.BSPP or ThreadStandard.BSPT => "55",
                ThreadStandard.Trapezoidal => "30",
                ThreadStandard.NPT => "60",
                _ => string.Empty,
            };
        }

        /// <summary>
        /// Высота профиля полученная суммой рассчитанных проходов
        /// </summary>
        /// <param name="threadStandard">Стандарт резьбы</param>
        /// <param name="type">Тип резьбы</param>
        /// <param name="threadPitch">Шаг резьбы</param>
        /// <returns></returns>
        private static double CalculatedProfile(ThreadStandard threadStandard, CuttingType type, double threadPitch)
        {
            return 2 * Passes(threadStandard, type, threadPitch).Sum();
        }

        /// <summary>
        /// Диаметр подвода 
        /// </summary>
        /// <param name="threadStandard">Стандарт резьбы</param>
        /// <param name="type">Тип резьбы</param>
        /// <param name="threadDiameter">Номинальный диаметр резьбы (для NPT наружный диаметр в основной плоскости)</param>
        /// <param name="threadPitch">Шаг резьбы</param>
        /// <param name="startPointZ">Начальная точка</param>
        /// <param name="planeLength">Длина до основной плоскости (только для NPT)</param>
        /// <param name="threadLength">Длина резьбы</param>
        /// <returns></returns>
        public static double ApproachDiameter(ThreadStandard threadStandard, CuttingType type, double threadDiameter, double threadPitch, double threadLength, double startPointZ, double planeLength = 0)
        {
            return threadStandard switch
            {
                ThreadStandard.Metric or ThreadStandard.UNC or ThreadStandard.UNF or ThreadStandard.UNEF => type == CuttingType.External
                ? threadDiameter + 1
                : threadDiameter - threadPitch - 1,
                ThreadStandard.BSPP => type == CuttingType.External
                ? threadDiameter + 1
                : threadDiameter - threadPitch - 1,
                ThreadStandard.Trapezoidal => type == CuttingType.External
                ? threadDiameter + 1
                : threadDiameter - threadPitch - 1,
                ThreadStandard.NPT or ThreadStandard.BSPT => type == CuttingType.External
                ? threadDiameter + 2 * ExtNptThreadShift(threadLength, planeLength) + 1
                : threadDiameter - CalculatedProfile(threadStandard, type, threadPitch) - 2 * IntNptThreadShift(threadLength, 0) - 1,
                _ => double.NaN,
            };
        }

        /// <summary>
        /// Конечный диаметр резьбы (вторая строка G76)
        /// </summary>
        /// <param name="threadStandard">Стандарт резьбы</param>
        /// <param name="type">Тип резьбы</param>
        /// <param name="threadDiameter">Номинальный диаметр резьбы (для NPT наружный диаметр в основной плоскости)</param>
        /// <param name="threadPitch">Шаг резьбы</param>
        /// <param name="startPointZ">Начальная точка</param>
        /// <param name="planeLength">Длина до основной плоскости (только для NPT)</param>
        /// <param name="threadLength">Длина резьбы</param>
        /// <returns></returns>
        public static double EndDiameter(ThreadStandard threadStandard, CuttingType type, double threadDiameter, double threadPitch, double threadLength, double startPointZ, double planeLength = 0)
        {
            return threadStandard switch
            {
                ThreadStandard.Metric or ThreadStandard.UNC or ThreadStandard.UNF or ThreadStandard.UNEF => type == CuttingType.External
                ? threadDiameter - CalculatedProfile(threadStandard, type, threadPitch)
                : threadDiameter + threadPitch / 16 / Math.Sin(60.Radians()),
                ThreadStandard.BSPP => type == CuttingType.External
                ? threadDiameter - CalculatedProfile(threadStandard, type, threadPitch)
                : threadDiameter,
                ThreadStandard.Trapezoidal => type == CuttingType.External
                ? threadDiameter - CalculatedProfile(threadStandard, type, threadPitch)
                : threadDiameter + 2 * TrapezoidalClearance(threadPitch),
                ThreadStandard.NPT or ThreadStandard.BSPT => type == CuttingType.External
                ? threadDiameter - CalculatedProfile(threadStandard, type, threadPitch) + 2 * ExtNptThreadShift(threadLength, planeLength)
                : threadDiameter - 2 * IntNptThreadShift(threadLength, 0),
                _ => double.NaN,
            };
        }


        public static bool Valid(double threadDiamer, double threadPitch)
        {
            if (threadDiamer > 0 && threadPitch > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #region ГОСТ 19257-73 и ГОСТ 19258-73 — Расчёт по полю допуска

        // ─────────────────────────────────────────────────────────────────────
        //  Величина подъёма витка A (мм) используется при расчёте диаметров
        //  отверстий (ГОСТ 19257-73) и стержней (ГОСТ 19258-73) для материалов
        //  повышенной вязкости.
        //
        //  Методы возвращают диапазон (Min, Max) по всем материалам для
        //  заданного шага — без привязки к конкретному материалу:
        //
        //    Min — наименьший подъём витка (наиболее благоприятный материал),
        //          подставляется при расчёте наибольшего диаметра (формула 2/3).
        //    Max — наибольший подъём витка (наиболее трудный материал),
        //          подставляется при расчёте наименьшего диаметра (формула 1).
        //
        //  Пример (ГОСТ 19257, M10×1.5, поле 6H):
        //    var (Amin, Amax) = Thread.ThreadRise19257(1.5);
        //    // Amin = 0.110  (Латунь)     → d0_наиб = D1 + (EI + TD1) + Amin
        //    // Amax = 0.255  (Коррозионностойкие) → d0_ном = D1 + EI + Amax
        //
        //  Источники коэффициентов:
        //    ГОСТ 19257-73, Приложение 2, Таблица 1 — 6 материалов, P 0.2…5.0 мм.
        //    ГОСТ 19258-73, Приложение,   Таблица 1 — 4 материала,  P 0.2…2.0 мм.
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Диапазон величины подъёма витка <b>A</b> для расчёта диаметров
        /// <b>отверстий</b> под резьбу для материалов повышенной вязкости
        /// (ГОСТ 19257-73, Приложение 2).
        /// </summary>
        /// <remarks>
        /// <para>A = C · P, где C — коэффициент материала (таблица ГОСТ 19257-73):</para>
        /// <list type="table">
        ///   <listheader><term>Материал</term><term>C</term></listheader>
        ///   <item><term>Алюминиевые сплавы</term>    <term>0.080</term></item>
        ///   <item><term>Магниевые сплавы</term>       <term>0.115</term></item>
        ///   <item><term>Латунь</term>                 <term>0.073</term></item>
        ///   <item><term>Титановые сплавы</term>       <term>0.130</term></item>
        ///   <item><term>Жаропрочные стали и сплавы</term><term>0.150</term></item>
        ///   <item><term>Коррозионностойкие жаростойкие стали на никелевой основе</term><term>0.170</term></item>
        /// </list>
        /// <para>
        /// Min = 0.073·P (Латунь) — наименьший подъём витка в группе материалов.<br/>
        /// Max = 0.170·P (Коррозионностойкие) — наибольший подъём витка.
        /// </para>
        /// </remarks>
        /// <param name="pitch">Шаг резьбы P, мм.</param>
        /// <returns>
        /// Кортеж <c>(Min, Max)</c> — наименьшее и наибольшее значения A в мм
        /// по всем материалам ГОСТ 19257-73 для данного шага.
        /// </returns>
        public static (double Min, double Max) ThreadRise19257(double pitch) =>
        (
            Math.Round(0.073 * pitch, 3, MidpointRounding.AwayFromZero),   // Латунь (C_min = 0.073)
            Math.Round(0.170 * pitch, 3, MidpointRounding.AwayFromZero)    // Коррозионностойкие (C_max = 0.170)
        );

        /// <summary>
        /// Диапазон величины подъёма витка <b>A</b> для расчёта диаметров
        /// <b>стержней</b> под резьбу для материалов повышенной вязкости
        /// (ГОСТ 19258-73, Приложение).
        /// </summary>
        /// <remarks>
        /// <para>
        /// Таблица включает 4 материала (P = 0.2 … 2.0 мм):
        /// </para>
        /// <list type="table">
        ///   <listheader><term>Материал</term><term>C (A = C·P)</term></listheader>
        ///   <item><term>Латунь</term>                  <term>—  (нелинейная зависимость)</term></item>
        ///   <item><term>Титановые сплавы</term>         <term>0.110</term></item>
        ///   <item><term>Жаропрочные стали и сплавы</term><term>0.120</term></item>
        ///   <item><term>Коррозионностойкие жаростойкие стали на никелевой основе</term><term>0.140</term></item>
        /// </list>
        /// <para>
        /// Минимальный A определяется:<br/>
        /// • при P ≤ 1.25 мм — Титановые сплавы (A = 0.110·P);<br/>
        /// • при P ≥ 1.50 мм — Латунь (значения из таблицы ГОСТ 19258-73).<br/>
        /// Максимальный A — всегда Коррозионностойкие (A = 0.140·P).
        /// </para>
        /// <para>
        /// Для P &gt; 2.0 мм следует использовать <see cref="ThreadRise19257"/>.
        /// </para>
        /// </remarks>
        /// <param name="pitch">Шаг резьбы P, мм.</param>
        /// <returns>
        /// Кортеж <c>(Min, Max)</c> — наименьшее и наибольшее значения A в мм
        /// по всем материалам ГОСТ 19258-73 для данного шага.
        /// </returns>
        public static (double Min, double Max) ThreadRise19258(double pitch) =>
        (
            ThreadRise19258_Min(pitch),
            Math.Round(0.140 * pitch, 3, MidpointRounding.AwayFromZero)    // Коррозионностойкие (C_max = 0.140)
        );

        /// <summary>
        /// Наименьшее значение подъёма витка по ГОСТ 19258-73:
        /// Титановые сплавы (P ≤ 1.25) или Латунь (P ≥ 1.50).
        /// </summary>
        private static double ThreadRise19258_Min(double pitch) => pitch switch
        {
            // Титановые сплавы: A = 0.110 · P  (C = 0.110, погрешность ≤ 0.001 мм)
            <= 1.25 => Math.Round(0.110 * pitch, 3, MidpointRounding.AwayFromZero),

            // Латунь: табличные значения ГОСТ 19258-73 (нелинейная зависимость)
            <= 1.50 => 0.160,
            <= 1.75 => 0.180,
            _ => 0.200,   // P = 2.0 мм (граница таблицы ГОСТ 19258)
        };

        // ─────────────────────────────────────────────────────────────────────
        //  Коэффициенты C отдельных материалов (для справки и при необходимости
        //  расчёта A для конкретного материала, а не диапазона).
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Величина подъёма витка A для конкретного материала по ГОСТ 19257-73.
        /// Формула: A = C · P, округление до 0.001 мм.
        /// </summary>
        /// <param name="pitch">Шаг резьбы P, мм.</param>
        /// <param name="materialCoefficient">
        /// Коэффициент C материала из таблицы ГОСТ 19257-73:
        /// 0.073 — Латунь, 0.080 — Алюминиевые сплавы, 0.115 — Магниевые сплавы,
        /// 0.130 — Титановые сплавы, 0.150 — Жаропрочные стали и сплавы,
        /// 0.170 — Коррозионностойкие жаростойкие стали на никелевой основе.
        /// </param>
        public static double ThreadRise19257(double pitch, double materialCoefficient) =>
            Math.Round(materialCoefficient * pitch, 3, MidpointRounding.AwayFromZero);

        // ─────────────────────────────────────────────────────────────────────
        //  Константы коэффициентов C для удобного обращения по имени материала
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Коэффициент C для алюминиевых сплавов (ГОСТ 19257-73).</summary>
        public const double ThreadRiseC_AluminumAlloys = 0.080;
        /// <summary>Коэффициент C для магниевых сплавов (ГОСТ 19257-73).</summary>
        public const double ThreadRiseC_MagnesiumAlloys = 0.115;
        /// <summary>Коэффициент C для латуни (ГОСТ 19257-73).</summary>
        public const double ThreadRiseC_Brass = 0.073;
        /// <summary>Коэффициент C для титановых сплавов (ГОСТ 19257-73).</summary>
        public const double ThreadRiseC_TitaniumAlloys = 0.130;
        /// <summary>Коэффициент C для жаропрочных сталей и сплавов (ГОСТ 19257-73).</summary>
        public const double ThreadRiseC_HeatResistant = 0.150;
        /// <summary>Коэффициент C для коррозионностойких жаростойких сталей на никелевой основе (ГОСТ 19257-73).</summary>
        public const double ThreadRiseC_CorrosionResistant = 0.170;

        // ─────────────────────────────────────────────────────────────────────
        //  Поля допуска метрической резьбы по ГОСТ 16093-81.
        //
        //  Наружная резьба (стержень): d, e, f, g, h
        //  Внутренняя резьба (отверстие): E, F, G, H
        // ─────────────────────────────────────────────────────────────────────
        public enum ThreadPosition
        {
            // Наружная резьба
            d,
            e,
            f,
            g,
            h,  // нулевое отклонение (es = 0)

            // Внутренняя резьба
            E,
            F,
            G,
            H,  // нулевое отклонение (EI = 0)
        }

        /// <summary>
        /// Предельные диаметры отверстия под метрическую резьбу (ГОСТ 19257-73).
        /// </summary>
        /// <param name="Position">Поле допуска (H, G, F, E).</param>
        /// <param name="Grade">Степень точности.</param>
        /// <param name="D1min">Наименьший диаметр отверстия D1 = D1_ном + EI, мм.</param>
        /// <param name="D1max">Наибольший диаметр отверстия D1 = D1_ном + EI + TD1, мм.</param>
        /// <param name="EI">Нижнее отклонение (основное отклонение поля), мм.</param>
        /// <param name="TD1">Допуск D1 по ГОСТ 16093-81, мм. Null если не определён.</param>
        /// <param name="TD2">Допуск D2 по ГОСТ 16093-81, мм. Null если не определён.</param>
        public readonly record struct Gost19257Result(
            ThreadPosition Position,
            int Grade,
            double D1min,
            double D1max,
            double EI,
            double? TD1,
            double? TD2,
            double Tolerance
        )
        {
            public override string ToString()
            {
                string field = $"{Grade}{Position}";
                string d1Range = double.IsNaN(D1max)
                    ? $"{D1min:F3} +???"
                    : $"{D1min:F3} … {D1max:F3}";
                string eiStr = EI >= 0 ? $"+{EI:F3}" : $"{EI:F3}";
                string td1Str = TD1.HasValue ? $"{TD1.Value:F3}" : "—";
                string td2Str = TD2.HasValue ? $"{TD2.Value:F3}" : "—";

                return $"{field}  D1: {d1Range}  EI={eiStr}  TD1={td1Str}  TD2={td2Str}";
            }
        }

        /// <summary>
        /// Предельные диаметры стержня под метрическую резьбу (ГОСТ 19258-73).
        /// </summary>
        /// <param name="Position">Поле допуска (h, g, f, e, d).</param>
        /// <param name="Grade">Степень точности.</param>
        /// <param name="dmax">Наибольший диаметр стержня d = d_ном + es, мм.</param>
        /// <param name="dmin">Наименьший диаметр стержня d = d_ном + es − Td, мм.</param>
        /// <param name="es">Верхнее отклонение (основное отклонение поля), мм.</param>
        /// <param name="Td">Допуск d по ГОСТ 16093-81, мм. Null если не определён.</param>
        /// <param name="Td2">Допуск d2 по ГОСТ 16093-81, мм. Null если не определён.</param>
        public readonly record struct Gost19258Result(
            ThreadPosition Position,
            int Grade,
            double dmax,
            double dmin,
            double es,
            double? Td,
            double? Td2,
            double Tolerance
        )
        {
            public override string ToString()
            {
                string field = $"{Grade}{Position}";
                string dRange = double.IsNaN(dmin)
                    ? $"??? … {dmax:F3}"
                    : $"{dmin:F3} … {dmax:F3}";
                string esStr = es <= 0 ? $"{es:F3}" : $"+{es:F3}";
                string tdStr = Td.HasValue ? $"{Td.Value:F3}" : "—";
                string td2Str = Td2.HasValue ? $"{Td2.Value:F3}" : "—";

                return $"{field}  d: {dRange}  es={esStr}  Td={tdStr}  Td2={td2Str}";
            }
        }




        /// <summary>
        /// Рассчитывает предельные диаметры отверстия по ГОСТ 19257-73.
        /// </summary>
        /// <param name="diameter">Номинальный диаметр резьбы d, мм.</param>
        /// <param name="pitch">Шаг резьбы P, мм.</param>
        /// <param name="position">Поле допуска внутренней резьбы (H, G, F, E).</param>
        /// <param name="grade">Степень точности (4–8 для D1; 4–9 для D2).</param>
        /// <exception cref="ArgumentException">
        /// Если передано поле наружной резьбы (d, e, f, g, h).
        /// </exception>
        public static Gost19257Result GetGost19257(
            double diameter, double pitch, ThreadPosition position, int grade)
        {
            if (position is ThreadPosition.d or ThreadPosition.e or ThreadPosition.f
                         or ThreadPosition.g or ThreadPosition.h)
                throw new ArgumentException(
                    $"Поле {position} предназначено для наружной резьбы. " +
                    "Используйте H, G, F или E для отверстия.", nameof(position));
            var (a_min, a_max) = ThreadRise19257(pitch);
            //a_min = 0.05 * pitch;
            //a_max = 0.05 * pitch;
            double ei = InternalFundamentalDeviation(position, pitch);
            double? tD1 = TD1(pitch, grade);
            double? tD2 = TD2(diameter, pitch, grade);
            double d1 = diameter - 1.0825 * pitch;
            double d1Min = Math.Round(d1 + ei + a_max, 2);
            double d1Max = tD1.HasValue ? Math.Round(d1 + (ei + tD1.Value) + a_min, 2) : double.NaN;

            return new Gost19257Result(
                Position: position,
                Grade: grade,
                D1min: d1Min,
                D1max: d1Max,
                EI: ei,
                TD1: tD1,
                TD2: tD2,
                Tolerance: Math.Round(d1Max - d1Min, 2)
            );
        }

        /// <summary>
        /// Рассчитывает предельные диаметры стержня по ГОСТ 19258-73.
        /// </summary>
        /// <param name="diameter">Номинальный диаметр резьбы d, мм.</param>
        /// <param name="pitch">Шаг резьбы P, мм.</param>
        /// <param name="position">Поле допуска наружной резьбы (h, g, f, e, d).</param>
        /// <param name="grade">Степень точности (4, 6, 8 для d; 3–10 для d2).</param>
        /// <exception cref="ArgumentException">
        /// Если передано поле внутренней резьбы (H, G, F, E).
        /// </exception>
        public static Gost19258Result GetGost19258(
            double diameter, double pitch, ThreadPosition position, int grade)
        {
            if (position is ThreadPosition.H or ThreadPosition.G or ThreadPosition.F
                         or ThreadPosition.E)
                throw new ArgumentException(
                    $"Поле {position} предназначено для внутренней резьбы. " +
                    "Используйте h, g, f, e или d для стержня.", nameof(position));

            var (a_min, a_max) = ThreadRise19258(pitch);
            double es = ExternalFundamentalDeviation(position, pitch);
            double? tD = Td(pitch, grade);
            double? tD2 = Td2(diameter, pitch, grade);
            double dMax = Math.Round(diameter - Math.Abs(es) - a_max, 2);
            double dMin = tD.HasValue ? Math.Round(diameter - (Math.Abs(es) + tD.Value) - a_min, 2) : double.NaN;

            return new Gost19258Result(
                Position: position,
                Grade: grade,
                dmax: dMax,
                dmin: dMin,
                es: es,
                Td: tD,
                Td2: tD2,
                Tolerance: Math.Round(dMin - dMax, 2)
            );
        }


        /// <summary>
        /// Основное отклонение EI для поля внутренней резьбы (ГОСТ 16093-81), мм.
        /// </summary>
        private static double InternalFundamentalDeviation(ThreadPosition pos, double pitch) => pos switch
        {
            ThreadPosition.H => EI_H(),
            ThreadPosition.G => EI_G(pitch),
            ThreadPosition.F => EI_F(pitch),
            ThreadPosition.E => EI_E(pitch),
            _ => throw new ArgumentOutOfRangeException(nameof(pos), pos, null),
        };

        /// <summary>
        /// Основное отклонение es для поля наружной резьбы (ГОСТ 16093-81), мм.
        /// </summary>
        private static double ExternalFundamentalDeviation(ThreadPosition pos, double pitch) => pos switch
        {
            ThreadPosition.h => Es_h(),
            ThreadPosition.g => Es_g(pitch),
            ThreadPosition.f => Es_f(pitch),
            ThreadPosition.e => Es_e(pitch),
            ThreadPosition.d => Es_d(pitch),
            _ => throw new ArgumentOutOfRangeException(nameof(pos), pos, null),
        };

        #endregion

        #region ГОСТ 16093-81 — Основные отклонения метрической резьбы

        //  Таблица основных отклонений по ГОСТ 16093-81.
        //  Значения переведены из мкм в мм; все верифицированы по таблице ГОСТ.
        //
        //  Наружная резьба (диаметры d и d2):
        //    es — верхнее отклонение (≤ 0 для всех полей кроме h).
        //    Поля по убыванию посадки: d < e < f < g < h (= 0).
        //
        //  Внутренняя резьба (диаметры D1 и D2):
        //    EI — нижнее отклонение (≥ 0 для всех полей кроме H).
        //    Поля по убыванию посадки: H (= 0) < G < F < E.
        //    Симметрия: EI_G = |es_g|, EI_F = |es_f|, EI_E = |es_e|.
        //
        //  При нестандартных шагах switch-диапазон возвращает значение
        //  для ближайшего меньшего табличного шага.

        /// <summary>
        /// Основное отклонение <b>es</b> поля <b>h</b> — всегда 0 мм.
        /// </summary>
        public static double Es_h() => 0.0;

        /// <summary>
        /// Основное отклонение <b>es</b> поля <b>g</b>, мм.
        /// Верхнее отклонение диаметров d, d2 наружной резьбы (значение ≤ 0).
        /// Определено для P = 0.2 … 6.0 мм.
        /// </summary>
        public static double Es_g(double pitch) => pitch switch
        {
            <= 0.20 => -0.017,
            <= 0.30 => -0.018,
            <= 0.40 => -0.019,
            <= 0.50 => -0.020,
            <= 0.60 => -0.021,
            <= 0.75 => -0.022,
            <= 0.80 => -0.024,
            <= 1.00 => -0.026,
            <= 1.25 => -0.028,
            <= 1.50 => -0.032,
            <= 1.75 => -0.034,
            <= 2.00 => -0.038,
            <= 2.50 => -0.042,
            <= 3.00 => -0.048,
            <= 3.50 => -0.053,
            <= 4.00 => -0.060,
            <= 4.50 => -0.063,
            <= 5.00 => -0.071,
            <= 5.50 => -0.075,
            _ => -0.080,   // P = 6.0
        };

        /// <summary>
        /// Основное отклонение <b>es</b> поля <b>f</b>, мм.
        /// Верхнее отклонение диаметров d, d2 (значение ≤ 0).
        /// Определено для P = 0.2 … 3.0 мм; при P &gt; 3.0 поле f не предусмотрено стандартом.
        /// </summary>
        public static double Es_f(double pitch) => pitch switch
        {
            <= 0.20 => -0.032,
            <= 0.30 => -0.033,
            <= 0.40 => -0.034,
            <= 0.45 => -0.035,
            <= 0.60 => -0.036,
            <= 0.80 => -0.038,
            <= 1.00 => -0.040,
            <= 1.25 => -0.042,
            <= 1.50 => -0.045,
            <= 1.75 => -0.048,
            <= 2.00 => -0.052,
            <= 2.50 => -0.058,
            _ => -0.063,
        };

        /// <summary>
        /// Основное отклонение <b>es</b> поля <b>e</b>, мм.
        /// Верхнее отклонение диаметров d, d2 (значение ≤ 0).
        /// Определено начиная с P = 0.5 мм.
        /// </summary>
        public static double Es_e(double pitch) => pitch switch
        {
            <= 0.50 => -0.050,
            <= 0.60 => -0.053,
            <= 0.75 => -0.056,
            <= 1.00 => -0.060,
            <= 1.25 => -0.063,
            <= 1.50 => -0.067,
            <= 2.00 => -0.071,
            <= 2.50 => -0.080,
            <= 3.00 => -0.085,
            <= 3.50 => -0.090,
            <= 4.00 => -0.095,
            <= 4.50 => -0.100,
            <= 5.00 => -0.106,
            <= 5.50 => -0.112,
            _ => -0.118,   // P = 6.0
        };

        /// <summary>
        /// Основное отклонение <b>es</b> поля <b>d</b>, мм.
        /// Верхнее отклонение диаметров d, d2 (значение ≤ 0).
        /// Определено начиная с P = 1.0 мм.
        /// </summary>
        public static double Es_d(double pitch) => pitch switch
        {
            <= 1.00 => -0.090,
            <= 1.50 => -0.095,   // P = 1.25 и P = 1.5
            <= 2.00 => -0.100,   // P = 1.75 и P = 2.0
            <= 2.50 => -0.106,
            <= 3.00 => -0.112,
            <= 3.50 => -0.118,
            <= 4.00 => -0.125,
            <= 5.00 => -0.132,   // P = 4.5 и P = 5.0
            <= 5.50 => -0.140,
            _ => -0.150,   // P = 6.0
        };

        /// <summary>
        /// Основное отклонение <b>EI</b> поля <b>H</b> — всегда 0 мм.
        /// </summary>
        public static double EI_H() => 0.0;

        /// <summary>
        /// Основное отклонение <b>EI</b> поля <b>G</b>, мм.
        /// Нижнее отклонение диаметров D1, D2 внутренней резьбы (значение ≥ 0).
        /// EI_G = |es_g| — симметрично полю g наружной резьбы.
        /// Определено для P = 0.2 … 6.0 мм.
        /// </summary>
        public static double EI_G(double pitch) => -Es_g(pitch);

        /// <summary>
        /// Основное отклонение <b>EI</b> поля <b>F</b>, мм.
        /// Нижнее отклонение D1, D2 (значение ≥ 0). EI_F = |es_f|.
        /// Определено для P = 0.2 … 2.0 мм; при P &gt; 2.0 поле F не применяется.
        /// </summary>
        public static double EI_F(double pitch) => -Es_f(pitch);

        /// <summary>
        /// Основное отклонение <b>EI</b> поля <b>E</b>, мм.
        /// Нижнее отклонение D1, D2 (значение ≥ 0). EI_E = |es_e|.
        /// Определено начиная с P = 0.5 мм.
        /// </summary>
        public static double EI_E(double pitch) => -Es_e(pitch);

        #endregion

        #region ГОСТ 16093-81 — Допуски диаметров метрической резьбы

        //  Таблицы допусков по ГОСТ 16093-81, мкм:
        //
        //  Td   — допуск наружного диаметра d наружной резьбы (степени 4, 6, 8)
        //  TD1  — допуск внутреннего диаметра D1 внутренней резьбы (степени 4–8)
        //  Td2  — допуск среднего диаметра d2 наружной резьбы (степени 3–10),
        //         зависит от диапазона номинального диаметра и шага
        //  TD2  — допуск среднего диаметра D2 внутренней резьбы (степени 4–9),
        //         зависит от диапазона номинального диаметра и шага
        //
        //  Публичные методы возвращают допуск в мм (double?).
        //  При нестандартном шаге возвращается значение для ближайшего
        //  табличного шага в данном диапазоне диаметров — аналогично
        //  остальным методам класса Thread.
        //  Null — сочетание степени точности и шага не предусмотрено стандартом.

        // ── Td: допуск наружного диаметра d ─────────────────────────────────

        /// <summary>
        /// Допуск Td наружного диаметра <b>d</b> наружной резьбы (ГОСТ 16093-81), мм.
        /// </summary>
        /// <param name="pitch">Шаг резьбы P, мм.</param>
        /// <param name="grade">Степень точности: 4, 6 или 8.</param>
        /// <returns>Допуск в мм, или null если степень точности не определена для данного шага.</returns>
        public static double? Td(double pitch, int grade)
        {
            int? µm = grade switch
            {
                4 => Td_grade4_µm(pitch),
                6 => Td_grade6_µm(pitch),
                8 => Td_grade8_µm(pitch),
                _ => null,
            };
            return µm.HasValue ? µm.Value / 1000.0 : null;
        }

        private static int? Td_grade4_µm(double pitch) => pitch switch
        {
            <= 0.20 => 36,
            <= 0.25 => 42,
            <= 0.30 => 48,
            <= 0.35 => 53,
            <= 0.40 => 60,
            <= 0.45 => 63,
            <= 0.50 => 67,
            <= 0.60 => 80,
            <= 0.75 => 90,
            <= 0.80 => 95,
            <= 1.00 => 112,
            <= 1.25 => 132,
            <= 1.50 => 150,
            <= 1.75 => 170,
            <= 2.00 => 180,
            <= 2.50 => 212,
            <= 3.00 => 236,
            <= 3.50 => 265,
            <= 4.00 => 300,
            <= 4.50 => 315,
            <= 5.00 => 335,
            <= 5.50 => 355,
            _ => 375,
        };

        private static int? Td_grade6_µm(double pitch) => pitch switch
        {
            <= 0.20 => 56,
            <= 0.25 => 67,
            <= 0.30 => 75,
            <= 0.35 => 85,
            <= 0.40 => 95,
            <= 0.45 => 100,
            <= 0.50 => 106,
            <= 0.60 => 125,
            <= 0.75 => 140,
            <= 0.80 => 150,
            <= 1.00 => 180,
            <= 1.25 => 212,
            <= 1.50 => 236,
            <= 1.75 => 265,
            <= 2.00 => 280,
            <= 2.50 => 335,
            <= 3.00 => 375,
            <= 3.50 => 425,
            <= 4.00 => 475,
            <= 4.50 => 500,
            <= 5.00 => 530,
            <= 5.50 => 560,
            _ => 600,
        };

        private static int? Td_grade8_µm(double pitch) => pitch switch
        {
            < 0.80 => null,
            <= 0.80 => 236,
            <= 1.00 => 280,
            <= 1.25 => 335,
            <= 1.50 => 375,
            <= 1.75 => 425,
            <= 2.00 => 450,
            <= 2.50 => 530,
            <= 3.00 => 600,
            <= 3.50 => 670,
            <= 4.00 => 750,
            <= 4.50 => 800,
            <= 5.00 => 850,
            <= 5.50 => 900,
            _ => 950,
        };

        /// <summary>
        /// Допуск TD1 внутреннего диаметра <b>D1</b> внутренней резьбы (ГОСТ 16093-81), мм.
        /// </summary>
        /// <param name="pitch">Шаг резьбы P, мм.</param>
        /// <param name="grade">Степень точности: 4, 5, 6, 7 или 8.</param>
        /// <returns>Допуск в мм, или null если степень не определена для данного шага.</returns>
        public static double? TD1(double pitch, int grade)
        {
            int? µm = grade switch
            {
                4 => TD1_grade4_µm(pitch),
                5 => TD1_grade5_µm(pitch),
                6 => TD1_grade6_µm(pitch),
                7 => TD1_grade7_µm(pitch),
                8 => TD1_grade8_µm(pitch),
                _ => null,
            };
            return µm.HasValue ? µm.Value / 1000.0 : null;
        }

        private static int? TD1_grade4_µm(double pitch) => pitch switch
        {
            <= 0.20 => 38,
            <= 0.25 => 45,
            <= 0.30 => 53,
            <= 0.35 => 63,
            <= 0.40 => 71,
            <= 0.45 => 80,
            <= 0.50 => 90,
            <= 0.60 => 100,
            <= 0.70 => 112,
            <= 0.75 => 118,
            <= 0.80 => 125,
            <= 1.00 => 150,
            <= 1.25 => 170,
            <= 1.50 => 190,
            <= 1.75 => 212,
            <= 2.00 => 236,
            <= 2.50 => 280,
            <= 3.00 => 315,
            <= 3.50 => 355,
            <= 4.00 => 375,
            <= 4.50 => 425,
            <= 5.00 => 450,
            <= 5.50 => 475,
            _ => 500,
        };

        private static int? TD1_grade5_µm(double pitch) => pitch switch
        {
            <= 0.20 => 48,
            <= 0.25 => 56,
            <= 0.30 => 67,
            <= 0.35 => 80,
            <= 0.40 => 90,
            <= 0.45 => 100,
            <= 0.50 => 112,
            <= 0.60 => 125,
            <= 0.70 => 140,
            <= 0.75 => 150,
            <= 0.80 => 160,
            <= 1.00 => 190,
            <= 1.25 => 212,
            <= 1.50 => 236,
            <= 1.75 => 265,
            <= 2.00 => 300,
            <= 2.50 => 355,
            <= 3.00 => 400,
            <= 3.50 => 450,
            <= 4.00 => 475,
            <= 4.50 => 530,
            <= 5.00 => 560,
            <= 5.50 => 600,
            _ => 630,
        };

        private static int? TD1_grade6_µm(double pitch) => pitch switch
        {
            <= 0.20 => 60,
            <= 0.25 => 71,
            <= 0.30 => 85,
            <= 0.35 => 100,
            <= 0.40 => 112,
            <= 0.45 => 125,
            <= 0.50 => 140,
            <= 0.60 => 160,
            <= 0.70 => 180,
            <= 0.75 => 190,
            <= 0.80 => 200,
            <= 1.00 => 236,
            <= 1.25 => 265,
            <= 1.50 => 300,
            <= 1.75 => 335,
            <= 2.00 => 375,
            <= 2.50 => 450,
            <= 3.00 => 500,
            <= 3.50 => 560,
            <= 4.00 => 600,
            <= 4.50 => 670,
            <= 5.00 => 710,
            <= 5.50 => 750,
            _ => 800,
        };

        private static int? TD1_grade7_µm(double pitch) => pitch switch
        {
            < 0.50 => null,   // 7 не предусмотрена для P < 0.5 мм
            <= 0.50 => 180,
            <= 0.60 => 200,
            <= 0.70 => 224,
            <= 0.75 => 236,
            <= 0.80 => 250,
            <= 1.00 => 300,
            <= 1.25 => 335,
            <= 1.50 => 375,
            <= 1.75 => 425,
            <= 2.00 => 475,
            <= 2.50 => 560,
            <= 3.00 => 630,
            <= 3.50 => 710,
            <= 4.00 => 750,
            <= 4.50 => 850,
            <= 5.00 => 900,
            <= 5.50 => 950,
            _ => 1000,
        };

        private static int? TD1_grade8_µm(double pitch) => pitch switch
        {
            < 0.80 => null,   // 8 не предусмотрена для P < 0.8 мм
            <= 0.80 => 315,
            <= 1.00 => 375,
            <= 1.25 => 425,
            <= 1.50 => 475,
            <= 1.75 => 530,
            <= 2.00 => 600,
            <= 2.50 => 710,
            <= 3.00 => 800,
            <= 3.50 => 900,
            <= 4.00 => 950,
            <= 4.50 => 1060,
            <= 5.00 => 1120,
            <= 5.50 => 1180,
            _ => 1250,
        };
        // ── Td2 / TD2: допуски среднего диаметра ─────────────────────────────

        // Ключ словаря: (rangeIdx, pitchKey, grade)
        //   rangeIdx — индекс диапазона номинального диаметра:
        //     0: d ≤ 1.4   1: (1.4, 2.8]   2: (2.8, 5.6]   3: (5.6, 11.2]
        //     4: (11.2, 22.4]  5: (22.4, 45]  6: (45, 90]    7: (90, 180]
        //     8: (180, 355]    9: (355, 600]
        //   pitchKey — шаг × 100, округлённый до int: P=1.25 → 125
        //   grade    — степень точности

        private static int DiameterRangeIndex(double d) => d switch
        {
            <= 1.4 => 0,
            <= 2.8 => 1,
            <= 5.6 => 2,
            <= 11.2 => 3,
            <= 22.4 => 4,
            <= 45.0 => 5,
            <= 90.0 => 6,
            <= 180.0 => 7,
            <= 355.0 => 8,
            _ => 9,
        };

        private static int PitchKey(double pitch) => (int)Math.Round(pitch * 100);

        /// <summary>
        /// Допуск Td2 среднего диаметра <b>d2</b> наружной резьбы (ГОСТ 16093-81), мм.
        /// </summary>
        /// <param name="diameter">Номинальный диаметр резьбы d, мм.</param>
        /// <param name="pitch">Шаг резьбы P, мм.</param>
        /// <param name="grade">Степень точности: 3…10.</param>
        /// <returns>Допуск в мм, или null если сочетание не предусмотрено стандартом.</returns>
        public static double? Td2(double diameter, double pitch, int grade)
        {
            int r = DiameterRangeIndex(diameter);
            int pk = PitchKey(pitch);
            // Точное совпадение
            if (_Td2.TryGetValue((r, pk, grade), out int exact)) return exact / 1000.0;
            // Ближайший шаг для данного диапазона и степени точности
            int? nearest = FindNearestTolerance(_Td2, r, pk, grade);
            return nearest.HasValue ? nearest.Value / 1000.0 : null;
        }

        /// <summary>
        /// Допуск TD2 среднего диаметра <b>D2</b> внутренней резьбы (ГОСТ 16093-81), мм.
        /// </summary>
        /// <param name="diameter">Номинальный диаметр резьбы d, мм.</param>
        /// <param name="pitch">Шаг резьбы P, мм.</param>
        /// <param name="grade">Степень точности: 4…9.</param>
        /// <returns>Допуск в мм, или null если сочетание не предусмотрено стандартом.</returns>
        public static double? TD2(double diameter, double pitch, int grade)
        {
            int r = DiameterRangeIndex(diameter);
            int pk = PitchKey(pitch);
            if (_TD2.TryGetValue((r, pk, grade), out int exact)) return exact / 1000.0;
            int? nearest = FindNearestTolerance(_TD2, r, pk, grade);
            return nearest.HasValue ? nearest.Value / 1000.0 : null;
        }

        /// <summary>
        /// Поиск допуска для ближайшего табличного шага в том же диапазоне диаметров.
        /// Возвращает null, если степень точности не определена ни для одного шага в диапазоне.
        /// </summary>
        private static int? FindNearestTolerance(
            Dictionary<(int range, int pitchKey, int grade), int> table,
            int rangeIdx, int pitchKey, int grade)
        {
            int bestDist = int.MaxValue;
            int? bestVal = null;
            foreach (var kv in table)
            {
                if (kv.Key.range != rangeIdx || kv.Key.grade != grade) continue;
                int dist = Math.Abs(kv.Key.pitchKey - pitchKey);
                if (dist < bestDist) { bestDist = dist; bestVal = kv.Value; }
            }
            return bestVal;
        }

        // ── Таблица Td2 ──────────────────────────────────────────────────────
        // Ключ: (rangeIdx, pitchKey, grade) → допуск, мкм

        private static readonly Dictionary<(int range, int pitchKey, int grade), int> _Td2 = new()
        {
            { (0,  20,  3),   24 },
            { (0,  20,  4),   30 },
            { (0,  20,  5),   38 },
            { (0,  20,  6),   48 },
            { (0,  20,  7),   60 },
            { (0,  20,  8),   75 },
            { (0,  25,  3),   26 },
            { (0,  25,  4),   34 },
            { (0,  25,  5),   42 },
            { (0,  25,  6),   53 },
            { (0,  25,  7),   67 },
            { (0,  25,  8),   85 },
            { (0,  30,  3),   28 },
            { (0,  30,  4),   36 },
            { (0,  30,  5),   45 },
            { (0,  30,  6),   56 },
            { (0,  30,  7),   71 },
            { (0,  30,  8),   90 },
            { (1,  20,  3),   25 },
            { (1,  20,  4),   32 },
            { (1,  20,  5),   40 },
            { (1,  20,  6),   50 },
            { (1,  20,  7),   63 },
            { (1,  20,  8),   80 },
            { (1,  25,  3),   28 },
            { (1,  25,  4),   36 },
            { (1,  25,  5),   45 },
            { (1,  25,  6),   56 },
            { (1,  25,  7),   71 },
            { (1,  25,  8),   90 },
            { (1,  35,  3),   32 },
            { (1,  35,  4),   40 },
            { (1,  35,  5),   50 },
            { (1,  35,  6),   63 },
            { (1,  35,  7),   80 },
            { (1,  35,  8),  100 },
            { (1,  40,  3),   34 },
            { (1,  40,  4),   42 },
            { (1,  40,  5),   53 },
            { (1,  40,  6),   67 },
            { (1,  40,  7),   85 },
            { (1,  40,  8),  106 },
            { (1,  45,  3),   36 },
            { (1,  45,  4),   45 },
            { (1,  45,  5),   56 },
            { (1,  45,  6),   71 },
            { (1,  45,  7),   90 },
            { (1,  45,  8),  112 },
            { (2,  25,  3),   28 },
            { (2,  25,  4),   36 },
            { (2,  25,  5),   45 },
            { (2,  25,  6),   56 },
            { (2,  25,  7),   71 },
            { (2,  35,  3),   34 },
            { (2,  35,  4),   42 },
            { (2,  35,  5),   53 },
            { (2,  35,  6),   67 },
            { (2,  35,  7),   85 },
            { (2,  35,  8),  106 },
            { (2,  50,  3),   38 },
            { (2,  50,  4),   48 },
            { (2,  50,  5),   60 },
            { (2,  50,  6),   75 },
            { (2,  50,  7),   95 },
            { (2,  50,  8),  118 },
            { (2,  60,  3),   42 },
            { (2,  60,  4),   53 },
            { (2,  60,  5),   67 },
            { (2,  60,  6),   85 },
            { (2,  60,  7),  106 },
            { (2,  60,  8),  132 },
            { (2,  70,  3),   45 },
            { (2,  70,  4),   56 },
            { (2,  70,  5),   71 },
            { (2,  70,  6),   90 },
            { (2,  70,  7),  112 },
            { (2,  70,  8),  140 },
            { (2,  75,  3),   45 },
            { (2,  75,  4),   56 },
            { (2,  75,  5),   71 },
            { (2,  75,  6),   90 },
            { (2,  75,  7),  112 },
            { (2,  75,  8),  140 },
            { (2,  80,  3),   48 },
            { (2,  80,  4),   60 },
            { (2,  80,  5),   75 },
            { (2,  80,  6),   95 },
            { (2,  80,  7),  118 },
            { (2,  80,  8),  150 },
            { (2,  80,  9),  190 },
            { (2,  80, 10),  236 },
            { (3,  25,  3),   32 },
            { (3,  25,  4),   40 },
            { (3,  25,  5),   50 },
            { (3,  25,  6),   63 },
            { (3,  25,  7),   80 },
            { (3,  35,  3),   36 },
            { (3,  35,  4),   45 },
            { (3,  35,  5),   56 },
            { (3,  35,  6),   71 },
            { (3,  35,  7),   90 },
            { (3,  50,  3),   42 },
            { (3,  50,  4),   53 },
            { (3,  50,  5),   67 },
            { (3,  50,  6),   85 },
            { (3,  50,  7),  106 },
            { (3,  50,  8),  132 },
            { (3,  75,  3),   50 },
            { (3,  75,  4),   63 },
            { (3,  75,  5),   80 },
            { (3,  75,  6),  100 },
            { (3,  75,  7),  125 },
            { (3,  75,  8),  160 },
            { (3, 100,  3),   56 },
            { (3, 100,  4),   71 },
            { (3, 100,  5),   90 },
            { (3, 100,  6),  112 },
            { (3, 100,  7),  140 },
            { (3, 100,  8),  180 },
            { (3, 100,  9),  224 },
            { (3, 100, 10),  280 },
            { (3, 125,  3),   60 },
            { (3, 125,  4),   75 },
            { (3, 125,  5),   95 },
            { (3, 125,  6),  118 },
            { (3, 125,  7),  150 },
            { (3, 125,  8),  190 },
            { (3, 125,  9),  236 },
            { (3, 125, 10),  300 },
            { (3, 150,  3),   67 },
            { (3, 150,  4),   85 },
            { (3, 150,  5),  106 },
            { (3, 150,  6),  132 },
            { (3, 150,  7),  170 },
            { (3, 150,  8),  212 },
            { (3, 150,  9),  265 },
            { (3, 150, 10),  335 },
            { (4,  35,  3),   38 },
            { (4,  35,  4),   48 },
            { (4,  35,  5),   60 },
            { (4,  35,  6),   75 },
            { (4,  35,  7),   95 },
            { (4,  50,  3),   45 },
            { (4,  50,  4),   56 },
            { (4,  50,  5),   71 },
            { (4,  50,  6),   90 },
            { (4,  50,  7),  112 },
            { (4,  50,  8),  140 },
            { (4,  75,  3),   53 },
            { (4,  75,  4),   67 },
            { (4,  75,  5),   85 },
            { (4,  75,  6),  106 },
            { (4,  75,  7),  132 },
            { (4,  75,  8),  170 },
            { (4, 100,  3),   60 },
            { (4, 100,  4),   75 },
            { (4, 100,  5),   95 },
            { (4, 100,  6),  118 },
            { (4, 100,  7),  150 },
            { (4, 100,  8),  190 },
            { (4, 100,  9),  236 },
            { (4, 100, 10),  300 },
            { (4, 125,  3),   67 },
            { (4, 125,  4),   85 },
            { (4, 125,  5),  106 },
            { (4, 125,  6),  132 },
            { (4, 125,  7),  170 },
            { (4, 125,  8),  212 },
            { (4, 125,  9),  265 },
            { (4, 125, 10),  335 },
            { (4, 150,  3),   71 },
            { (4, 150,  4),   90 },
            { (4, 150,  5),  112 },
            { (4, 150,  6),  140 },
            { (4, 150,  7),  180 },
            { (4, 150,  8),  224 },
            { (4, 150,  9),  280 },
            { (4, 150, 10),  355 },
            { (4, 175,  3),   75 },
            { (4, 175,  4),   95 },
            { (4, 175,  5),  118 },
            { (4, 175,  6),  150 },
            { (4, 175,  7),  190 },
            { (4, 175,  8),  236 },
            { (4, 175,  9),  300 },
            { (4, 175, 10),  375 },
            { (4, 200,  3),   80 },
            { (4, 200,  4),  100 },
            { (4, 200,  5),  125 },
            { (4, 200,  6),  160 },
            { (4, 200,  7),  200 },
            { (4, 200,  8),  250 },
            { (4, 200,  9),  315 },
            { (4, 200, 10),  400 },
            { (4, 250,  3),   85 },
            { (4, 250,  4),  106 },
            { (4, 250,  5),  132 },
            { (4, 250,  6),  170 },
            { (4, 250,  7),  212 },
            { (4, 250,  8),  265 },
            { (4, 250,  9),  335 },
            { (4, 250, 10),  425 },
            { (5,  50,  3),   48 },
            { (5,  50,  4),   60 },
            { (5,  50,  5),   75 },
            { (5,  50,  6),   95 },
            { (5,  50,  7),  118 },
            { (5,  75,  3),   56 },
            { (5,  75,  4),   71 },
            { (5,  75,  5),   90 },
            { (5,  75,  6),  112 },
            { (5,  75,  7),  140 },
            { (5,  75,  8),  180 },
            { (5, 100,  3),   63 },
            { (5, 100,  4),   80 },
            { (5, 100,  5),  100 },
            { (5, 100,  6),  125 },
            { (5, 100,  7),  160 },
            { (5, 100,  8),  200 },
            { (5, 100,  9),  250 },
            { (5, 100, 10),  315 },
            { (5, 150,  3),   75 },
            { (5, 150,  4),   95 },
            { (5, 150,  5),  118 },
            { (5, 150,  6),  150 },
            { (5, 150,  7),  190 },
            { (5, 150,  8),  236 },
            { (5, 150,  9),  300 },
            { (5, 150, 10),  375 },
            { (5, 200,  3),   85 },
            { (5, 200,  4),  106 },
            { (5, 200,  5),  132 },
            { (5, 200,  6),  170 },
            { (5, 200,  7),  212 },
            { (5, 200,  8),  265 },
            { (5, 200,  9),  335 },
            { (5, 200, 10),  425 },
            { (5, 300,  3),  100 },
            { (5, 300,  4),  125 },
            { (5, 300,  5),  160 },
            { (5, 300,  6),  200 },
            { (5, 300,  7),  250 },
            { (5, 300,  8),  315 },
            { (5, 300,  9),  400 },
            { (5, 300, 10),  500 },
            { (5, 350,  3),  106 },
            { (5, 350,  4),  132 },
            { (5, 350,  5),  170 },
            { (5, 350,  6),  212 },
            { (5, 350,  7),  265 },
            { (5, 350,  8),  335 },
            { (5, 350,  9),  425 },
            { (5, 350, 10),  530 },
            { (5, 400,  3),  112 },
            { (5, 400,  4),  140 },
            { (5, 400,  5),  180 },
            { (5, 400,  6),  224 },
            { (5, 400,  7),  280 },
            { (5, 400,  8),  355 },
            { (5, 400,  9),  450 },
            { (5, 400, 10),  560 },
            { (5, 450,  3),  118 },
            { (5, 450,  4),  150 },
            { (5, 450,  5),  190 },
            { (5, 450,  6),  236 },
            { (5, 450,  7),  300 },
            { (5, 450,  8),  375 },
            { (5, 450,  9),  475 },
            { (5, 450, 10),  600 },
            { (6,  50,  3),   50 },
            { (6,  50,  4),   63 },
            { (6,  50,  5),   80 },
            { (6,  50,  6),  100 },
            { (6,  50,  7),  125 },
            { (6,  75,  3),   60 },
            { (6,  75,  4),   75 },
            { (6,  75,  5),   95 },
            { (6,  75,  6),  118 },
            { (6,  75,  7),  150 },
            { (6, 100,  3),   71 },
            { (6, 100,  4),   90 },
            { (6, 100,  5),  112 },
            { (6, 100,  6),  140 },
            { (6, 100,  7),  180 },
            { (6, 100,  8),  224 },
            { (6, 100,  9),  280 },
            { (6, 100, 10),  355 },
            { (6, 150,  3),   80 },
            { (6, 150,  4),  100 },
            { (6, 150,  5),  125 },
            { (6, 150,  6),  160 },
            { (6, 150,  7),  200 },
            { (6, 150,  8),  250 },
            { (6, 150,  9),  315 },
            { (6, 150, 10),  400 },
            { (6, 200,  3),   90 },
            { (6, 200,  4),  112 },
            { (6, 200,  5),  140 },
            { (6, 200,  6),  180 },
            { (6, 200,  7),  224 },
            { (6, 200,  8),  280 },
            { (6, 200,  9),  355 },
            { (6, 200, 10),  450 },
            { (6, 300,  3),  106 },
            { (6, 300,  4),  132 },
            { (6, 300,  5),  170 },
            { (6, 300,  6),  212 },
            { (6, 300,  7),  265 },
            { (6, 300,  8),  335 },
            { (6, 300,  9),  425 },
            { (6, 300, 10),  530 },
            { (6, 400,  3),  118 },
            { (6, 400,  4),  150 },
            { (6, 400,  5),  190 },
            { (6, 400,  6),  236 },
            { (6, 400,  7),  300 },
            { (6, 400,  8),  375 },
            { (6, 400,  9),  475 },
            { (6, 400, 10),  600 },
            { (6, 500,  3),  125 },
            { (6, 500,  4),  160 },
            { (6, 500,  5),  200 },
            { (6, 500,  6),  250 },
            { (6, 500,  7),  315 },
            { (6, 500,  8),  400 },
            { (6, 500,  9),  500 },
            { (6, 500, 10),  630 },
            { (6, 550,  3),  132 },
            { (6, 550,  4),  170 },
            { (6, 550,  5),  212 },
            { (6, 550,  6),  265 },
            { (6, 550,  7),  335 },
            { (6, 550,  8),  425 },
            { (6, 550,  9),  530 },
            { (6, 550, 10),  670 },
            { (6, 600,  3),  140 },
            { (6, 600,  4),  180 },
            { (6, 600,  5),  224 },
            { (6, 600,  6),  280 },
            { (6, 600,  7),  355 },
            { (6, 600,  8),  450 },
            { (6, 600,  9),  560 },
            { (6, 600, 10),  710 },
            { (7,  75,  3),   63 },
            { (7,  75,  4),   80 },
            { (7,  75,  5),  100 },
            { (7,  75,  6),  125 },
            { (7,  75,  7),  160 },
            { (7, 100,  3),   75 },
            { (7, 100,  4),   95 },
            { (7, 100,  5),  118 },
            { (7, 100,  6),  150 },
            { (7, 100,  7),  190 },
            { (7, 150,  3),   85 },
            { (7, 150,  4),  106 },
            { (7, 150,  5),  132 },
            { (7, 150,  6),  170 },
            { (7, 150,  7),  212 },
            { (7, 150,  8),  265 },
            { (7, 150,  9),  335 },
            { (7, 150, 10),  425 },
            { (7, 200,  3),   95 },
            { (7, 200,  4),  118 },
            { (7, 200,  5),  150 },
            { (7, 200,  6),  190 },
            { (7, 200,  7),  236 },
            { (7, 200,  8),  300 },
            { (7, 200,  9),  375 },
            { (7, 200, 10),  475 },
            { (7, 300,  3),  112 },
            { (7, 300,  4),  140 },
            { (7, 300,  5),  180 },
            { (7, 300,  6),  224 },
            { (7, 300,  7),  280 },
            { (7, 300,  8),  355 },
            { (7, 300,  9),  450 },
            { (7, 300, 10),  560 },
            { (7, 400,  3),  125 },
            { (7, 400,  4),  160 },
            { (7, 400,  5),  200 },
            { (7, 400,  6),  250 },
            { (7, 400,  7),  315 },
            { (7, 400,  8),  400 },
            { (7, 400,  9),  500 },
            { (7, 400, 10),  630 },
            { (7, 600,  3),  150 },
            { (7, 600,  4),  190 },
            { (7, 600,  5),  236 },
            { (7, 600,  6),  300 },
            { (7, 600,  7),  375 },
            { (7, 600,  8),  475 },
            { (7, 600,  9),  600 },
            { (7, 600, 10),  750 },
            { (8, 150,  3),   90 },
            { (8, 150,  4),  112 },
            { (8, 150,  5),  140 },
            { (8, 150,  6),  180 },
            { (8, 150,  7),  224 },
            { (8, 150,  8),  280 },
            { (8, 150,  9),  355 },
            { (8, 200,  3),  106 },
            { (8, 200,  4),  132 },
            { (8, 200,  5),  170 },
            { (8, 200,  6),  212 },
            { (8, 200,  7),  265 },
            { (8, 200,  8),  335 },
            { (8, 200,  9),  425 },
            { (8, 200, 10),  530 },
            { (8, 300,  3),  125 },
            { (8, 300,  4),  160 },
            { (8, 300,  5),  200 },
            { (8, 300,  6),  250 },
            { (8, 300,  7),  315 },
            { (8, 300,  8),  400 },
            { (8, 300,  9),  500 },
            { (8, 300, 10),  630 },
            { (8, 400,  3),  140 },
            { (8, 400,  4),  180 },
            { (8, 400,  5),  224 },
            { (8, 400,  6),  280 },
            { (8, 400,  7),  355 },
            { (8, 400,  8),  450 },
            { (8, 400,  9),  560 },
            { (8, 400, 10),  710 },
            { (8, 600,  3),  160 },
            { (8, 600,  4),  200 },
            { (8, 600,  5),  250 },
            { (8, 600,  6),  315 },
            { (8, 600,  7),  400 },
            { (8, 600,  8),  500 },
            { (8, 600,  9),  630 },
            { (8, 600, 10),  800 },
            { (9, 200,  3),  112 },
            { (9, 200,  4),  140 },
            { (9, 200,  5),  180 },
            { (9, 200,  6),  224 },
            { (9, 200,  7),  280 },
            { (9, 200,  8),  355 },
            { (9, 200,  9),  450 },
            { (9, 400,  3),  150 },
            { (9, 400,  4),  190 },
            { (9, 400,  5),  236 },
            { (9, 400,  6),  300 },
            { (9, 400,  7),  375 },
            { (9, 400,  8),  475 },
            { (9, 400,  9),  600 },
            { (9, 400, 10),  750 },
            { (9, 600,  3),  170 },
            { (9, 600,  4),  212 },
            { (9, 600,  5),  265 },
            { (9, 600,  6),  335 },
            { (9, 600,  7),  425 },
            { (9, 600,  8),  530 },
            { (9, 600,  9),  670 },
            { (9, 600, 10),  850 },
        };

        // ── Таблица TD2 ──────────────────────────────────────────────────────

        private static readonly Dictionary<(int range, int pitchKey, int grade), int> _TD2 = new()
        {
            { (0,  20,  4),   40 },
            { (0,  20,  5),   50 },
            { (0,  20,  6),   63 },
            { (0,  25,  4),   45 },
            { (0,  25,  5),   56 },
            { (0,  25,  6),   71 },
            { (0,  30,  4),   48 },
            { (0,  30,  5),   60 },
            { (0,  30,  6),   75 },
            { (1,  20,  4),   42 },
            { (1,  20,  5),   53 },
            { (1,  20,  6),   67 },
            { (1,  25,  4),   48 },
            { (1,  25,  5),   60 },
            { (1,  25,  6),   75 },
            { (1,  35,  4),   53 },
            { (1,  35,  5),   67 },
            { (1,  35,  6),   85 },
            { (1,  40,  4),   56 },
            { (1,  40,  5),   71 },
            { (1,  40,  6),   90 },
            { (1,  45,  4),   60 },
            { (1,  45,  5),   75 },
            { (1,  45,  6),   95 },
            { (2,  25,  4),   48 },
            { (2,  25,  5),   60 },
            { (2,  25,  6),   75 },
            { (2,  35,  4),   56 },
            { (2,  35,  5),   71 },
            { (2,  35,  6),   90 },
            { (2,  50,  4),   63 },
            { (2,  50,  5),   80 },
            { (2,  50,  6),  100 },
            { (2,  50,  7),  125 },
            { (2,  60,  4),   71 },
            { (2,  60,  5),   90 },
            { (2,  60,  6),  112 },
            { (2,  60,  7),  140 },
            { (2,  70,  4),   75 },
            { (2,  70,  5),   95 },
            { (2,  70,  6),  118 },
            { (2,  70,  7),  150 },
            { (2,  75,  4),   75 },
            { (2,  75,  5),   95 },
            { (2,  75,  6),  118 },
            { (2,  75,  7),  150 },
            { (2,  80,  4),   80 },
            { (2,  80,  5),  100 },
            { (2,  80,  6),  125 },
            { (2,  80,  7),  160 },
            { (2,  80,  8),  200 },
            { (2,  80,  9),  250 },
            { (3,  25,  4),   53 },
            { (3,  25,  5),   67 },
            { (3,  25,  6),   85 },
            { (3,  35,  4),   60 },
            { (3,  35,  5),   75 },
            { (3,  35,  6),   95 },
            { (3,  50,  4),   71 },
            { (3,  50,  5),   90 },
            { (3,  50,  6),  112 },
            { (3,  50,  7),  140 },
            { (3,  75,  4),   85 },
            { (3,  75,  5),  106 },
            { (3,  75,  6),  132 },
            { (3,  75,  7),  170 },
            { (3, 100,  4),   95 },
            { (3, 100,  5),  118 },
            { (3, 100,  6),  150 },
            { (3, 100,  7),  190 },
            { (3, 100,  8),  236 },
            { (3, 100,  9),  300 },
            { (3, 125,  4),  100 },
            { (3, 125,  5),  125 },
            { (3, 125,  6),  160 },
            { (3, 125,  7),  200 },
            { (3, 125,  8),  250 },
            { (3, 125,  9),  315 },
            { (3, 150,  4),  112 },
            { (3, 150,  5),  140 },
            { (3, 150,  6),  180 },
            { (3, 150,  7),  224 },
            { (3, 150,  8),  280 },
            { (3, 150,  9),  355 },
            { (4,  35,  4),   63 },
            { (4,  35,  5),   80 },
            { (4,  35,  6),  100 },
            { (4,  50,  4),   75 },
            { (4,  50,  5),   95 },
            { (4,  50,  6),  118 },
            { (4,  50,  7),  150 },
            { (4,  75,  4),   90 },
            { (4,  75,  5),  112 },
            { (4,  75,  6),  140 },
            { (4,  75,  7),  180 },
            { (4, 100,  4),  100 },
            { (4, 100,  5),  125 },
            { (4, 100,  6),  160 },
            { (4, 100,  7),  200 },
            { (4, 100,  8),  250 },
            { (4, 100,  9),  315 },
            { (4, 125,  4),  112 },
            { (4, 125,  5),  140 },
            { (4, 125,  6),  180 },
            { (4, 125,  7),  224 },
            { (4, 125,  8),  280 },
            { (4, 125,  9),  355 },
            { (4, 150,  4),  118 },
            { (4, 150,  5),  150 },
            { (4, 150,  6),  190 },
            { (4, 150,  7),  236 },
            { (4, 150,  8),  300 },
            { (4, 150,  9),  375 },
            { (4, 175,  4),  125 },
            { (4, 175,  5),  160 },
            { (4, 175,  6),  200 },
            { (4, 175,  7),  250 },
            { (4, 175,  8),  315 },
            { (4, 175,  9),  400 },
            { (4, 200,  4),  132 },
            { (4, 200,  5),  170 },
            { (4, 200,  6),  212 },
            { (4, 200,  7),  265 },
            { (4, 200,  8),  335 },
            { (4, 200,  9),  425 },
            { (4, 250,  4),  140 },
            { (4, 250,  5),  180 },
            { (4, 250,  6),  224 },
            { (4, 250,  7),  280 },
            { (4, 250,  8),  355 },
            { (4, 250,  9),  450 },
            { (5,  50,  4),   80 },
            { (5,  50,  5),  100 },
            { (5,  50,  6),  125 },
            { (5,  75,  4),   95 },
            { (5,  75,  5),  118 },
            { (5,  75,  6),  150 },
            { (5,  75,  7),  190 },
            { (5, 100,  4),  106 },
            { (5, 100,  5),  132 },
            { (5, 100,  6),  170 },
            { (5, 100,  7),  212 },
            { (5, 100,  8),  265 },
            { (5, 100,  9),  335 },
            { (5, 150,  4),  125 },
            { (5, 150,  5),  160 },
            { (5, 150,  6),  200 },
            { (5, 150,  7),  250 },
            { (5, 150,  8),  315 },
            { (5, 150,  9),  400 },
            { (5, 200,  4),  140 },
            { (5, 200,  5),  180 },
            { (5, 200,  6),  224 },
            { (5, 200,  7),  280 },
            { (5, 200,  8),  355 },
            { (5, 200,  9),  450 },
            { (5, 300,  4),  170 },
            { (5, 300,  5),  212 },
            { (5, 300,  6),  265 },
            { (5, 300,  7),  335 },
            { (5, 300,  8),  425 },
            { (5, 300,  9),  530 },
            { (5, 350,  4),  180 },
            { (5, 350,  5),  224 },
            { (5, 350,  6),  280 },
            { (5, 350,  7),  355 },
            { (5, 350,  8),  450 },
            { (5, 350,  9),  560 },
            { (5, 400,  4),  190 },
            { (5, 400,  5),  236 },
            { (5, 400,  6),  300 },
            { (5, 400,  7),  375 },
            { (5, 400,  8),  475 },
            { (5, 400,  9),  600 },
            { (5, 450,  4),  200 },
            { (5, 450,  5),  250 },
            { (5, 450,  6),  315 },
            { (5, 450,  7),  400 },
            { (5, 450,  8),  500 },
            { (5, 450,  9),  630 },
            { (6,  50,  4),   85 },
            { (6,  50,  5),  106 },
            { (6,  50,  6),  132 },
            { (6,  75,  4),  100 },
            { (6,  75,  5),  125 },
            { (6,  75,  6),  160 },
            { (6, 100,  4),  118 },
            { (6, 100,  5),  150 },
            { (6, 100,  6),  190 },
            { (6, 100,  7),  236 },
            { (6, 100,  8),  300 },
            { (6, 100,  9),  375 },
            { (6, 150,  4),  132 },
            { (6, 150,  5),  170 },
            { (6, 150,  6),  212 },
            { (6, 150,  7),  265 },
            { (6, 150,  8),  335 },
            { (6, 150,  9),  425 },
            { (6, 200,  4),  150 },
            { (6, 200,  5),  190 },
            { (6, 200,  6),  236 },
            { (6, 200,  7),  300 },
            { (6, 200,  8),  375 },
            { (6, 200,  9),  475 },
            { (6, 300,  4),  180 },
            { (6, 300,  5),  224 },
            { (6, 300,  6),  280 },
            { (6, 300,  7),  355 },
            { (6, 300,  8),  450 },
            { (6, 300,  9),  560 },
            { (6, 400,  4),  200 },
            { (6, 400,  5),  250 },
            { (6, 400,  6),  315 },
            { (6, 400,  7),  400 },
            { (6, 400,  8),  500 },
            { (6, 400,  9),  630 },
            { (6, 500,  4),  212 },
            { (6, 500,  5),  265 },
            { (6, 500,  6),  335 },
            { (6, 500,  7),  425 },
            { (6, 500,  8),  530 },
            { (6, 500,  9),  670 },
            { (6, 550,  4),  224 },
            { (6, 550,  5),  280 },
            { (6, 550,  6),  355 },
            { (6, 550,  7),  450 },
            { (6, 550,  8),  560 },
            { (6, 550,  9),  710 },
            { (6, 600,  4),  236 },
            { (6, 600,  5),  300 },
            { (6, 600,  6),  375 },
            { (6, 600,  7),  475 },
            { (6, 600,  8),  600 },
            { (6, 600,  9),  750 },
            { (7,  75,  4),  106 },
            { (7,  75,  5),  132 },
            { (7,  75,  6),  170 },
            { (7, 100,  4),  125 },
            { (7, 100,  5),  160 },
            { (7, 100,  6),  200 },
            { (7, 100,  7),  250 },
            { (7, 150,  4),  140 },
            { (7, 150,  5),  180 },
            { (7, 150,  6),  224 },
            { (7, 150,  7),  280 },
            { (7, 150,  8),  355 },
            { (7, 150,  9),  450 },
            { (7, 200,  4),  160 },
            { (7, 200,  5),  200 },
            { (7, 200,  6),  250 },
            { (7, 200,  7),  315 },
            { (7, 200,  8),  400 },
            { (7, 200,  9),  500 },
            { (7, 300,  4),  190 },
            { (7, 300,  5),  236 },
            { (7, 300,  6),  300 },
            { (7, 300,  7),  375 },
            { (7, 300,  8),  475 },
            { (7, 300,  9),  600 },
            { (7, 400,  4),  212 },
            { (7, 400,  5),  265 },
            { (7, 400,  6),  335 },
            { (7, 400,  7),  425 },
            { (7, 400,  8),  530 },
            { (7, 400,  9),  670 },
            { (7, 600,  4),  250 },
            { (7, 600,  5),  315 },
            { (7, 600,  6),  400 },
            { (7, 600,  7),  500 },
            { (7, 600,  8),  630 },
            { (7, 600,  9),  800 },
            { (8, 150,  4),  150 },
            { (8, 150,  5),  190 },
            { (8, 150,  6),  236 },
            { (8, 150,  7),  300 },
            { (8, 150,  8),  375 },
            { (8, 200,  4),  180 },
            { (8, 200,  5),  224 },
            { (8, 200,  6),  280 },
            { (8, 200,  7),  355 },
            { (8, 200,  8),  450 },
            { (8, 200,  9),  560 },
            { (8, 300,  4),  212 },
            { (8, 300,  5),  265 },
            { (8, 300,  6),  335 },
            { (8, 300,  7),  425 },
            { (8, 300,  8),  530 },
            { (8, 300,  9),  670 },
            { (8, 400,  4),  236 },
            { (8, 400,  5),  300 },
            { (8, 400,  6),  375 },
            { (8, 400,  7),  475 },
            { (8, 400,  8),  600 },
            { (8, 400,  9),  750 },
            { (8, 600,  4),  265 },
            { (8, 600,  5),  335 },
            { (8, 600,  6),  425 },
            { (8, 600,  7),  530 },
            { (8, 600,  8),  670 },
            { (8, 600,  9),  850 },
            { (9, 200,  4),  190 },
            { (9, 200,  5),  236 },
            { (9, 200,  6),  300 },
            { (9, 200,  7),  375 },
            { (9, 200,  8),  475 },
            { (9, 400,  4),  250 },
            { (9, 400,  5),  315 },
            { (9, 400,  6),  400 },
            { (9, 400,  7),  500 },
            { (9, 400,  8),  630 },
            { (9, 400,  9),  800 },
            { (9, 600,  4),  280 },
            { (9, 600,  5),  355 },
            { (9, 600,  6),  450 },
            { (9, 600,  7),  560 },
            { (9, 600,  8),  710 },
            { (9, 600,  9),  900 },
        };

        #endregion

        // ─────────────────────────────────────────────────────────────────────
        //  ГОСТ 19257 / 19258 — номинальные диаметры из табличных данных.
        //  Поправки зависят только от шага P, не от d → достаточно двух словарей.
        //  Ключ: (int)(P × 100), чтобы избежать сравнения double.
        // ─────────────────────────────────────────────────────────────────────

        public readonly record struct HoleNominal(
            double H,           // 4H5H; 5H; 5H6H; 6H; 7H — номинал
            double G,           // 6G; 7G                   — номинал
            double Dev4H5H,     // пред. откл. 4H5H; 5H      (+)
            double Dev5H6H6G,   // пред. откл. 5H6H; 6H; 6G (+)
            double? Dev7H7G     // пред. откл. 7H; 7G        (+), null = н/а
        );

        public readonly record struct RodNominal(
            double Nom,         // 4h и 6h — общий номинал
            double Dev4h,       // откл. 4h  (−)
            double Nom6g,       // 6g номинал
            double? Nom6e,      // 6e номинал, null = н/а
            double? Nom6d,      // 6d номинал, null = н/а
            double Dev6hGroup,  // откл. 6h; 6g; 6e; 6d  (−)
            double? Nom8h,      // = Nom, null если поля нет
            double? Nom8g,      // = Nom6g, null если поля нет
            double? Dev8hGroup  // откл. 8h; 8g  (−), null = н/а
        );

        private readonly record struct HoleEntry(
            double HCorr, double HGOff,
            double Dev4H5H, double Dev5H6H6G, double? Dev7H7G);

        private readonly record struct RodEntry(
            double RCorr, double Dev4h, double RGOff,
            double? REOff, double? RDOff,
            double Dev6hGroup, double? Dev8hGroup);

        private static readonly Dictionary<int, HoleEntry> _holeTable = new()
{
    //  P×100  HCorr  hG+   4H5H  5H6H   7H
    {  20, new(0.20, 0.02, 0.04, 0.05,  null) },
    {  25, new(0.25, 0.02, 0.04, 0.06,  null) },
    {  30, new(0.30, 0.02, 0.04, 0.06,  null) },
    {  35, new(0.35, 0.02, 0.05, 0.07,  null) },
    {  40, new(0.40, 0.02, 0.06, 0.08,  null) },
    {  45, new(0.45, 0.02, 0.07, 0.09,  null) },
    {  50, new(0.50, 0.02, 0.08, 0.10, 0.14)  },
    {  60, new(0.60, 0.03, 0.08, 0.11, 0.15)  },
    {  70, new(0.70, 0.03, 0.08, 0.12, 0.16)  },
    {  75, new(0.80, 0.03, 0.09, 0.13, 0.18)  }, // HCorr ≠ P — округление ГОСТ
    {  80, new(0.80, 0.03, 0.11, 0.17, 0.22)  },
    { 100, new(1.05, 0.05, 0.17, 0.20, 0.26)  },
    { 125, new(1.30, 0.05, 0.19, 0.22, 0.30)  },
    { 150, new(1.57, 0.07, 0.19, 0.22, 0.30)  }, // HGOff=0.07 — уникально
    { 175, new(1.80, 0.05, 0.21, 0.27, 0.36)  },
    { 200, new(2.10, 0.05, 0.24, 0.30, 0.40)  },
    { 250, new(2.65, 0.05, 0.30, 0.40, 0.53)  },
    { 300, new(3.15, 0.05, 0.30, 0.40, 0.53)  },
    { 350, new(3.70, 0.05, 0.36, 0.48, 0.62)  },
    { 400, new(4.20, 0.05, 0.36, 0.48, 0.62)  },
    { 450, new(4.75, 0.05, 0.41, 0.55, 0.73)  },
    { 500, new(5.30, 0.10, 0.45, 0.60, 0.80)  },
    { 550, new(5.80, 0.10, 0.45, 0.60, 0.80)  },
    { 600, new(6.30, 0.10, 0.45, 0.60, 0.80)  },
};

        private static readonly Dictionary<int, RodEntry> _rodTable = new()
{
    //  P×100  RCorr  4h-dev  g     e       d       6h-группа  8h-группа
    {  20, new(0.02, -0.03, 0.02,  null,   null,  -0.04,  null)  },
    {  25, new(0.03, -0.03, 0.02,  null,   null,  -0.04,  null)  },
    {  30, new(0.04, -0.03, 0.02,  null,   null,  -0.04,  null)  },
    {  35, new(0.05, -0.03, 0.02,  null,   null,  -0.04,  null)  },
    {  40, new(0.05, -0.04, 0.02,  null,   null,  -0.05,  null)  },
    {  45, new(0.05, -0.04, 0.02,  null,   null,  -0.06,  null)  },
    {  50, new(0.06, -0.04, 0.02,  0.05,   null,  -0.06,  null)  },
    {  60, new(0.06, -0.05, 0.02,  0.05,   null,  -0.07,  null)  },
    {  70, new(0.06, -0.06, 0.02,  0.05,   null,  -0.08,  null)  },
    {  75, new(0.06, -0.06, 0.06,  0.06,   null,  -0.09,  null)  },
    {  80, new(0.06, -0.07, 0.02,  0.06,   null,  -0.10, -0.18)  },
    { 100, new(0.08, -0.07, 0.03,  0.06,   0.09,  -0.10, -0.20)  },
    { 125, new(0.10, -0.08, 0.03,  0.06,   0.10,  -0.11, -0.24)  },
    { 150, new(0.12, -0.09, 0.03,  0.07,   0.10,  -0.12, -0.26)  },
    { 175, new(0.14, -0.10, 0.03,  0.06,   0.10,  -0.13, -0.29)  },
    { 200, new(0.16, -0.10, 0.04,  0.07,   0.10,  -0.13, -0.29)  },
    { 250, new(0.16, -0.13, 0.04,  0.08,   0.11,  -0.18, -0.37)  },
    { 300, new(0.16, -0.16, 0.05,  0.09,   0.11,  -0.22, -0.44)  },
    { 350, new(0.16, -0.18, 0.05,  0.09,   0.12,  -0.27, -0.51)  },
    { 400, new(0.16, -0.22, 0.06,  0.10,   0.13,  -0.32, -0.59)  },
    { 450, new(0.16, -0.24, 0.06,  0.10,   0.13,  -0.34, -0.64)  },
    { 500, new(0.16, -0.26, 0.07,  0.11,   0.13,  -0.37, -0.69)  },
    { 550, new(0.16, -0.28, 0.08,  0.11,   0.14,  -0.40, -0.74)  },
    { 600, new(0.16, -0.30, 0.08,  0.12,   0.15,  -0.44, -0.79)  },
};

        /// <summary>
        /// ГОСТ 19257 — номинальные диаметры отверстия под резьбу.
        /// </summary>
        public static HoleNominal? GetHoleNominal(double d, double pitch)
        {
            if (!_holeTable.TryGetValue((int)Math.Round(pitch * 100), out var e))
                return null;
            double h = Math.Round(d - e.HCorr, 2, MidpointRounding.AwayFromZero);
            double g = Math.Round(h + e.HGOff, 2, MidpointRounding.AwayFromZero);
            return new(h, g, e.Dev4H5H, e.Dev5H6H6G, e.Dev7H7G);
        }

        /// <summary>
        /// ГОСТ 19258 — номинальные диаметры стержня под резьбу.
        /// </summary>
        public static RodNominal? GetRodNominal(double d, double pitch)
        {
            if (!_rodTable.TryGetValue((int)Math.Round(pitch * 100), out var e))
                return null;
            double nom = Math.Round(d - e.RCorr, 2, MidpointRounding.AwayFromZero);
            double nom6g = Math.Round(nom - e.RGOff, 2, MidpointRounding.AwayFromZero);
            return new(
                Nom: nom,
                Dev4h: e.Dev4h,
                Nom6g: nom6g,
                Nom6e: e.REOff.HasValue ? Math.Round(nom - e.REOff!.Value, 2, MidpointRounding.AwayFromZero) : null,
                Nom6d: e.RDOff.HasValue ? Math.Round(nom - e.RDOff!.Value, 2, MidpointRounding.AwayFromZero) : null,
                Dev6hGroup: e.Dev6hGroup,
                Nom8h: e.Dev8hGroup.HasValue ? nom : null,
                Nom8g: e.Dev8hGroup.HasValue ? nom6g : null,
                Dev8hGroup: e.Dev8hGroup
            );
        }
    }
}
