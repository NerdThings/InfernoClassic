#if DESKTOP

using System;
using System.Collections.Generic;
using SDL2;

namespace Inferno.Runtime.Graphics
{
    internal class PlatformRenderer
    {
        public PlatformRenderer()
        {
            
        }

        public void Render(Renderable renderable)
        {
            if (renderable.HasTexture)
            {
                var c = renderable.Color;
                SDL.SDL_SetRenderDrawColor(PlatformGameWindow.Renderer, c.A, c.B, c.G, c.A);

                SDL.SDL_Rect destrect;
                SDL.SDL_Rect srcrect;

                var r = renderable.DestinationRectangle;
                destrect = new SDL.SDL_Rect
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


                SDL.SDL_RenderCopy(PlatformGameWindow.Renderer, renderable.Texture.PlatformTexture2D.Handle, ref srcrect, ref destrect);
            }
        }

        public void EndRender()
        {
            SDL.SDL_RenderPresent(PlatformGameWindow.Renderer);
        }
    }
}

#endif