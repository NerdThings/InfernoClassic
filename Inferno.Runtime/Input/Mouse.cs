using Inferno.Runtime.Core;
using Microsoft.Xna.Framework.Input;

namespace Inferno.Runtime.Input
{
    /// <summary>
    /// A Mouse helper, this repairs damages done by the Game scaling and Cameras
    /// </summary>
    public class Mouse
    {
        /// <summary>
        /// Gets mouse state and modifies to work with camera
        /// </summary>
        /// <param name="currentState">The current game state</param>
        /// <returns>The Mouse State Information</returns>
        public static MouseState GetMouseState(State currentState)
        {
            //TODO: Make a custom mouse state
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

            //Return the modified mouse state
            return new MouseState((int)npos.X, (int)npos.Y, s.ScrollWheelValue, s.LeftButton, s.MiddleButton, s.RightButton, s.XButton1, s.XButton2);
        }
    }
}
