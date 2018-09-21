#if DESKTOP

using System;
using System.Collections.Generic;
using SDL2;

namespace Inferno.Runtime.Graphics
{
    internal class PlatformRenderer
    {
        internal IntPtr Renderer;
        public PlatformRenderer(GraphicsManager graphicsManager)
        {
            Renderer = graphicsManager.PlatformGraphicsManager.Renderer;
        }

        public void Render(Renderable renderable)
        {
            if (renderable.HasTexture)
            {
                var c = renderable.Color;
                //SDL.SDL_SetRenderDrawColor(PlatformGameWindow.Renderer, c.A, c.B, c.G, c.A);
                SDL.SDL_SetTextureColorMod(renderable.Texture.PlatformTexture2D.Handle, c.R, c.B, c.G);
                SDL.SDL_SetTextureAlphaMod(renderable.Texture.PlatformTexture2D.Handle, c.A);

                SDL.SDL_Rect srcrect;

                var r = renderable.DestinationRectangle;
                var destrect = new SDL.SDL_Rect
                {
                    x = r.X,
                    y = r.Y,
                    w = r.Width,
                    h = r.Height
                };

                if (renderable.SourceRectangle.HasValue)
                {
                    r = renderable.SourceRectangle.Value;
                    srcrect = new SDL.SDL_Rect
                    {
                        x = r.X,
                        y = r.Y,
                        w = r.Width,
                        h = r.Height
                    };
                }
                else
                {
                    srcrect = new SDL.SDL_Rect
                    {
                        x = 0,
                        y = 0,
                        w = renderable.Texture.Width,
                        h = renderable.Texture.Height
                    };
                }

                SDL.SDL_RenderCopy(Renderer, renderable.Texture.PlatformTexture2D.Handle, ref srcrect, ref destrect);
            }
        }

        public void EndRender()
        {
            SDL.SDL_RenderPresent(Renderer);
        }
    }
}

#endif