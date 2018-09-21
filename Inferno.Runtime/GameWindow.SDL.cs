﻿#if DESKTOP

using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Inferno.Runtime.Graphics;
using SDL2;

namespace Inferno.Runtime
{
    /// <summary>
    /// SDL Specific GameWindow features
    /// </summary>
    public class PlatformGameWindow
    {
        internal IntPtr Handle { get; set; }
        internal IntPtr Surface { get; set; }

        public PlatformGameWindow(string title, int width, int height)
        {
            //Create window using specified settings
            Handle = SDL.SDL_CreateWindow(title, SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, width, height, SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL);

            if (Handle == null)
            {
                throw new Exception("Window could not be created.");
            }

            //Save surface
            Surface = SDL.SDL_GetWindowSurface(Handle);

            //Update window surface
            SDL.SDL_UpdateWindowSurface(Handle);

            //Get the values given to us
            SDL.SDL_GetWindowPosition(Handle, out var x, out var y);
            SDL.SDL_GetWindowSize(Handle, out var w, out var h);

            //Set these properties
            Position = new Point(x, y);
            Bounds = new Rectangle(Position.X, Position.Y, w, h);
        }

        
        public bool AllowResize
        {
            get
            {
                var flags = (SDL.SDL_WindowFlags)SDL.SDL_GetWindowFlags(Handle);
                return (flags & SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE) != 0;
            }
            set => SDL.SDL_SetWindowResizable(Handle, value ? SDL.SDL_bool.SDL_TRUE : SDL.SDL_bool.SDL_FALSE);
        }

        public Rectangle Bounds
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
                SDL.SDL_GetWindowPosition(Handle, out var x, out var y);
                return new Point(x, y);
            }
            set => SDL.SDL_SetWindowPosition(Handle, value.X, value.Y);
        }

        public bool AllowAltF4
        {
            get => true;
            set => SDL.SDL_SetHint(SDL.SDL_HINT_WINDOWS_NO_CLOSE_ON_ALT_F4, value ? "0" : "1");
        }

        public string Title
        {
            get => SDL.SDL_GetWindowTitle(Handle);
            set => SDL.SDL_SetWindowTitle(Handle, value);
        }

        public bool Run()
        {
            while (SDL.SDL_PollEvent(out var e) != 0)
            {
                if (e.type == SDL.SDL_EventType.SDL_QUIT)
                {
                    return false;
                }
            }

            return true;
        }

        public void Exit()
        {
            SDL.SDL_DestroyWindow(Handle);
            Handle = IntPtr.Zero;
        }
    }
}

#endif