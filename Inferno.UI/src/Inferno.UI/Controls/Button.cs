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
    public class Button : Instance, IControl
    {
        /// <summary>
        /// The Text value of this Button
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The Font that the Button uses
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

        public ControlState State { get; set; }

        /// <summary>
        /// Create a new Button
        /// </summary>
        /// <param name="Position"></param>
        /// <param name="parentState"></param>
        /// <param name="Text"></param>
        /// <param name="Font"></param>
        /// <param name="TextColor"></param>
        /// <param name="backgroundColor"></param>
        /// <param name="BorderColor"></param>
        /// <param name="BorderWidth"></param>
        public Button(Vector2 Position, State parentState, string Text, SpriteFont Font, Color TextColor, Color backgroundColor, Color BorderColor, int BorderWidth = 1) : base(parentState, Position, 0, null, true, true)
        {
            this.Text = Text;
            this.Font = Font;
            this.ForeColor = TextColor;
            this.BackColor = backgroundColor;
            this.BorderColor = BorderColor;
            this.BorderWidth = BorderWidth;

            ControlBounds = new Rectangle((int)Position.X, (int)Position.Y, (int)Font.MeasureString(Text).X, (int)Font.MeasureString(Text).Y);

            State = ControlState.None;
        }

        protected override void Draw()
        {
            //Update the control bounds
            ControlBounds = new Rectangle((int)Position.X, (int)Position.Y, (int)Font.MeasureString(Text).X, (int)Font.MeasureString(Text).Y);

            //Draw back color
            Drawing.Set_Color(BackColor);
            Drawing.Draw_Rectangle(Bounds);

            //Add a darker highlight
            if (State == ControlState.Hover)
            {
                Drawing.Set_Color(Color.Black);
                Drawing.Set_Alpha(0.2f);
                Drawing.Draw_Rectangle(Bounds);
                Drawing.Set_Alpha(1);
            }

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

            base.Draw();
        }

        protected override void Update(GameTime gameTime)
        {
            MouseState state = Inferno.Runtime.Input.Mouse.GetMouseState(ParentState);

            if (Bounds.Contains(new Vector2(state.X, state.Y)))
            {
                if (state.LeftButton == ButtonState.Pressed)
                {
                    ButtonClicked?.Invoke();
                    State = ControlState.Click;
                }
                else
                {
                    ButtonHovered?.Invoke();
                    State = ControlState.Hover;
                }
            }
            else
            {
                State = ControlState.None;
            }

            base.Update(gameTime);
        }

        public delegate void ButtonClickedEvent();
        public delegate void ButtonHoveredEvent();

        public event ButtonClickedEvent ButtonClicked;
        public event ButtonHoveredEvent ButtonHovered;
    }
}
