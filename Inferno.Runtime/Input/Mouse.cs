using System;
using Inferno.Runtime.Core;

namespace Inferno.Runtime.Input
{
    public class Mouse
    {
        internal static int ScrollY;
        internal static int ScrollX;

        public static MouseState GetState(State currentState)
        {
            var mouseState = PlatformMouse.GetState(currentState);

            Console.WriteLine(mouseState.X + "," + mouseState.Y);

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

            Console.WriteLine("A" + barwidth);
            Console.WriteLine("B" + barheight);
            Console.WriteLine("C" + viewWidth);
            Console.WriteLine("D" + viewHeight);

            pos.X *= currentState.ParentGame.VirtualWidth;
            pos.X /= viewWidth;

            pos.Y *= currentState.ParentGame.VirtualHeight;
            pos.Y /= viewHeight;

            //Camera scaling
            var npos = currentState.Camera.ScreenToWorld(pos);

            Console.WriteLine("XXX" + npos.X + "," + npos.Y);

            mouseState.X = (int)npos.X;
            mouseState.Y = (int)npos.Y;

            Console.WriteLine(mouseState.X + "," + mouseState.Y);

            return mouseState;
        }
    }
}
