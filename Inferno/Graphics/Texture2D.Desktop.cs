#if DESKTOP

using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
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

        public PlatformTexture2D(string filename) : this(new Bitmap(filename), true)
        {
        }

        public unsafe PlatformTexture2D(Bitmap bitmap, bool dispose = false)
        {
            Width = bitmap.Width;
            Height = bitmap.Height;

            Id = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, Id);

            var data = new Color[Width * Height];

            System.Drawing.Imaging.BitmapData bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);

            switch (bitmap.PixelFormat)
            {
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                {
                    var ptr = (byte*)bitmapData.Scan0;
                    var i = 0;
                    for (var y = 0; y < bitmap.Height; ++y)
                    {
                        for (var x = 0; x < bitmap.Width; ++x)
                        {
                            var c = new Color(*(ptr + 2), *(ptr + 1), *ptr, *(ptr + 3));
                            data[i] = c;

                            i++;
                            ptr += 4;
                        }
                    }
                    break;
                }

                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                {
                    var ptr = (byte*)bitmapData.Scan0;
                    var i = 0;
                    for (var y = 0; y < bitmap.Height; ++y)
                    {
                        for (var x = 0; x < bitmap.Width; ++x)
                        {
                            var c = new Color(*(ptr + 2), *(ptr + 1), *ptr, (byte)255);
                            data[i] = c;

                            i++;
                            ptr += 3;
                        }
                    }
                        break;
                }
            }

            bitmap.UnlockBits(bitmapData);

            //Convert to opengl data
            var glData = new uint[Width * Height];

            for (var i = 0; i < Width * Height; i++)
            {
                glData[i] = data[i].PackedValue;
            }

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bitmap.Width, bitmap.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, glData);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Nearest);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            GL.BindTexture(TextureTarget.Texture2D, 0);

            if (dispose)
                bitmap.Dispose();
        }

        //public PlatformTexture2D(int width, int height, Color[] data) : this(width, height)
        //{
            //SetData(data);
        //}

        public PlatformTexture2D(int width, int height, Color[] data)
        {
            var glData = new uint[Width * Height];

            for (var i = 0; i < Width * Height; i++)
            {
                glData[i] = data[i].PackedValue;
            }

            Width = width;
            Height = height;

            Id = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, Id);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, glData);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.LinearMipmapLinear);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void SetData(Color[] data)
        {
            throw new NotImplementedException();
            var glData = new uint[Width * Height];

            for (var i = 0; i < Width * Height; i++)
            {
                glData[i] = data[i].PackedValue;
            }

            GL.BindTexture(TextureTarget.Texture2D, Id);

            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, Width, Height, PixelFormat.Rgba, PixelType.UnsignedByte, glData);
            //GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, glData);

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public Color[] GetData()
        {
            var glData = new uint[Width * Height];

            GL.BindTexture(TextureTarget.Texture2D, Id);
            GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.Rgba, PixelType.UnsignedByte, glData);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            //Convert
            var data = new Color[Width * Height];
            for (var i = 0; i < Width * Height; i++)
            {
                data[i] = new Color(glData[i]);
            }

            return data;
        }

        public int Width { get; private set; }
        public int Height { get; private set; }
    }
}

#endif