#if DESKTOP

using System;
using System.Collections.Generic;
using SDL2;

namespace Inferno.Runtime.Graphics
{
    internal class PlatformGraphicsManager
    {
        internal IntPtr Renderer { get; set; }

        public PlatformGraphicsManager()
        {
            //Init SDL
            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
            {
                throw new Exception("SDL Failed to initialise");
            }

            //Init images, SUPPORT ALL THE THINGS
            const SDL_image.IMG_InitFlags flags = SDL_image.IMG_InitFlags.IMG_INIT_JPG
                                                  & SDL_image.IMG_InitFlags.IMG_INIT_PNG
                                                  & SDL_image.IMG_InitFlags.IMG_INIT_TIF
                                                  & SDL_image.IMG_InitFlags.IMG_INIT_WEBP;

            if ((SDL_image.IMG_Init(flags) & (int)flags) != (int)flags)
            {
                throw new Exception("SDL_image failed to intialiise.");
            }

            //Init texture array
            LoadedTextures = new List<IntPtr>();
        }

        internal void Setup(GameWindow Window)
        {
            //Create renderer
            Renderer = SDL.SDL_CreateRenderer(Window.PlatformWindow.Handle, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);

            if (Renderer == IntPtr.Zero)
            {
                throw new Exception("Failed to create renderer");
            }

            //Init render color
            SDL.SDL_SetRenderDrawColor(Renderer, 0xFF, 0xFF, 0xFF, 0xFF);
        }

        public void Clear(Color color)
        {
            SDL.SDL_SetRenderDrawColor(Renderer, color.R, color.G, color.B, color.A);
            SDL.SDL_RenderClear(Renderer);
        }

        //Texture management

        internal List<IntPtr> LoadedTextures;

        internal void RegisterTexture(IntPtr handle)
        {
            LoadedTextures.Add(handle);
        }

        internal void UnRegisterTexture(IntPtr handle)
        {
            LoadedTextures.Remove(handle);
        }

        internal void Dispose()
        {
            foreach (var tex in LoadedTextures)
            {
                SDL.SDL_DestroyTexture(tex);
            }

            LoadedTextures.Clear();

            SDL.SDL_DestroyRenderer(Renderer);
            Renderer = IntPtr.Zero;

            SDL_image.IMG_Quit();
            SDL.SDL_Quit();
        }
    }
}

#endif