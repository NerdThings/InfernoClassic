#if DESKTOP

using System;
using System.Data.Odbc;
using System.Runtime.InteropServices;
using SDL2;

namespace Inferno.Runtime.Graphics
{
    /// <summary>
    /// SDL Specific texture code
    /// </summary>
    internal class PlatformTexture2D : IDisposable
    {
        internal IntPtr Handle { get; set; }

        public PlatformTexture2D(string filename)
        {
            //Create the texture
            Handle = SDL_image.IMG_LoadTexture(Game.Instance.GraphicsManager.PlatformGraphicsManager.Renderer, filename);

            //Check the texture was created
            if (Handle == IntPtr.Zero)
            {
                throw new Exception("Unable to load image. " + SDL.SDL_GetError());
            }
        }

        [Obsolete("This is not ready for use yet")]
        public unsafe PlatformTexture2D(Color[] data, int width, int height)
        {
            /*var surfacePtr = SDL.SDL_CreateRGBSurface(0, width, height, 32, 0, 0, 0, 0);
            var surface = (SDL.SDL_Surface*) surfacePtr;
            var pixelsPtr = (*surface).pixels;
            var pixels = (uint*) pixelsPtr;

            for (var i = 0; i < data.Length - 1; i++)
            {
                if (pixels != null) pixels[i] = data[i].PackedValue;
            }

            Handle = SDL.SDL_CreateTextureFromSurface(Game.Instance.GraphicsManager.PlatformGraphicsManager.Renderer, surfacePtr);

            if (Handle == IntPtr.Zero)
            {
                throw new Exception("Unable to create image. " + SDL.SDL_GetError());
            }

            SDL.SDL_FreeSurface(surfacePtr);*/

            uint format = SDL.SDL_PIXELFORMAT_RGBA8888;

            Handle = SDL.SDL_CreateTexture(Game.Instance.GraphicsManager.PlatformGraphicsManager.Renderer, format,
                (int) SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_STATIC, width, height);

            uint* pixels = stackalloc uint[width * height];

            var fmt = new SDL.SDL_PixelFormat();
            fmt.format = format;
            fmt.BitsPerPixel = 32;
            fmt.BytesPerPixel = 4;
            //fmt.Rmask = 0;
            //fmt.Gmask = 0;
            //fmt.Bmask = 0;
            //fmt.Amask = 0;

            var ptr = Marshal.AllocHGlobal(Marshal.SizeOf(fmt));
            Marshal.StructureToPtr(fmt, ptr, false);
            
            for (var i = 0; i < data.Length - 1; i++)
            {
                pixels[i] = SDL.SDL_MapRGB(ptr, data[i].R, data[i].G, data[i].B);//, data[i].A);
            }

            var r = new SDL.SDL_Rect
            {
                x = 0,
                y = 0,
                w = width,
                h = height
            };

            SDL.SDL_UpdateTexture(Handle, ref r, (IntPtr)pixels, width * 4);
        }

        public void SetData(Color[] data)
        {
            
        }

        public int Width
        {
            get
            {
                if (Handle == IntPtr.Zero)
                {
                    throw new Exception("Attempt to use disposed texture");
                }

                SDL.SDL_QueryTexture(Handle, out var format, out var access, out var w, out var h);
                return w;
            }
        }

        public int Height
        {
            get
            {
                if (Handle == IntPtr.Zero)
                {
                    throw new Exception("Attempt to use disposed texture");
                }

                SDL.SDL_QueryTexture(Handle, out var format, out var access, out var w, out var h);
                return h;
            }
        }

        public void Dispose()
        {
            SDL.SDL_DestroyTexture(Handle);
            Handle = IntPtr.Zero;
        }
    }
}

#endif