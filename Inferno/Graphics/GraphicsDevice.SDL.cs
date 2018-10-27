#if SDL

using System;
using SDL2;

namespace Inferno.Graphics
{
    public partial class GraphicsDevice
    {
        /// <summary>
        /// The Bounds of the Screen
        /// </summary>
        public Rectangle ScreenBounds
        {
            get
            {
                SDL.SDL_GetCurrentDisplayMode(0, out var mode);
                return new Rectangle(0, 0, mode.w, mode.h);
            }
        }
        
        /// <summary>
        /// Initialise the device
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void Init()
        {
            if (SDL.SDL_Init(SDL.SDL_INIT_EVERYTHING) < 0)
                throw new Exception("SDL Failed to initialise.");
        }

        /// <summary>
        /// Push all drawings to the display
        /// </summary>
        private void Present()
        {
            SDL.SDL_GL_SwapWindow(_gameWindow.Handle);
        }

        /// <summary>
        /// Dispose part 2
        /// </summary>
        private void Dispose1()
        {
            SDL.SDL_Quit();
        }
    }
}

#endif