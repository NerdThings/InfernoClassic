using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Policy;
using Inferno.Runtime.Graphics.Text;

namespace Inferno.Runtime.Graphics
{
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
    public class Renderer : IDisposable
    {
        private bool _rendering;
        private List<Renderable> _renderList;
        private RenderSortMode _sortMode;
        private Matrix _matrix;
        internal PlatformRenderer PlatformRenderer;

        public Renderer(GraphicsManager graphicsManager)
        {
            PlatformRenderer = new PlatformRenderer(graphicsManager);
            _matrix = Matrix.Identity;
        }

        public void Begin(RenderSortMode sortMode = RenderSortMode.Immediate, Matrix? translationMatrix = null)
        {
            //Enable drawing
            if (_renderList == null)
                _renderList = new List<Renderable>();

            _renderList.Clear();
            _rendering = true;
            _sortMode = sortMode;
            if (translationMatrix.HasValue)
                _matrix = translationMatrix.Value;
            else
                _matrix = Matrix.Identity;

            PlatformRenderer.BeginRender();
        }

        public void End()
        {
            var renderables = _sortMode == RenderSortMode.Depth ? _renderList.OrderBy(o => o.Depth).ToList() : _renderList;

            foreach (var renderable in renderables)
            {
                PlatformRenderer.Render(renderable);
            }
            PlatformRenderer.EndRender();

            _rendering = false;
        }

        public void Draw(Texture2D texture, Vector2 position)
        {
            Draw(texture, position, Color.White);
        }

        public void Draw(Texture2D texture, Vector2 position, float depth)
        {
            Draw(texture, position, Color.White, depth);
        }

        public void Draw(Texture2D texture, Vector2 position, Color color)
        {
            Draw(texture, position, color, 0f);
        }

        public void Draw(Texture2D texture, Vector2 position, Color color, float depth)
        {
            Draw(texture, color, depth, new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height), null);
        }

        public void Draw(Texture2D texture, Color color, float depth, Rectangle destRectangle, Rectangle? sourceRectangle)
        {
            if (!_rendering)
                throw new Exception("Cannot call Draw(...) before calling BeginRender.");

            destRectangle.X += (int)_matrix.M41;
            destRectangle.Y += (int)_matrix.M42;
            destRectangle.Width *= (int) _matrix.M11;
            destRectangle.Height *= (int) _matrix.M22;
            
            //TODO: Rotation support

            _renderList.Add(new Renderable
                {
                    Texture = texture,
                    Color = color,
                    Depth = depth,
                    DestinationRectangle = destRectangle,
                    SourceRectangle = sourceRectangle
                }
            );
        }

        public void Draw(RenderTarget target, Rectangle destRectangle, Color color)
        {
            if (!_rendering)
                throw new Exception("Cannot call Draw(...) before calling BeginRender.");

            _renderList.Add(new Renderable
                {
                    RenderTarget = target,
                    Color = color,
                    DestinationRectangle = destRectangle
                }
            );
        }

        public void DrawText(string text, Vector2 position, Font font, Color color)
        {
            if (!_rendering)
                throw new Exception("Cannot call Draw(...) before calling BeginRender.");

            var size = font.MeasureString(text);

            _renderList.Add(new Renderable
                {
                    Font = font,
                    Text = text,
                    DestinationRectangle = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y),
                    Color = color
                }
            );
        }

        public void DrawLine(Vector2 pointA, Vector2 pointB, Color color)
        {
            if (!_rendering)
                throw new Exception("Cannot call Draw(...) before calling BeginRender.");

            _renderList.Add(new Renderable
                {
                    Line = true,
                    PointA = pointA,
                    PointB = pointB,
                    Color = color
                }
            );
        }

        public void DrawRectangle(Rectangle rect, Color color, bool fill = false)
        {
            if (!_rendering)
                throw new Exception("Cannot call Draw(...) before calling BeginRender.");

            _renderList.Add(new Renderable
                {
                    Rectangle = true,
                    Color = color, 
                    DestinationRectangle = rect,
                    FillRectangle = fill
                }
            );
        }

        public void Dispose()
        {
            _renderList?.Clear();
            _renderList = null;
        }
    }
}
