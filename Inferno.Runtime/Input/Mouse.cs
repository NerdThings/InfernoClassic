using Inferno.Runtime.Core;
using Microsoft.Xna.Framework.Input;

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
            //This currently converts the Monogame state to the inferno state, this will change in phase 2

            //Grab unmodified state
            var s = Microsoft.Xna.Framework.Input.Mouse.GetState();

            var pos = new Vector2(s.X, s.Y);

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

            //Enum conversion (Ugly, but only till phase 2)
            var left = ButtonState.Released;
            var middle = ButtonState.Released;
            var right = ButtonState.Released;
            var x1 = ButtonState.Released;
            var x2 = ButtonState.Released;

            if (s.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                left = ButtonState.Pressed;
            if (s.MiddleButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                middle = ButtonState.Pressed;
            if (s.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                right = ButtonState.Pressed;
            if (s.XButton1 == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                x1 = ButtonState.Pressed;
            if (s.XButton2 == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                x2 = ButtonState.Pressed;

            //Return the modified mouse state
            return new MouseState((int)npos.X, (int)npos.Y, s.ScrollWheelValue, left, middle, right, x1, x2);
        }
    }
}
