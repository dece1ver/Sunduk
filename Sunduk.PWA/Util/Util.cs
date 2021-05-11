using System;
using System.Globalization;

namespace Sunduk.PWA.Util
{
    public static class Util
    {
        public enum PassesOption { FullPasses, Infeed }

        public static double GetDouble(string stringNumber, double defaultValue = 0)
        {
            NumberFormatInfo numberFomat = new() { NumberDecimalSeparator = "," };
            if (Double.TryParse(stringNumber, NumberStyles.Any, numberFomat, out double result))
            {
                return result;
            }
            return defaultValue;
        }
        public static int GetInt(string stringNumber, int defaultValue = 0)
        {
            NumberFormatInfo numberFomat = new() { NumberDecimalSeparator = "," };
            if (Int32.TryParse(stringNumber, NumberStyles.Any, numberFomat, out int result))
            {
                return result;
            }
            return defaultValue;
        }

        public static double Degrees(double radians)
        {
            return radians * 180 / Math.PI;
        }

        public static double Radians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        public static string NCFormat(double value, int precision = 3)
        {
            return value.ToString($"F{precision}").Replace(",", ".").TrimEnd('0');
        }

        public static string ToPrettyString(double value, int precision = 3)
        {
            return value.ToString($"F{precision}").Replace(",", ".").TrimEnd('0').TrimEnd('.');
        }

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
    }


}
