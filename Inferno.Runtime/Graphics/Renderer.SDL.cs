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

        public void BeginRender()
        {
            //SDL.SDL_RenderClear(Renderer);
        }

        public void Render(Renderable renderable)
        {
            if (renderable.Texture != null)
            {
                //Draw a texture
                var c = renderable.Color;
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
            else if (renderable.RenderTarget != null)
            {
                //Draw a render target
                var c = renderable.Color;
                SDL.SDL_SetTextureColorMod(renderable.RenderTarget.PlatformRenderTarget.Handle, c.R, c.B, c.G);
                SDL.SDL_SetTextureAlphaMod(renderable.RenderTarget.PlatformRenderTarget.Handle, c.A);

                var r = renderable.DestinationRectangle;
                var destrect = new SDL.SDL_Rect
                {
                    x = r.X,
                    y = r.Y,
                    w = r.Width,
                    h = r.Height
                };

                var srcrect = new SDL.SDL_Rect
                {
                    x = 0,
                    y = 0,
                    w = renderable.RenderTarget.Width,
                    h = renderable.RenderTarget.Height
                };

                SDL.SDL_RenderCopy(Renderer, renderable.RenderTarget.PlatformRenderTarget.Handle, ref srcrect, ref destrect);
            }
        }

        public void EndRender()
        {
            if (Game.Instance.GraphicsManager.GetRenderTarget() == null)
                SDL.SDL_RenderPresent(Renderer);
        }
    }
}

#endif