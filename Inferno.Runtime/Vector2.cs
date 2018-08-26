using System;

namespace Inferno.Runtime
{
    public struct Vector2 : IEquatable<Vector2>
    {
        public float X;
        public float Y;

        public float Magnitude => (float)Math.Sqrt((X * X) + (Y * Y));

        //Temp, while phase 2 is not done
        internal Microsoft.Xna.Framework.Vector2 Monogame => new Microsoft.Xna.Framework.Vector2(X, Y);

        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        internal Vector2(Microsoft.Xna.Framework.Vector2 v)
        {
            X = v.X;
            Y = v.Y;
        }

        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2()
            {
                X = a.X + b.X,
                Y = a.Y + b.Y
            };
        }

        public static Vector2 operator +(Vector2 a, int b)
        {
            return new Vector2()
            {
                X = a.X + b,
                Y = a.Y + b
            };
        }

        public static Vector2 operator +(Vector2 a, float b)
        {
            return new Vector2()
            {
                X = a.X + b,
                Y = a.Y + b
            };
        }

        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2()
            {
                X = a.X - b.X,
                Y = a.Y - b.Y
            };
        }

        public static Vector2 operator -(Vector2 a, int b)
        {
            return new Vector2()
            {
                X = a.X - b,
                Y = a.Y - b
            };
        }

        public static Vector2 operator -(Vector2 a, float b)
        {
            return new Vector2()
            {
                X = a.X - b,
                Y = a.Y - b
            };
        }

        public static Vector2 operator *(Vector2 a, Vector2 b)
        {
            return new Vector2()
            {
                X = a.X * b.X,
                Y = a.Y * b.Y
            };
        }

        public static Vector2 operator *(Vector2 a, int b)
        {
            return new Vector2()
            {
                X = a.X * b,
                Y = a.Y * b
            };
        }

        public static Vector2 operator *(Vector2 a, float b)
        {
            return new Vector2()
            {
                X = a.X * b,
                Y = a.Y * b
            };
        }

        public static Vector2 operator /(Vector2 a, Vector2 b)
        {
            return new Vector2()
            {
                X = a.X / b.X,
                Y = a.Y / b.Y
            };
        }

        public static Vector2 operator /(Vector2 a, int b)
        {
            return new Vector2()
            {
                X = a.X / b,
                Y = a.Y / b
            };
        }

        public static Vector2 operator /(Vector2 a, float b)
        {
            return new Vector2()
            {
                X = a.X / b,
                Y = a.Y / b
            };
        }

        public bool Equals(Vector2 other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector2 newtmp)
            {
                return Equals(newtmp);
            }

            return false;
        }

        public static bool operator ==(Vector2 a, Vector2 b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Vector2 a, Vector2 b)
        {
            return a.X != b.X || a.Y != b.Y;
        }

        public static float Dot(Vector2 a, Vector2 b)
        {
            return (a.X * b.X) + (a.Y * b.Y);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() + Y.GetHashCode();
        }

        public override string ToString()
        {
            return "{ X:" + X + " , Y:" + Y + " }";
        }
    }
}
