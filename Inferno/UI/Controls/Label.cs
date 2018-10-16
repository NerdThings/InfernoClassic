using Inferno.Core;
using Inferno.Graphics;
using Inferno.Graphics.Text;

namespace Inferno.UI.Controls
{
    public class Label : Control
    {
        /// <inheritdoc />
        /// <summary>
        /// Create a new Label without a background or border and with a default font color of black
        /// </summary>
        /// <param name="position"></param>
        /// <param name="message"></param>
        /// <param name="font"></param>
        public Label(Vector2 position, string message, Font font) : this(position, message, font, Color.Black, Color.Transparent, Color.Transparent, 0) { }

        /// <inheritdoc />
        /// <summary>
        /// Create a new Label without a background or border
        /// </summary>
        /// <param name="position"></param>
        /// <param name="message"></param>
        /// <param name="font"></param>
        /// <param name="textColor"></param>
        public Label(Vector2 position, string message, Font font, Color textColor) : this(position, message, font, textColor, Color.Transparent, Color.Transparent, 0) { }

        /// <summary>
        /// Create a new Label
        /// </summary>
        /// <param name="position"></param>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="textColor"></param>
        /// <param name="backgroundColor"></param>
        /// <param name="borderColor"></param>
        /// <param name="borderWidth"></param>
        /// <param name="backgroundImage"></param>
        public Label(Vector2 position, string text, Font font, Color textColor,
            Color backgroundColor, Color borderColor, int borderWidth = 1, Sprite backgroundImage = null)
            : base(position, text, font, textColor, backgroundColor, borderColor, borderWidth, backgroundImage,
                (int) font.MeasureString(text).X, (int) font.MeasureString(text).Y)
        {
            OnTextChanged += (sender, args) =>
            {
                if (TextFont == null) 
                    return;
                
                var size = TextFont.MeasureString(args.Text);
                Width = (int) size.X;
                Height = (int) size.Y;
            };
        }
    }
}
