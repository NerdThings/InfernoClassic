using System;
using System.Collections.Generic;

namespace Inferno.Runtime.Graphics
{
    public abstract class BaseRenderer : IDisposable
    {
        protected bool Rendering = false;
        protected List<Renderable> RenderList;

        public abstract void Begin();

        public abstract void End();

        public abstract void Draw(Texture2D texture, Vector2 position);

        public abstract void Draw(Texture2D texture, Vector2 position, int depth);

        public abstract void Dispose();
    }
}
