#if DESKTOP

using System;
using System.Collections.Generic;
using System.Text;
using Inferno.Runtime.Input;
using SDL2;

namespace Inferno.Runtime
{
    internal class PlatformGame
    {
        public bool RunEvents()
        {
            while (SDL.SDL_PollEvent(out var e) != 0)
            {
                if (e.type == SDL.SDL_EventType.SDL_QUIT)
                {
                    return false;
                }
                else if (e.type == SDL.SDL_EventType.SDL_MOUSEWHEEL)
                {
                    Mouse.ScrollY += e.wheel.y * 120;
                    Mouse.ScrollX += e.wheel.x * 120;
                }
            }

            return true;
        }
    }
}

#endif