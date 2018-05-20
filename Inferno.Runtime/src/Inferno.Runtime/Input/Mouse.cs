using Inferno.Runtime.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inferno.Runtime.Input
{
    public class Mouse
    {
        /// <summary>
        /// Gets mouse state and modifies to work with camera
        /// </summary>
        /// <param name="CurrentState"></param>
        /// <returns></returns>
        public static MouseState GetMouseState(State CurrentState)
        {
            MouseState s = Microsoft.Xna.Framework.Input.Mouse.GetState();

            var x = s.X;
            var y = s.Y;

            Game game = CurrentState.ParentGame;

            Vector2 pos = new Vector2(x, y);

            //Account for black bars
            float outputAspect = game.Window.ClientBounds.Width / (float)game.Window.ClientBounds.Height;
            float preferredAspect = game.VirtualWidth / (float)game.VirtualHeight;

            int barHeight = 0;
            int barWidth = 0;

            if (outputAspect <= preferredAspect)
            {
                // output is taller than it is wider, bars on top/bottom
                int presentHeight = (int)((game.Window.ClientBounds.Width / preferredAspect) + 0.5f);
                barHeight = (game.Window.ClientBounds.Height - presentHeight) / 2;

                pos.Y -= barHeight;
            }
            else
            {
                // output is wider than it is tall, bars left/right
                int presentWidth = (int)((game.Window.ClientBounds.Height * preferredAspect) + 0.5f);
                barWidth = (game.Window.ClientBounds.Width - presentWidth) / 2;

                pos.X -= barWidth;
            }

            //Account for scaling
            float XScale = (game.Window.ClientBounds.Width - (barWidth * 2f)) / (float)game.VirtualWidth;
            float YScale = (game.Window.ClientBounds.Height - (barHeight * 2f)) / (float)game.VirtualHeight;

            Console.WriteLine(XScale);
            Console.WriteLine(YScale);

            pos.X /= XScale;
            pos.Y /= YScale;

            //Camera scaling
            Vector2 npos = CurrentState.Camera.ScreenToWorld(pos);

            return new MouseState((int)npos.X, (int)npos.Y, s.ScrollWheelValue, s.LeftButton, s.MiddleButton, s.RightButton, s.XButton1, s.XButton2);
        }
    }
}
