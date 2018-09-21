using System;
using System.Collections.Generic;
using System.Text;

namespace Inferno.Runtime.Graphics
{
    public class GraphicsManager : IDisposable
    {
        internal PlatformGraphicsManager PlatformGraphicsManager { get; set; }
        private RenderTarget CurrentTarget;

        public GraphicsManager()
        {
            PlatformGraphicsManager = new PlatformGraphicsManager();
        }

        public void Clear(Color color)
        {
            PlatformGraphicsManager.Clear(color);
        }

        public void SetRenderTarget(RenderTarget target)
        {
            PlatformGraphicsManager.SetRenderTarget(target);
            CurrentTarget = target;
        }

        public RenderTarget GetRenderTarget()
        {
            return CurrentTarget;
        }

        public void Setup(GameWindow window)
        {
            PlatformGraphicsManager.Setup(window);
        }

        public void Dispose()
        {
            PlatformGraphicsManager.Dispose();
        }
    }
}
