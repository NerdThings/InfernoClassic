#if OPENGL

using System;
using OpenTK.Graphics.OpenGL;

namespace Inferno.Graphics
{
    public partial class Texture2D
    {
        internal int Id { get; private set; }

        private void CreateTexture(Color[] data)
        {
            //Clear any bound textures
            GL.BindTexture(TextureTarget.Texture2D, 0);

            //Cache data
            _cachedData = data;
            
            //Create Texture
            Id = GL.GenTexture();
            
            //Bind Texture
            GL.BindTexture(TextureTarget.Texture2D, Id);
            
            //Convert data
            var glData = new uint[Width * Height];

            for (var i = 0; i < Width * Height; i++)
            {
                glData[i] = data[i].PackedValue;
            }

            //Set data
            //ES20:
            //GL.TexImage2D(TextureTarget2d.Texture2D, 0, TextureComponentCount.Rgba, Width, Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, glData);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, glData);

            //Set parameters
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Nearest);

            //Unbind Texture
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
    }
}

#endif