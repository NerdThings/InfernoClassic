#if WINDOWS_UWP

using System;

namespace Inferno.Graphics
{
    /// <summary>
    /// UWP Specific texture code
    /// </summary>
    internal class PlatformTexture2D : IDisposable
    {
        public PlatformTexture2D(string filename)
        {
            throw new NotImplementedException();
        }

        public PlatformTexture2D(Color[] data, int width, int height)
        {
            throw new NotImplementedException();
        }

        public void SetData(Color[] data)
        {
            
        }

        public int Width
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int Height
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}

#endif