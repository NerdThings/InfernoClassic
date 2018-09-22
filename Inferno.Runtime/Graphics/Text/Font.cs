using System;
using System.Collections.Generic;
using System.Text;

namespace Inferno.Runtime.Graphics.Text
{
    /// <summary>
    /// A Text Font
    /// </summary>
    public class Font : IDisposable
    {
        internal PlatformFont PlatformFont;

        public Font(string filename, int ptSize)
        {
            PlatformFont = new PlatformFont(filename, ptSize);
        }

        public Vector2 MeasureString(string text)
        {
            return PlatformFont.MeasureString(text);
        }

        public void Dispose()
        {
            PlatformFont.Dispose();
        }

        //TODO: More options (Font size, style etc.)
    }
}
