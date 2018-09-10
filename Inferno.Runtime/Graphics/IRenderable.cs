using System;
using System.Collections.Generic;
using System.Text;

namespace Inferno.Runtime.Graphics
{
    public class Renderable
    {
        public int Depth { get; set; }
        public bool HasTexture { get; set; }
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
    }
}
