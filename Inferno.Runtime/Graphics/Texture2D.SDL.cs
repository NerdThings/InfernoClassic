#if DESKTOP

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using SDL2;
using IntPtr = System.IntPtr;

namespace Inferno.Runtime.Graphics
{
    internal class PlatformTexture2D
    {
        internal IntPtr Handle { get; set; }

        public PlatformTexture2D(string filename)
        {
            //Create the texture
            Handle = SDL_image.IMG_LoadTexture(PlatformGameWindow.Renderer, filename);

            //Check the texture was created
            if (Handle == IntPtr.Zero)
            {
                throw new Exception("Unable to load image. " + SDL.SDL_GetError());
            }

            //Register
            PlatformGameWindow.RegisterTexture(Handle);
        }

        public int Width
        {
            get
            {
                SDL.SDL_QueryTexture(Handle, out var format, out var access, out var w, out var h);
                return w;
            }
        }

        public int Height
        {
            get
            {
                SDL.SDL_QueryTexture(Handle, out var format, out var access, out var w, out var h);
                return h;
            }
        }
    }
}

#endif