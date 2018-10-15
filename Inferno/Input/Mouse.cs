using Inferno.Core;

namespace Inferno.Input
{
    /// <summary>
    /// Mouse input
    /// </summary>
    public class Mouse
    {
        internal static int ScrollY;
        internal static int ScrollX;

        public static MouseState GetState()
        {
            var mouseState = PlatformMouse.GetState();

            var pos = new Vector2(mouseState.X, mouseState.Y);

            //Account for render target scaling
            var viewWidth = Game.Instance.Window.Width;
            var viewHeight = Game.Instance.Window.Height;

            var outputAspect = Game.Instance.Window.Width / (float)Game.Instance.Window.Height;
            var preferredAspect = Game.Instance.VirtualWidth / (float)Game.Instance.VirtualHeight;

            var barwidth = 0;
            var barheight = 0;

            if (outputAspect <= preferredAspect)
            {
                viewHeight = (int)(Game.Instance.Window.Width / preferredAspect + 0.5f);
                barheight = (Game.Instance.Window.Height - viewHeight) / 2;
            }
            else
            {
                viewWidth = (int)(Game.Instance.Window.Height * preferredAspect + 0.5f);
                barwidth = (Game.Instance.Window.Width - viewWidth) / 2;
            }

            //Apply modifications
            pos.X -= barwidth;
            pos.Y -= barheight;

            pos.X *= Game.Instance.VirtualWidth;
            pos.X /= viewWidth;

            pos.Y *= Game.Instance.VirtualHeight;
            pos.Y /= viewHeight;

            mouseState.X = (int)pos.X;
            mouseState.Y = (int)pos.Y;

            return mouseState;
        }

        public static MouseState GetState(State currentState)
        {
            var mouseState = GetState();

            var pos = new Vector2(mouseState.X, mouseState.Y);
            
            //Camera scaling
            var npos = currentState.Camera.ScreenToWorld(pos);

            mouseState.X = (int)npos.X;
            mouseState.Y = (int)npos.Y;

            return mouseState;
        }
    }
}
