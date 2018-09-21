using System;
using System.Collections.Generic;
using System.Text;

namespace Inferno.Runtime.Graphics
{
    public class Renderable
    {
        public Texture2D Texture { get; set; }
        public RenderTarget RenderTarget { get; set; }
        public Color Color;
        public float Depth { get; set; }
        public Rectangle? SourceRectangle { get; set; }
        public Rectangle DestinationRectangle { get; set; }
    }
}
