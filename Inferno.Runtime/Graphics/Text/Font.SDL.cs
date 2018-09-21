using System;
using System.Collections.Generic;
using System.Text;
using SDL2;

namespace Inferno.Runtime.Graphics.Text
{
    internal class PlatformFont : IDisposable
    {
        internal IntPtr Handle;

        public PlatformFont(string filename, int ptSize)
        {
            Handle = SDL_ttf.TTF_OpenFont(filename, ptSize);

            if (Handle == IntPtr.Zero)
                throw new Exception("Unable to open font. " + SDL.SDL_GetError());
        }

        public Vector2 MeasureString(string text)
        {
            if (Handle == IntPtr.Zero)
                throw new Exception("Attempt to access disposed font.");

            SDL_ttf.TTF_SizeUTF8(Handle, text, out var w, out var h);
            return new Vector2(w, h);
        }

        public void Dispose()
        {
            SDL_ttf.TTF_CloseFont(Handle);
        }
    }
}
