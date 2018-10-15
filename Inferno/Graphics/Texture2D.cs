using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace Inferno.Graphics
{
    /// <summary>
    /// A drawable texture
    /// </summary>
    public class Texture2D : IDisposable
    {
        internal PlatformTexture2D PlatformTexture2D;

        public Rectangle Bounds => new Rectangle(0, 0, Width, Height);

        public int Width => PlatformTexture2D.Width;
        public int Height => PlatformTexture2D.Height;

        internal Texture2D(Bitmap bitmap)
        {
            PlatformTexture2D = new PlatformTexture2D(bitmap);
        }

        public Texture2D(string filename)
        {
            //Ammend the uri if it is an indirect one
            if (!Uri.IsWellFormedUriString(filename, UriKind.Absolute))
            {
                filename = Directory.GetCurrentDirectory() + "\\" + filename;
            }

            PlatformTexture2D = new PlatformTexture2D(filename);
        }

        public Texture2D(int width, int height)
        {
            PlatformTexture2D = new PlatformTexture2D(width, height);
        }

        public Texture2D(int width, int height, Color[] data)
        {
            PlatformTexture2D = new PlatformTexture2D(width, height, data);
        }

        public static Texture2D FromStream(Stream stream)
        {
            return new Texture2D(new Bitmap(Image.FromStream(stream)));
        }

        public void SetData(Color[] data)
        {
            PlatformTexture2D.SetData(data);
        }

        ~Texture2D()
        {
            Dispose();
        }

        public void Dispose()
        {
            GraphicsDevice.DisposeTexture(this);
        }
    }
}
