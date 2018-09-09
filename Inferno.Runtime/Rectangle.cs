using System;
using System.Diagnostics.CodeAnalysis;

namespace Inferno.Runtime
{
    public struct Rectangle : IEquatable<Rectangle>
    {
        #region Fields

        /// <summary>
        /// The X coordinate of the rectangle.
        /// </summary>
        public int X;

        /// <summary>
        /// The Y coordinate of the rectangle
        /// </summary>
        public int Y;

        /// <summary>
        /// The width of the rectangle.
        /// </summary>
        public int Width;

        /// <summary>
        /// The height of the rectangle
        /// </summary>
        public int Height;

        #endregion

        #region Properties

        /// <summary>
        /// The coordinate of the leftmost side
        /// </summary>
        public int Left => X;

        /// <summary>
        /// The coordinate of the rightmost side
        /// </summary>
        public int Right => X + Width;

        /// <summary>
        /// The coordinate of the topmost side
        /// </summary>
        public int Top => Y;

        /// <summary>
        /// The coordinate of the bottommost side
        /// </summary>
        public int Bottom => Y + Height;

        /// <summary>
        /// The rectangle location expressed as a point
        /// </summary>
        public Point Location
        {
            get => new Point(X, Y);
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        /// <summary>
        /// The size of the rectangle expressed as a point
        /// </summary>
        public Point Size
        {
            get => new Point(Width, Height);
            set
            {
                Width = value.X;
                Height = value.Y;
            }
        }

        /// <summary>
        /// The center point of the rectanlge
        /// </summary>
        public Point Center => new Point(X + (Width / 2), Y + (Height / 2));

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new rectangle
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        public Rectangle(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        #endregion

        public bool Equals(Rectangle other)
        {
            return X == other.X && Y == other.Y && Width == other.Width && Height == other.Height;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Rectangle rectangle && Equals(rectangle);
        }

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = 17;
                hashCode = hashCode * 23 ^ X.GetHashCode();
                hashCode = hashCode * 23 ^ Y.GetHashCode();
                hashCode = hashCode * 23 ^ Width.GetHashCode();
                hashCode = hashCode * 23 ^ Height.GetHashCode();
                return hashCode;
            }
        }

        #region Operators
        public static bool operator ==(Rectangle a, Rectangle b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Rectangle a, Rectangle b)
        {
            return !a.Equals(b);
        }

        #endregion

        #region Collisions

        public bool Contains(float x, float y)
        {
            return ((((X <= x) && (x < (X + Width))) && (Y <= y)) && (y < (Y + Height)));
        }

        public bool Contains(int x, int y)
        {
            return Contains((float)x, y);
        }

        public bool Contains(Point value)
        {
            return Contains(value.X, value.Y);
        }

        public bool Contains(Vector2 value)
        {
            return Contains(value.X, value.Y);
        }

        public bool Intersects(Rectangle value)
        {
            return value.Left < Right &&
                   Left < value.Right &&
                   value.Top < Bottom &&
                   Top < value.Bottom;
        }

        public void Offset(int x, int y)
        {
            X += x;
            Y += y;
        }

        public void Offset(float x, float y)
        {
            X += (int)x;
            Y += (int)y;
        }

        public void Offset(Point offset)
        {
            X += offset.X;
            Y += offset.Y;
        }

        public bool TouchingLeft(Rectangle b)
        {
            return Right > b.Left &&
                   Left < b.Left &&
                   Bottom > b.Top &&
                   Top < b.Bottom;
        }

        public static bool TouchingLeft(Rectangle a, Rectangle b)
        {
            return a.TouchingLeft(b);
        }

        public bool TouchingTop(Rectangle b)
        {
            return Bottom > b.Top &&
                   Top < b.Top &&
                   Right > b.Left &&
                   Left < b.Right;
        }

        public static bool TouchingTop(Rectangle a, Rectangle b)
        {
            return a.TouchingTop(b);
        }

        public bool TouchingBottom(Rectangle b)
        {
            return Top < b.Bottom &&
                   Bottom > b.Bottom &&
                   Right > b.Left &&
                   Left < b.Right;
        }

        public static bool TouchingBottom(Rectangle a, Rectangle b)
        {
            return a.TouchingBottom(b);
        }

        public bool TouchingRight(Rectangle b)
        {
            return Left < b.Right &&
                   Right > b.Right &&
                   Bottom > b.Top &&
                   Top < b.Bottom;
        }

        public static bool TouchingRight(Rectangle a, Rectangle b)
        {
            return a.TouchingRight(b);
        }

        public bool Touching(Rectangle b)
        {
            return TouchingLeft(b) || TouchingRight(b) || TouchingTop(b) || TouchingBottom(b);
        }

        #endregion
    }
}
