#if DESKTOP

using System;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;
using SDL2;

namespace Inferno.Graphics
{
    internal class PlatformRenderer
    {
        public PlatformRenderer(GraphicsManager graphicsManager)
        {
            
        }

        public void BeginRender()
        {
            
        }

        public void Render(Renderable renderable)
        {
            //Switch different batch types
            switch (renderable.Type)
            {
                case RenderableType.Line:
                {
                    GL.LineWidth(renderable.LineWidth);

                    var x1 = renderable.DestinationRectangle.X;
                    var y1 = renderable.DestinationRectangle.Y;
                    var x2 = renderable.DestinationRectangle.Width;
                    var y2 = renderable.DestinationRectangle.Height;

                    //TODO: Rotation

                    var color = renderable.Color;
                    GL.Color4(color.R, color.G, color.B, color.A);
                    GL.Begin(PrimitiveType.Lines);
                    GL.Vertex2(x1, y1);
                    GL.Vertex2(x2, y2);
                    GL.End();
                    break;
                }
                case RenderableType.Texture:
                {
                    var id = renderable.Texture.PlatformTexture2D.Id;

                    var color = renderable.Color;

                    GL.Color4(color.R, color.G, color.B, color.A);
                    GL.BindTexture(TextureTarget.Texture2D, id);

                    var x = renderable.DestinationRectangle.X;
                    var y = renderable.DestinationRectangle.Y;
                    var width = renderable.DestinationRectangle.Width;
                    var height = renderable.DestinationRectangle.Height;

                    //TODO: Source rectangle support

                    //TODO: Rotation

                    GL.Begin(PrimitiveType.Quads);
                    GL.TexCoord2(0, 0);
                    GL.Vertex2(x, y); //Top-Left
                    GL.TexCoord2(1, 0);
                    GL.Vertex2(x + width, y); //Top-Right
                    GL.TexCoord2(1, 1);
                    GL.Vertex2(x + width, y + height); //Bottom-Right
                    GL.TexCoord2(0, 1);
                    GL.Vertex2(x, y + height); //Bottom-Left
                    GL.End();

                    GL.BindTexture(TextureTarget.Texture2D, 0);

                    break;
                }

                case RenderableType.RenderTarget:
                {
                    var id = renderable.RenderTarget.PlatformRenderTarget.RenderedTexture;

                    GL.BindTexture(TextureTarget.Texture2D, id);

                    var x = renderable.DestinationRectangle.X;
                    var y = renderable.DestinationRectangle.Y;
                    var width = renderable.DestinationRectangle.Width;
                    var height = renderable.DestinationRectangle.Height;

                    //Don't ask me why the texture coordinates are different to the texture renderer, if it works dont change, amirite

                    GL.Color4(1.0, 1.0, 1.0, 1.0);
                    GL.Begin(PrimitiveType.Quads);
                    GL.TexCoord2(0, 1);
                    GL.Vertex2(x, y); //Top-Left
                    GL.TexCoord2(1, 1);
                    GL.Vertex2(x + width, y); //Top-Right
                    GL.TexCoord2(1, 0);
                    GL.Vertex2(x + width, y + height); //Bottom Right
                    GL.TexCoord2(0, 0);
                    GL.Vertex2(x, y + height); //Bottom Left
                    GL.End();

                    GL.BindTexture(TextureTarget.Texture2D, 0);
                    break;
                }

                case RenderableType.Text:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void EndRender()
        {
        }

        public void Dispose()
        {
        }
    }
}

#endif