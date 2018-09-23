﻿#if DESKTOP

using System;
using Inferno.Runtime.Input;
using Inferno.Runtime.UI;
using SDL2;

namespace Inferno.Runtime
{
    /// <summary>
    /// SDL specific game code
    /// </summary>
    internal class PlatformGame
    {
        public bool RunEvents()
        {
            while (SDL2.SDL.SDL_PollEvent(out var e) != 0)
            {
                Key k;
                switch (e.type)
                {
                    case SDL2.SDL.SDL_EventType.SDL_QUIT:
                        return false;
                    case SDL2.SDL.SDL_EventType.SDL_MOUSEWHEEL:
                        Mouse.ScrollY += e.wheel.y * 120;
                        Mouse.ScrollX += e.wheel.x * 120;
                        break;
                    case SDL2.SDL.SDL_EventType.SDL_APP_LOWMEMORY:
                        MessageBox.Show("Low Memory", "The program is running low on memory, please close some programs.", MessageBoxType.Warning);
                        break;
                    case SDL2.SDL.SDL_EventType.SDL_WINDOWEVENT:
                        switch (e.window.windowEvent)
                        {
                            case SDL2.SDL.SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_GAINED:
                                Game.Instance.HasFocus = true;
                                break;
                            case SDL2.SDL.SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_LOST:
                                Game.Instance.HasFocus = false;
                                break;
                        }
                        break;
                    case SDL2.SDL.SDL_EventType.SDL_MOUSEMOTION:
                        Game.Instance.Window.MouseState.X = e.motion.x;
                        Game.Instance.Window.MouseState.Y = e.motion.y;
                        break;
                    case SDL2.SDL.SDL_EventType.SDL_KEYDOWN:
                        k = KeyConverter.ToKey((int) e.key.keysym.sym);
                        if (!Game.Instance.Keys.Contains(k))
                            Game.Instance.Keys.Add(k);
                        break;
                    case SDL2.SDL.SDL_EventType.SDL_KEYUP:
                        k = KeyConverter.ToKey((int) e.key.keysym.sym);
                        while (Game.Instance.Keys.Contains(k))
                            Game.Instance.Keys.Remove(k);
                        break;
                }
            }

            //Draw Game
            Game.Instance.InvokeDraw();

            return true;
        }
    }
}

#endif