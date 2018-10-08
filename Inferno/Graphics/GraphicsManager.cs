using System;
using System.Collections.Generic;
using System.Text;

namespace Inferno.Graphics
{
    /// <summary>
    /// Manager for all Graphics related features.
    /// </summary>
    public class GraphicsManager : IDisposable
    {
        internal PlatformGraphicsManager PlatformGraphicsManager { get; set; }
        private RenderTarget _currentTarget;

        public GraphicsManager()
        {
            PlatformGraphicsManager = new PlatformGraphicsManager();
        }

        public void Clear(Color color)
        {
            PlatformGraphicsManager.Clear(color);
        }

        /// <summary>
        /// Set a render target to draw to
        /// </summary>
        /// <param name="target"></param>
        public void SetRenderTarget(RenderTarget target)
        {
            PlatformGraphicsManager.SetRenderTarget(target);
            _currentTarget = target;
        }

        /// <summary>
        /// Get the current rendertarget
        /// </summary>
        /// <returns></returns>
        public RenderTarget GetRenderTarget()
        {
            return _currentTarget;
        }

        /// <summary>
        /// Configure the manager
        /// </summary>
        /// <param name="window"></param>
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
