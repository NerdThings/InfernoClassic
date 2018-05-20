using Inferno.Runtime.Core;
using Inferno.Runtime.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inferno.UI.Controls
{
    public class Label : Instance, IControl
    {
        /// <summary>
        /// The Text value of this Label
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The Font that the label uses
        /// </summary>
        public SpriteFont Font { get; set; }

        /// <summary>
        /// The Fore Color of the Control
        /// </summary>
        public Color ForeColor { get; set; }

        /// <summary>
        /// The Back Color of the Control
        /// </summary>
        public Color BackColor { get; set; }

        /// <summary>
        /// The Control Border Color
        /// </summary>
        public Color BorderColor { get; set; }

        /// <summary>
        /// The Control Border Width
        /// </summary>
        public int BorderWidth { get; set; }

        /// <summary>
        /// The Bounds of the Control
        /// </summary>
        public Rectangle ControlBounds { get; set; }

        /// <summary>
        /// The Bounds of the Control
        /// </summary>
        public new Rectangle Bounds
        {
            get
            {
                return ControlBounds;
            }
        }

        public ControlState State
        {
            get
            {
                MouseState state = Inferno.Runtime.Input.Mouse.GetMouseState(ParentState);

                if (Bounds.Contains(new Vector2(state.X, state.Y)))
                {
                    return ControlState.Hover;
                }
                else
                {
                    return ControlState.None;
                }
            }
            set
            {
                //Do nothing
            }
        }

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
        public Label(Vector2 Position, State parentState, string Text, SpriteFont Font, Color TextColor, Color backgroundColor, Color BorderColor, int BorderWidth=1) : base(parentState, Position, 0, null, false, true)
        {
            this.Text = Text;
            this.Font = Font;
            this.ForeColor = TextColor;
            this.BackColor = backgroundColor;
            this.BorderColor = BorderColor;
            this.BorderWidth = BorderWidth;

            ControlBounds = new Rectangle((int)Position.X, (int)Position.Y, (int)Font.MeasureString(Text).X, (int)Font.MeasureString(Text).Y);
        }

        protected override void Draw()
        {
            //Update the control bounds
            ControlBounds = new Rectangle((int)Position.X, (int)Position.Y, (int)Font.MeasureString(Text).X, (int)Font.MeasureString(Text).Y);

            //Draw back color
            Drawing.Set_Color(BackColor);
            Drawing.Draw_Rectangle(Bounds);

            //Draw border
            int BorderStartX = (int)Position.X - BorderWidth;
            int BorderStartY = (int)Position.Y - BorderWidth;
            int BorderEndX = (int)Position.X + Bounds.Width + BorderWidth;
            int BorderEndY = (int)Position.Y + Bounds.Height + BorderWidth;

            Drawing.Set_Color(BorderColor);
            Drawing.Draw_Rectangle(new Rectangle(BorderStartX, BorderStartY, BorderEndX, BorderEndY), true, BorderWidth);

            //Draw text
            Drawing.Set_Font(Font);
            Drawing.Set_Color(ForeColor);
            Drawing.Draw_Text(Position, Text, 0);

            //Draw the rest of the instance
            base.Draw();
        }
    }
}
