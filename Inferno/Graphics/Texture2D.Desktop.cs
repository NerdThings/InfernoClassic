#if DESKTOP

using System;
using System.Data.Odbc;
using System.Drawing;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;
using SDL2;

namespace Inferno.Graphics
{
    /// <summary>
    /// Desktop Specific texture code
    /// </summary>
    internal class PlatformTexture2D : IDisposable
    {
        internal int Id { get; set; }

        public PlatformTexture2D(string filename)
        {
            var textureSource = new Bitmap(filename);

            Width = textureSource.Width;
            Height = textureSource.Height;

            Id = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, Id);

            System.Drawing.Imaging.BitmapData bitmapData = textureSource.LockBits(new System.Drawing.Rectangle(0, 0, textureSource.Width, textureSource.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, textureSource.PixelFormat);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, textureSource.Width, textureSource.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, bitmapData.Scan0);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.LinearMipmapLinear);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            textureSource.UnlockBits(bitmapData);

            GL.BindTexture(TextureTarget.ProxyTexture2D, 0);
        }

        [Obsolete("This is not ready for use yet")]
        public unsafe PlatformTexture2D(Color[] data, int width, int height)
        {
            //NOPE, NOT NOW
        }

        public void SetData(Color[] data)
        {
            
        }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public void Dispose()
        {
            Id = -1;
        }
    }
}

#endif