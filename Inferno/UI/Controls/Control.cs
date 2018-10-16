using System;
using System.Text;
using Inferno.Core;
using Inferno.Graphics;
using Inferno.Graphics.Text;
using Inferno.Input;
using OpenTK.Graphics.ES20;

namespace Inferno.UI.Controls
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
    public abstract class Control
    {
        public Vector2 Position;
        public int Width;
        public int Height;
        public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, Width, Height);

        /// <summary>
        /// The forecolor
        /// </summary>
        public Color ForeColor;

        /// <summary>
        /// The back color
        /// </summary>
        public Color BackColor;

        /// <summary>
        /// The border color
        /// </summary>
        public Color BorderColor;

        /// <summary>
        /// The border width
        /// </summary>
        public int BorderWidth;

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
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                OnTextChanged?.Invoke(this, new TextChangedEventArgs(value));
            }
        }        
        private string _text;

        public EventHandler<TextChangedEventArgs> OnTextChanged;

        public class TextChangedEventArgs
        {
            public string Text { get; set; }
            
            public TextChangedEventArgs(string text)
            {
                Text = text;
            }
        }


        public bool HighlightOnHover;

        /// <summary>
        /// The coordinate offset for mouse input
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public Vector2 UIOffset = Vector2.Zero;

        protected Control(Vector2? position = null, string text = "", Font textFont = null, Color? foreColor = null, Color? backColor = null, Color? borderColor = null, int borderWidth = 1, Sprite background = null, int width = 0, int height = 0, bool highlightOnHover = false)
        {
            Position = position ?? Vector2.Zero;
            
            Text = text;
            TextFont = textFont;
            ForeColor = foreColor ?? Color.Transparent;
            BackColor = backColor ?? Color.Transparent;
            BorderColor = borderColor ?? Color.Transparent;
            BorderWidth = borderWidth;
            Background = background;
            Width = width;
            Height = height;
            HighlightOnHover = highlightOnHover;
        }

        public virtual void Draw(Renderer renderer)
        {
            //Draw back color
            renderer.DrawRectangle(Bounds, BackColor);

            if (Background != null)
            {
                renderer.Draw(Background, Position);
            }

            if (HighlightOnHover)
            {
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

        public virtual void Update()
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
        }

        public delegate void ControlClickedEvent();
        public delegate void ControlHoveredEvent();

        public event ControlClickedEvent ControlClicked;
        public event ControlHoveredEvent ControlHovered;
    }
}
