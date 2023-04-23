using Sunduk.PWA.Infrastructure.CAM;
using Sunduk.PWA.Infrastructure.Sequences;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sunduk.PWA.Infrastructure.Templates
{
    public static class Thread
    {
        public enum PassesOption { FullPasses, Infeed }



        #region Шаблоны
        public static readonly HashSet<string> MetricTemplates = new()
        {
            //"M2x0.4 (Стандартный)", "M2x0.25 (Мелкий)",
            //"M2.5x0.45 (Стандартный)", "M2.5x0.35 (Мелкий)",
            //"M3x0.5 (Стандартный)", "M3x0.35 (Мелкий)",
            //"M3.5x0.6 (Стандартный)", "M3.5x0.35 (Мелкий)",
            //"M4x0.7 (Стандартный)", "M4x0.5 (Мелкий)", "M4.5x0.75", "M4.5x0.5 (Мелкий)",
            //"M5x0.8 (Стандартный)", "M5x0.5 (Мелкий)", "M5.5x0.5 (Стандартный)",
            "M6x1 (Стандартный)",
            "M6x0.75 (Мелкий)",
            "M6x0.5 (Мелкий)",
            //"M7x1 (Стандартный)", "M7x0.75 (Мелкий)", "M7x0.5 (Мелкий)",
            "M8x1.25 (Стандартный)",
            "M8x1 (Мелкий)",
            "M8x0.75 (Мелкий)",
            "M8x0.5 (Мелкий)",
            //"M9x1.25 (Стандартный)", "M9x1 (Мелкий)", "M9x0.75 (Мелкий)", "M9x0.5 (Мелкий)",
            "M10x1.5 (Стандартный)",
            "M10x1.25 (Мелкий)",
            "M10x1 (Мелкий)",
            "M10x0.75 (Мелкий)",
            "M10x0.5 (Мелкий)",
            //"M11x1.5 (Стандартный)", "M11x1 (Мелкий)", "M11x0.75 (Мелкий)", "M11x0.5 (Мелкий)",
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
            //"M15x1.5 (Стандартный)", "M15x1 (Мелкий)",
            "M16x2 (Стандартный)",
            "M16x1.5 (Мелкий)",
            "M16x1 (Мелкий)",
            "M16x0.75 (Мелкий)",
            "M16x0.5 (Мелкий)",
            //"M17x1.5 (Стандартный)", "M17x1 (Мелкий)",
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
            //"M22x2.5 (Стандартный)", "M22x2 (Мелкий)", "M22x1.5 (Мелкий)", "M22x1 (Мелкий)", "M22x0.75 (Мелкий)", "M22x0.5 (Мелкий)",
            "M24x3 (Стандартный)",
            "M24x2 (Мелкий)",
            "M24x1.5 (Мелкий)",
            "M24x1 (Мелкий)",
            "M24x0.75 (Мелкий)",
            //"M25x2 (Стандартный)", "M25x1.5 (Мелкий)", "M25x1 (Мелкий)",
            //"M26x1.5 (Стандартный)",
            //"M27x3 (Стандартный)", "M27x2 (Мелкий)", "M27x1.5 (Мелкий)", "M27x1 (Мелкий)", "M27x0.75 (Мелкий)",
            //"M27x2 (Стандартный)", "M27x1.5 (Мелкий)", "M27x1 (Мелкий)",
            "M30x3.5 (Стандартный)",
            "M30x3 (Мелкий)",
            "M30x2 (Мелкий)",
            "M30x1.5 (Мелкий)",
            "M30x1 (Мелкий)",
            "M30x0.75 (Мелкий)",
            //"M32x2 (Стандартный)", "M32x1.5 (Мелкий)",
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
            //"M38x1.5 (Стандартный)",
            //"M39x4 (Стандартный)", "M39x3 (Мелкий)", "M39x2 (Мелкий)", "M39x1.5 (Мелкий)", "M39x1 (Мелкий)",
            "M40x3 (Стандартный)",
            "M40x2 (Мелкий)",
            "M40x1.5 (Мелкий)",
            //"M42x4.5 (Стандартный)", "M42x4 (Мелкий)", "M42x3 (Мелкий)", "M42x2 (Мелкий)", "M42x1.5 (Мелкий)", "M42x1 (Мелкий)",
            //"M45x4.5 (Стандартный)", "M45x4 (Мелкий)", "M45x3 (Мелкий)", "M45x2 (Мелкий)", "M45x1.5 (Мелкий)", "M45x1 (Мелкий)",
            //"M48x5 (Стандартный)", "M48x4 (Мелкий)", "M48x3 (Мелкий)", "M48x2 (Мелкий)", "M48x1.5 (Мелкий)", "M48x1 (Мелкий)",
            "M50x3 (Стандартный)",
            "M50x2 (Мелкий)",
            "M50x1.5 (Мелкий)",
            //"M52x5 (Стандартный)", "M52x4 (Мелкий)", "M52x3 (Мелкий)", "M52x2 (Мелкий)", "M52x1.5 (Мелкий)", "M52x1 (Мелкий)",
            //"M55x4 (Стандартный)", "M55x3 (Мелкий)", "M55x2 (Мелкий)", "M55x1.5 (Мелкий)",
            //"M56x5.5 (Стандартный)", "M56x4 (Мелкий)", "M56x3 (Мелкий)", "M56x2 (Мелкий)", "M56x1.5 (Мелкий)", "M56x1 (Мелкий)",
            //"M58x4 (Стандартный)", "M58x3 (Мелкий)", "M58x2 (Мелкий)", "M58x1.5 (Мелкий)",
            "M60x5.5 (Стандартный)",
            "M60x4 (Мелкий)",
            "M60x3 (Мелкий)",
            "M60x2 (Мелкий)",
            "M60x1.5 (Мелкий)",
            "M60x1 (Мелкий)",
            //"M62x4 (Стандартный)", "M62x3 (Мелкий)", "M62x2 (Мелкий)", "M62x1.5 (Мелкий)",
            //"M64x6 (Стандартный)", "M64x4 (Мелкий)", "M64x3 (Мелкий)", "M64x2 (Мелкий)", "M64x1.5 (Мелкий)", "M64x1 (Мелкий)",
            //"M65x4 (Стандартный)", "M65x3 (Мелкий)", "M65x2 (Мелкий)", "M65x1.5 (Мелкий)",
            //"M68x6 (Стандартный)", "M68x4 (Мелкий)", "M68x3 (Мелкий)", "M68x2 (Мелкий)", "M68x1.5 (Мелкий)", "M68x1 (Мелкий)",
            "M70x6 (Стандартный)",
            "M70x4 (Мелкий)",
            "M70x3 (Мелкий)",
            "M70x2 (Мелкий)",
            "M70x1.5 (Мелкий)",
            //"M72x6 (Стандартный)", "M72x4 (Мелкий)", "M72x3 (Мелкий)", "M72x2 (Мелкий)", "M72x1.5 (Мелкий)", "M72x1 (Мелкий)",
            //"M75x4 (Стандартный)", "M75x3 (Мелкий)", "M75x2 (Мелкий)", "M75x1.5 (Мелкий)",
            //"M76x6 (Стандартный)", "M76x4 (Мелкий)", "M76x3 (Мелкий)", "M76x2 (Мелкий)", "M76x1.5 (Мелкий)", "M76x1 (Мелкий)",
            //"M78x2 (Стандартный)",
            "M80x6 (Стандартный)",
            "M80x4 (Мелкий)",
            "M80x3 (Мелкий)",
            "M80x2 (Мелкий)",
            "M80x1.5 (Мелкий)",
            "M80x1 (Мелкий)",
            //"M82x2 (Стандартный)",
            //"M85x6 (Стандартный)", "M85x4 (Мелкий)", "M85x3 (Мелкий)", "M85x2 (Мелкий)", "M85x1.5 (Мелкий)",
            "M90x6 (Стандартный)",
            "M90x4 (Мелкий)",
            "M90x3 (Мелкий)",
            "M90x2 (Мелкий)",
            "M90x1.5 (Мелкий)",
            //"M95x6 (Стандартный)", "M95x4 (Мелкий)", "M95x3 (Мелкий)", "M95x2 (Мелкий)", "M95x1.5 (Мелкий)",
            "M100x6 (Стандартный)",
            "M100x4 (Мелкий)",
            "M100x3 (Мелкий)",
            "M100x2 (Мелкий)",
            "M100x1.5 (Мелкий)",
            //"M105x6 (Стандартный)", "M105x4 (Мелкий)", "M105x3 (Мелкий)", "M105x2 (Мелкий)", "M105x1.5 (Мелкий)",
            //"M110x6 (Стандартный)", "M110x4 (Мелкий)", "M110x3 (Мелкий)", "M110x2 (Мелкий)", "M110x1.5 (Мелкий)",
            //"M115x6 (Стандартный)", "M115x4 (Мелкий)", "M115x3 (Мелкий)", "M115x2 (Мелкий)", "M115x1.5 (Мелкий)",
            //"M120x6 (Стандартный)", "M120x4 (Мелкий)", "M120x3 (Мелкий)", "M120x2 (Мелкий)", "M120x1.5 (Мелкий)",
            //"M125x8 (Стандартный)", "M125x6 (Мелкий)", "M125x4 (Мелкий)", "M125x3 (Мелкий)", "M125x2 (Мелкий)", "M125x1.5 (Мелкий)",
            //"M130x8 (Стандартный)", "M130x6 (Мелкий)", "M130x4 (Мелкий)", "M130x3 (Мелкий)", "M130x2 (Мелкий)", "M130x1.5 (Мелкий)",
            //"M135x6 (Стандартный)", "M135x4 (Мелкий)", "M135x3 (Мелкий)", "M135x2 (Мелкий)", "M135x1.5 (Мелкий)",
            //"M140x8 (Стандартный)", "M140x6 (Мелкий)", "M140x4 (Мелкий)", "M140x3 (Мелкий)", "M140x2 (Мелкий)", "M140x1.5 (Мелкий)",
            //"M145x6 (Стандартный)", "M145x4 (Мелкий)", "M145x3 (Мелкий)", "M145x2 (Мелкий)", "M145x1.5 (Мелкий)",
            //"M150x8 (Стандартный)", "M150x6 (Мелкий)", "M150x4 (Мелкий)", "M150x3 (Мелкий)", "M150x2 (Мелкий)", "M150x1.5 (Мелкий)",
            //"M155x6 (Стандартный)", "M155x4 (Мелкий)", "M155x3 (Мелкий)", "M155x2 (Мелкий)",
            //"M160x8 (Стандартный)", "M160x6 (Мелкий)", "M160x4 (Мелкий)", "M160x3 (Мелкий)", "M160x2 (Мелкий)",
            //"M165x6 (Стандартный)", "M165x4 (Мелкий)", "M165x3 (Мелкий)", "M165x2 (Мелкий)",
            //"M170x8 (Стандартный)", "M170x6 (Мелкий)", "M170x4 (Мелкий)", "M170x3 (Мелкий)", "M170x2 (Мелкий)",
            //"M175x6 (Стандартный)", "M175x4 (Мелкий)", "M175x3 (Мелкий)", "M175x2 (Мелкий)",
            //"M180x8 (Стандартный)", "M180x6 (Мелкий)", "M180x4 (Мелкий)", "M180x3 (Мелкий)", "M180x2 (Мелкий)",
            //"M185x6 (Стандартный)", "M185x4 (Мелкий)", "M185x3 (Мелкий)", "M185x2 (Мелкий)",
            //"M190x8 (Стандартный)", "M190x6 (Мелкий)", "M190x4 (Мелкий)", "M190x3 (Мелкий)", "M190x2 (Мелкий)",
            //"M195x6 (Стандартный)", "M195x4 (Мелкий)", "M195x3 (Мелкий)", "M195x2 (Мелкий)",
            //"M200x8 (Стандартный)", "M200x6 (Мелкий)", "M200x4 (Мелкий)", "M200x3 (Мелкий)", "M200x2 (Мелкий)",
            //"M205x6 (Стандартный)", "M205x4 (Мелкий)", "M205x3 (Мелкий)",
            //"M210x8 (Стандартный)", "M210x6 (Мелкий)", "M210x4 (Мелкий)", "M210x3 (Мелкий)",
            //"M215x6 (Стандартный)", "M215x4 (Мелкий)", "M215x3 (Мелкий)",
            //"M220x8 (Стандартный)", "M220x6 (Мелкий)", "M220x4 (Мелкий)", "M220x3 (Мелкий)",
            //"M225x6 (Стандартный)", "M225x4 (Мелкий)", "M225x3 (Мелкий)",
            //"M230x8 (Стандартный)", "M230x6 (Мелкий)", "M230x4 (Мелкий)", "M230x3 (Мелкий)",
            //"M235x6 (Стандартный)", "M235x4 (Мелкий)", "M235x3 (Мелкий)",
            //"M240x8 (Стандартный)", "M240x6 (Мелкий)", "M240x4 (Мелкий)", "M240x3 (Мелкий)",
            //"M245x6 (Стандартный)", "M245x4 (Мелкий)", "M245x3 (Мелкий)",
            //"M250x8 (Стандартный)", "M250x6 (Мелкий)", "M250x4 (Мелкий)", "M250x3 (Мелкий)",
            //"M255x6 (Стандартный)", "M255x4 (Мелкий)", "M255x3 (Мелкий)",
            //"M260x8 (Стандартный)", "M260x6 (Мелкий)", "M260x4 (Мелкий)", "M260x3 (Мелкий)",
            //"M265x6 (Стандартный)", "M265x4 (Мелкий)", "M265x3 (Мелкий)",
            //"M270x8 (Стандартный)", "M270x6 (Мелкий)", "M270x4 (Мелкий)", "M270x3 (Мелкий)",
            //"M275x6 (Стандартный)", "M275x4 (Мелкий)", "M275x3 (Мелкий)",
            //"M280x8 (Стандартный)", "M280x6 (Мелкий)", "M280x4 (Мелкий)", "M280x3 (Мелкий)",
            //"M285x6 (Стандартный)", "M285x4 (Мелкий)", "M285x3 (Мелкий)",
            //"M290x8 (Стандартный)", "M290x6 (Мелкий)", "M290x4 (Мелкий)", "M290x3 (Мелкий)",
            //"M295x6 (Стандартный)", "M295x4 (Мелкий)", "M295x3 (Мелкий)",
            //"M300x8 (Стандартный)", "M300x6 (Мелкий)", "M300x4 (Мелкий)", "M300x3 (Мелкий)",
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
        public const string Bspp2 = "G1¾";
        public const string Bspp2N1d4 = "G2¼";
        public const string Bspp2N1d2 = "G2½";
        public const string Bspp2N3d4 = "G2¾";
        public const string Bspp3N = "G3";
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
            Bspp3N,
            Bspp3N1d4,
            Bspp3N1d2,
            Bspp3N3d4,
            Bspp4,
            Bspp4N1d2,
            Bspp5,
            Bspp5N1d2,
            Bspp6
        };

        public static readonly HashSet<string> TrapezoidalTemplates = new()
        {
            //"Tr8x2 (Стандартный)", "Tr8x1.5 (Мелкий)",
            //"Tr9x2 (Стандартный)", "Tr9x1.5 (Мелкий)",
            //"Tr10x2 (Стандартный)", "Tr10x1.5 (Мелкий)",
            //"Tr11x3 (Стандартный)", "Tr11x2 (Мелкий)",
            //"Tr12x3 (Стандартный)", "Tr12x2 (Мелкий)",
            //"Tr14x3 (Стандартный)", "Tr14x2 (Мелкий)",
            "Tr16x4 (Стандартный)",
            "Tr16x2 (Мелкий)",
            "Tr18x4 (Стандартный)",
            "Tr18x2 (Мелкий)",
            "Tr20x4 (Стандартный)",
            "Tr20x2 (Мелкий)",
            //"Tr22x8 (Стандартный)", "Tr22x5 (Мелкий)", "Tr22x3 (Мелкий)", "Tr22x2 (Мелкий)",
            "Tr24x8 (Стандартный)",
            "Tr24x5 (Мелкий)",
            "Tr24x3 (Мелкий)",
            "Tr24x2 (Мелкий)",
            //"Tr26x8 (Стандартный)", "Tr26x5 (Мелкий)", "Tr26x3 (Мелкий)", "Tr26x2 (Мелкий)",
            //"Tr28x8 (Стандартный)", "Tr28x5 (Мелкий)", "Tr28x3 (Мелкий)", "Tr28x2 (Мелкий)",
            "Tr30x10 (Стандартный)",
            "Tr30x6 (Мелкий)",
            "Tr30x3 (Мелкий)",
            //"Tr32x10 (Стандартный)", "Tr32x6 (Мелкий)", "Tr32x3 (Мелкий)",
            //"Tr34x10 (Стандартный)", "Tr34x6 (Мелкий)", "Tr34x3 (Мелкий)",
            //"Tr36x10 (Стандартный)", "Tr36x6 (Мелкий)", "Tr36x3 (Мелкий)",
            //"Tr38x10 (Стандартный)", "Tr38x7 (Мелкий)", "Tr38x6 (Мелкий)", "Tr38x3 (Мелкий)",
            "Tr40x10 (Стандартный)",
            "Tr40x7 (Мелкий)",
            "Tr40x6 (Мелкий)",
            "Tr40x3 (Мелкий)",
            //"Tr42x10 (Стандартный)", "Tr42x7 (Мелкий)", "Tr42x6 (Мелкий)", "Tr42x3 (Мелкий)",
            //"Tr44x12 (Стандартный)", "Tr44x8 (Мелкий)", "Tr44x7 (Мелкий)", "Tr44x3 (Мелкий)",
            //"Tr46x12 (Стандартный)", "Tr46x8 (Мелкий)", "Tr46x3 (Мелкий)",
            //"Tr48x12 (Стандартный)", "Tr48x8 (Мелкий)", "Tr48x3 (Мелкий)",
            "Tr50x12 (Стандартный)",
            "Tr50x8 (Мелкий)",
            "Tr50x3 (Мелкий)",
            //"Tr52x12 (Стандартный)", "Tr52x8 (Мелкий)", "Tr52x3 (Мелкий)",
            //"Tr55x14 (Стандартный)", "Tr55x12 (Мелкий)", "Tr55x9 (Мелкий)", "Tr55x8 (Мелкий)", "Tr55x3 (Мелкий)",
            "Tr60x14 (Стандартный)",
            "Tr60x12 (Мелкий)",
            "Tr60x9 (Мелкий)",
            "Tr60x8 (Мелкий)",
            "Tr60x3 (Мелкий)",
            //"Tr65x16 (Стандартный)", "Tr65x10 (Мелкий)", "Tr65x4 (Мелкий)",
            "Tr70x16 (Стандартный)",
            "Tr70x10 (Мелкий)",
            "Tr70x4 (Мелкий)",
            //"Tr75x16 (Стандартный)", "Tr75x10 (Мелкий)", "Tr75x4 (Мелкий)",
            "Tr80x16 (Стандартный)",
            "Tr80x10 (Мелкий)",
            "Tr80x4 (Мелкий)",
            //"Tr85x20 (Стандартный)", "Tr85x18 (Мелкий)", "Tr85x12 (Мелкий)", "Tr85x5 (Мелкий)", "Tr85x4 (Мелкий)",
            "Tr90x20 (Стандартный)",
            "Tr90x18 (Мелкий)",
            "Tr90x12 (Мелкий)",
            "Tr90x5 (Мелкий)",
            "Tr90x4 (Мелкий)",
            //"Tr95x20 (Стандартный)", "Tr95x18 (Мелкий)", "Tr95x12 (Мелкий)", "Tr95x5 (Мелкий)", "Tr95x4 (Мелкий)",
            "Tr100x20 (Стандартный)",
            "Tr100x12 (Мелкий)",
            "Tr100x5 (Мелкий)",
            "Tr100x4 (Мелкий)",
            //"Tr110x20 (Стандартный)", "Tr110x12 (Мелкий)", "Tr110x5 (Мелкий)", "Tr110x4 (Мелкий)",
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
        public const string Npt2 = "K1½";

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
            diameter = template.Split("M")[1].Split('x')[0];
            pitch = template.Split("M")[1].Split('x')[1].Split()[0];
        }

        public static void GetBsppValues(string template, out string diameter, out string pitch)
        {
            switch (template)
            {
                case "G1/16":
                    diameter = "7.723";
                    pitch = "0.907";
                    break;
                case "G1/8":
                    diameter = "9.728";
                    pitch = "0.907";
                    break;
                case "G1/4":
                    diameter = "13.157";
                    pitch = "1.337";
                    break;
                case "G3/8":
                    diameter = "16.662";
                    pitch = "1.337";
                    break;
                case "G1/2":
                    diameter = "20.995";
                    pitch = "1.814";
                    break;
                case "G5/8":
                    diameter = "22.911";
                    pitch = "1.814";
                    break;
                case "G3/4":
                    diameter = "26.441";
                    pitch = "1.814";
                    break;
                case "G7/8":
                    diameter = "30.201";
                    pitch = "1.814";
                    break;
                case "G1":
                    diameter = "33.249";
                    pitch = "2.309";
                    break;
                case "G1⅛":
                    diameter = "37.897";
                    pitch = "2.309";
                    break;
                case "G1¼":
                    diameter = "41.91";
                    pitch = "2.309";
                    break;
                case "G1⅜":
                    diameter = "44.323";
                    pitch = "2.309";
                    break;
                case "G1½":
                    diameter = "47.803";
                    pitch = "2.309";
                    break;
                case "G1¾":
                    diameter = "53.746";
                    pitch = "2.309";
                    break;
                case "G2":
                    diameter = "59.614";
                    pitch = "2.309";
                    break;
                case "G2¼":
                    diameter = "65.71";
                    pitch = "2.309";
                    break;
                case "G2½":
                    diameter = "75.184";
                    pitch = "2.309";
                    break;
                case "G2¾":
                    diameter = "81.534";
                    pitch = "2.309";
                    break;
                case "G3":
                    diameter = "87.884";
                    pitch = "2.309";
                    break;
                case "G3¼":
                    diameter = "93.98";
                    pitch = "2.309";
                    break;
                case "G3½":
                    diameter = "100.33";
                    pitch = "2.309";
                    break;
                case "G3¾":
                    diameter = "106.68";
                    pitch = "2.309";
                    break;
                case "G4":
                    diameter = "113.03";
                    pitch = "2.309";
                    break;
                case "G4½":
                    diameter = "125.73";
                    pitch = "2.309";
                    break;
                case "G5":
                    diameter = "138.43";
                    pitch = "2.309";
                    break;
                case "G5½":
                    diameter = "151.13";
                    pitch = "2.309";
                    break;
                case "G6":
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
                case "K1/16":
                    externalDiameter = "7.895";
                    internalDiameter = "6.389";
                    pitch = "0.941";
                    planeLength = 4.064;
                    threadLength = 6.5;
                    break;
                case "K1/8":
                    externalDiameter = "10.272";
                    internalDiameter = "8.766";
                    pitch = "0.941";
                    planeLength = 4.572;
                    threadLength = 7;
                    break;
                case "K1/4":
                    externalDiameter = "13.572";
                    internalDiameter = "11.314";
                    pitch = "1.411";
                    planeLength = 5.08;
                    threadLength = 9.5;
                    break;
                case "K3/8":
                    externalDiameter = "17.055";
                    internalDiameter = "14.797";
                    pitch = "1.411";
                    planeLength = 6.096;
                    threadLength = 10.5;
                    break;
                case "K1/2":
                    externalDiameter = "21.223";
                    internalDiameter = "18.321";
                    pitch = "1.814";
                    planeLength = 8.128;
                    threadLength = 13.5;
                    break;
                case "K3/4":
                    externalDiameter = "26.568";
                    internalDiameter = "23.666";
                    pitch = "1.814";
                    planeLength = 8.611;
                    threadLength = 14;
                    break;
                case "K1":
                    externalDiameter = "33.228";
                    internalDiameter = "29.694";
                    pitch = "2.209";
                    planeLength = 10.16;
                    threadLength = 17.5;
                    break;
                case "K1¼":
                    externalDiameter = "41.985";
                    internalDiameter = "38.451";
                    pitch = "2.209";
                    planeLength = 10.668;
                    threadLength = 18;
                    break;
                case "K1½":
                    externalDiameter = "48.054";
                    internalDiameter = "44.52";
                    pitch = "2.209";
                    planeLength = 10.668;
                    threadLength = 18.5;
                    break;
                case "K2":
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
                ThreadStandard.Metric => Math.Sqrt(3) / 2 * threadPitch,
                ThreadStandard.BSPP => 0.960491 * threadPitch,
                ThreadStandard.Trapezoidal => 1.866 * threadPitch,
                ThreadStandard.NPT => 0.866 * threadPitch,
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
                ThreadStandard.Metric => type == CuttingType.External
                ? (17.0 / 24.0 * NominalHeight(threadStandard, threadPitch))
                : (5.0 / 8.0 * NominalHeight(threadStandard, threadPitch)),
                ThreadStandard.BSPP => 0.640327 * threadPitch,
                ThreadStandard.Trapezoidal => 0.5 * threadPitch + TrapezoidalClearance(threadPitch),
                ThreadStandard.NPT => 0.8 * threadPitch,
                _ => 0,
            };
        }

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
            if (threadStandard == ThreadStandard.Metric)
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
            else if (threadStandard == ThreadStandard.BSPP)
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
            else if (threadStandard == ThreadStandard.Trapezoidal)
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
            else if (threadStandard == ThreadStandard.NPT)
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
                case ThreadStandard.BSPP:
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
                case ThreadStandard.NPT:
                    return threadPitch switch
                    {
                        <= 0.941 => // 27
                            1,
                        <= 1.814 => // 14
                            1.6,
                        > 1.814 => //>14
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
                case ThreadStandard.BSPP:
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
                    return threadPitch switch
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
                    };
                case ThreadStandard.Trapezoidal:
                    return 0;
                default:
                    return 0;
            }
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
                ThreadStandard.Metric => "60",
                ThreadStandard.BSPP => "55",
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
                ThreadStandard.Metric => type == CuttingType.External
                ? threadDiameter + 1
                : threadDiameter - threadPitch - 1,
                ThreadStandard.BSPP => type == CuttingType.External
                ? threadDiameter + 1
                : threadDiameter - threadPitch - 1,
                ThreadStandard.Trapezoidal => type == CuttingType.External
                ? threadDiameter + 1
                : threadDiameter - threadPitch - 1,
                ThreadStandard.NPT => type == CuttingType.External
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
                ThreadStandard.Metric => type == CuttingType.External
                ? threadDiameter - CalculatedProfile(threadStandard, type, threadPitch)
                : threadDiameter + threadPitch / 16 / Math.Sin(60.Radians()),
                ThreadStandard.BSPP => type == CuttingType.External
                ? threadDiameter - CalculatedProfile(threadStandard, type, threadPitch)
                : threadDiameter,
                ThreadStandard.Trapezoidal => type == CuttingType.External
                ? threadDiameter - CalculatedProfile(threadStandard, type, threadPitch)
                : threadDiameter + 2 * TrapezoidalClearance(threadPitch),
                ThreadStandard.NPT => type == CuttingType.External
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
    }
}
