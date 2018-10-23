using System;
using SDL2;

namespace Inferno.Graphics
{
    public partial class GraphicsDevice
    {
        public Rectangle ScreenBounds
        {
            get
            {
                SDL.SDL_GetCurrentDisplayMode(0, out var mode);
                return new Rectangle(0, 0, mode.w, mode.h);
            }
        }
        
        private void Init()
        {
            if (SDL.SDL_Init(SDL.SDL_INIT_EVERYTHING) < 0)
                throw new Exception("SDL Failed to initialise.");
        }

        private void Present()
        {
            SDL.SDL_GL_SwapWindow(_gameWindow.Handle);
        }

        private void Dispose1()
        {
            SDL.SDL_Quit();
        }
    }
}