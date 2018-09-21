using Inferno.Runtime.Graphics.Text;

namespace Inferno.Runtime.Graphics
{
    public class Renderable
    {
        public Texture2D Texture { get; set; }
        public RenderTarget RenderTarget { get; set; }
        public Font Font { get; set; }
        public string Text { get; set; }
        public Color Color { get; set; }
        public float Depth { get; set; }
        public Rectangle? SourceRectangle { get; set; }
        public Rectangle DestinationRectangle { get; set; }
    }
}
