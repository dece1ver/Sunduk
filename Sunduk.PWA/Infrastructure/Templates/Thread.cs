using Sunduk.PWA.Infrastructure.Sequences;
using Sunduk.PWA.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Sunduk.PWA.Dialogs.SequenceDialog;

namespace Sunduk.PWA.Infrastructure.Templates
{
    public static class Thread
    {
        public enum PassesOption { FullPasses, Infeed }

        /// <summary>
        /// Считает количество проходов при нарезании резьбы
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

        public static readonly IEnumerable<string> metricTemplates = new HashSet<string> {
        //"M2x0.4 (Стандартный)", "M2x0.25 (Мелкий)",
        //"M2.5x0.45 (Стандартный)", "M2.5x0.35 (Мелкий)",
        //"M3x0.5 (Стандартный)", "M3x0.35 (Мелкий)",
        //"M3.5x0.6 (Стандартный)", "M3.5x0.35 (Мелкий)",
        //"M4x0.7 (Стандартный)", "M4x0.5 (Мелкий)", "M4.5x0.75", "M4.5x0.5 (Мелкий)",
        //"M5x0.8 (Стандартный)", "M5x0.5 (Мелкий)", "M5.5x0.5 (Стандартный)",
        "M6x1 (Стандартный)", "M6x0.75 (Мелкий)", "M6x0.5 (Мелкий)",
        "M7x1 (Стандартный)", "M7x0.75 (Мелкий)", "M7x0.5 (Мелкий)",
        "M8x1.25 (Стандартный)", "M8x1 (Мелкий)", "M8x0.75 (Мелкий)", "M8x0.5 (Мелкий)",
        "M9x1.25 (Стандартный)", "M9x1 (Мелкий)", "M9x0.75 (Мелкий)", "M9x0.5 (Мелкий)",
        "M10x1.5 (Стандартный)", "M10x1.25 (Мелкий)", "M10x1 (Мелкий)", "M10x0.75 (Мелкий)", "M10x0.5 (Мелкий)",
        "M11x1.5 (Стандартный)", "M11x1 (Мелкий)", "M11x0.75 (Мелкий)", "M11x0.5 (Мелкий)",
        "M12x1.75 (Стандартный)", "M12x1.5 (Мелкий)", "M12x1.25 (Мелкий)", "M12x1 (Мелкий)", "M12x0.75 (Мелкий)", "M12x0.5 (Мелкий)",
        "M14x2 (Стандартный)", "M14x1.5 (Мелкий)", "M14x1.25 (Мелкий)", "M14x1 (Мелкий)", "M14x0.75 (Мелкий)", "M14x0.5 (Мелкий)",
        "M15x1.5 (Стандартный)", "M15x1 (Мелкий)",
        "M16x2 (Стандартный)", "M16x1.5 (Мелкий)", "M16x1 (Мелкий)", "M16x0.75 (Мелкий)", "M16x0.5 (Мелкий)",
        "M17x1.5 (Стандартный)", "M17x1 (Мелкий)",
        "M18x2.5 (Стандартный)", "M18x2 (Мелкий)", "M18x1.5 (Мелкий)", "M18x1 (Мелкий)", "M18x0.75 (Мелкий)", "M18x0.5 (Мелкий)",
        "M20x2.5 (Стандартный)", "M20x2 (Мелкий)", "M20x1.5 (Мелкий)", "M20x1 (Мелкий)", "M20x0.75 (Мелкий)", "M20x0.5 (Мелкий)",
        "M22x2.5 (Стандартный)", "M22x2 (Мелкий)", "M22x1.5 (Мелкий)", "M22x1 (Мелкий)", "M22x0.75 (Мелкий)", "M22x0.5 (Мелкий)",
        "M24x3 (Стандартный)", "M24x2 (Мелкий)", "M24x1.5 (Мелкий)", "M24x1 (Мелкий)", "M24x0.75 (Мелкий)",
        "M25x2 (Стандартный)", "M25x1.5 (Мелкий)", "M25x1 (Мелкий)",
        "M26x1.5 (Стандартный)",
        "M27x3 (Стандартный)", "M27x2 (Мелкий)", "M27x1.5 (Мелкий)", "M27x1 (Мелкий)", "M27x0.75 (Мелкий)",
        "M27x2 (Стандартный)", "M27x1.5 (Мелкий)", "M27x1 (Мелкий)",
        "M30x3.5 (Стандартный)", "M30x3 (Мелкий)", "M30x2 (Мелкий)", "M30x1.5 (Мелкий)", "M30x1 (Мелкий)", "M30x0.75 (Мелкий)",
        "M32x2 (Стандартный)", "M32x1.5 (Мелкий)",
        "M33x3.5 (Стандартный)", "M33x3 (Мелкий)", "M33x2 (Мелкий)", "M33x1.5 (Мелкий)", "M33x1 (Мелкий)", "M33x0.75 (Мелкий)",
        "M35x1.5 (Стандартный)",
        "M36x4 (Стандартный)", "M36x3 (Мелкий)", "M36x2 (Мелкий)", "M36x1.5 (Мелкий)", "M36x1 (Мелкий)",
        "M38x1.5 (Стандартный)",
        "M39x4 (Стандартный)", "M39x3 (Мелкий)", "M39x2 (Мелкий)", "M39x1.5 (Мелкий)", "M39x1 (Мелкий)",
        "M40x3 (Стандартный)", "M40x2 (Мелкий)", "M40x1.5 (Мелкий)",
        //"M42x4.5 (Стандартный)", "M42x4 (Мелкий)", "M42x3 (Мелкий)", "M42x2 (Мелкий)", "M42x1.5 (Мелкий)", "M42x1 (Мелкий)",
        //"M45x4.5 (Стандартный)", "M45x4 (Мелкий)", "M45x3 (Мелкий)", "M45x2 (Мелкий)", "M45x1.5 (Мелкий)", "M45x1 (Мелкий)",
        //"M48x5 (Стандартный)", "M48x4 (Мелкий)", "M48x3 (Мелкий)", "M48x2 (Мелкий)", "M48x1.5 (Мелкий)", "M48x1 (Мелкий)",
        "M50x3 (Стандартный)", "M50x2 (Мелкий)", "M50x1.5 (Мелкий)",
        //"M52x5 (Стандартный)", "M52x4 (Мелкий)", "M52x3 (Мелкий)", "M52x2 (Мелкий)", "M52x1.5 (Мелкий)", "M52x1 (Мелкий)",
        //"M55x4 (Стандартный)", "M55x3 (Мелкий)", "M55x2 (Мелкий)", "M55x1.5 (Мелкий)",
        //"M56x5.5 (Стандартный)", "M56x4 (Мелкий)", "M56x3 (Мелкий)", "M56x2 (Мелкий)", "M56x1.5 (Мелкий)", "M56x1 (Мелкий)",
        //"M58x4 (Стандартный)", "M58x3 (Мелкий)", "M58x2 (Мелкий)", "M58x1.5 (Мелкий)",
        "M60x5.5 (Стандартный)", "M60x4 (Мелкий)", "M60x3 (Мелкий)", "M60x2 (Мелкий)", "M60x1.5 (Мелкий)", "M60x1 (Мелкий)",
        //"M62x4 (Стандартный)", "M62x3 (Мелкий)", "M62x2 (Мелкий)", "M62x1.5 (Мелкий)",
        //"M64x6 (Стандартный)", "M64x4 (Мелкий)", "M64x3 (Мелкий)", "M64x2 (Мелкий)", "M64x1.5 (Мелкий)", "M64x1 (Мелкий)",
        //"M65x4 (Стандартный)", "M65x3 (Мелкий)", "M65x2 (Мелкий)", "M65x1.5 (Мелкий)",
        //"M68x6 (Стандартный)", "M68x4 (Мелкий)", "M68x3 (Мелкий)", "M68x2 (Мелкий)", "M68x1.5 (Мелкий)", "M68x1 (Мелкий)",
        "M70x6 (Стандартный)", "M70x4 (Мелкий)", "M70x3 (Мелкий)", "M70x2 (Мелкий)", "M70x1.5 (Мелкий)",
        //"M72x6 (Стандартный)", "M72x4 (Мелкий)", "M72x3 (Мелкий)", "M72x2 (Мелкий)", "M72x1.5 (Мелкий)", "M72x1 (Мелкий)",
        //"M75x4 (Стандартный)", "M75x3 (Мелкий)", "M75x2 (Мелкий)", "M75x1.5 (Мелкий)",
        //"M76x6 (Стандартный)", "M76x4 (Мелкий)", "M76x3 (Мелкий)", "M76x2 (Мелкий)", "M76x1.5 (Мелкий)", "M76x1 (Мелкий)",
        //"M78x2 (Стандартный)",
        "M80x6 (Стандартный)", "M80x4 (Мелкий)", "M80x3 (Мелкий)", "M80x2 (Мелкий)", "M80x1.5 (Мелкий)", "M80x1 (Мелкий)",
        //"M82x2 (Стандартный)",
        //"M85x6 (Стандартный)", "M85x4 (Мелкий)", "M85x3 (Мелкий)", "M85x2 (Мелкий)", "M85x1.5 (Мелкий)",
        "M90x6 (Стандартный)", "M90x4 (Мелкий)", "M90x3 (Мелкий)", "M90x2 (Мелкий)", "M90x1.5 (Мелкий)",
        //"M95x6 (Стандартный)", "M95x4 (Мелкий)", "M95x3 (Мелкий)", "M95x2 (Мелкий)", "M95x1.5 (Мелкий)",
        "M100x6 (Стандартный)", "M100x4 (Мелкий)", "M100x3 (Мелкий)", "M100x2 (Мелкий)", "M100x1.5 (Мелкий)",
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

        public static void GetMetricValues(string template, out string diameter, out string pitch)
        {
            diameter = template.TrimStart('M').Split('x')[0];
            pitch = template.TrimStart('M').Split('x')[1].Split()[0];
        }

        public static readonly IEnumerable<string> bsppTemplates = new HashSet<string> {
            "G1/16", "G1/8", "G1/4", "G3/8", "G1/2", "G5/8", "G3/4", "G7/8",
            "G1", "G1⅛", "G1¼", "G1⅜", "G1½", "G1¾",
            "G2", "G2¼", "G2½", "G2¾",
            "G3", "G3¼", "G3½", "G3¾",
            "G4", "G4½",
            "G5", "G5½",
            "G6"
        };

        public static void GetBSPPValues(string template, out string diameter, out string pitch)
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

        public static double NominalHeight(ThreadStandart threadStandart, double threadPitch)
        {
            if (threadStandart == ThreadStandart.Metric)
            {
                return Math.Sqrt(3) / 2 * threadPitch;
            }
            else
            {
                return 0.960491 * threadPitch;
            }
        }

        public static double ProfileHeight(ThreadStandart threadStandart, CuttingType type, double threadPitch)
        {

            if (threadStandart == ThreadStandart.Metric)
            {
                return type == CuttingType.External ? ((17.0 / 24.0) * NominalHeight(threadStandart, threadPitch)) : ((5.0 / 8.0) * NominalHeight(threadStandart, threadPitch));
            }
            else
            {
                return 0.640327 * threadPitch;
            }

        }

        public static double Angle(double threadDiameter, double threadPitch)
        {
            double result = Math.Atan(threadPitch / (threadDiameter * Math.PI));
            result = result / Math.PI * 180;
            return result;
        }

        public static int PassesCount(double threadPitch)
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
        } // написать зависимость от шага

        public static double[] Passes(ThreadStandart threadStandart, CuttingType type, double threadPitch) 
            => CalcPasses(Thread.ProfileHeight(threadStandart, type, threadPitch), Thread.PassesCount(threadPitch), PassesOption.Infeed);
        public static double[] TotalPasses(ThreadStandart threadStandart, CuttingType type, double threadPitch) 
            => CalcPasses(Thread.ProfileHeight(threadStandart, type, threadPitch), Thread.PassesCount(threadPitch), PassesOption.FullPasses);

        public static string Profile(ThreadStandart threadStandart) => threadStandart == ThreadStandart.BSPP ? "55" : "60";

        public static string ApproachDiameter(CuttingType type, double threadDiameter, double threadPitch)
        {
            if (type == CuttingType.External)
            {
                return (threadDiameter + 2).NC(1);
            }
            else if (type == CuttingType.Internal)
            {
                return (threadDiameter - threadPitch - 1).NC(1);
            }
            else
            {
                return string.Empty;
            }

        }

        public static string EndDiameter(ThreadStandart threadStandart, CuttingType type, double threadDiameter, double threadPitch) 
            => type == CuttingType.External ? (threadDiameter - (2 * Passes(threadStandart, type, threadPitch).Sum())).NC() : threadDiameter.NC();

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
