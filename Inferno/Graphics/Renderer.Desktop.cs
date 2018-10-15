#if DESKTOP

using System;
using OpenTK.Graphics.OpenGL;

namespace Inferno.Graphics
{
    internal class PlatformRenderer
    {
        private readonly GraphicsDevice _graphicsDevice;
        public PlatformRenderer(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }

        public void BeginRender(Matrix matrix)
        {
            GL.LoadIdentity();
            
            if (_graphicsDevice.GetCurrentRenderTarget() != null)
                GL.Ortho(0, _graphicsDevice.GetCurrentRenderTarget().Width, _graphicsDevice.GetCurrentRenderTarget().Height, 0, -1, 1);
            else
                GL.Ortho(0, Game.Instance.Window.Width, Game.Instance.Window.Height, 0, -1, 1);

            GL.Translate(matrix.Translation.X, matrix.Translation.Y, matrix.Translation.Z);
            GL.Scale(matrix.Scale.X, matrix.Scale.Y, matrix.Scale.Z);
        }

        public void Render(Renderable renderable)
        {
            var color = renderable.Color;

            GL.Color4(color.R, color.G, color.B, color.A);
            GL.LineWidth(renderable.LineWidth);
            //TODO: Rotation

            //Switch different batch types
            switch (renderable.Type)
            {
                case RenderableType.Lines:
                    {
                        GL.Begin(PrimitiveType.Lines);
                        foreach (var vertex in renderable.Verticies)
                            GL.Vertex2(vertex.X, vertex.Y);
                        GL.End();
                        break;
                    }
                case RenderableType.Texture:
                    {
                        var id = renderable.Texture.PlatformTexture2D.Id;
                        GL.BindTexture(TextureTarget.Texture2D, id);

                        var x = renderable.DestinationRectangle.X;
                        var y = renderable.DestinationRectangle.Y;
                        var width = renderable.DestinationRectangle.Width;
                        var height = renderable.DestinationRectangle.Height;

                        float texLeft = 0;
                        float texRight = 1;
                        float texTop = 0;
                        float texBottom = 1;

                        var sourceRectangle = renderable.SourceRectangle;
                        if (sourceRectangle.HasValue)
                        {
                            var src = sourceRectangle.Value;
                            texLeft = (float)src.X / renderable.Texture.Width;
                            texRight = texLeft + (float)src.Width / renderable.Texture.Width;
                            texTop = (float)src.Y / renderable.Texture.Height;
                            texBottom = texTop + (float)src.Height / renderable.Texture.Height;
                        }

                        GL.Begin(PrimitiveType.Quads);
                        GL.TexCoord2(texLeft, texTop);
                        GL.Vertex2(x, y); //Top-Left
                        GL.TexCoord2(texRight, texTop);
                        GL.Vertex2(x + width, y); //Top-Right
                        GL.TexCoord2(texRight, texBottom);
                        GL.Vertex2(x + width, y + height); //Bottom-Right
                        GL.TexCoord2(texLeft, texBottom);
                        GL.Vertex2(x, y + height); //Bottom-Left
                        GL.End();

                        break;
                    }

                case RenderableType.Rectangle:
                    {
                        var x = renderable.DestinationRectangle.X;
                        var y = renderable.DestinationRectangle.Y;
                        var width = renderable.DestinationRectangle.Width;
                        var height = renderable.DestinationRectangle.Height;

                        GL.BindTexture(TextureTarget.Texture2D, 0);
                        GL.Begin(PrimitiveType.Quads);
                        GL.Vertex2(x, y); //Top-Left
                        GL.Vertex2(x + width, y); //Top-Right
                        GL.Vertex2(x + width, y + height); //Bottom-Right
                        GL.Vertex2(x, y + height); //Bottom-Left
                        GL.End();

                        break;
                    }

                case RenderableType.FilledCircle:
                    {
                        var x = renderable.DestinationRectangle.X;
                        var y = renderable.DestinationRectangle.Y;

                        GL.Begin(PrimitiveType.Polygon);
                        for (double i = 0; i < 2 * Math.PI; i+= Math.PI / renderable.Precision)
                            GL.Vertex2(x + (Math.Cos(i) * (renderable.Radius)), y + (Math.Sin(i) * (renderable.Radius)));

                        GL.End();

                        break;
                    }

                case RenderableType.Circle:
                {
                    GL.Begin(PrimitiveType.LineLoop);
                    for (double i = 0; i < renderable.Precision; i++)
                    {
                        var theta = 2f * Math.PI * i / renderable.Precision;

                        var x = renderable.Radius * (float)Math.Cos(theta);
                        var y = renderable.Radius * (float)Math.Sin(theta);

                        GL.Vertex2(renderable.DestinationRectangle.X + x, renderable.DestinationRectangle.Y + y);
                    }

                    GL.End();

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
                        break;
                    }

                case RenderableType.Text:
                    {
                        var font = renderable.Font;
                        GL.BindTexture(TextureTarget.Texture2D, font.Texture.PlatformTexture2D.Id);

                        var x = renderable.DestinationRectangle.X;
                        var y = renderable.DestinationRectangle.Y;
                        var lineHeight = 0;

                        foreach (var c in renderable.Text)
                        {
                            if (c == '\n')
                            {
                                lineHeight += renderable.Font.LineHeight;
                                x = renderable.DestinationRectangle.X;
                            }

                            var src = font.GetRectangleForChar(c);

                            var width = src.Width;
                            var height = src.Height;

                            var texLeft = (float)src.X / renderable.Font.Texture.Width;
                            var texRight = texLeft + (float)src.Width / renderable.Font.Texture.Width;
                            var texTop = (float)src.Y / renderable.Font.Texture.Height;
                            var texBottom = texTop + (float)src.Height / renderable.Font.Texture.Height;

                            GL.Begin(PrimitiveType.Quads);
                            GL.TexCoord2(texLeft, texTop);
                            GL.Vertex2(x, y + lineHeight); //Top-Left
                            GL.TexCoord2(texRight, texTop);
                            GL.Vertex2(x + width, y + lineHeight); //Top-Right
                            GL.TexCoord2(texRight, texBottom);
                            GL.Vertex2(x + width, y + height + lineHeight); //Bottom-Right
                            GL.TexCoord2(texLeft, texBottom);
                            GL.Vertex2(x, y + height + lineHeight); //Bottom-Left
                            GL.End();

                            x += width;
                        }

                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void EndRender()
        {
            //GL.LoadIdentity();
        }

        public void Dispose()
        {
        }
    }
}

#endif