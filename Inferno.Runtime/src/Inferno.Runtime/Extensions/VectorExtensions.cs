using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inferno.Runtime.Extensions
{
    public static class VectorExtensions
    {
        public static Point ToPoint(this Vector2 vector2)
        {
            return new Point((int)Math.Floor(vector2.X), (int)Math.Floor(vector2.Y));
        }
    }
}
