using Inferno.Runtime.Input;

namespace Inferno.Runtime
{
    //This basically just implements a wrapper for PlatformGameWindow

    /// <summary>
    /// The gamewindow is the window that the game is displayed in
    /// </summary>
    public class GameWindow
    {
        #region Constructors

        public GameWindow(string title, int width, int height)
        {
            PlatformWindow = new PlatformGameWindow(title, width, height);
        }

        #endregion

        #region Properties

        public bool AllowResize
        {
            get => PlatformWindow.AllowResize;
            set => PlatformWindow.AllowResize = value;
        }

        public Rectangle Bounds
        {
            get => PlatformWindow.Bounds;
            set => PlatformWindow.Bounds = value;
        }

        public int Width
        {
            get => PlatformWindow.Width;
            set => PlatformWindow.Width = value;
        }

        public int Height
        {
            get => PlatformWindow.Height;
            set => PlatformWindow.Height = value;
        }

        public bool AllowAltF4
        {
            get => PlatformWindow.AllowAltF4;
            set => PlatformWindow.AllowAltF4 = value;
        }

        public Point Position
        {
            get => PlatformWindow.Position;
            set => PlatformWindow.Position = value;
        }

        //TODO: Orientation (Mobile??)

        public string Title
        {
            get => PlatformWindow.Title;
            set => PlatformWindow.Title = value;
        }

        internal MouseState MouseState;

        internal PlatformGameWindow PlatformWindow;

        public void Exit()
        {
            PlatformWindow.Exit();
        }

#endregion
    }
}
