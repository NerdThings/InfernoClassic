#if DESKTOP

using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Inferno.Graphics;
using SDL2;

namespace Inferno
{
    /// <summary>
    /// SDL Specific GameWindow code
    /// </summary>
    public class PlatformGameWindow
    {
        internal IntPtr Handle { get; set; }
        internal IntPtr Surface { get; set; }

        public PlatformGameWindow(string title, int width, int height)
        {
            //Create window using specified settings
            Handle = SDL2.SDL.SDL_CreateWindow(title, SDL2.SDL.SDL_WINDOWPOS_UNDEFINED, SDL2.SDL.SDL_WINDOWPOS_UNDEFINED, width, height, SDL2.SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL);

            if (Handle == IntPtr.Zero)
                throw new Exception("Window could not be created. " + SDL2.SDL.SDL_GetError());

            //Save surface
            Surface = SDL2.SDL.SDL_GetWindowSurface(Handle);

            //Update window surface
            if (SDL2.SDL.SDL_UpdateWindowSurface(Handle) < 0)
                throw new Exception("Unable to update window surface. " + SDL2.SDL.SDL_GetError());
            
            //Get the values given to us
            SDL2.SDL.SDL_GetWindowPosition(Handle, out var x, out var y);
            SDL2.SDL.SDL_GetWindowSize(Handle, out var w, out var h);

            //Set these properties
            Position = new Point(x, y);
            Bounds = new Rectangle(Position.X, Position.Y, w, h);
        }

        
        public bool AllowResize
        {
            get
            {
                var flags = (SDL2.SDL.SDL_WindowFlags)SDL2.SDL.SDL_GetWindowFlags(Handle);
                return (flags & SDL2.SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE) != 0;
            }
            set => SDL2.SDL.SDL_SetWindowResizable(Handle, value ? SDL2.SDL.SDL_bool.SDL_TRUE : SDL2.SDL.SDL_bool.SDL_FALSE);
        }

        public Rectangle Bounds
        {
            get
            {
                SDL2.SDL.SDL_GetWindowSize(Handle, out var width, out var height);
                return new Rectangle(Position.X, Position.Y, width, height);
            }
            set
            {
                SDL2.SDL.SDL_SetWindowSize(Handle, value.Width, value.Height);
                SDL2.SDL.SDL_SetWindowPosition(Handle, value.X, value.Y);
            }
        }

        public int Width
        {
            get => Bounds.Width;
            set
            {
                var bounds = Bounds;
                bounds.Width = value;
                Bounds = bounds;
            }
        }

        public int Height
        {
            get => Bounds.Height;
            set
            {
                var bounds = Bounds;
                bounds.Height = value;
                Bounds = bounds;
            }
        }

        public Point Position
        {
            get
            {
                SDL2.SDL.SDL_GetWindowPosition(Handle, out var x, out var y);
                return new Point(x, y);
            }
            set => SDL2.SDL.SDL_SetWindowPosition(Handle, value.X, value.Y);
        }

        public bool AllowAltF4
        {
            get => true;
            set => SDL2.SDL.SDL_SetHint(SDL2.SDL.SDL_HINT_WINDOWS_NO_CLOSE_ON_ALT_F4, value ? "0" : "1");
        }

        public string Title
        {
            get => SDL2.SDL.SDL_GetWindowTitle(Handle);
            set => SDL2.SDL.SDL_SetWindowTitle(Handle, value);
        }

        public void Exit()
        {
            SDL2.SDL.SDL_DestroyWindow(Handle);
            Handle = IntPtr.Zero;
        }
    }
}

#endif