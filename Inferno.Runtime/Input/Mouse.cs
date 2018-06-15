using Inferno.Runtime.Core;
using Inferno.Runtime.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inferno.Runtime.Input
{
    //TODO: Get this working soon
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

            //Game scaling
            int viewWidth = game.WindowWidth;
            int viewHeight = game.WindowHeight;

            float outputAspect = game.WindowWidth / (float)game.WindowHeight;
            float preferredAspect = game.VirtualWidth / (float)game.VirtualHeight;

            int barwidth = 0;
            int barheight = 0;

            if (outputAspect <= preferredAspect)
            {
                viewHeight = (int)((game.WindowWidth / preferredAspect) + 0.5f);
                barheight = (game.WindowHeight - viewHeight) / 2;
            }
            else
            {
                viewWidth = (int)((game.WindowHeight * preferredAspect) + 0.5f);
                barwidth = (game.WindowWidth - viewWidth) / 2;
            }

            pos.X -= barwidth;
            pos.Y -= barheight;

            pos.X *= game.VirtualWidth;
            pos.X /= viewWidth;

            pos.Y *= game.VirtualHeight;
            pos.Y /= viewHeight;

            //Camera scaling
            Vector2 npos = CurrentState.Camera.ScreenToWorld(pos);

            return new MouseState((int)npos.X, (int)npos.Y, s.ScrollWheelValue, s.LeftButton, s.MiddleButton, s.RightButton, s.XButton1, s.XButton2);
        }
    }
}
