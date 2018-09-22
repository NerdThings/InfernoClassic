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
            _matrix = translationMatrix ?? Matrix.Identity;

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
            Draw(texture, color, depth, new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height), null, new Vector2(0, 0));
        }

        public void Draw(Texture2D texture, Color color, float depth, Rectangle destRectangle, Rectangle? sourceRectangle, Vector2 origin, double rotation = 0, bool disposeAfterDraw = false)
        {
            if (!_rendering)
                throw new Exception("Cannot call Draw(...) before calling BeginRender.");

            var pos = new Vector2(destRectangle.X, destRectangle.Y);
            pos.X -= origin.X;
            pos.Y -= origin.Y;

            pos = Vector2.Transform(pos, _matrix);

            destRectangle.X = (int)pos.X;
            destRectangle.Y = (int)pos.Y;
            destRectangle.Width *= (int) _matrix.M11;
            destRectangle.Height *= (int) _matrix.M22;
            
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

        public void Draw(RenderTarget target, Rectangle destRectangle, Color color)
        {
            if (!_rendering)
                throw new Exception("Cannot call Draw(...) before calling BeginRender.");

            var pos = Vector2.Transform(new Vector2(destRectangle.X, destRectangle.Y), _matrix);

            destRectangle.X = (int) pos.X;
            destRectangle.Y = (int) pos.Y;
            destRectangle.Width *= (int)_matrix.M11;
            destRectangle.Height *= (int)_matrix.M22;

            _renderList.Add(new Renderable
                {
                    Type = RenderableType.RenderTarget,
                    RenderTarget = target,
                    Color = color,
                    DestinationRectangle = destRectangle
                }
            );
        }

        public void DrawText(string text, Vector2 position, Font font)
        {
            DrawText(text, position, font, Color.White);
        }

        public void DrawText(string text, Vector2 position, Font font, Color color, int depth = 0)
        {
            DrawText(text, position, font, color, depth, new Vector2(0, 0), 0);
        }

        public void DrawText(string text, Vector2 position, Font font, Color color, int depth, Vector2 origin, double rotation)
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
                    Depth = depth
                }
            );
        }

        public void DrawLine(Vector2 pointA, Vector2 pointB, Color color, int lineWidth = 1, float depth = 0)
        {
            if (!_rendering)
                throw new Exception("Cannot call Draw(...) before calling BeginRender.");

            //Scale
            var xDist = (pointB.X - pointA.X);
            var yDist = (pointB.Y - pointA.Y);

            //Get end point
            pointB.X = pointA.X + xDist;
            pointB.Y = pointA.Y + yDist;

            pointA = Vector2.Transform(pointA, _matrix);
            pointB = Vector2.Transform(pointB, _matrix);

            _renderList.Add(new Renderable
                {
                    Type = RenderableType.Line,
                    PointA = pointA,
                    PointB = pointB,
                    Color = color,
                    Depth = depth,
                    LineWidth = lineWidth
                }
            );
        }

        public void DrawRectangle(Rectangle rect, Color color, bool fill = false)
        {
            if (!_rendering)
                throw new Exception("Cannot call Draw(...) before calling BeginRender.");

            var pos = Vector2.Transform(new Vector2(rect.X, rect.Y), _matrix);

            rect.X = (int)pos.X;
            rect.Y = (int)pos.Y;
            rect.Width = (int)(rect.Width * _matrix.M11);
            rect.Height = (int)(rect.Height * _matrix.M22);

            _renderList.Add(new Renderable
                {
                    Type = fill ? RenderableType.FilledRectangle : RenderableType.Rectangle,
                    Color = color, 
                    DestinationRectangle = rect,
                }
            );
        }

        public void DrawCircle(Vector2 position, int radius, Color color)
        {
            if (!_rendering)
                throw new Exception("Cannot call Draw(...) before calling BeginRender.");

            position = Vector2.Transform(position, _matrix);

            var destRectangle = new Rectangle((int)position.X, (int)position.Y, radius, radius);

            destRectangle.Width *= (int)_matrix.M11;
            destRectangle.Height *= (int)_matrix.M22;

            _renderList.Add(new Renderable
                {
                    Type = RenderableType.Ellipse,
                    DestinationRectangle = destRectangle,
                    Color = color
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
