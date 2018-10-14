#if DESKTOP

using System;
using System.Drawing;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Inferno.Graphics
{
    /// <summary>
    /// Desktop Specific texture code
    /// </summary>
    internal class PlatformTexture2D
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

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public PlatformTexture2D(int width, int height, Color[] data) : this(width, height)
        {
            SetData(data);
        }

        public PlatformTexture2D(int width, int height)
        {
            Width = width;
            Height = height;

            Id = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, Id);

            var glData = new uint[Width * Height];

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.LinearMipmapLinear);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void SetData(Color[] data)
        {
            var glData = new uint[Width * Height];

            for (var i = 0; i < Width * Height; i++)
            {
                glData[i] = data[i].PackedValue;
            }

            GL.BindTexture(TextureTarget.Texture2D, Id);

            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, Width, Height, PixelFormat.Rgba, PixelType.UnsignedByte, glData);

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public int Width { get; private set; }
        public int Height { get; private set; }
    }
}

#endif