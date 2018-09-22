#if DESKTOP

using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using Inferno.Runtime.Graphics.Text;
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
            
        }

        public unsafe void Render(Renderable renderable)
        {
            if (renderable.Texture != null)
            {
                //Draw a texture
                var c = renderable.Color;
                SDL.SDL_SetTextureColorMod(renderable.Texture.PlatformTexture2D.Handle, c.R, c.G, c.B);
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

                var centre = new SDL.SDL_Point();
                centre.x = (int)renderable.Origin.X;
                centre.y = (int)renderable.Origin.Y;

                var ptr = Marshal.AllocHGlobal(Marshal.SizeOf(centre));
                Marshal.StructureToPtr(centre, ptr, false);

                if (SDL.SDL_RenderCopyEx(Renderer, renderable.Texture.PlatformTexture2D.Handle, ref srcrect,
                        ref destrect, renderable.Rotation, ptr, SDL.SDL_RendererFlip.SDL_FLIP_NONE) < 0)
                    throw new Exception("Failed to render Texture. " + SDL.SDL_GetError());

                if (renderable.Dispose)
                    renderable.Texture.Dispose();
            }
            else if (renderable.RenderTarget != null)
            {
                //Draw a render target
                var c = renderable.Color;
                SDL.SDL_SetTextureColorMod(renderable.RenderTarget.PlatformRenderTarget.Handle, c.R, c.G, c.B);
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

                if (SDL.SDL_RenderCopyEx(Renderer, renderable.RenderTarget.PlatformRenderTarget.Handle, ref srcrect,
                    ref destrect, renderable.Rotation, IntPtr.Zero, SDL.SDL_RendererFlip.SDL_FLIP_NONE) < 0)
                    throw new Exception("Failed to render RenderTarget. " + SDL.SDL_GetError());
            }
            else if (renderable.Font != null)
            {
                var c = renderable.Color;
                var color = new SDL.SDL_Color
                {
                    r = c.R,
                    g = c.G,
                    b = c.B,
                    a = c.A
                };

                var surface = SDL_ttf.TTF_RenderText_Solid(renderable.Font.PlatformFont.Handle, renderable.Text, color);

                var msg = SDL.SDL_CreateTextureFromSurface(Renderer, surface);

                var r = renderable.DestinationRectangle;
                var destrect = new SDL.SDL_Rect
                {
                    x = r.X,
                    y = r.Y,
                    w = r.Width,
                    h = r.Height
                };

                SDL.SDL_RenderCopy(Renderer, msg, IntPtr.Zero, ref destrect);

                SDL.SDL_DestroyTexture(msg);
                SDL.SDL_FreeSurface(surface);
            }
            else if (renderable.Line)
            {
                var c = renderable.Color;
                SDL.SDL_SetRenderDrawColor(Renderer, c.R, c.G, c.B, c.A);
                SDL.SDL_RenderDrawLine(Renderer, (int) renderable.PointA.X, (int) renderable.PointA.Y,(int) renderable.PointB.X, (int) renderable.PointB.Y);
            }
            else if (renderable.Rectangle)
            {
                var c = renderable.Color;
                SDL.SDL_SetRenderDrawColor(Renderer, c.R, c.G, c.B, c.A);

                var r = renderable.DestinationRectangle;
                var destrect = new SDL.SDL_Rect
                {
                    x = r.X,
                    y = r.Y,
                    w = r.Width,
                    h = r.Height
                };

                if (renderable.FillRectangle)
                    SDL.SDL_RenderFillRect(Renderer, ref destrect);
                else
                    SDL.SDL_RenderDrawRect(Renderer, ref destrect);
            }
            else if (renderable.Ellipse)
            {
                var c = renderable.Color;
                SDL.SDL_SetRenderDrawColor(Renderer, c.R, c.G, c.B, c.A);

                var radius = renderable.DestinationRectangle.Width / 2;
                var x = renderable.DestinationRectangle.X;
                var y = renderable.DestinationRectangle.Y;

                for (int w = 0; w < radius * 2; w++)
                {
                    for (int h = 0; h < radius * 2; h++)
                    {
                        int dx = radius - w;
                        int dy = radius - h;
                        if ((dx * dx + dy * dy) <= (radius * radius))
                        {
                            SDL.SDL_RenderDrawPoint(Renderer, x + dx, y + dy);
                        }
                    }
                }
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