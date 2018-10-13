#if DESKTOP

using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using SDL2;

namespace Inferno.Graphics
{
    /// <summary>
    /// Desktop Specific management code
    /// </summary>
    internal class PlatformGraphicsManager
    {
        public PlatformGraphicsManager()
        {
            if (SDL.SDL_Init(SDL.SDL_INIT_EVERYTHING) < 0)
                throw new Exception("SDL Failed to initialise.");
        }

        internal void Setup(GameWindow window)
        {
            //Set initial clear color
            Clear(Color.White);
        }

        public void Clear(Color color)
        {
            GL.ClearColor(color.R, color.G, color.B, color.A);
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }

        public void SetRenderTarget(RenderTarget target)
        {
            if (target != null)
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, target.PlatformRenderTarget.Framebuffer);
                GL.Viewport(0, 0, target.Width, target.Height);
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadIdentity();
                GL.Ortho(0, target.Width, target.Height, 0, -1, 1);
            }
            else
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
                GL.Viewport(0, 0, Game.Instance.Window.Width, Game.Instance.Window.Height);
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadIdentity();
                GL.Ortho(0, Game.Instance.Window.Width, Game.Instance.Window.Height, 0, -1, 1);
            }
        }

        private readonly List<int> _deleteTextures = new List<int>();

        public void DisposeTexture(Texture2D texture)
        {
            _deleteTextures.Add(texture.PlatformTexture2D.Id);
        }

        public void Present()
        {
            //Delete textures
            lock (_deleteTextures)
            {
                foreach (var tex in _deleteTextures)
                {
                    if (tex > 0)
                        GL.DeleteTexture(tex);
                }
            }
            _deleteTextures.Clear();

            //Swap window
            SDL.SDL_GL_SwapWindow(Game.Instance.Window.PlatformWindow.Handle);
        }

        internal void Dispose()
        {
            SDL.SDL_Quit();
        }
    }
}

#endif