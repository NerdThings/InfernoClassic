using System;

namespace Inferno.Runtime.Graphics
{
    /// <summary>
    /// A special texture that can be drawn to
    /// </summary>
    public class RenderTarget : IDisposable
    {
        internal PlatformRenderTarget PlatformRenderTarget;

        public Rectangle Bounds => new Rectangle(0, 0, Width, Height);

        public int Width => PlatformRenderTarget.Width;
        public int Height => PlatformRenderTarget.Height;

        public RenderTarget(int width, int height)
        {
            PlatformRenderTarget = new PlatformRenderTarget(width, height);
        }

        ~RenderTarget()
        {
            Dispose();
        }

        public void Dispose()
        {
            PlatformRenderTarget.Dispose();
        }
    }
}
