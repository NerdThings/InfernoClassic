using System.Net;

namespace Inferno.Input
{
    /// <summary>
    /// Mouse input
    /// </summary>
    public static partial class Mouse
    {
        internal static int ScrollY;
        internal static int ScrollX;
        internal static bool LeftButton;
        internal static bool RightButton;
        internal static bool MiddleButton;
        internal static bool XButton1;
        internal static bool XButton2;

        public static void ClearLeftButton()
        {
            LeftButton = false;
        }

        public static void ClearRightButton()
        {
            RightButton = false;
        }

        public static void ClearMiddleButton()
        {
            MiddleButton = false;
        }

        public static void ClearXButton1()
        {
            XButton1 = false;
        }

        public static void ClearXButton2()
        {
            XButton2 = false;
        }

        private static MouseState Scale(MouseState state)
        {
            //Account for render target scaling
            var pos = new Vector2(state.X, state.Y);
            
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

            state.X = (int)pos.X;
            state.Y = (int)pos.Y;
            
            return state;
        }

        private static MouseState ScaleCamera(MouseState state, GameState currentState)
        {
            var pos = new Vector2(state.X, state.Y);
            
            //Camera scaling
            var npos = currentState.Camera.ScreenToWorld(pos);

            state.X = (int)npos.X;
            state.Y = (int)npos.Y;
            
            return state;
        }

        //TODO: No Current State parameter
        public static MouseState GetState(GameState currentState)
        {
            var mouseState = GetState();

            mouseState = ScaleCamera(mouseState, currentState);

            return mouseState;
        }
    }
}
