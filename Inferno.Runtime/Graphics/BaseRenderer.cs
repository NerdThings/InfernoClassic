using System;
using System.Collections.Generic;

namespace Inferno.Runtime.Graphics
{
    public class Renderer : IDisposable
    {
        private bool _rendering;
        private List<Renderable> _renderList;
        internal PlatformRenderer PlatformRenderer;

        public Renderer()
        {
            PlatformRenderer = new PlatformRenderer();
        }

        public void Begin()
        {
            //Enable drawing
            if (_renderList == null)
                _renderList = new List<Renderable>();

            _renderList.Clear();
            _rendering = true;
        }

        public void End()
        {
            foreach (var renderable in _renderList)
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
                    HasTexture = true,
                    Texture = texture,
                    Color = color,
                    Depth = depth,
                    DestinationRectangle = destRectangle,
                    SourceRectangle = sourceRectangle
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
