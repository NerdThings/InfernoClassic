using System;
using System.Collections.Generic;
using System.Linq;
using Inferno.Graphics.Text;

namespace Inferno.Graphics
{
    /// <summary>
    /// The sort mode for the renderer
    /// </summary>
    public enum RenderSortMode
    {
        /// <summary>
        /// Draw order
        /// </summary>
        Immediate,

        /// <summary>
        /// Sorted by the depth argument
        /// </summary>
        Depth
    }

    /// <summary>
    /// Graphics rendering and batching
    /// </summary>
    public class Renderer : IDisposable
    {
        private bool _rendering;
        private List<Renderable> _renderList;
        private RenderSortMode _sortMode;
        private Matrix _matrix;
        internal PlatformRenderer PlatformRenderer;

        /// <summary>
        /// Create a new renderer
        /// </summary>
        /// <param name="graphicsManager"></param>
        public Renderer(GraphicsManager graphicsManager)
        {
            PlatformRenderer = new PlatformRenderer(graphicsManager);
            _matrix = Matrix.Identity;
        }

        /// <summary>
        /// Begin a drawing batch
        /// </summary>
        /// <param name="sortMode"></param>
        /// <param name="translationMatrix"></param>
        public void Begin(RenderSortMode sortMode = RenderSortMode.Immediate, Matrix? translationMatrix = null)
        {
            //Enable drawing
            if (_renderList == null)
                _renderList = new List<Renderable>();

            _renderList.Clear();
            _rendering = true;
            _sortMode = sortMode;
            _matrix = translationMatrix ?? Matrix.Identity;

            PlatformRenderer.BeginRender();
        }

        /// <summary>
        /// End a drawing batch
        /// </summary>
        public void End()
        {
            var renderables =  _renderList;

            if (_sortMode == RenderSortMode.Depth)
            {
                renderables = _renderList.OrderBy(o => o.Depth).ToList();
            }

            foreach (var renderable in renderables)
            {
                PlatformRenderer.Render(renderable);

                //Dispose if we have to
            if (renderable.Dispose)
            {
                renderable.Texture?.Dispose();
                renderable.RenderTarget?.Dispose();
                renderable.Font?.Dispose();
            }
            }
            PlatformRenderer.EndRender();

            _rendering = false;
        }

        /// <summary>
        /// Draw a texture
        /// </summary>
        /// <param name="texture">Texture to draw</param>
        /// <param name="position">Position</param>
        public void Draw(Texture2D texture, Vector2 position)
        {
            Draw(texture, position, Color.White);
        }

        /// <summary>
        /// Draw texture
        /// </summary>
        /// <param name="texture">Texture to draw</param>
        /// <param name="position">Position</param>
        /// <param name="depth">Depth to draw at</param>
        [Obsolete("Draw(Texture2D, Vector2, float) will be removed before #20 is merged. Use Draw(Texture2D, Vector2, Color, float) instead.")]
        public void Draw(Texture2D texture, Vector2 position, float depth)
        {
            Draw(texture, position, Color.White, depth);
        }

        /// <summary>
        /// Draw texture
        /// </summary>
        /// <param name="texture">Texture to draw</param>
        /// <param name="position">Position</param>
        /// <param name="color">Color modifier</param>
        public void Draw(Texture2D texture, Vector2 position, Color color)
        {
            Draw(texture, position, color, 0f);
        }

        /// <summary>
        /// Draw texture
        /// </summary>
        /// <param name="texture">Textyre to draw</param>
        /// <param name="position">Position</param>
        /// <param name="color">Color modifier</param>
        /// <param name="depth">Depth to draw at</param>
        public void Draw(Texture2D texture, Vector2 position, Color color, float depth)
        {
            Draw(texture, color, depth, position, null, new Vector2(0, 0));
        }

        /// <summary>
        /// Draw texture
        /// </summary>
        /// <param name="texture">Texture to draw</param>
        /// <param name="color">Color modifier</param>
        /// <param name="depth">Depth to draw at</param>
        /// <param name="position">Position</param>
        /// <param name="sourceRectangle">Source rectangle</param>
        /// <param name="origin">Origin</param>
        /// <param name="rotation">Rotation</param>
        /// <param name="disposeAfterDraw">Dispose texture after draw</param>
        public void Draw(Texture2D texture, Color color, float depth, Vector2 position, Rectangle? sourceRectangle, Vector2 origin, double rotation = 0, bool disposeAfterDraw = false)
        {
            if (!_rendering)
                throw new Exception("Cannot call Draw(...) before calling BeginRender.");

            var pos = new Vector2(position.X, position.Y);
            pos.X -= origin.X;
            pos.Y -= origin.Y;

            pos = Vector2.Transform(pos, _matrix);

            var destRectangle = new Rectangle
            {
                X = (int) pos.X,
                Y = (int) pos.Y
            };

            if (sourceRectangle.HasValue)
            {
                destRectangle.Width = (int)(sourceRectangle.Value.Width * _matrix.M11);
                destRectangle.Height = (int)(sourceRectangle.Value.Height * _matrix.M22);
            }
            else
            {
                destRectangle.Width = (int)(texture.Width * _matrix.M11);
                destRectangle.Height = (int)(texture.Height * _matrix.M22);
            }
                        
            _renderList.Add(new Renderable
                {
                    Type = RenderableType.Texture,
                    Texture = texture,
                    Color = color,
                    Depth = depth,
                    DestinationRectangle = destRectangle,
                    SourceRectangle = sourceRectangle,
                    Origin = origin,
                    Rotation = rotation,
                    Dispose = disposeAfterDraw
                }
            );
        }

        /// <summary>
        /// Draw Render Target
        /// </summary>
        /// <param name="target">Render target</param>
        /// <param name="position">Position</param>
        /// <param name="color">Color modifier</param>
        /// <param name="disposeAfterDraw">Dispose target after draw</param>
        public void Draw(RenderTarget target, Vector2 position, Color color, bool disposeAfterDraw = false)
        {
            Draw(target, new Rectangle((int) position.X, (int) position.Y, target.Width, target.Height), color,
                disposeAfterDraw);
        }

        /// <summary>
        /// Draw Render Target
        /// </summary>
        /// <param name="target">Render target</param>
        /// <param name="destRectangle">Destination rectangle</param>
        /// <param name="color">Color modifier</param>
        /// <param name="disposeAfterDraw">Dispose target after draw</param>
        public void Draw(RenderTarget target, Rectangle destRectangle, Color color, bool disposeAfterDraw = false)
        {
            if (!_rendering)
                throw new Exception("Cannot call Draw(...) before calling BeginRender.");

            var pos = Vector2.Transform(new Vector2(destRectangle.X, destRectangle.Y), _matrix);

            destRectangle.X = (int) pos.X;
            destRectangle.Y = (int) pos.Y;
            destRectangle.Width = (int)(destRectangle.Width * _matrix.M11);
            destRectangle.Height = (int)(destRectangle.Height * _matrix.M22);

            _renderList.Add(new Renderable
                {
                    Type = RenderableType.RenderTarget,
                    RenderTarget = target,
                    Color = color,
                    DestinationRectangle = destRectangle,
                    Dispose = disposeAfterDraw
                }
            );
        }

        /// <summary>
        /// Draw text
        /// </summary>
        /// <param name="text">Text to draw</param>
        /// <param name="position">Position to draw</param>
        /// <param name="font">Font to draw in</param>
        public void DrawText(string text, Vector2 position, Font font)
        {
            DrawText(text, position, font, Color.Black);
        }

        /// <summary>
        /// Draw text
        /// </summary>
        /// <param name="text">Text to draw</param>
        /// <param name="position">Position to draw</param>
        /// <param name="font">Font to draw in</param>
        /// <param name="color">Text color</param>
        /// <param name="depth">Depth to draw at</param>
        public void DrawText(string text, Vector2 position, Font font, Color color, float depth = 0)
        {
            DrawText(text, position, font, color, depth, new Vector2(0, 0), 0);
        }

        /// <summary>
        /// Draw text
        /// </summary>
        /// <param name="text">Text to draw</param>
        /// <param name="position">Position to draw</param>
        /// <param name="font">Font to draw in</param>
        /// <param name="color">Text color</param>
        /// <param name="depth">Depth to draw at</param>
        /// <param name="origin">Origin of text</param>
        /// <param name="rotation">Text Rotation</param>
        /// <param name="disposeAfterDraw">Dispose font after drawing</param>
        public void DrawText(string text, Vector2 position, Font font, Color color, float depth, Vector2 origin, double rotation, bool disposeAfterDraw = false)
        {
            if (!_rendering)
                throw new Exception("Cannot call Draw(...) before calling BeginRender.");

            var size = font.MeasureString(text);

            position = Vector2.Transform(position, _matrix);

            var destRectangle = new Rectangle((int) position.X, (int) position.Y, (int)(size.X * _matrix.M11), (int)(size.Y * _matrix.M22));

            _renderList.Add(new Renderable
                {
                    Type = RenderableType.Text,
                    Font = font,
                    Text = text,
                    DestinationRectangle = destRectangle,
                    Color = color,
                    Origin = origin,
                    Rotation =  rotation,
                    Depth = depth,
                    Dispose = disposeAfterDraw
                }
            );
        }

        /// <summary>
        /// Clean the renderer
        /// </summary>
        public void Dispose()
        {
            PlatformRenderer.Dispose();
            _renderList?.Clear();
            _renderList = null;
        }
    }
}
