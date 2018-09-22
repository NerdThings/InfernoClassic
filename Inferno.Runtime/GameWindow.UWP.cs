#if WINDOWS_UWP

using System;
using System.Collections.Generic;
using System.Text;

namespace Inferno.Runtime
{
    /// <summary>
    /// UWP Specific GameWindow code
    /// </summary>
    internal class PlatformGameWindow
    {
        public PlatformGameWindow(string title, int width, int height)
        {
            throw new NotImplementedException();
        }
        
        public bool AllowResize
        {
            get
            {
                throw new NotImplementedException();
            }
            set => throw new NotImplementedException();
        }

        public Rectangle Bounds
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public int Width
        {
            get => throw new NotImplementedException();
            set
            {
                throw new NotImplementedException();
            }
        }

        public int Height
        {
            get => throw new NotImplementedException();
            set
            {
                throw new NotImplementedException();
            }
        }

        public Point Position
        {
            get
            {
                throw new NotImplementedException();
            }
            set => throw new NotImplementedException();
        }

        public bool AllowAltF4
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public string Title
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public void Exit()
        {
            throw new NotImplementedException();
        }
    }
}

#endif