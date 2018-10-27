#if SDL

using System;
using SDL2;

namespace Inferno.UI
{
    public static partial class MessageBox
    {
        public static void Display(string title, string message, MessageBoxType type)
        {
            SDL.SDL_MessageBoxFlags flags;

            switch (type)
            {
                case MessageBoxType.Information:
                    flags = SDL.SDL_MessageBoxFlags.SDL_MESSAGEBOX_INFORMATION;
                    break;
                case MessageBoxType.Warning:
                    flags = SDL.SDL_MessageBoxFlags.SDL_MESSAGEBOX_WARNING;
                    break;
                case MessageBoxType.Error:
                    flags = SDL.SDL_MessageBoxFlags.SDL_MESSAGEBOX_ERROR;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            if (SDL.SDL_ShowSimpleMessageBox(flags, title, message,
                    Game.Instance.Window.Handle) < 0)
                throw new Exception("Failed to show message box. " + SDL.SDL_GetError());
        }
    }
}

#endif