using Inferno.Graphics;
using Inferno.Graphics.Text;

namespace Inferno.UI.Controls
{
    public class Button : Control
    {
        /// <inheritdoc />
        /// <summary>
        /// Create a new Button without a background or border and with a default font color of black
        /// </summary>
        /// <param name="position"></param>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="highlightOnHover"></param>
        public Button(Vector2 position, string text, Font font, bool highlightOnHover = true) : this(position, text, font, Color.Black, Color.Transparent, Color.Transparent, 0, null, highlightOnHover) { }

        /// <inheritdoc />
        /// <summary>
        /// Create a new Button without a background or border
        /// </summary>
        /// <param name="position"></param>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="textColor"></param>
        /// <param name="highlightOnHover"></param>
        public Button(Vector2 position, string text, Font font, Color textColor, bool highlightOnHover = true) : this(position, text, font, textColor, Color.Transparent, Color.Transparent, 0, null, highlightOnHover) { }

        /// <summary>
        /// Create a new Button
        /// </summary>
        /// <param name="position"></param>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="textColor"></param>
        /// <param name="backgroundColor"></param>
        /// <param name="borderColor"></param>
        /// <param name="borderWidth"></param>
        /// <param name="backgroundImage"></param>
        /// <param name="highlightOnHover"></param>
        public Button(Vector2 position, string text, Font font, Color textColor, Color backgroundColor, Color borderColor, int borderWidth = 1, Sprite backgroundImage = null, bool highlightOnHover = true)
            : base(position, text, font, textColor, backgroundColor, borderColor, borderWidth, backgroundImage,
                (int) font.MeasureString(text).X, (int) font.MeasureString(text).Y, highlightOnHover)
        {
        }
    }
}
