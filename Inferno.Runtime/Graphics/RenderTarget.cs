namespace Inferno.Runtime.Graphics
{
    public class RenderTarget
    {
        internal PlatformRenderTarget PlatformRenderTarget;

        public Rectangle Bounds => new Rectangle(0, 0, Width, Height);

        public int Width => PlatformRenderTarget.Width;
        public int Height => PlatformRenderTarget.Height;

        public RenderTarget(int width, int height)
        {
            PlatformRenderTarget = new PlatformRenderTarget(width, height);
        }
    }
}
