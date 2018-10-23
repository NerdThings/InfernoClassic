using System;
using Inferno.Graphics;
using Inferno.Input;
using Inferno.UI;

namespace Inferno
{
    /// <summary>
    /// The gamewindow is the window that the game is displayed in
    /// </summary>
    public sealed partial class GameWindow
    {
        #region Private Fields

        private readonly Game _game;
        private readonly GraphicsDevice _graphicsDevice;
        
        #endregion
        
        #region Internal Fields
        
        internal MouseState MouseState;
        
        #endregion
        
        #region Constructors

        internal GameWindow(Game game, GraphicsDevice graphicsDevice, string title, int width, int height)
        {
            _game = game;
            _graphicsDevice = graphicsDevice;
            Init(title, width, height);
        }

        #endregion
    }
}
