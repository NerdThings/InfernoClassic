#if DESKTOP

using System;
using System.Drawing;
using OpenTK.Graphics.OpenGL;

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

        public PlatformTexture2D(Color[] data, int width, int height)
        {
            //TODO
            throw new NotImplementedException();
        }

        public void SetData(Color[] data)
        {
            //TODO
            throw new NotImplementedException();
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