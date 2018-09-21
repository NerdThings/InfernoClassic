using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Inferno.Runtime.Graphics
{
    public class Texture2D
    {
        internal PlatformTexture2D PlatformTexture2D;
        private int _width;
        private int _height;
        private int ArraySize;

        public Rectangle Bounds => new Rectangle(0, 0, _width, _height);

        public int Width => _width;
        public int Height => _height;

        public Texture2D(string filename)
        {
            //if (!Uri.IsWellFormedUriString(filename, UriKind.Absolute))
            //{
                //filename = Directory.GetCurrentDirectory() + "\\" + filename;
           // }

            PlatformTexture2D = new PlatformTexture2D(filename);
        }

        //TODO: Proper Texture2D stuff (And native stuffs)
    }
}
