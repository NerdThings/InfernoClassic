#if SDL

using System;
using OpenTK.Graphics.OpenGL;

namespace Inferno.Graphics
{
    public partial class GraphicsDevice
    {
        public const PlatformType PlatformType = Graphics.PlatformType.OpenGL;

        private int _glShaderProgram;

        public void AttachWindow(GameWindow window)
        {
            _gameWindow = window;

            //Initial clear
            Clear(Color.White);
            
            _glShaderProgram = GL.CreateProgram();
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

        private void DetachShader(Shader shader)
        {
            GL.DetachShader(_glShaderProgram, shader.OpenGLShader.ShaderId);
        }

        private void AttachShader(Shader shader)
        {
            GL.AttachShader(_glShaderProgram, shader.OpenGLShader.ShaderId);
        }

        public void SetShader(Shader newShader, Shader previousShader)
        {
            if (previousShader != null)
                DetachShader(previousShader);
            AttachShader(newShader);
            
            if (newShader.Type == ShaderType.Fragment)
                _fragmentShader = newShader;
            else
                _vertexShader = newShader;
        }

        public void BeginDraw()
        {
            GL.UseProgram(_glShaderProgram);
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