using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Inferno.Runtime.Graphics
{
    public class Texture2D
    {
        internal PlatformTexture2D PlatformTexture2D;

        public Rectangle Bounds => new Rectangle(0, 0, Width, Height);

        public int Width => PlatformTexture2D.Width;
        public int Height => PlatformTexture2D.Height;

        public Texture2D(string filename)
        {
            //Ammend the uri if it is an indirect one
            if (!Uri.IsWellFormedUriString(filename, UriKind.Absolute))
            {
                filename = Directory.GetCurrentDirectory() + "\\" + filename;
            }

            PlatformTexture2D = new PlatformTexture2D(filename);
        }

        ~Texture2D()
        {

        }

        //TODO: Proper Texture2D stuff (And native stuffs)
    }
}
