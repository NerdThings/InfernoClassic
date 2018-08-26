using System;
using System.Diagnostics.CodeAnalysis;

namespace Inferno.Runtime
{
    public struct Point : IEquatable<Point>
    {
        #region Fields

        /// <summary>
        /// The X coordinate of the point
        /// </summary>
        public int X;

        /// <summary>
        /// The Y coordinate of the point
        /// </summary>
        public int Y;

        #endregion

        #region Properties

        /// <summary>
        /// For conversion purposes in phase 1
        /// </summary>
        internal Microsoft.Xna.Framework.Point Monogame => new Microsoft.Xna.Framework.Point(X, Y);

        #endregion

        #region Constructors

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        #endregion

        #region Operators

        public static Point operator +(Point a, Point b)
        {
            return new Point()
            {
                X = a.X + b.X,
                Y = a.Y + b.Y
            };
        }

        public static Point operator -(Point a, Point b)
        {
            return new Point()
            {
                X = a.X - b.X,
                Y = a.Y - b.Y
            };
        }

        public static Point operator *(Point a, Point b)
        {
            return new Point()
            {
                X = a.X * b.X,
                Y = a.Y * b.Y
            };
        }

        public static Point operator /(Point a, Point b)
        {
            return new Point()
            {
                X = a.X + b.X,
                Y = a.Y + b.Y
            };
        }

        public static bool operator ==(Point a, Point b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Point a, Point b)
        {
            return !a.Equals(b);
        }

        public bool Equals(Point other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Point point && Equals(point);
        }


        #endregion

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            //From monogame
            unchecked
            {
                var hash = 17;
                hash = hash * 23 + X.GetHashCode();
                hash = hash * 23 + Y.GetHashCode();
                return hash;
            }
        }

        public Vector2 ToVector2()
        {
            return new Vector2(X, Y);
        }
    }
}
