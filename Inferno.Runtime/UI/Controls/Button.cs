using Inferno.Runtime.Core;
using Inferno.Runtime.Graphics;

namespace Inferno.Runtime.UI.Controls
{
    public class Button : Control
    {
        /// <inheritdoc />
        /// <summary>
        /// Create a new Button without a background or border and with a default font color of black
        /// </summary>
        /// <param name="position"></param>
        /// <param name="parentState"></param>
        /// <param name="text"></param>
        /// <param name="font"></param>
        public Button(Vector2 position, State parentState, string text, SpriteFont font) : this(position, parentState, text, font, Color.Black, Color.Transparent, Color.Transparent, 0) { }

        /// <inheritdoc />
        /// <summary>
        /// Create a new Button without a background or border
        /// </summary>
        /// <param name="position"></param>
        /// <param name="parentState"></param>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="textColor"></param>
        public Button(Vector2 position, State parentState, string text, SpriteFont font, Color textColor) : this(position, parentState, text, font, textColor, Color.Transparent, Color.Transparent, 0) { }

        /// <summary>
        /// Create a new Button
        /// </summary>
        /// <param name="position"></param>
        /// <param name="parentState"></param>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="textColor"></param>
        /// <param name="backgroundColor"></param>
        /// <param name="borderColor"></param>
        /// <param name="borderWidth"></param>
        /// <param name="backgroundImage"></param>
        public Button(Vector2 position, State parentState, string text, SpriteFont font, Color textColor, Color backgroundColor, Color borderColor, int borderWidth = 1, Sprite backgroundImage = null) : base(parentState, position)
        {
            Text = text;
            TextFont = font;
            ForeColor = textColor;
            BackColor = backgroundColor;
            BorderColor = borderColor;
            BorderWidth = borderWidth;
            Background = backgroundImage;
            Position = position;

            if (font == null)
            {
                Width = 0;
                Height = 0;
            }
            else
            {
                Width = (int)font.MeasureString(text).X;
                Height = (int)font.MeasureString(text).Y;
            }
        }
    }
}
