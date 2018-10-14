#if DESKTOP

using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;
using SDL2;

namespace Inferno.Graphics
{
    internal class PlatformGraphicsDevice : IDisposable
    {
        public const PlatformType PlatformType = Graphics.PlatformType.OpenGL;

        private readonly GraphicsDevice _graphicsDevice;
        private int _GLShaderProgram;

        public PlatformGraphicsDevice(GraphicsDevice graphicsDevice)
        {
            if (SDL.SDL_Init(SDL.SDL_INIT_EVERYTHING) < 0)
                throw new Exception("SDL Failed to initialise.");

            _graphicsDevice = graphicsDevice;
        }

        public void WindowAttached(GameWindow window)
        {
            _GLShaderProgram = GL.CreateProgram();
        }

        public void Clear(Color color)
        {
            GL.ClearColor(color.R, color.G, color.B, color.A);
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }

        public void SetRenderTarget(RenderTarget target)
        {
            var width = target?.Width ?? _graphicsDevice.GameWindow.Width;
            var height = target?.Height ?? _graphicsDevice.GameWindow.Height;

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, target?.PlatformRenderTarget.Framebuffer ?? 0);
            GL.Viewport(0, 0, width, height);
            GL.LoadIdentity();
            GL.Ortho(0, width, height, 0, -1, 1);
        }

        private void DetachShader(Shader shader)
        {
            GL.DetachShader(_GLShaderProgram, shader.OpenGLShader.ShaderId);
        }

        private void AttachShader(Shader shader)
        {
            GL.AttachShader(_GLShaderProgram, shader.OpenGLShader.ShaderId);
        }

        public void SetShader(Shader newShader, Shader previousShader)
        {
            if (previousShader != null)
                DetachShader(previousShader);
            AttachShader(newShader);
        }

        public void BeginDraw()
        {
            GL.UseProgram(_GLShaderProgram);
        }

        public void EndDraw()
        {
            SDL.SDL_GL_SwapWindow(_graphicsDevice.GameWindow.PlatformWindow.Handle);
        }

        public void DisposeTexture(Texture2D texture)
        {
            //Confirm no textures are bound
            GL.BindTexture(TextureTarget.Texture2D, 0);

            //Delete the texture
            GL.DeleteTexture(texture.PlatformTexture2D.Id);
        }

        public void DisposeRenderTarget(RenderTarget target)
        {
            //Delete framebuffer
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.DeleteFramebuffer(target.PlatformRenderTarget.Framebuffer);

            //Delete texture
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.DeleteTexture(target.PlatformRenderTarget.RenderedTexture);

            //Delete depth buffer
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
            GL.DeleteRenderbuffer(target.PlatformRenderTarget.DepthRenderBuffer);
        }

        public void Dispose()
        {
            SDL.SDL_Quit();
        }
    }
}

#endif