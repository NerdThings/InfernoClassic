using System;

namespace Inferno.Runtime.Extensions
{
    public static class MathExtensions
    {
        public static int NearestMultiple(this int number, int multiple)
        {
            return (int)Math.Round(
                         (number / (double)multiple),
                         MidpointRounding.AwayFromZero
                     ) * multiple;
        }

        public static double NearestMultiple(this double number, double multiple)
        {
            return Math.Round(
                         (number / multiple),
                         MidpointRounding.AwayFromZero
                     ) * multiple;
        }
    }
}
