#if SDL

using System;
using OpenTK.Graphics.OpenGL;

namespace Inferno.Graphics
{
    public partial class GraphicsDevice
    {
        public const PlatformType PlatformType = Graphics.PlatformType.OpenGL;

        internal int ProgramId;

        public void AttachWindow(GameWindow window)
        {
            _gameWindow = window;

            //Initial clear
            Clear(Color.White);
            
            ProgramId = GL.CreateProgram();
        }

        public void Clear(Color color)
        {
            if (_gameWindow == null)
                throw new Exception("Cannot use Graphics Device until a game window is attached.");
            
            GL.ClearColor(color.R, color.G, color.B, color.A);
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }

        public void SetRenderTarget(RenderTarget target)
        {
            if (_gameWindow == null)
                throw new Exception("Cannot use Graphics Device until a game window is attached.");

            _currentRenderTarget = target;
            
            var width = target?.Width ?? _gameWindow.Width;
            var height = target?.Height ?? _gameWindow.Height;

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, target?.Framebuffer ?? 0);
            GL.Viewport(0, 0, width, height);
        }

        public void SetShader(Shader shader)
        {
            if (shader.Type == ShaderType.Fragment)
                FragmentShader = shader;
            else
                VertexShader = shader;
        }

        public void BeginDraw()
        {
            GL.UseProgram(ProgramId);
        }

        private void DisposeTextureNow(Texture2D texture)
        {
            //Check that the device is initialised
            if (_self._gameWindow == null)
                throw new Exception("Cannot use Graphics Device until a game window is attached.");
            
            //Confirm no textures are bound
            GL.BindTexture(TextureTarget.Texture2D, 0);

            //Delete the texture
            GL.DeleteTexture(texture.Id);
        }

        private void DisposeRenderTargetNow(RenderTarget target)
        {            
            //Check that the device is initialised
            if (_self._gameWindow == null)
                throw new Exception("Cannot use Graphics Device until a game window is attached.");
            
            //Delete framebuffer
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.DeleteFramebuffer(target.Framebuffer);

            //Delete texture
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.DeleteTexture(target.RenderedTexture);

            //Delete depth buffer
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
            GL.DeleteRenderbuffer(target.DepthRenderBuffer);
        }
    }
}

#endif