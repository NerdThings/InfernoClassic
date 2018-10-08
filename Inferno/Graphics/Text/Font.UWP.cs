#if WINDOWS_UWP

using System;
using System.Collections.Generic;
using System.Text;

namespace Inferno.Graphics.Text
{
    /// <summary>
    /// UWP Specific font code
    /// </summary>
    internal class PlatformFont : IDisposable
    {
        internal IntPtr Handle;

        public PlatformFont(string filename, int ptSize)
        {
            throw new NotImplementedException();
        }

        public Vector2 MeasureString(string text)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}

#endif