﻿using Inferno.Graphics;
using Inferno.Input;

namespace Inferno.UI.Controls
{
    public class Cursor : Control
    {
        public Cursor(Sprite cursorSprite) :
            base(null, "", null, null, null, null, 1, cursorSprite,
                cursorSprite.Width, cursorSprite.Height, false)
        {

        }

        public override void Update()
        {
            var mState = Mouse.GetState();

            Position = new Vector2(mState.X, mState.Y);

            base.Update();
        }
    }
}