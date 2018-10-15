using System;
using System.Collections.Generic;
using System.Text;

namespace Inferno.Graphics
{
    public enum PlatformType
    {
        OpenGL,
        DirectX
    }
    public class GraphicsDevice : IDisposable
    {
        internal readonly PlatformGraphicsDevice PlatformGraphicsDevice;
        internal GameWindow GameWindow;
        internal Shader FragmentShader;
        internal Shader VertexShader;

        private RenderTarget _currentRenderTarget;
        private static GraphicsDevice _self;
        private readonly List<Texture2D> _textureDisposeList;
        private readonly List<RenderTarget> _targetDisposeList;

        public RenderTarget RenderTarget
        {
            get => GetCurrentRenderTarget();
            set => SetRenderTarget(value);
        }

        public Rectangle ScreenBounds => PlatformGraphicsDevice.ScreenBounds;

        public static PlatformType PlatformType => PlatformGraphicsDevice.PlatformType;

        public GraphicsDevice()
        {
            PlatformGraphicsDevice = new PlatformGraphicsDevice(this);

            _textureDisposeList = new List<Texture2D>();
            _targetDisposeList = new List<RenderTarget>();
            _self = this;
        }

        public void AttachWindow(GameWindow window)
        {
            GameWindow = window;

            //Initial clear
            Clear(Color.White);

            PlatformGraphicsDevice.WindowAttached(window);
        }

        public void ApplyShader(Shader shader)
        {
            PlatformGraphicsDevice.SetShader(shader, shader.Type == ShaderType.Fragment ? FragmentShader : VertexShader);

            if (shader.Type == ShaderType.Fragment)
                FragmentShader = shader;
            else
                VertexShader = shader;
        }

        public void Clear(Color color)
        {
            if (GameWindow == null)
                throw new Exception("Cannot use Graphics Device until a game window is attached.");

            PlatformGraphicsDevice.Clear(color);
        }

        public void SetRenderTarget(RenderTarget target)
        {
            if (GameWindow == null)
                throw new Exception("Cannot use Graphics Device until a game window is attached.");

            _currentRenderTarget = target;
            PlatformGraphicsDevice.SetRenderTarget(target);
        }

        public RenderTarget GetCurrentRenderTarget()
        {
            if (GameWindow == null)
                throw new Exception("Cannot use Graphics Device until a game window is attached.");

            return _currentRenderTarget;
        }

        public void BeginDraw()
        {
            PlatformGraphicsDevice.BeginDraw();
        }

        public void EndDraw()
        {
            if (GameWindow == null)
                throw new Exception("Cannot use Graphics Device until a game window is attached.");

            //Present
            PlatformGraphicsDevice.EndDraw();
            
            //Dispose textures
            lock (_textureDisposeList)
            {
                foreach (var tex in _textureDisposeList)
                {
                    PlatformGraphicsDevice.DisposeTexture(tex);
                }
            }

            //Dispose rendertargets
            lock (_targetDisposeList)
            {
                foreach (var target in _targetDisposeList)
                {
                    PlatformGraphicsDevice.DisposeRenderTarget(target);
                }
            }
        }

        public void Dispose()
        {
            PlatformGraphicsDevice.Dispose();
        }

        internal static void DisposeTexture(Texture2D texture)
        {
            if (_self.GameWindow == null)
                throw new Exception("Cannot use Graphics Device until a game window is attached.");

            _self._textureDisposeList.Add(texture);
        }

        internal static void DisposeRenderTarget(RenderTarget target)
        {
            if (_self.GameWindow == null)
                throw new Exception("Cannot use Graphics Device until a game window is attached.");

            _self._targetDisposeList.Add(target);
        }
    }
}
