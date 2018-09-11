using System;
using System.Diagnostics.CodeAnalysis;

namespace Inferno.Runtime
{
    public struct Vector4
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
        
        /// <summary>
        /// The W component of the vector
        /// </summary>
        public float W;

        #endregion

        #region Properties

        /// <summary>
        /// The magnitude of the vector.
        /// </summary>
        public float Magnitude => (float)Math.Sqrt((X * X) + (Y * Y) + (Z * Z) + (W * W));

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new vector2.
        /// </summary>
        /// <param name="x">X component</param>
        /// <param name="y">Y component</param>
        /// <param name="z">Z component</param>
        /// <param name="w">W component</param>
        public Vector4(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public Vector4(Vector3 value, float w)
        {
            X = value.X;
            Y = value.Y;
            Z = value.Z;
            W = w;
        }

        #endregion

        #region Operators

        public static Vector4 operator +(Vector4 a, Vector4 b)
        {
            return new Vector4()
            {
                X = a.X + b.X,
                Y = a.Y + b.Y,
                Z = a.Z + b.Z,
                W = a.W + b.W
            };
        }

        public static Vector4 operator +(Vector4 a, int b)
        {
            return new Vector4()
            {
                X = a.X + b,
                Y = a.Y + b,
                Z = a.Z + b,
                W = a.W + b
            };
        }

        public static Vector4 operator +(Vector4 a, float b)
        {
            return new Vector4()
            {
                X = a.X + b,
                Y = a.Y + b,
                Z = a.Z + b,
                W = a.W + b
            };
        }

        public static Vector4 operator -(Vector4 a, Vector4 b)
        {
            return new Vector4()
            {
                X = a.X - b.X,
                Y = a.Y - b.Y,
                Z = a.Z - b.Z,
                W = a.W - b.W
            };
        }

        public static Vector4 operator -(Vector4 a, int b)
        {
            return new Vector4()
            {
                X = a.X - b,
                Y = a.Y - b,
                Z = a.Z - b,
                W = a.W - b
            };
        }

        public static Vector4 operator -(Vector4 a, float b)
        {
            return new Vector4()
            {
                X = a.X - b,
                Y = a.Y - b,
                Z = a.Z - b,
                W = a.W - b
            };
        }

        public static Vector4 operator *(Vector4 a, Vector4 b)
        {
            return new Vector4()
            {
                X = a.X * b.X,
                Y = a.Y * b.Y,
                Z = a.Z * b.Z,
                W = a.W * b.W
            };
        }

        public static Vector4 operator *(Vector4 a, int b)
        {
            return new Vector4()
            {
                X = a.X * b,
                Y = a.Y * b,
                Z = a.Z * b,
                W = a.W * b
            };
        }

        public static Vector4 operator *(Vector4 a, float b)
        {
            return new Vector4()
            {
                X = a.X * b,
                Y = a.Y * b,
                Z = a.Z * b,
                W = a.W * b
            };
        }

        public static Vector4 operator /(Vector4 a, Vector4 b)
        {
            return new Vector4()
            {
                X = a.X / b.X,
                Y = a.Y / b.Y,
                Z = a.Z / b.Z,
                W = a.W / b.W
            };
        }

        public static Vector4 operator /(Vector4 a, int b)
        {
            return new Vector4()
            {
                X = a.X / b,
                Y = a.Y / b,
                Z = a.Z / b,
                W = a.W / b
            };
        }

        public static Vector4 operator /(Vector4 a, float b)
        {
            return new Vector4()
            {
                X = a.X / b,
                Y = a.Y / b,
                Z = a.Z / b,
                W = a.W / b
            };
        }

        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
        public bool Equals(Vector4 other)
        {
            return X == other.X && Y == other.Y && Z == other.Z && W == other.W;
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector4 newtmp)
            {
                return Equals(newtmp);
            }

            return false;
        }

        public static bool operator ==(Vector4 a, Vector4 b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Vector4 a, Vector4 b)
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
        public static float Dot(Vector4 a, Vector4 b)
        {
            return (a.X * b.X) + (a.Y * b.Y) + (a.Z * b.Z) + (a.W * b.W);
        }
        
        public static Vector4 Transform(Vector4 position, Matrix matrix)
        {
           return new Vector4()
           {
              X = (position.X * matrix.M11) + (position.Y * matrix.M21) + (position.Z * matrix.M31) + (position.W * matrix.M41),
              Y = (position.X * matrix.M12) + (position.Y * matrix.M22) + (position.Z * matrix.M32) + (position.W * matrix.M42),
              Z = (position.X * matrix.M13) + (position.Y * matrix.M23) + (position.Z * matrix.M33) + (position.W * matrix.M43),
              W = (position.X * matrix.M14) + (position.Y * matrix.M24) + (position.Z * matrix.M34) + (position.W * matrix.M44)
           };
        }

        #endregion

        #region Other

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            return X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode() + W.GetHashCode();
        }

        public override string ToString()
        {
            return "{ X:" + X + " , Y:" + Y + " , Z:" + Z + " , W:" + W + " }";
        }

        #endregion
    }
}
