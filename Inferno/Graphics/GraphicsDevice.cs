using System;
using System.Collections.Generic;

namespace Inferno.Graphics
{
    public enum PlatformType
    {
        OpenGL,
        DirectX
    }
    
    public partial class GraphicsDevice : IDisposable
    {
        private GameWindow _gameWindow;
        private Shader _fragmentShader;
        private Shader _vertexShader;
        private RenderTarget _currentRenderTarget;
        private static GraphicsDevice _self;
        private readonly List<Texture2D> _textureDisposeList;
        private readonly List<RenderTarget> _targetDisposeList;

        public RenderTarget RenderTarget
        {
            get => GetCurrentRenderTarget();
            set => SetRenderTarget(value);
        }

        public GraphicsDevice()
        {
            Init();
            _textureDisposeList = new List<Texture2D>();
            _targetDisposeList = new List<RenderTarget>();
            _self = this;
        }

        public RenderTarget GetCurrentRenderTarget()
        {
            if (_gameWindow == null)
                throw new Exception("Cannot use Graphics Device until a game window is attached.");

            return _currentRenderTarget;
        }

        public void EndDraw()
        {
            if (_gameWindow == null)
                throw new Exception("Cannot use Graphics Device until a game window is attached.");

            //Present
            Present();
            
            //Dispose textures
            lock (_textureDisposeList)
            {
                foreach (var tex in _textureDisposeList)
                {
                    DisposeTextureNow(tex);
                }
            }

            //Dispose rendertargets
            lock (_targetDisposeList)
            {
                foreach (var target in _targetDisposeList)
                {
                    DisposeRenderTargetNow(target);
                }
            }
        }

        public void Dispose()
        {
            Dispose1();
        }

        internal static void DisposeTexture(Texture2D texture)
        {
            if (_self._gameWindow == null)
                throw new Exception("Cannot use Graphics Device until a game window is attached.");

            _self._textureDisposeList.Add(texture);
        }

        internal static void DisposeRenderTarget(RenderTarget target)
        {
            if (_self._gameWindow == null)
                throw new Exception("Cannot use Graphics Device until a game window is attached.");

            _self._targetDisposeList.Add(target);
        }
    }
}
