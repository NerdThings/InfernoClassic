using Inferno.Runtime.Core;
using Inferno.Runtime.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

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
        /// <param name="CurrentState">The current game state</param>
        /// <returns>The Mouse State Information</returns>
        public static MouseState GetMouseState(State CurrentState)
        {
            //Grab unmodified state
            MouseState s = Microsoft.Xna.Framework.Input.Mouse.GetState();

            Vector2 pos = new Vector2(s.X, s.Y);

            //Account for render target scaling
            int viewWidth = CurrentState.ParentGame.WindowWidth;
            int viewHeight = CurrentState.ParentGame.WindowHeight;

            float outputAspect = CurrentState.ParentGame.WindowWidth / (float)CurrentState.ParentGame.WindowHeight;
            float preferredAspect = CurrentState.ParentGame.VirtualWidth / (float)CurrentState.ParentGame.VirtualHeight;

            int barwidth = 0;
            int barheight = 0;

            if (outputAspect <= preferredAspect)
            {
                viewHeight = (int)((CurrentState.ParentGame.WindowWidth / preferredAspect) + 0.5f);
                barheight = (CurrentState.ParentGame.WindowHeight - viewHeight) / 2;
            }
            else
            {
                viewWidth = (int)((CurrentState.ParentGame.WindowHeight * preferredAspect) + 0.5f);
                barwidth = (CurrentState.ParentGame.WindowWidth - viewWidth) / 2;
            }

            //Apply modifications
            pos.X -= barwidth;
            pos.Y -= barheight;

            pos.X *= CurrentState.ParentGame.VirtualWidth;
            pos.X /= viewWidth;

            pos.Y *= CurrentState.ParentGame.VirtualHeight;
            pos.Y /= viewHeight;

            //Camera scaling
            Vector2 npos = CurrentState.Camera.ScreenToWorld(pos);

            //Return the modified mouse state
            return new MouseState((int)npos.X, (int)npos.Y, s.ScrollWheelValue, s.LeftButton, s.MiddleButton, s.RightButton, s.XButton1, s.XButton2);
        }
    }
}
