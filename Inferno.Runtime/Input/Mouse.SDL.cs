#if DESKTOP

using System;
using Inferno.Runtime.Core;
using SDL2;

namespace Inferno.Runtime.Input
{
    /// <summary>
    /// Mouse accessor
    /// </summary>
    public class PlatformMouse
    {
        /// <summary>
        /// Gets mouse state.
        /// </summary>
        /// <param name="currentState">The current game state</param>
        /// <returns>The Mouse State Information</returns>
        public static MouseState GetState(State currentState)
        {
            var respState = new MouseState();

            //Get x and y
            SDL.SDL_GetMouseState(out var x, out var y);

            var winFlags = SDL.SDL_GetWindowFlags(Game.Instance.Window.Handle);

            var state = SDL.SDL_GetMouseState(out x, out y);

            //Get mouse buttons
            if ((winFlags & (uint)SDL.SDL_GetMouseFocus()) != 0)
            {
                respState.LeftButton = (state & SDL.SDL_BUTTON_LEFT) != 0 ? ButtonState.Pressed : ButtonState.Released;
                respState.MiddleButton = (state & SDL.SDL_BUTTON_MIDDLE) != 0 ? ButtonState.Pressed : ButtonState.Released;
                respState.RightButton = (state & SDL.SDL_BUTTON_RIGHT) != 0 ? ButtonState.Pressed : ButtonState.Released;
                respState.XButton1 = (state & SDL.SDL_BUTTON_X1MASK) != 0 ? ButtonState.Pressed : ButtonState.Released;
                respState.XButton2 = (state & SDL.SDL_BUTTON_X2MASK) != 0 ? ButtonState.Pressed : ButtonState.Released;
            }

            respState.X = x;
            respState.Y = y;

            //Return the mouse state
            return respState;
        }
    }
}

#endif