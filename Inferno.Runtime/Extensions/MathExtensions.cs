using System;
using System.Collections.Generic;
using System.Text;

namespace Inferno.Runtime.Extensions
{
    public static class MathExtensions
    {
        public static int NearestMultiple(this int number, int multiple)
        {
            return (int)System.Math.Round(
                         (number / (double)multiple),
                         MidpointRounding.AwayFromZero
                     ) * multiple;
        }

        public static double NearestMultiple(this double number, double multiple)
        {
            return System.Math.Round(
                         (number / (double)multiple),
                         MidpointRounding.AwayFromZero
                     ) * multiple;
        }
    }
}
