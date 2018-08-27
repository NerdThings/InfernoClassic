using System;
using System.Diagnostics.CodeAnalysis;

namespace Inferno.Runtime.Graphics
{
    /// <summary>
    /// A 32-bit packed color.
    /// </summary>
    public struct Color : IEquatable<Color>
    {
        //TODO: Comments
        public uint PackedValue;

        public Color(uint packedValue)
        {
            PackedValue = packedValue;
        }

        public Color(Color color, int alpha)
        {
            if ((alpha & 0xFFFFFF00) != 0)
            {
                var clampedA = (uint)MathHelper.Clamp(alpha, Byte.MinValue, Byte.MaxValue);

                PackedValue = (color.PackedValue & 0x00FFFFFF) | (clampedA << 24);
            }
            else
            {
                PackedValue = (color.PackedValue & 0x00FFFFFF) | ((uint)alpha << 24);
            }
        }

        public Color(Color color, float alpha) :
            this(color, (int)(alpha * 255))
        {
        }

        public Color(float r, float g, float b)
            : this((int)(r * 255), (int)(g * 255), (int)(b * 255))
        {
        }

        public Color(float r, float g, float b, float alpha)
            : this((int)(r * 255), (int)(g * 255), (int)(b * 255), (int)(alpha * 255))
        {
        }

        public Color(int r, int g, int b)
        {
            PackedValue = 0xFF000000; // A = 255

            if (((r | g | b) & 0xFFFFFF00) != 0)
            {
                var clampedR = (uint)MathHelper.Clamp(r, byte.MinValue, byte.MaxValue);
                var clampedG = (uint)MathHelper.Clamp(g, byte.MinValue, byte.MaxValue);
                var clampedB = (uint)MathHelper.Clamp(b, byte.MinValue, byte.MaxValue);

                PackedValue |= (clampedB << 16) | (clampedG << 8) | (clampedR);
            }
            else
            {
                PackedValue |= ((uint)b << 16) | ((uint)g << 8) | ((uint)r);
            }
        }

        public Color(int r, int g, int b, int alpha)
        {
            if (((r | g | b | alpha) & 0xFFFFFF00) != 0)
            {
                var clampedR = (uint)MathHelper.Clamp(r, Byte.MinValue, Byte.MaxValue);
                var clampedG = (uint)MathHelper.Clamp(g, Byte.MinValue, Byte.MaxValue);
                var clampedB = (uint)MathHelper.Clamp(b, Byte.MinValue, Byte.MaxValue);
                var clampedA = (uint)MathHelper.Clamp(alpha, Byte.MinValue, Byte.MaxValue);

                PackedValue = (clampedA << 24) | (clampedB << 16) | (clampedG << 8) | (clampedR);
            }
            else
            {
                PackedValue = ((uint)alpha << 24) | ((uint)b << 16) | ((uint)g << 8) | ((uint)r);
            }
        }

        public Color(byte r, byte g, byte b, byte alpha)
        {
            PackedValue = ((uint)alpha << 24) | ((uint)b << 16) | ((uint)g << 8) | (r);
        }

        public byte B
        {
            get
            {
                unchecked
                {
                    return (byte)(PackedValue >> 16);
                }
            }
            set => PackedValue = (PackedValue & 0xff00ffff) | ((uint)value << 16);
        }

        public byte G
        {
            get
            {
                unchecked
                {
                    return (byte)(PackedValue >> 8);
                }
            }
            set => PackedValue = (PackedValue & 0xffff00ff) | ((uint)value << 8);
        }

        public byte R
        {
            get
            {
                unchecked
                {
                    return (byte)PackedValue;
                }
            }
            set => PackedValue = (PackedValue & 0xffffff00) | value;
        }

        public byte A
        {
            get
            {
                unchecked
                {
                    return (byte)(PackedValue >> 24);
                }
            }
            set => PackedValue = (PackedValue & 0x00ffffff) | ((uint)value << 24);
        }

        public static bool operator ==(Color a, Color b)
        {
            return (a.PackedValue == b.PackedValue);
        }

        public static bool operator !=(Color a, Color b)
        {
            return (a.PackedValue != b.PackedValue);
        }

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            return PackedValue.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return ((obj is Color color) && Equals(color));
        }

        public static Color Lerp(Color value1, Color value2, int amount)
        {
            amount = MathHelper.Clamp(amount, 0, 1);
            return new Color(
                (int)MathHelper.Lerp(value1.R, value2.R, amount),
                (int)MathHelper.Lerp(value1.G, value2.G, amount),
                (int)MathHelper.Lerp(value1.B, value2.B, amount),
                (int)MathHelper.Lerp(value1.A, value2.A, amount));
        }

        public static Color operator *(Color value, float scale)
        {
            return new Color((int)(value.R * scale), (int)(value.G * scale), (int)(value.B * scale), (int)(value.A * scale));
        }

        public static Color FromNonPremultiplied(int r, int g, int b, int a)
        {
            return new Color(r * a / 255, g * a / 255, b * a / 255, a);
        }

        #region Operators

        public bool Equals(Color other)
        {
            return PackedValue == other.PackedValue;
        }

        public override string ToString()
        {
            return "{ R:"+ R + " , G:" + G + " , B:" + B + " , A:" + A + " }";
        }

        #endregion

        #region Known Colors

        public static Color Black => new Color(0, 0, 0, 255);
        public static Color Blue => new Color(0, 0, 255, 255);
        public static Color Transparent => new Color(0, 0, 0, 0);
        public static Color Red => new Color(255, 0, 0, 255);
        public static Color White => new Color(255, 255,255,255);

        //TODO: Other colors

        #endregion
    }
}