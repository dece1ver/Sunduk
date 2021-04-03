using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.Util
{
    public static class Util
    {
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
    }

    
}
