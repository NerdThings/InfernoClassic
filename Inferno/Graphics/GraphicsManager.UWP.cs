#if WINDOWS_UWP

using System;
using System.Collections.Generic;
using Microsoft.Graphics.Canvas.UI.Xaml;

namespace Inferno.Graphics
{
    /// <summary>
    /// UWP Specific management code
    /// </summary>
    internal class PlatformGraphicsManager
    {
        internal CanvasControl Canvas;
        public PlatformGraphicsManager()
        {
        }

        internal void Setup(GameWindow window)
        {
            Canvas = window.PlatformWindow.Canvas;
        }

        public void Clear(Color color)
        {
            var c = new Windows.UI.Color
            {
                R = color.R,
                G = color.G,
                B = color.B,
                A = color.A
            };

            Canvas.ClearColor = c;
        }

        public void SetRenderTarget(RenderTarget target)
        {
        }

        internal void Dispose()
        {
        }
    }
}

#endif