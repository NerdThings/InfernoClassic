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

            var mState = SDL.SDL_GetMouseState(out x, out y);

            //Get mouse buttons
            if ((winFlags & 0x00000400) != 0)
            {
                state.LeftButton = (mState & 1 << 0) != 0 ? ButtonState.Pressed : ButtonState.Released;
                state.MiddleButton = (mState & 1 << 1) != 0 ? ButtonState.Pressed : ButtonState.Released;
                state.RightButton = (mState & 1 << 2) != 0 ? ButtonState.Pressed : ButtonState.Released;
                state.XButton1 = (mState & 1 << 3) != 0 ? ButtonState.Pressed : ButtonState.Released;
                state.XButton2 = (mState & 1 << 4) != 0 ? ButtonState.Pressed : ButtonState.Released;

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