#if DESKTOP

using System;
using SDL2;

namespace Inferno.Runtime.Graphics
{
    /// <summary>
    /// SDL Specific render target code
    /// </summary>
    internal class PlatformRenderTarget
    {
        internal IntPtr Handle { get; set; }

        public PlatformRenderTarget(int width, int height)
        {
            //Create the texture
            Handle = SDL.SDL_CreateTexture(Game.Instance.GraphicsManager.PlatformGraphicsManager.Renderer, SDL.SDL_PIXELFORMAT_RGBA8888, (int)SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET, width, height);

            //Check the texture was created
            if (Handle == IntPtr.Zero)
            {
                throw new Exception("Unable to create rendertarget. " + SDL.SDL_GetError());
            }
        }

        public int Width
        {
            get
            {
                SDL.SDL_QueryTexture(Handle, out _, out _, out var w, out _);
                return w;
            }
        }

        public int Height
        {
            get
            {
                SDL.SDL_QueryTexture(Handle, out _, out _, out _, out var h);
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