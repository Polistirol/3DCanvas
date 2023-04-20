using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserLibrary.Models.Media
{
    public static class M3DUtil
    {
        internal static double RadiansToDegrees(double radians)
        {
            return radians * (180.0 / Math.PI);
        }

        internal static double DegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180.0);
        }

        public static bool IsOne(double value)
        {
            return Math.Abs(value - 1.0) < 2.2204460492503131E-15;
        }
    }
}
