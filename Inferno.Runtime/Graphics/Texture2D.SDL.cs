#if DESKTOP

using System;
using SDL2;

namespace Inferno.Runtime.Graphics
{
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