#if DESKTOP

using System;
using System.Collections.Generic;
using System.Text;
using SDL2;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Inferno.Graphics.Text
{
    /// <summary>
    /// Desktop Specific font code
    /// </summary>
    internal class PlatformFont : IDisposable
    {
        //TODO: OpenGL Implementation
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