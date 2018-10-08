#if WINDOWS_UWP

using System;

namespace Inferno.Graphics
{
    /// <summary>
    /// UWP Specific render target code
    /// </summary>
    internal class PlatformRenderTarget
    {
        public PlatformRenderTarget(int width, int height)
        {
        }

        public int Width
        {
            get { return 0; }
        }

        public int Height
        {
            get { return 0; }
        }

        public void Dispose()
        {
        }
    }
}

#endif