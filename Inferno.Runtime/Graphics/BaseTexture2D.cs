using System;
using System.Collections.Generic;
using System.Text;

namespace Inferno.Runtime.Graphics
{
    public abstract class BaseTexture2D
    {
        // ReSharper disable once InconsistentNaming
        internal int _width;
        // ReSharper disable once InconsistentNaming
        internal int _height;
        internal int ArraySize;

        public Rectangle Bounds => new Rectangle(0, 0, _width, _height);

        public int Width => _width;
        public int Height => _height;

        //TODO: Proper Texture2D stuff (And native stuffs)
    }
}
