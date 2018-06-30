using Inferno.Runtime.Core;
using Inferno.Runtime.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inferno.Runtime.UI.Controls
{
    public class Label : Control
    {
        /// <summary>
        /// Create a new Label without a background or border and with a default font color of black
        /// </summary>
        /// <param name="Position"></param>
        /// <param name="parentState"></param>
        /// <param name="Message"></param>
        /// <param name="Font"></param>
        public Label(Vector2 Position, State parentState, string Message, SpriteFont Font) : this(Position, parentState, Message, Font, Color.Black, Color.Transparent, Color.Transparent, 0) { }

        /// <summary>
        /// Create a new Label without a background or border
        /// </summary>
        /// <param name="Position"></param>
        /// <param name="parentState"></param>
        /// <param name="Message"></param>
        /// <param name="Font"></param>
        /// <param name="TextColor"></param>
        public Label(Vector2 Position, State parentState, string Message, SpriteFont Font, Color TextColor) : this(Position, parentState, Message, Font, TextColor, Color.Transparent, Color.Transparent, 0) { }

        /// <summary>
        /// Create a new Label
        /// </summary>
        /// <param name="Position"></param>
        /// <param name="parentState"></param>
        /// <param name="Text"></param>
        /// <param name="Font"></param>
        /// <param name="TextColor"></param>
        /// <param name="backgroundColor"></param>
        /// <param name="BorderColor"></param>
        /// <param name="BorderWidth"></param>
        public Label(Vector2 Position, State parentState, string Text, SpriteFont Font, Color TextColor, Color backgroundColor, Color BorderColor, int BorderWidth = 1, Sprite BackgroundImage = null) : base(parentState, Position)
        {
            this.Text = Text;
            this.TextFont = Font;
            this.ForeColor = TextColor;
            this.BackColor = backgroundColor;
            this.BorderColor = BorderColor;
            this.BorderWidth = BorderWidth;
            this.Background = BackgroundImage;
            this.Position = Position;

            Width = (int)Font.MeasureString(Text).X;
            Height = (int)Font.MeasureString(Text).Y;
        }
    }
}
