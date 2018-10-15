using System;
using Inferno.Core;
using Inferno.Graphics;
using Inferno.Graphics.Text;
using Inferno.Input;

namespace Inferno.UI.Controls
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
    //TODO: Make separate from Instance class (Allowing full independancy from the core engine)
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
        public Font TextFont = null;

        /// <summary>
        /// The text to be drawn at the center of the Control
        /// </summary>
        public string Text = "";

        /// <summary>
        /// The coordinate offset for mouse input
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public Vector2 UIOffset = Vector2.Zero;

        public Control(State parent, Vector2 position) : base(parent, position, 0, null, true, true) { }

        public override void Draw(Renderer renderer)
        {
            //Draw back color
            renderer.DrawRectangle(Bounds, BackColor);

            if (Background != null)
            {
                renderer.Draw(Background, Position);
            }

            switch (State)
            {
                //Add a darker highlight
                case ControlState.Hover:
                    renderer.DrawRectangle(Bounds, Color.Black * 0.2f);
                    break;
                case ControlState.Click:
                    renderer.DrawRectangle(Bounds, Color.Black * 0.4f);
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

            renderer.DrawRectangle(new Rectangle(borderStartX, borderStartY, borderEndX, borderEndY), BorderColor, 0f, false);

            if (TextFont != null)
            {
                //Draw text
                renderer.DrawText(Text, new Vector2(Bounds.X, Bounds.Y), TextFont, ForeColor);
            }
        }

        public override void Update()
        {
            //Grab mouse
            var state = Mouse.GetState();

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

            base.Update();
        }

        public delegate void ControlClickedEvent();
        public delegate void ControlHoveredEvent();

        public event ControlClickedEvent ControlClicked;
        public event ControlHoveredEvent ControlHovered;
    }
}
