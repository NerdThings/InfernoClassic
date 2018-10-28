#if SDL

using SDL2;

namespace Inferno.Input
{
    /// <summary>
    /// Desktop specific mouse code
    /// </summary>
    public partial class Mouse
    {
        /// <summary>
        /// Gets mouse state.
        /// </summary>
        /// <returns>The Mouse State Information</returns>
        public static MouseState GetState()
        {
            var state = Game.Instance.Window.MouseState;

            //Get x and y
            SDL.SDL_GetGlobalMouseState(out var x, out var y);

            var winFlags = SDL.SDL_GetWindowFlags(Game.Instance.Window.Handle);

            SDL.SDL_GetMouseState(out x, out y);

            //Get mouse buttons
            if ((winFlags & 0x00000400) != 0)
            {
                state.LeftButton = LeftButton ? ButtonState.Pressed : ButtonState.Released;
                state.MiddleButton = MiddleButton ? ButtonState.Pressed : ButtonState.Released;
                state.RightButton = RightButton ? ButtonState.Pressed : ButtonState.Released;
                state.XButton1 = XButton1 ? ButtonState.Pressed : ButtonState.Released;
                state.XButton2 = XButton2 ? ButtonState.Pressed : ButtonState.Released;

                state.ScrollWheelValue = Mouse.ScrollX;
            }
            else
            {
                var clientBounds = Game.Instance.Window.Bounds;
                Game.Instance.Window.MouseState.X = x - clientBounds.X;
                Game.Instance.Window.MouseState.Y = y - clientBounds.Y;
            }

            Game.Instance.Window.MouseState = state;
            
            //Scale
            state = Scale(state);

            //Return the mouse state
            return state;
        }
    }
}

#endif