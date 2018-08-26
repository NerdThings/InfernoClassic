using System;
using System.Collections.Generic;
using System.Text;

namespace Inferno.Runtime
{
    public static class MathHelper
    {
        public static int NearestMultiple(int number, int multiple)
        {
            return (int)Math.Round(
                       (number / (double)multiple),
                       MidpointRounding.AwayFromZero
                   ) * multiple;
        }

        public static double NearestMultiple(double number, double multiple)
        {
            return Math.Round(
                       (number / multiple),
                       MidpointRounding.AwayFromZero
                   ) * multiple;
        }

        public static float Clamp(float value, float min, float max)
        {
            value = (value > max) ? max : value;
            value = (value < min) ? min : value;
            return value;
        }

        public static int Clamp(int value, int min, int max)
        {
            value = (value > max) ? max : value;
            value = (value < min) ? min : value;
            return value;
        }

        public static float Distance(float value1, float value2)
        {
            return Math.Abs(value1 - value2);
        }

        public static float Lerp(float value1, float value2, float amount)
        {
            return value1 + (value2 - value1) * amount;
        }

        public static float LerpPrecise(float value1, float value2, float amount)
        {
            return ((1 - amount) * value1) + (value2 * amount);
        }
    }
}
