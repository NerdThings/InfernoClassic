#if DESKTOP

using System;
using System.Collections.Generic;
using System.Text;
using SDL2;

namespace Inferno.Graphics.Text
{
    /// <summary>
    /// SDL Specific font code
    /// </summary>
    internal class PlatformFont : IDisposable
    {
        internal IntPtr Handle;

        public PlatformFont(string filename, int ptSize)
        {
            
        }

        public Vector2 MeasureString(string text)
        {
            return new Vector2(0, 0);
        }

        public void Dispose()
        {
        }
    }
}

#endif