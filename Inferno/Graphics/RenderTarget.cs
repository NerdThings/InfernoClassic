using System;

namespace Inferno.Graphics
{
    /// <summary>
    /// A special texture that can be drawn to
    /// </summary>
    public partial class RenderTarget : IDisposable
    {
        public Rectangle Bounds => new Rectangle(0, 0, Width, Height);
        public int Width { get; private set; }
        public int Height { get; private set; }
        
        ~RenderTarget()
        {
            Dispose();
        }

        public void Dispose()
        {
            GraphicsDevice.DisposeRenderTarget(this);
        }
    }
}
