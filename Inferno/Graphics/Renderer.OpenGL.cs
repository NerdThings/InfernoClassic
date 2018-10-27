#if OPENGL

using System;
using OpenTK.Graphics.OpenGL;

namespace Inferno.Graphics
{
    public partial class Renderer
    {
        #region Public Methods
        
        /// <summary>
        /// Start the renderer and apply the translation matrix
        /// </summary>
        /// <param name="matrix">Translation matrix</param>
        private void BeginRender(Matrix matrix)
        {
            var ortho = _graphicsDevice.GetCurrentRenderTarget() != null
                ? Matrix.CreateOrthographicOffCenter(0, _graphicsDevice.GetCurrentRenderTarget().Width,
                    _graphicsDevice.GetCurrentRenderTarget().Height, 0, -1, 1)
                : Matrix.CreateOrthographicOffCenter(0, Game.Instance.Window.Width, Game.Instance.Window.Height, 0, -1,
                    1);

            GL.LoadMatrix((matrix * ortho).Array);
        }
        
        /// <summary>
        /// Render a batch item
        /// </summary>
        /// <param name="renderable">Batch item</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void Render(Renderable renderable)
        {
            //Save matrix
            GL.PushMatrix();
            
            //Apply color
            var color = renderable.Color;
            GL.Color4(color.R, color.G, color.B, color.A);
            
            //Apply line width
            GL.LineWidth(renderable.LineWidth);

            //Apply origin
            ApplyOrigin(renderable.Origin);
            
            //Apply rotation
            ApplyRotate(renderable.Rotation, renderable.Origin);

            //Switch different batch types
            switch (renderable.Type)
            {
                case RenderableType.Lines:
                    {
                        //Draw vertex array
                        GL.Begin(PrimitiveType.Lines);
                        foreach (var vertex in renderable.Verticies)
                            GL.Vertex2(vertex.X, vertex.Y);
                        GL.End();
                        break;
                    }
                case RenderableType.Texture:
                    {
                        //Bind texture
                        BindTexture(renderable.Texture.Id);

                        //Get properties
                        var x = renderable.DestinationRectangle.X;
                        var y = renderable.DestinationRectangle.Y;
                        var width = renderable.DestinationRectangle.Width;
                        var height = renderable.DestinationRectangle.Height;

                        //Calculate texture coordinates
                        float texLeft = 0;
                        float texRight = 1;
                        float texTop = 0;
                        float texBottom = 1;

                        //Convert source rectangle into texture coordinates 
                        var sourceRectangle = renderable.SourceRectangle;
                        if (sourceRectangle.HasValue)
                        {
                            var src = sourceRectangle.Value;
                            texLeft = (float)src.X / renderable.Texture.Width;
                            texRight = texLeft + (float)src.Width / renderable.Texture.Width;
                            texTop = (float)src.Y / renderable.Texture.Height;
                            texBottom = texTop + (float)src.Height / renderable.Texture.Height;
                        }

                        //This is different to render target because of how we convert bitmap to opengl texture
                        //Draw texture
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

                        //Unbind texture
                        BindTexture(0);
                        break;
                    }

                case RenderableType.Rectangle:
                    {
                        //Get rectangle properties
                        var x = renderable.DestinationRectangle.X;
                        var y = renderable.DestinationRectangle.Y;
                        var width = renderable.DestinationRectangle.Width;
                        var height = renderable.DestinationRectangle.Height;

                        //Draw rectangle
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
                        //Get properties
                        var x = renderable.DestinationRectangle.X;
                        var y = renderable.DestinationRectangle.Y;

                        //Draw circle
                        GL.Begin(PrimitiveType.Polygon);
                        for (double i = 0; i < 2 * Math.PI; i += Math.PI / renderable.Precision)
                            GL.Vertex2(x + (Math.Cos(i) * (renderable.Radius)), y + (Math.Sin(i) * (renderable.Radius)));

                        GL.End();

                        break;
                    }

                case RenderableType.Circle:
                    {
                        //Draw circle outline
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
                        //Bind rendertarget
                        BindTexture(renderable.RenderTarget.RenderedTexture);

                        //Get properties
                        var x = renderable.DestinationRectangle.X;
                        var y = renderable.DestinationRectangle.Y;
                        var width = renderable.DestinationRectangle.Width;
                        var height = renderable.DestinationRectangle.Height;

                        //Draw rendertarget
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

                        //Unbind target
                        BindTexture(0);
                        break;
                    }

                case RenderableType.Text:
                    {
                        //Bind font texture
                        var font = renderable.Font;
                        BindTexture(font.Texture.Id);

                        //Get properties
                        var x = renderable.DestinationRectangle.X;
                        var y = renderable.DestinationRectangle.Y;
                        var lineHeight = 0;

                        foreach (var c in renderable.Text)
                        {
                            //Handle special characters
                            switch (c)
                            {
                                case '\n':
                                    lineHeight += renderable.Font.LineHeight;
                                    x = renderable.DestinationRectangle.X;
                                    continue;
                                case ' ':
                                    x += renderable.Font.SpaceSize;
                                    continue;
                            }

                            //Get bounds for character
                            var src = font.GetRectangleForChar(c);

                            //Get properties
                            var width = src.Width;
                            var height = src.Height;

                            //Calculate texture coordinates
                            var texLeft = (float)src.X / renderable.Font.Texture.Width;
                            var texRight = texLeft + (float)src.Width / renderable.Font.Texture.Width;
                            var texTop = (float)src.Y / renderable.Font.Texture.Height;
                            var texBottom = texTop + (float)src.Height / renderable.Font.Texture.Height;

                            //Draw character
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

                        //Unbind texture
                        BindTexture(0);
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            //Flush
            GL.Flush();
            
            //Restore matrix
            GL.PopMatrix();
        }
        
        #endregion
        
        #region Private Methods

        /// <summary>
        /// Apply origin
        /// </summary>
        /// <param name="origin">Origin</param>
        private void ApplyOrigin(Vector2 origin)
        {
            GL.Translate(origin.X, origin.Y, 0f);
        }
        
        /// <summary>
        /// Apply rotation
        /// </summary>
        /// <param name="degrees">Rotation in degrees</param>
        private void ApplyRotate(double degrees, Vector2 origin)
        {
            //below does not work
            //GL.Rotate(degrees, 0f, 0f, 1f);
        }

        /// <summary>
        /// Bind texture
        /// </summary>
        /// <param name="id">Texture id</param>
        private void BindTexture(int id)
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, id);
        }
        
        #endregion
    }
}

#endif