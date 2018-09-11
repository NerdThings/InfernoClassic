using System;
using Inferno.Runtime.Core;
using Inferno.Runtime.Graphics;
using Inferno.Runtime.Input;

namespace Inferno.Runtime.UI.Controls
{
    /// <summary>
    /// The state of the control
    /// </summary>
    public enum ControlState
    {
        None, Hover, Click
    }

    /// <inheritdoc />
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

        public override void Draw(Renderer renderer)
        {
            //Draw back color
            Drawing.Set_Color(BackColor);
            Drawing.Draw_Rectangle(Bounds);

            if (Background != null)
            {
                Drawing.Draw_Sprite(Position, Background);
            }

            switch (State)
            {
                //Add a darker highlight
                case ControlState.Hover:
                    Drawing.Set_Color(Color.Black);
                    Drawing.Set_Alpha(0.2f);
                    Drawing.Draw_Rectangle(Bounds);
                    Drawing.Set_Alpha(1);
                    break;
                case ControlState.Click:
                    Drawing.Set_Color(Color.Black);
                    Drawing.Set_Alpha(0.4f);
                    Drawing.Draw_Rectangle(Bounds);
                    Drawing.Set_Alpha(1);
                    break;
                case ControlState.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            //Draw border
            var borderStartX = (int)Position.X - BorderWidth;
            var borderStartY = (int)Position.Y - BorderWidth;
            var borderEndX = (int)Position.X + Bounds.Width + BorderWidth;
            var borderEndY = (int)Position.Y + Bounds.Height + BorderWidth;

            Drawing.Set_Color(BorderColor);
            Drawing.Draw_Rectangle(new Rectangle(borderStartX, borderStartY, borderEndX, borderEndY), true, BorderWidth);

            if (TextFont != null)
            {
                //Draw text
                Drawing.Set_Font(TextFont);

                Drawing.Set_Color(ForeColor);
                Drawing.Draw_Text(new Vector2(Bounds.X, Bounds.Y), Text);
            }

            base.Draw(renderer);
        }

        public override void Update(float delta)
        {
            //Grab mouse
            var state = Mouse.GetState(ParentState);

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

            base.Update(delta);
        }

        public delegate void ControlClickedEvent();
        public delegate void ControlHoveredEvent();

        public event ControlClickedEvent ControlClicked;
        public event ControlHoveredEvent ControlHovered;
    }
}
