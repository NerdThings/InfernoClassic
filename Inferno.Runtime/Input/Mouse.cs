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

            //Camera scaling
            Vector2 npos = CurrentState.Camera.ScreenToWorld(pos);

            return new MouseState((int)npos.X, (int)npos.Y, s.ScrollWheelValue, s.LeftButton, s.MiddleButton, s.RightButton, s.XButton1, s.XButton2);
        }
    }
}
