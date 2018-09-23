#if DESKTOP

using System;
using System.Runtime.InteropServices;
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

        public void Render(Renderable renderable)
        {
            //Switch different batch types
            switch (renderable.Type)
            {
                case RenderableType.Texture:
                    {
                        //Draw a texture
                        var c = renderable.Color;
                        SDL2.SDL.SDL_SetTextureColorMod(renderable.Texture.PlatformTexture2D.Handle, c.R, c.G, c.B);
                        SDL2.SDL.SDL_SetTextureAlphaMod(renderable.Texture.PlatformTexture2D.Handle, c.A);

                        SDL2.SDL.SDL_Rect srcrect;

                        var r = renderable.DestinationRectangle;
                        var destrect = new SDL2.SDL.SDL_Rect
                        {
                            x = r.X,
                            y = r.Y,
                            w = r.Width,
                            h = r.Height
                        };

                        if (renderable.SourceRectangle.HasValue)
                        {
                            r = renderable.SourceRectangle.Value;
                            srcrect = new SDL2.SDL.SDL_Rect
                            {
                                x = r.X,
                                y = r.Y,
                                w = r.Width,
                                h = r.Height
                            };
                        }
                        else
                        {
                            srcrect = new SDL2.SDL.SDL_Rect
                            {
                                x = 0,
                                y = 0,
                                w = renderable.Texture.Width,
                                h = renderable.Texture.Height
                            };
                        }

                        var centre = new SDL2.SDL.SDL_Point
                        {
                            x = (int) renderable.Origin.X,
                            y = (int) renderable.Origin.Y
                        };

                        var ptr = Marshal.AllocHGlobal(Marshal.SizeOf(centre));
                        Marshal.StructureToPtr(centre, ptr, false);

                        //if (SDL2.SDL.SDL_RenderCopyEx(Renderer, renderable.Texture.PlatformTexture2D.Handle, ref srcrect,
                        //        ref destrect, renderable.Rotation, ptr, SDL2.SDL.SDL_RendererFlip.SDL_FLIP_NONE) < 0)
                        //    throw new Exception("Failed to render Texture. " + SDL2.SDL.SDL_GetError());

                        SDL2.SDL.SDL_RenderCopy(Renderer, renderable.Texture.PlatformTexture2D.Handle, ref srcrect,
                            ref destrect);

                        Marshal.FreeHGlobal(ptr);

                        break;
                    }

                case RenderableType.RenderTarget:
                    {
                        //Draw a render target
                        var c = renderable.Color;
                        SDL2.SDL.SDL_SetTextureColorMod(renderable.RenderTarget.PlatformRenderTarget.Handle, c.R, c.G, c.B);
                        SDL2.SDL.SDL_SetTextureAlphaMod(renderable.RenderTarget.PlatformRenderTarget.Handle, c.A);

                        var r = renderable.DestinationRectangle;
                        var destrect = new SDL2.SDL.SDL_Rect
                        {
                            x = r.X,
                            y = r.Y,
                            w = r.Width,
                            h = r.Height
                        };

                        var srcrect = new SDL2.SDL.SDL_Rect
                        {
                            x = 0,
                            y = 0,
                            w = renderable.RenderTarget.Width,
                            h = renderable.RenderTarget.Height
                        };

                        if (SDL2.SDL.SDL_RenderCopyEx(Renderer, renderable.RenderTarget.PlatformRenderTarget.Handle, ref srcrect,
                            ref destrect, renderable.Rotation, IntPtr.Zero, SDL2.SDL.SDL_RendererFlip.SDL_FLIP_NONE) < 0)
                            throw new Exception("Failed to render RenderTarget. " + SDL2.SDL.SDL_GetError());
                        break;
                    }

                case RenderableType.Text:
                    {
                        var c = renderable.Color;
                        var color = new SDL2.SDL.SDL_Color
                        {
                            r = c.R,
                            g = c.G,
                            b = c.B,
                            a = c.A
                        };

                        var surface = SDL_ttf.TTF_RenderText_Solid(renderable.Font.PlatformFont.Handle, renderable.Text, color);

                        var msg = SDL2.SDL.SDL_CreateTextureFromSurface(Renderer, surface);

                        var r = renderable.DestinationRectangle;
                        var destrect = new SDL2.SDL.SDL_Rect
                        {
                            x = r.X,
                            y = r.Y,
                            w = r.Width,
                            h = r.Height
                        };

                        var centre = new SDL2.SDL.SDL_Point
                        {
                            x = (int)renderable.Origin.X,
                            y = (int)renderable.Origin.Y
                        };

                        var ptr = Marshal.AllocHGlobal(Marshal.SizeOf(centre));
                        Marshal.StructureToPtr(centre, ptr, false);

                        SDL2.SDL.SDL_RenderCopyEx(Renderer, msg, IntPtr.Zero, ref destrect, renderable.Rotation, ptr, SDL2.SDL.SDL_RendererFlip.SDL_FLIP_NONE);

                        Marshal.FreeHGlobal(ptr);

                        SDL2.SDL.SDL_DestroyTexture(msg);
                        SDL2.SDL.SDL_FreeSurface(surface);
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void EndRender()
        {
            if (Game.Instance.GraphicsManager.GetRenderTarget() == null)
                SDL2.SDL.SDL_RenderPresent(Renderer);
        }

        public void Dispose()
        {
        }
    }
}

#endif