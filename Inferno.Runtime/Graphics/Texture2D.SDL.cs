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
        private IntPtr Handle { get; set; }

        public PlatformTexture2D(string filename)
        {
            Handle = SDL_image.IMG_LoadTexture(PlatformGameWindow.Renderer, filename);

            //Create texture from surface
            if (Handle == IntPtr.Zero)
            {
                throw new Exception("Unable to load image. " + SDL.SDL_GetError());
            }
        }
    }
}

#endif