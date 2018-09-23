﻿#if DESKTOP

using System;
using System.Collections.Generic;
using SDL2;

namespace Inferno.Runtime.Graphics
{
    /// <summary>
    /// SDL Specific management code
    /// </summary>
    internal class PlatformGraphicsManager
    {
        internal IntPtr Renderer { get; set; }

        public PlatformGraphicsManager()
        {
            //Init SDL
            if (SDL.Init(SDL.InitFlags.Video) < 0)
                throw new Exception("SDL Failed to initialise.");

            //Init images, SUPPORT ALL THE THINGS
            const SDL_image.IMG_InitFlags flags = SDL_image.IMG_InitFlags.IMG_INIT_JPG
                                                  & SDL_image.IMG_InitFlags.IMG_INIT_PNG
                                                  & SDL_image.IMG_InitFlags.IMG_INIT_TIF
                                                  & SDL_image.IMG_InitFlags.IMG_INIT_WEBP;

            if ((SDL_image.IMG_Init(flags) & (int) flags) != (int) flags)
                throw new Exception("SDL_image failed to intialiise. " + SDL2.SDL.SDL_GetError());

            if (SDL_ttf.TTF_Init() < 0)
                throw new Exception("SDL_ttf Fialed to initialise. " + SDL2.SDL.SDL_GetError());
        }

        internal void Setup(GameWindow window)
        {
            //Create renderer
            Renderer = SDL2.SDL.SDL_CreateRenderer(window.PlatformWindow.Handle, -1, SDL2.SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL2.SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);

            if (Renderer == IntPtr.Zero)
                throw new Exception("Failed to create renderer." + SDL2.SDL.SDL_GetError());

            //Enable alpha blending
            if (SDL2.SDL.SDL_SetRenderDrawBlendMode(Renderer, SDL2.SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND) < 0)
                throw new Exception("Failed to enable alpha blending. " + SDL2.SDL.SDL_GetError());

            //Init render color
            SDL2.SDL.SDL_SetRenderDrawColor(Renderer, 0xFF, 0xFF, 0xFF, 0xFF);
        }

        public void Clear(Color color)
        {
            SDL2.SDL.SDL_SetRenderDrawColor(Renderer, color.R, color.G, color.B, color.A);
            SDL2.SDL.SDL_RenderClear(Renderer);
        }

        public void SetRenderTarget(RenderTarget target)
        {
            if (target != null)
            {
                if (SDL2.SDL.SDL_SetRenderTarget(Renderer, target.PlatformRenderTarget.Handle) < 0)
                    throw new Exception("Failed to set render target. " + SDL2.SDL.SDL_GetError());
            }
            else
            {
                if (SDL2.SDL.SDL_SetRenderTarget(Renderer, IntPtr.Zero) < 0)
                    throw new Exception("Failed to set render target. " + SDL2.SDL.SDL_GetError());
            }
        }

        internal void Dispose()
        {
            SDL2.SDL.SDL_DestroyRenderer(Renderer);
            Renderer = IntPtr.Zero;

            SDL_image.IMG_Quit();
            SDL_ttf.TTF_Quit();
            SDL.Quit();
        }
    }
}

#endif