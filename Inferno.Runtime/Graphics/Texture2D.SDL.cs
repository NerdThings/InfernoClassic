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

            var ptr = Marshal.AllocHGlobal(Marshal.SizeOf(fmt));
            Marshal.StructureToPtr(fmt, ptr, false);
            
            for (var i = 0; i < data.Length; i++)
            {
                pixels[i] = data[i].PackedValue;
            }

            var r = new SDL.SDL_Rect
            {
                x = 0,
                y = 0,
                w = width,
                h = height
            };

            SDL.SDL_UpdateTexture(Handle, ref r, (IntPtr)pixels, width * 4);
            
            

            /*var texture = SDL.SDL_CreateTexture(Game.Instance.GraphicsManager.PlatformGraphicsManager.Renderer,
                SDL.SDL_PIXELFORMAT_RGBA8888, (int) SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_STREAMING, width, height);

            SDL.SDL_QueryTexture(texture, out var format, out _, out _, out _);

            if (SDL.SDL_LockTexture(texture, IntPtr.Zero, out var pixels, out var pitch) < 0)
            {
                throw new Exception("Unable to create texture. " + SDL.SDL_GetError());
            }

            var pixelFormat = new SDL.SDL_PixelFormat {format = format};

            var pixelFormatPtr = Marshal.AllocHGlobal(Marshal.SizeOf(pixelFormat));
            Marshal.StructureToPtr(pixelFormat, pixelFormatPtr, false);

            var pixelsArray = (uint*) pixels;

            for (var i = 0; i < data.Length; i++)
            {
                pixelsArray[i] = SDL.SDL_MapRGB(pixelFormatPtr, data[i].R, data[i].G, data[i].B);//, data[i].A);
            }

            SDL.SDL_UnlockTexture(texture);
            SDL.SDL_UpdateTexture(texture, IntPtr.Zero, pixels, pitch);

            Handle = texture;*/
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