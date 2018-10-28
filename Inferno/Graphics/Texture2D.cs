using System;
using System.IO;

namespace Inferno.Graphics
{
    /// <summary>
    /// A drawable texture
    /// </summary>
    public partial class Texture2D : IDisposable
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        
        /// <summary>
        /// The cached texture data.
        /// </summary>
        private Color[] _cachedData = {};
        
        public Texture2D(int width, int height, Color[] data)
        {
            //Check data
            if (data.Length != width * height)
                throw new InvalidDataException();

            //Set properties
            Width = width;
            Height = height;
            
            //Create texture
            CreateTexture(data);
        }
        
        public void SetData(Color[] data)
        {
            //Destroy current texture
            GraphicsDevice.DisposeTexture(this);
            
            //Create a new one
            CreateTexture(data);
        }

        public Color[] GetData()
        {
            return _cachedData;
        }

        ~Texture2D()
        {
            Dispose();
        }

        public void Dispose()
        {
            GraphicsDevice.DisposeTexture(this);
            _cachedData = null;
        }
    }
}