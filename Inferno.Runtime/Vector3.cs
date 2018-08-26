using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Inferno.Runtime
{
    public struct Vector3
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

        /// <summary>
        /// The Z component of the vector
        /// </summary>
        public float Z;

        #endregion

        #region Properties

        /// <summary>
        /// The magnitude of the vector.
        /// </summary>
        public float Magnitude => (float)Math.Sqrt((X * X) + (Y * Y) + (Z*Z));

        /// <summary>
        /// Temporary conversion for phase 1 compatibility
        /// </summary>
        internal Microsoft.Xna.Framework.Vector3 Monogame => new Microsoft.Xna.Framework.Vector3(X, Y, Z);

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new vector2.
        /// </summary>
        /// <param name="x">X component</param>
        /// <param name="y">Y component</param>
        /// <param name="z">Z component</param>
        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Temporary conversion for phase 1 compatibility
        /// </summary>
        /// <param name="v"></param>
        internal Vector3(Microsoft.Xna.Framework.Vector3 v)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
        }

        public Vector3(Vector2 value, float z)
        {
            X = value.X;
            Y = value.Y;
            Z = z;
        }

        #endregion

        #region Operators

        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3()
            {
                X = a.X + b.X,
                Y = a.Y + b.Y,
                Z = a.Z + b.Z
            };
        }

        public static Vector3 operator +(Vector3 a, int b)
        {
            return new Vector3()
            {
                X = a.X + b,
                Y = a.Y + b,
                Z = a.Z + b
            };
        }

        public static Vector3 operator +(Vector3 a, float b)
        {
            return new Vector3()
            {
                X = a.X + b,
                Y = a.Y + b,
                Z = a.Z + b
            };
        }

        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3()
            {
                X = a.X - b.X,
                Y = a.Y - b.Y,
                Z = a.Z - b.Z
            };
        }

        public static Vector3 operator -(Vector3 a, int b)
        {
            return new Vector3()
            {
                X = a.X - b,
                Y = a.Y - b,
                Z = a.Z - b
            };
        }

        public static Vector3 operator -(Vector3 a, float b)
        {
            return new Vector3()
            {
                X = a.X - b,
                Y = a.Y - b,
                Z = a.Z - b
            };
        }

        public static Vector3 operator *(Vector3 a, Vector3 b)
        {
            return new Vector3()
            {
                X = a.X * b.X,
                Y = a.Y * b.Y,
                Z = a.Z * b.Z
            };
        }

        public static Vector3 operator *(Vector3 a, int b)
        {
            return new Vector3()
            {
                X = a.X * b,
                Y = a.Y * b,
                Z = a.Z * b
            };
        }

        public static Vector3 operator *(Vector3 a, float b)
        {
            return new Vector3()
            {
                X = a.X * b,
                Y = a.Y * b,
                Z = a.Z * b
            };
        }

        public static Vector3 operator /(Vector3 a, Vector3 b)
        {
            return new Vector3()
            {
                X = a.X / b.X,
                Y = a.Y / b.Y,
                Z = a.Z / b.Z
            };
        }

        public static Vector3 operator /(Vector3 a, int b)
        {
            return new Vector3()
            {
                X = a.X / b,
                Y = a.Y / b,
                Z = a.Z / b
            };
        }

        public static Vector3 operator /(Vector3 a, float b)
        {
            return new Vector3()
            {
                X = a.X / b,
                Y = a.Y / b,
                Z = a.Z / b
            };
        }

        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
        public bool Equals(Vector3 other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector2 newtmp)
            {
                return Equals(newtmp);
            }

            return false;
        }

        public static bool operator ==(Vector3 a, Vector3 b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Vector3 a, Vector3 b)
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
        public static float Dot(Vector3 a, Vector3 b)
        {
            return (a.X * b.X) + (a.Y * b.Y) + (a.Z * b.Z);
        }

        #endregion

        #region Other

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            return X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode();
        }

        public override string ToString()
        {
            return "{ X:" + X + " , Y:" + Y + " , Z:" + Z + " }";
        }

        #endregion
    }
}