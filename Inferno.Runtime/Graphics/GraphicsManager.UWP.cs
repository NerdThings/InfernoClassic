#if WINDOWS_UWP

using System;
using System.Collections.Generic;

namespace Inferno.Runtime.Graphics
{
    /// <summary>
    /// UWP Specific management code
    /// </summary>
    internal class PlatformGraphicsManager
    {
        internal IntPtr Renderer { get; set; }

        public PlatformGraphicsManager()
        {
            throw new NotImplementedException();
        }

        internal void Setup(GameWindow window)
        {
            throw new NotImplementedException();
        }

        public void Clear(Color color)
        {
            throw new NotImplementedException();
        }

        public void SetRenderTarget(RenderTarget target)
        {
            throw new NotImplementedException();
        }

        internal void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}

#endif