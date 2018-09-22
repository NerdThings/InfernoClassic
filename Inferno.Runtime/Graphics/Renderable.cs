﻿using Inferno.Runtime.Graphics.Text;

namespace Inferno.Runtime.Graphics
{
    public struct Renderable
    {
        public Texture2D Texture { get; set; }
        public RenderTarget RenderTarget { get; set; }
        public Font Font { get; set; }
        public string Text { get; set; }
        public Color Color { get; set; }
        public float Depth { get; set; }
        public Rectangle? SourceRectangle { get; set; }
        public Rectangle DestinationRectangle { get; set; }
        public bool Line { get; set; }
#warning LineWidth is not implemented yet, this is a placeholder
        public int LineWidth { get; set; }
        public Vector2 PointA { get; set; }
        public Vector2 PointB { get; set; }
        public bool Rectangle { get; set; }
        public bool Ellipse { get; set; }
        public bool FillRectangle { get; set; }
        public double Rotation { get; set; }
        public Vector2 Origin { get; set; }
        public bool Dispose { get; set; }
    }
}
