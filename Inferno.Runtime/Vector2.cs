using System;
using System.Diagnostics.CodeAnalysis;

namespace Inferno.Runtime
{
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

        /// <summary>
        /// Temporary conversion for phase 1 compatibility
        /// </summary>
        internal Microsoft.Xna.Framework.Vector2 Monogame => new Microsoft.Xna.Framework.Vector2(X, Y);

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

        /// <summary>
        /// Temporary conversion for phase 1 compatibility
        /// </summary>
        /// <param name="v"></param>
        internal Vector2(Microsoft.Xna.Framework.Vector2 v)
        {
            X = v.X;
            Y = v.Y;
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

        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
        public static bool operator ==(Vector2 a, Vector2 b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
        public static bool operator !=(Vector2 a, Vector2 b)
        {
            return a.X != b.X || a.Y != b.Y;
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

        #endregion
    }
}
