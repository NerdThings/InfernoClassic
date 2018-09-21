using System;
using System.Collections.Generic;
using System.Linq;

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
        private RenderSortMode SortMode;
        internal PlatformRenderer PlatformRenderer;

        public Renderer(GraphicsManager graphicsManager)
        {
            PlatformRenderer = new PlatformRenderer(graphicsManager);
        }

        public void Begin(RenderSortMode sortMode = RenderSortMode.Immediate)
        {
            //Enable drawing
            if (_renderList == null)
                _renderList = new List<Renderable>();

            _renderList.Clear();
            _rendering = true;
            SortMode = sortMode;

            PlatformRenderer.BeginRender();
        }

        public void End()
        {
            var renderables = SortMode == RenderSortMode.Depth ? _renderList.OrderBy(o => o.Depth).ToList() : _renderList;

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

        public void Dispose()
        {
            _renderList?.Clear();
            _renderList = null;
        }
    }
}
