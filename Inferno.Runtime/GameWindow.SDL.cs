#if DESKTOP

using SDL2;

namespace Inferno.Runtime
{
    /// <summary>
    /// SDL Specific GameWindow features
    /// </summary>
    public class GameWindow : BaseGameWindow
    {
        public GameWindow(string title, int width, int height)
        {
            //Create window using specified settings
            Handle = SDL.SDL_CreateWindow(title, SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, width, height, SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL);

            //Get the values given to us
            SDL.SDL_GetWindowPosition(Handle, out var x, out var y);
            SDL.SDL_GetWindowSize(Handle, out var w, out var h);

            //Set these properties
            Position = new Point(x, y);
            Bounds = new Rectangle(Position.X, Position.Y, w, h);
        }

        public sealed override bool AllowResize
        {
            get
            {
                var flags = (SDL.SDL_WindowFlags)SDL.SDL_GetWindowFlags(Handle);
                return (flags & SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE) != 0;
            }
            set => SDL.SDL_SetWindowResizable(Handle, value ? SDL.SDL_bool.SDL_TRUE : SDL.SDL_bool.SDL_FALSE);
        }

        public sealed override Rectangle Bounds
        {
            get
            {
                SDL.SDL_GetWindowSize(Handle, out var width, out var height);
                return new Rectangle(Position.X, Position.Y, width, height);
            }
            set
            {
                SDL.SDL_SetWindowSize(Handle, value.Width, value.Height);
                SDL.SDL_SetWindowPosition(Handle, value.X, value.Y);
            }
        }

        public sealed override int Width
        {
            get => Bounds.Width;
            set
            {
                var bounds = Bounds;
                bounds.Width = value;
                Bounds = bounds;
            }
        }

        public sealed override int Height
        {
            get => Bounds.Height;
            set
            {
                var bounds = Bounds;
                bounds.Height = value;
                Bounds = bounds;
            }
        }

        public sealed override Point Position
        {
            get
            {
                SDL.SDL_GetWindowPosition(Handle, out var x, out var y);
                return new Point(x, y);
            }
            set => SDL.SDL_SetWindowPosition(Handle, value.X, value.Y);
        }

        public sealed override bool AllowAltF4 { get; set; }

        public sealed override string Title
        {
            get => SDL.SDL_GetWindowTitle(Handle);
            set => SDL.SDL_SetWindowTitle(Handle, value);
        }
    }
}

#endif