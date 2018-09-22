#if WINDOWS_UWP

using System;

namespace Inferno.Runtime.Graphics
{
    /// <summary>
    /// UWP Specific render target code
    /// </summary>
    internal class PlatformRenderTarget
    {
        public PlatformRenderTarget(int width, int height)
        {
            throw new NotImplementedException();
        }

        public int Width
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int Height
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}

#endif