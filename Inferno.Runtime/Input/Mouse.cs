using Inferno.Runtime.Core;

namespace Inferno.Runtime.Input
{
    public class Mouse
    {
        public static MouseState GetState(State currentState)
        {
            var mouseState = PlatformMouse.GetState(currentState);

            var pos = new Vector2(mouseState.X, mouseState.Y);

            //Account for render target scaling
            var viewWidth = currentState.ParentGame.Window.Width;
            var viewHeight = currentState.ParentGame.Window.Height;

            var outputAspect = currentState.ParentGame.Window.Width / (float)currentState.ParentGame.Window.Height;
            var preferredAspect = currentState.ParentGame.VirtualWidth / (float)currentState.ParentGame.VirtualHeight;

            var barwidth = 0;
            var barheight = 0;

            if (outputAspect <= preferredAspect)
            {
                viewHeight = (int)(currentState.ParentGame.Window.Width / preferredAspect + 0.5f);
                barheight = (currentState.ParentGame.Window.Height - viewHeight) / 2;
            }
            else
            {
                viewWidth = (int)(currentState.ParentGame.Window.Height * preferredAspect + 0.5f);
                barwidth = (currentState.ParentGame.Window.Width - viewWidth) / 2;
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

            mouseState.X = (int)npos.X;
            mouseState.Y = (int)npos.Y;
            return mouseState;
        }
    }
}
