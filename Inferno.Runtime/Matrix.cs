using System;
using System.Diagnostics.CodeAnalysis;

namespace Inferno.Runtime
{
    public struct Matrix : IEquatable<Matrix>
    {
        public float M11;
        public float M12;
        public float M13;
        public float M14;
        public float M21;
        public float M22;
        public float M23;
        public float M24;
        public float M31;
        public float M32;
        public float M33;
        public float M34;
        public float M41;
        public float M42;
        public float M43;
        public float M44;

        #region Constructors

        public Matrix(float m11, float m12, float m13, float m14,
            float m21, float m22, float m23, float m24,
            float m31, float m32, float m33, float m34,
            float m41, float m42, float m43, float m44)
        {
            M11 = m11;
            M12 = m12;
            M13 = m13;
            M14 = m14;
            M21 = m21;
            M22 = m22;
            M23 = m23;
            M24 = m24;
            M31 = m31;
            M32 = m32;
            M33 = m33;
            M34 = m34;
            M41 = m41;
            M42 = m42;
            M43 = m43;
            M44 = m44;
        }

        #endregion

        #region Indexers

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return M11;
                    case 1: return M12;
                    case 2: return M13;
                    case 3: return M14;
                    case 4: return M21;
                    case 5: return M22;
                    case 6: return M23;
                    case 7: return M24;
                    case 8: return M31;
                    case 9: return M32;
                    case 10: return M33;
                    case 11: return M34;
                    case 12: return M41;
                    case 13: return M42;
                    case 14: return M43;
                    case 15: return M44;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            set
            {
                switch (index)
                {
                    case 0: M11 = value; break;
                    case 1: M12 = value; break;
                    case 2: M13 = value; break;
                    case 3: M14 = value; break;
                    case 4: M21 = value; break;
                    case 5: M22 = value; break;
                    case 6: M23 = value; break;
                    case 7: M24 = value; break;
                    case 8: M31 = value; break;
                    case 9: M32 = value; break;
                    case 10: M33 = value; break;
                    case 11: M34 = value; break;
                    case 12: M41 = value; break;
                    case 13: M42 = value; break;
                    case 14: M43 = value; break;
                    case 15: M44 = value; break;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        public float this[int row, int column]
        {
            get => this[(row * 4) + column];

            set => this[(row * 4) + column] = value;
        }

        #endregion

        #region Properties

        internal Microsoft.Xna.Framework.Matrix Monogame => new Microsoft.Xna.Framework.Matrix(M11, M12, M13, M14, M21, M22, M23, M24, M31, M32, M33, M34, M41, M42, M43, M44);

        private static readonly Matrix Identity = new Matrix(1f, 0f,  0f, 0f,
                                                    0f, 1f, 0f, 0f,
                                                    0f, 0f, 1f, 0f,
                                                    0f, 0f, 0f, 1f);

        public Vector3 Backward
        {
            get => new Vector3(M31, M32, M33);
            set
            {
                M31 = value.X;
                M32 = value.Y;
                M33 = value.Z;
            }
        }

        public Vector3 Down
        {
            get => new Vector3(-M21, -M22, -M23);
            set
            {
                M21 = -value.X;
                M22 = -value.Y;
                M23 = -value.Z;
            }
        }

        public Vector3 Forward
        {
            get => new Vector3(-M31, -M32, -M33);
            set
            {
                M31 = -value.X;
                M32 = -value.Y;
                M33 = -value.Z;
            }
        }

        public Vector3 Left
        {
            get => new Vector3(-M11, -M12, -M13);
            set
            {
                M11 = -value.X;
                M12 = -value.Y;
                M13 = -value.Z;
            }
        }

        public Vector3 Right
        {
            get => new Vector3(M11, M12, M13);
            set
            {
                M11 = value.X;
                M12 = value.Y;
                M13 = value.Z;
            }
        }

        //TODO: Rotation

        public Vector3 Translation
        {
            get => new Vector3(M41, M42, M43);
            set
            {
                M41 = value.X;
                M42 = value.Y;
                M43 = value.Z;
            }
        }

        public Vector3 Scale
        {
            get => new Vector3(M11, M22, M33);
            set
            {
                M11 = value.X;
                M22 = value.Y;
                M33 = value.Z;
            }
        }

        public Vector3 Up
        {
            get => new Vector3(M21, M22, M23);
            set
            {
                M21 = value.X;
                M22 = value.Y;
                M23 = value.Z;
            }
        }

        #endregion

        #region Operators

        public static Matrix operator +(Matrix a, Matrix b)
        {
            return new Matrix()
            {
                M11 = a.M11 + b.M11,
                M12 = a.M12 + b.M12,
                M13 = a.M13 + b.M13,
                M14 = a.M14 + b.M14,
                M21 = a.M21 + b.M21,
                M22 = a.M22 + b.M22,
                M23 = a.M23 + b.M23,
                M24 = a.M24 + b.M24,
                M31 = a.M31 + b.M31,
                M32 = a.M32 + b.M32,
                M33 = a.M33 + b.M33,
                M34 = a.M34 + b.M34,
                M41 = a.M41 + b.M41,
                M42 = a.M42 + b.M42,
                M43 = a.M43 + b.M43,
                M44 = a.M44 + b.M44,
            };
        }

        public static Matrix operator -(Matrix a, Matrix b)
        {
            return new Matrix()
            {
                M11 = a.M11 - b.M11,
                M12 = a.M12 - b.M12,
                M13 = a.M13 - b.M13,
                M14 = a.M14 - b.M14,
                M21 = a.M21 - b.M21,
                M22 = a.M22 - b.M22,
                M23 = a.M23 - b.M23,
                M24 = a.M24 - b.M24,
                M31 = a.M31 - b.M31,
                M32 = a.M32 - b.M32,
                M33 = a.M33 - b.M33,
                M34 = a.M34 - b.M34,
                M41 = a.M41 - b.M41,
                M42 = a.M42 - b.M42,
                M43 = a.M43 - b.M43,
                M44 = a.M44 - b.M44,
            };
        }

        public static Matrix operator -(Matrix a)
        {
            return new Matrix()
            {
                M11 = -a.M11,
                M12 = -a.M12,
                M13 = -a.M13,
                M14 = -a.M14,
                M21 = -a.M21,
                M22 = -a.M22,
                M23 = -a.M23,
                M24 = -a.M24,
                M31 = -a.M31,
                M32 = -a.M32,
                M33 = -a.M33,
                M34 = -a.M34,
                M41 = -a.M41,
                M42 = -a.M42,
                M43 = -a.M43,
                M44 = -a.M44,
            };
        }

        public static Matrix operator /(Matrix a, Matrix b)
        {
            return new Matrix()
            {
                M11 = a.M11 / b.M11,
                M12 = a.M12 / b.M12,
                M13 = a.M13 / b.M13,
                M14 = a.M14 / b.M14,
                M21 = a.M21 / b.M21,
                M22 = a.M22 / b.M22,
                M23 = a.M23 / b.M23,
                M24 = a.M24 / b.M24,
                M31 = a.M31 / b.M31,
                M32 = a.M32 / b.M32,
                M33 = a.M33 / b.M33,
                M34 = a.M34 / b.M34,
                M41 = a.M41 / b.M41,
                M42 = a.M42 / b.M42,
                M43 = a.M43 / b.M43,
                M44 = a.M44 / b.M44,
            };
        }

        public static Matrix operator *(Matrix a, Matrix b)
        {
            return new Matrix()
            {
                M11 = a.M11 * b.M11,
                M12 = a.M12 * b.M12,
                M13 = a.M13 * b.M13,
                M14 = a.M14 * b.M14,
                M21 = a.M21 * b.M21,
                M22 = a.M22 * b.M22,
                M23 = a.M23 * b.M23,
                M24 = a.M24 * b.M24,
                M31 = a.M31 * b.M31,
                M32 = a.M32 * b.M32,
                M33 = a.M33 * b.M33,
                M34 = a.M34 * b.M34,
                M41 = a.M41 * b.M41,
                M42 = a.M42 * b.M42,
                M43 = a.M43 * b.M43,
                M44 = a.M44 * b.M44,
            };
        }

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            return (((((((((((((((M11.GetHashCode() + M12.GetHashCode()) + M13.GetHashCode()) + M14.GetHashCode()) + M21.GetHashCode()) + M22.GetHashCode()) + M23.GetHashCode()) + M24.GetHashCode()) + M31.GetHashCode()) + M32.GetHashCode()) + M33.GetHashCode()) + M34.GetHashCode()) + M41.GetHashCode()) + M42.GetHashCode()) + M43.GetHashCode()) + M44.GetHashCode());
        }

        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
        public bool Equals(Matrix other)
        {
            return ((((((M11 == other.M11) && (M22 == other.M22)) && ((M33 == other.M33) && (M44 == other.M44))) && (((M12 == other.M12) && (M13 == other.M13)) && ((M14 == other.M14) && (M21 == other.M21)))) && ((((M23 == other.M23) && (M24 == other.M24)) && ((M31 == other.M31) && (M32 == other.M32))) && (((M34 == other.M34) && (M41 == other.M41)) && (M42 == other.M42)))) && (M43 == other.M43));
        }

        #endregion

        #region Methods

        public static Matrix CreateRotationX(float radians)
        {
            var result = Identity;

            var val1 = (float) Math.Cos(radians);
            var val2 = (float) Math.Sin(radians);

            result.M22 = val1;
            result.M23 = val2;
            result.M32 = -val2;
            result.M33 = val1;

            return result;
        }

        public static Matrix CreateRotationZ(float radians)
        {
            var result = Identity;

            var val1 = (float)Math.Cos(radians);
            var val2 = (float)Math.Sin(radians);

            result.M11 = val1;
            result.M12 = val2;
            result.M21 = -val2;
            result.M22 = val1;

            return result;
        }

        public static Matrix CreateScale(float scale)
        {
            return CreateScale(scale, scale, scale);
        }

        public static Matrix CreateScale(Vector3 scales)
        {
            return CreateScale(scales.X, scales.Y, scales.Z);
        }

        public static Matrix CreateScale(float xScale, float yScale, float zScale)
        {
            return new Matrix
            {
                M11 = xScale,
                M12 = 0,
                M13 = 0,
                M14 = 0,
                M21 = 0,
                M22 = yScale,
                M23 = 0,
                M24 = 0,
                M31 = 0,
                M32 = 0,
                M33 = zScale,
                M34 = 0,
                M41 = 0,
                M42 = 0,
                M43 = 0,
                M44 = 1
            };
        }

        public static Matrix CreateTranslation(Vector3 position)
        {
            return CreateTranslation(position.X, position.Y, position.Z);
        }

        public static Matrix CreateTranslation(float xPos, float yPos, float zPos)
        {
            return new Matrix
            {
                M11 = 1,
                M12 = 0,
                M13 = 0,
                M14 = 0,
                M21 = 0,
                M22 = 1,
                M23 = 0,
                M24 = 0,
                M31 = 0,
                M32 = 0,
                M33 = 1,
                M34 = 0,
                M41 = xPos,
                M42 = yPos,
                M43 = zPos,
                M44 = 1
            };
        }

        public static Matrix Invert(Matrix matrix)
        {
            var result = new Matrix();

            var num1 = matrix.M11;
            var num2 = matrix.M12;
            var num3 = matrix.M13;
            var num4 = matrix.M14;
            var num5 = matrix.M21;
            var num6 = matrix.M22;
            var num7 = matrix.M23;
            var num8 = matrix.M24;
            var num9 = matrix.M31;
            var num10 = matrix.M32;
            var num11 = matrix.M33;
            var num12 = matrix.M34;
            var num13 = matrix.M41;
            var num14 = matrix.M42;
            var num15 = matrix.M43;
            var num16 = matrix.M44;
            var num17 = num11 * num16 - num12 * num15;
            var num18 = num10 * num16 - num12 * num14;
            var num19 = num10 * num15 - num11 * num14;
            var num20 = num9 * num16 - num12 * num13;
            var num21 = num9 * num15 - num11 * num13;
            var num22 = num9 * num14 - num10 * num13;
            var num23 = num6 * num17 - num7 * num18 + num8 * num19;
            var num24 = -(num5 * num17 - num7 * num20 + num8 * num21);
            var num25 = num5 * num18 - num6 * num20 + num8 * num22;
            var num26 = -(num5 * num19 - num6 * num21 + num7 * num22);
            var num27 = (float)(1.0 / (num1 * num23 + num2 * num24 + num3 * num25 + num4 * num26));

            result.M11 = num23 * num27;
            result.M21 = num24 * num27;
            result.M31 = num25 * num27;
            result.M41 = num26 * num27;
            result.M12 = -(num2 * num17 - num3 * num18 + num4 * num19) * num27;
            result.M22 = (num1 * num17 - num3 * num20 + num4 * num21) * num27;
            result.M32 = -(num1 * num18 - num2 * num20 + num4 * num22) * num27;
            result.M42 = (num1 * num19 - num2 * num21 + num3 * num22) * num27;
            var num28 = (num7 * num16 - num8 * num15);
            var num29 = (num6 * num16 - num8 * num14);
            var num30 = (num6 * num15 - num7 * num14);
            var num31 = (num5 * num16 - num8 * num13);
            var num32 = (num5 * num15 - num7 * num13);
            var num33 = (num5 * num14 - num6 * num13);
            result.M13 = (num2 * num28 - num3 * num29 + num4 * num30) * num27;
            result.M23 = -(num1 * num28 - num3 * num31 + num4 * num32) * num27;
            result.M33 = (num1 * num29 - num2 * num31 + num4 * num33) * num27;
            result.M43 = -(num1 * num30 - num2 * num32 + num3 * num33) * num27;
            var num34 = (num7 * num12 - num8 * num11);
            var num35 = (num6 * num12 - num8 * num10);
            var num36 = (num6 * num11 - num7 * num10);
            var num37 = (num5 * num12 - num8 * num9);
            var num38 = (num5 * num11 - num7 * num9);
            var num39 = (num5 * num10 - num6 * num9);
            result.M14 = -(num2 * num34 - num3 * num35 + num4 * num36) * num27;
            result.M24 = (num1 * num34 - num3 * num37 + num4 * num38) * num27;
            result.M34 = -(num1 * num35 - num2 * num37 + num4 * num39) * num27;
            result.M44 = (num1 * num36 - num2 * num38 + num3 * num39) * num27;

            return result;
        }

        #endregion
    }
}
