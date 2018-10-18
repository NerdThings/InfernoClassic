using Inferno.Graphics;
using Inferno.Input;

namespace Inferno.UI.Controls
{
    /// <summary>
    /// This control adds a custom cursor image
    /// </summary>
    public class Cursor : Control
    {
        /// <summary>
        /// Create a new Custom Cursor
        /// </summary>
        /// <param name="cursorSprite">Sprite for the cursor</param>
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