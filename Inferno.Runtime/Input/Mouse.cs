using System;
using Inferno.Runtime.Core;
using SDL2;

namespace Inferno.Runtime.Input
{
    /// <summary>
    /// Mouse accessor
    /// </summary>
    public class Mouse
    {
        /// <summary>
        /// Gets mouse state.
        /// </summary>
        /// <param name="currentState">The current game state</param>
        /// <returns>The Mouse State Information</returns>
        public static MouseState GetMouseState(State currentState)
        {
            var respState = new MouseState();
            //Get x and y
            SDL.SDL_GetMouseState(out var x, out var y);

            var winFlags = SDL.SDL_GetWindowFlags((IntPtr)0); //TODO: Actual pointer

            var state = SDL.SDL_GetMouseState(out x, out y);

            //Get mouse buttons
            if ((winFlags & (uint)SDL.SDL_GetMouseFocus()) != 0)
            {
                respState.LeftButton = (state & SDL.SDL_BUTTON_LEFT) != 0 ? ButtonState.Pressed : ButtonState.Released;
                respState.MiddleButton = (state & SDL.SDL_BUTTON_MIDDLE) != 0 ? ButtonState.Pressed : ButtonState.Released;
                respState.RightButton = (state & SDL.SDL_BUTTON_RIGHT) != 0 ? ButtonState.Pressed : ButtonState.Released;
                respState.XButton1 = (state & SDL.SDL_BUTTON_X1MASK) != 0 ? ButtonState.Pressed : ButtonState.Released;
                respState.XButton2 = (state & SDL.SDL_BUTTON_X2MASK) != 0 ? ButtonState.Pressed : ButtonState.Released;
                SDL.
            }

            var pos = new Vector2(x, y);

            //TODO: Apply clientbounds

            //Account for render target scaling
            var viewWidth = currentState.ParentGame.WindowWidth;
            var viewHeight = currentState.ParentGame.WindowHeight;

            var outputAspect = currentState.ParentGame.WindowWidth / (float)currentState.ParentGame.WindowHeight;
            var preferredAspect = currentState.ParentGame.VirtualWidth / (float)currentState.ParentGame.VirtualHeight;

            var barwidth = 0;
            var barheight = 0;

            if (outputAspect <= preferredAspect)
            {
                viewHeight = (int)((currentState.ParentGame.WindowWidth / preferredAspect) + 0.5f);
                barheight = (currentState.ParentGame.WindowHeight - viewHeight) / 2;
            }
            else
            {
                viewWidth = (int)((currentState.ParentGame.WindowHeight * preferredAspect) + 0.5f);
                barwidth = (currentState.ParentGame.WindowWidth - viewWidth) / 2;
            }

            //Apply modifications
            pos.X -= barwidth;
            pos.Y -= barheight;

            pos.X *= currentState.ParentGame.VirtualWidth;
            pos.X /= viewWidth;

            pos.Y *= currentState.ParentGame.VirtualHeight;
            pos.Y /= viewHeight;

            //Camera scaling
            var npos = currentState.Camera.ScreenToWorld(pos);

            respState.X = (int) npos.X;
            respState.Y = (int) npos.Y;

            //Return the modified mouse state
            return respState;
        }
    }
}
