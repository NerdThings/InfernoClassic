using System;
using System.Diagnostics.CodeAnalysis;

namespace Inferno.Runtime
{
    /// <summary>
    /// A Vector with two (X and Y) components.
    /// </summary>
    public struct Vector2 : IEquatable<Vector2>
    {
        #region Fields

        /// <summary>
        /// The X component of the vector.
        /// </summary>
        public float X;

        /// <summary>
        /// The Y component of the vector
        /// </summary>
        public float Y;

        #endregion

        #region Properties

        /// <summary>
        /// The magnitude of the vector.
        /// </summary>
        public float Magnitude => (float)Math.Sqrt((X * X) + (Y * Y));

        public static Vector2 Zero = new Vector2(0, 0);
        public static Vector2 UnitX = new Vector2(1, 0);
        public static Vector2 UnitY = new Vector2(0, 1);

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new vector2.
        /// </summary>
        /// <param name="x">X component</param>
        /// <param name="y">Y component</param>
        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Vector2(float value)
        {
            X = value;
            Y = value;
        }

        #endregion

        #region Operators

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

        public static Vector2 operator +(int b, Vector2 a)
        {
            return new Vector2()
            {
                X = a.X + b,
                Y = a.Y + b
            };
        }

        public static Vector2 operator +(float b, Vector2 a)
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

        public static Vector2 operator -(int b, Vector2 a)
        {
            return new Vector2()
            {
                X = a.X - b,
                Y = a.Y - b
            };
        }

        public static Vector2 operator -(float b, Vector2 a)
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

        public static Vector2 operator *(int b, Vector2 a)
        {
            return new Vector2()
            {
                X = a.X * b,
                Y = a.Y * b
            };
        }

        public static Vector2 operator *(float b, Vector2 a)
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

        public static Vector2 operator /(int b, Vector2 a)
        {
            return new Vector2()
            {
                X = a.X / b,
                Y = a.Y / b
            };
        }

        public static Vector2 operator /(float b, Vector2 a)
        {
            return new Vector2()
            {
                X = a.X / b,
                Y = a.Y / b
            };
        }

        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
        public bool Equals(Vector2 other)
        {
            return X == other.X && Y == other.Y;
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
            return a.Equals(b);
        }

        public static bool operator !=(Vector2 a, Vector2 b)
        {
            return !a.Equals(b);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the dot product of 2 vectors.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float Dot(Vector2 a, Vector2 b)
        {
            return (a.X * b.X) + (a.Y * b.Y);
        }

        public float Dot(Vector2 b)
        {
            return (X * b.X) + (Y * b.Y);
        }

        public static float Distance(Vector2 a, Vector2 b)
        {
            return a.Distance(b);
        }

        public float Distance(Vector2 to)
        {
            return (float) Math.Sqrt((X - to.X) * (X - to.X) + (Y - to.Y) * (Y - to.Y));
        }

        public static float Length(Vector2 vector)
        {
            return vector.Length();
        }

        public float Length()
        {
            return (float)Math.Sqrt(X * X + Y * Y);
        }

        public static float LengthSquared(Vector2 vector)
        {
            return vector.LengthSquared();
        }

        public float LengthSquared()
        {
            return X * X + Y * Y;
        }

        #endregion

        #region Other

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            return X.GetHashCode() + Y.GetHashCode();
        }

        public override string ToString()
        {
            return "{ X:" + X + " , Y:" + Y + " }";
        }

        public static Vector2 Transform(Vector2 position, Matrix matrix)
        {
            return new Vector2
            {
                X = (position.X * matrix.M11) + (position.Y * matrix.M21) + matrix.M41,
                Y = (position.X * matrix.M12) + (position.Y * matrix.M22) + matrix.M42
            };
        }

        #endregion
    }
}
