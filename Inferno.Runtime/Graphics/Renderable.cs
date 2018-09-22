using Inferno.Runtime.Graphics.Text;

namespace Inferno.Runtime.Graphics
{
    public enum RenderableType
    {
        Texture,
        RenderTarget,
        Text,
        Line,
        Rectangle,
        FilledRectangle,
        Ellipse
    }
    /// <summary>
    /// A Renderable object
    /// </summary>
    public struct Renderable
    {
        /// <summary>
        /// The type of item
        /// </summary>
        public RenderableType Type { get; set; }

        /// <summary>
        /// The texture for a texture batch item
        /// </summary>
        public Texture2D Texture { get; set; }

        /// <summary>
        /// The render target for a render target batch item
        /// </summary>
        public RenderTarget RenderTarget { get; set; }

        /// <summary>
        /// The font for a text batch item
        /// </summary>
        public Font Font { get; set; }

        /// <summary>
        /// The text for a text batch item
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The color for a batch item
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// The depth for a batch item
        /// </summary>
        public float Depth { get; set; }

        /// <summary>
        /// The source rectangle for texture batch items
        /// </summary>
        public Rectangle? SourceRectangle { get; set; }

        /// <summary>
        /// The destination rectangle to draw to the screen
        /// </summary>
        public Rectangle DestinationRectangle { get; set; }

#warning LineWidth is not implemented yet, this is a placeholder
        /// <summary>
        /// Placeholder for line widths for rectangle outlines and lines
        /// </summary>
        public int LineWidth { get; set; }

        /// <summary>
        /// Point A for a line
        /// </summary>
        public Vector2 PointA { get; set; }

        /// <summary>
        /// Point B for a line
        /// </summary>
        public Vector2 PointB { get; set; }

        /// <summary>
        /// Rotation of the object
        /// </summary>
        public double Rotation { get; set; }

        /// <summary>
        /// Origin of the object
        /// </summary>
        public Vector2 Origin { get; set; }

        /// <summary>
        /// Whether or not to dispose after rendering
        /// </summary>
        public bool Dispose { get; set; }
    }
}
