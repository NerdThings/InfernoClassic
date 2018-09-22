#if WINDOWS_UWP

using System;

namespace Inferno.Runtime.Graphics
{
    internal class PlatformRenderer
    {
        public PlatformRenderer(GraphicsManager graphicsManager)
        {
            throw new NotImplementedException();
        }

        public void BeginRender()
        {
            throw new NotImplementedException();
        }

        public void Render(Renderable renderable)
        {
            throw new NotImplementedException();
        }

        public void EndRender()
        {
            throw new NotImplementedException();
        }
    }
}

#endif