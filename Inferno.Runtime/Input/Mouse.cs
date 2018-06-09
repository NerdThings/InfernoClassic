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

            if (game.Window.ClientBounds.Width != game.VirtualWidth)
            {
                int presentHeight = (int)((game.Window.ClientBounds.Width / preferredAspect) + 0.5f);
                barHeight = (game.Window.ClientBounds.Height - presentHeight) / 2;
            }

            if (game.Window.ClientBounds.Height != game.VirtualHeight)
            {
                int presentWidth = (int)((game.Window.ClientBounds.Height * preferredAspect) + 0.5f);
                barWidth = (game.Window.ClientBounds.Width - presentWidth) / 2;
            }

            if (barHeight < 0)
                barHeight = 0;

            if (barWidth < 0)
                barWidth = 0;

            Vector2 Modifier = new Vector2(-barWidth, -barHeight);

            pos += Modifier;

            //Account for scaling
            float XScale = (float)game.VirtualWidth / (game.Window.ClientBounds.Width - (barWidth * 2f));
            float YScale = (float)game.VirtualHeight / (game.Window.ClientBounds.Height - (barHeight * 2f));

            pos.X *= XScale;
            pos.Y *= YScale;

            //pos *= outputAspect;

            //Camera scaling
            Vector2 npos = CurrentState.Camera.ScreenToWorld(pos);

            return new MouseState((int)npos.X, (int)npos.Y, s.ScrollWheelValue, s.LeftButton, s.MiddleButton, s.RightButton, s.XButton1, s.XButton2);
        }
    }
}
