using System;
using Inferno.Graphics;
using Inferno.Input;
using Inferno.UI;

namespace Inferno
{
    /// <summary>
    /// The gamewindow is the window that the game is displayed in
    /// </summary>
    public abstract class Window
    {
        #region Private Fields

        protected readonly Game Game;
        protected readonly GraphicsDevice GraphicsDevice;
        
        #endregion
        
        #region Constructors

        internal Window(Game game, GraphicsDevice graphicsDevice, string title, int width, int height)
        {
            Game = game;
            GraphicsDevice = graphicsDevice;
        }

        #endregion

        #region Properties

        public abstract bool AllowResize { get; set; }

        public abstract Rectangle Bounds { get; set; }

        public abstract int Width { get; set; }

        public abstract int Height { get; set; }

        public abstract bool AllowAltF4 { get; set; }

        public abstract Point Position { get; set; }

        public abstract string Title { get; set; }

        public abstract bool Fullscreen { get; set; }
        /*{
            get => PlatformWindow.Fullscreen;
            set
            {
                Bounds = value ? _graphicsDevice.ScreenBounds : new Rectangle(-1, -1, _game.VirtualWidth, _game.VirtualHeight);

                PlatformWindow.Fullscreen = value;
                
                _game.OnResize?.Invoke(this, new Game.OnResizeEventArgs(Bounds));
            }
        }*/

        public abstract bool VSync { get; set; }

        public abstract bool ShowCursor { get; set; }

#endregion

        internal MouseState MouseState;

        public abstract void Exit();

    }
}
