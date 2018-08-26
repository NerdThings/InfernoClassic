using System;

namespace Inferno.Runtime
{
    public struct Rectangle : IEquatable<Rectangle>
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;

        public int Left => X;
        public int Right => X + Width;
        public int Top => Y;
        public int Bottom => Y + Height;

        public Point Location
        {
            get => new Point(X, Y);
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        public Point Size
        {
            get => new Point(Width, Height);
            set
            {
                Width = value.X;
                Height = value.Y;
            }
        }

        public Point Center => new Point(X + (Width / 2), Y + (Height / 2));

        internal Microsoft.Xna.Framework.Rectangle Monogame => new Microsoft.Xna.Framework.Rectangle(X, Y, Width, Height);

        public Rectangle(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public bool Equals(Rectangle other)
        {
            return X == other.X && Y == other.Y && Width == other.Width && Height == other.Height;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Rectangle rectangle && Equals(rectangle);
        }

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

        public static bool operator ==(Rectangle a, Rectangle b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Rectangle a, Rectangle b)
        {
            return !a.Equals(b);
        }

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
    }
}
