#if WINDOWS_UWP

using System;
using Microsoft.Graphics.Canvas.UI.Xaml;

namespace Inferno.Graphics
{
    internal class PlatformRenderer
    {
        private CanvasControl _canvas;

        public PlatformRenderer(GraphicsDevice graphicsManager)
        {
            _canvas = graphicsManager.PlatformGraphicsManager.Canvas;
        }

        public void BeginRender()
        {
        }

        public void Render(Renderable renderable)
        {
        }

        public void EndRender()
        {
            _canvas.Invalidate();
        }
    }
}

#endif