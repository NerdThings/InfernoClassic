#if SDL

using System;
using Inferno.Input;
using Inferno.UI;
using OpenTK.Graphics.OpenGL;
using SDL2;

namespace Inferno
{
    /// <summary>
    /// SDL specific game code
    /// </summary>
    internal class PlatformGame
    {
        private readonly Game _parentGame;
        
        internal PlatformGame(Game parentGame)
        {
            _parentGame = parentGame;
        }

        internal bool RunEvents()
        {
            while (SDL.SDL_PollEvent(out var e) != 0)
            {
                Key k;
                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (e.type)
                {
                    case SDL.SDL_EventType.SDL_QUIT:
                        return false;
                    case SDL.SDL_EventType.SDL_MOUSEWHEEL:
                        Mouse.ScrollY += e.wheel.y * 120;
                        Mouse.ScrollX += e.wheel.x * 120;
                        break;
                    case SDL.SDL_EventType.SDL_APP_LOWMEMORY:
                        MessageBox.Show("Low Memory", "The program is running low on memory, please close some programs.", MessageBoxType.Warning);
                        break;
                    case SDL.SDL_EventType.SDL_WINDOWEVENT:
                        // ReSharper disable once SwitchStatementMissingSomeCases
                        switch (e.window.windowEvent)
                        {
                            case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_GAINED:
                                _parentGame.OnActivated?.Invoke(_parentGame, EventArgs.Empty);
                                break;
                            case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_LOST:
                                _parentGame.OnDeactivated?.Invoke(_parentGame, EventArgs.Empty);
                                break;
                            case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED:
                                _parentGame.OnResize?.Invoke(_parentGame,
                                    new Game.OnResizeEventArgs(_parentGame.Window.Bounds));

                                var width = _parentGame.Window.Bounds.Width;
                                var height = _parentGame.Window.Bounds.Height;

                                //OpenGL Viewport
                                GL.Viewport(0, 0, width, height);
                                break;
                        }

                        break;
                    case SDL.SDL_EventType.SDL_MOUSEMOTION:
                        _parentGame.Window.MouseState.X = e.motion.x;
                        _parentGame.Window.MouseState.Y = e.motion.y;
                        break;
                    case SDL.SDL_EventType.SDL_KEYDOWN:
                        k = KeyConverter.ToKey((int) e.key.keysym.sym);
                        if (!_parentGame.Keys.Contains(k))
                            _parentGame.Keys.Add(k);
                        Keyboard.KeyPressed.Invoke(_parentGame, new Keyboard.KeyEventArgs(k));
                        break;
                    case SDL.SDL_EventType.SDL_KEYUP:
                        k = KeyConverter.ToKey((int) e.key.keysym.sym);
                        while (_parentGame.Keys.Contains(k))
                            _parentGame.Keys.Remove(k);
                        Keyboard.KeyReleased.Invoke(_parentGame, new Keyboard.KeyEventArgs(k));
                        break;
                }
            }

            return true;
        }
    }
}

#endif