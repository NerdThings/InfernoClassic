using Inferno.Runtime.Core;
using Inferno.Runtime.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inferno.Runtime.UI.Controls
{
    /// <summary>
    /// The state of the control
    /// </summary>
    public enum ControlState
    {
        None, Hover, Click
    }

    /// <summary>
    /// This interface implements new manditory additions which aren't present in Instance
    /// </summary>
    public abstract class Control : Instance
    {
        /// <summary>
        /// The forecolor
        /// </summary>
        public Color ForeColor = Color.White;

        /// <summary>
        /// The back color
        /// </summary>
        public Color BackColor = Color.Black;

        /// <summary>
        /// The border color
        /// </summary>
        public Color BorderColor = Color.Black;

        /// <summary>
        /// The border width
        /// </summary>
        public int BorderWidth = 1;

        /// <summary>
        /// The control state
        /// </summary>
        public ControlState State = ControlState.None;

        /// <summary>
        /// The background applied to the control
        /// </summary>
        public Sprite Background = null;

        /// <summary>
        /// The tab index (-1 = not indexed)
        /// </summary>
        public int TabIndex = -1;

        /// <summary>
        /// The font applied to text
        /// </summary>
        public SpriteFont TextFont = null;

        /// <summary>
        /// The text to be drawn at the center of the Control
        /// </summary>
        public string Text = "";

        public Control(State parent, Vector2 position) : base(parent, position, 0, null, true, true) { }

        protected override void Draw()
        {
            //Draw back color
            Drawing.Set_Color(BackColor);
            Drawing.Draw_Rectangle(Bounds);

            if (Background != null)
            {
                Drawing.Draw_Sprite(Position, Background);
            }

            //Add a darker highlight
            if (State == ControlState.Hover)
            {
                Drawing.Set_Color(Color.Black);
                Drawing.Set_Alpha(0.2f);
                Drawing.Draw_Rectangle(Bounds);
                Drawing.Set_Alpha(1);
            }
            else if (State == ControlState.Click)
            {
                Drawing.Set_Color(Color.Black);
                Drawing.Set_Alpha(0.4f);
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

            if (TextFont != null)
            {
                //Draw text
                Drawing.Set_Font(TextFont);

                Vector2 TextSize = TextFont.MeasureString(Text);

                Drawing.Set_Color(ForeColor);
                Drawing.Draw_Text(new Vector2(Bounds.X, Bounds.Y), Text);
            }

            base.Draw();
        }

        protected override void Update(GameTime gameTime)
        {
            //Grab mouse
            MouseState state = Input.Mouse.GetMouseState(ParentState);

            //Do state checks
            if (Bounds.Contains(new Vector2(state.X, state.Y)))
            {
                if (state.LeftButton == ButtonState.Pressed)
                {
                    ControlClicked?.Invoke();
                    State = ControlState.Click;
                }
                else
                {
                    ControlHovered?.Invoke();
                    State = ControlState.Hover;
                }
            }
            else
            {
                State = ControlState.None;
            }

            base.Update(gameTime);
        }

        public delegate void ControlClickedEvent();
        public delegate void ControlHoveredEvent();

        public event ControlClickedEvent ControlClicked;
        public event ControlHoveredEvent ControlHovered;
    }
}
