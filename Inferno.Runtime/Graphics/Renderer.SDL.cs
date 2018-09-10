#if DESKTOP

using System;
using System.Collections.Generic;

namespace Inferno.Runtime.Graphics
{
    public class Renderer : BaseRenderer
    {
        public override void Begin()
        {
            //Enable drawing
            if (RenderList == null)
                RenderList = new List<Renderable>();

            RenderList.Clear();
            Rendering = true;
        }

        public override void End()
        {
            //TODO: When end render, actually render
            Rendering = false;
        }

        public override void Draw(Texture2D texture, Vector2 position)
        {
            Draw(texture, position, 0);
        }

        public override void Draw(Texture2D texture, Vector2 position, int depth)
        {
            if (!Rendering)
                throw new Exception("Cannot call Draw(...) before calling BeginRender.");

            RenderList.Add(new Renderable
                {
                    Depth = depth,
                    HasTexture = true,
                    Position = position,
                    Texture = texture
                }
            );
        }

        public override void Dispose()
        {
            RenderList.Clear();
            RenderList = null;
        }
    }
}

#endif