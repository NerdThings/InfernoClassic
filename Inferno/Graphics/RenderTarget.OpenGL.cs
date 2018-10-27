#if OPENGL

using System;
using OpenTK.Graphics.OpenGL;

namespace Inferno.Graphics
{
    /// <summary>
    /// Desktop Specific render target code
    /// </summary>
    public partial class RenderTarget
    {
        internal int Framebuffer { get; set; }
        internal int RenderedTexture { get; set; }
        internal int DepthRenderBuffer { get; set; }

        public RenderTarget(int width, int height)
        {
            Width = width;
            Height = height;

            //Create the frame buffer
            GL.GenFramebuffers(1, out int frameBuffer);

            Framebuffer = frameBuffer;

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, Framebuffer);

            //Create the texture we will render to
            GL.GenTextures(1, out int texture);
            RenderedTexture = texture;

            //Bind the texture
            GL.BindTexture(TextureTarget.Texture2D, RenderedTexture);

            //Empty image
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, width, height, 0, PixelFormat.Rgb,
                PixelType.UnsignedByte, IntPtr.Zero);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Nearest);

            //Depth buffer
            GL.GenRenderbuffers(1, out int renderBuffer);
            DepthRenderBuffer = renderBuffer;
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, DepthRenderBuffer);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent, width, height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, DepthRenderBuffer);

            //Set the texture as color attachment
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, RenderedTexture, 0);

            //Set the list of draw buffers
            DrawBuffersEnum[] drawBuffers = { DrawBuffersEnum.ColorAttachment0 };
            GL.DrawBuffers(1, drawBuffers);

            //Check the buffer is okay
            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
                throw new Exception("Failed to create Frame Buffer");

            //Detach the framebuffer
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }
    }
}

#endif