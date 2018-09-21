#if DESKTOP

using System;
using System.Collections.Generic;
using System.Text;
using SDL2;

namespace Inferno.Runtime.UI
{
    internal static class PlatformMessageBox
    {
        public static void Show(string title, string message)
        {
            if (SDL.SDL_ShowSimpleMessageBox(SDL.SDL_MessageBoxFlags.SDL_MESSAGEBOX_INFORMATION, title, message,
                    Game.Instance.Window.PlatformWindow.Handle) < 0)
                throw new Exception("Failed to show message box. " + SDL.SDL_GetError());
        }
    }
}

#endif