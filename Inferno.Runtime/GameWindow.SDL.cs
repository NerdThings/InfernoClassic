#if DESKTOP

using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using SDL2;

namespace Inferno.Runtime
{
    /// <summary>
    /// SDL Specific GameWindow features
    /// </summary>
    public class PlatformGameWindow
    {
        internal static IntPtr Handle { get; set; }
        internal static IntPtr Surface { get; set; }
        internal static IntPtr Renderer { get; set; }

        internal static List<IntPtr> LoadedTextures;

        internal static void RegisterTexture(IntPtr handle)
        {
            LoadedTextures.Add(handle);
        }

        internal static void UnRegisterTexture(IntPtr handle)
        {
            LoadedTextures.Remove(handle);
        }

        public PlatformGameWindow(string title, int width, int height)
        {
            //Init SDL
            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
            {
                throw new Exception("SDL Failed to initialise");
            }

            //Create window using specified settings
            Handle = SDL.SDL_CreateWindow(title, SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, width, height, SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL);

            if (Handle == null)
            {
                throw new Exception("Window could not be created.");
            }

            //Create renderer
            Renderer = SDL.SDL_CreateRenderer(Handle, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);

            if (Renderer == IntPtr.Zero)
            {
                throw new Exception("Failed to create renderer");
            }

            //Init render color
            SDL.SDL_SetRenderDrawColor(Renderer, 0xFF, 0xFF, 0xFF, 0xFF);

            //Init images, SUPPORT ALL THE THINGS
            const SDL_image.IMG_InitFlags flags = SDL_image.IMG_InitFlags.IMG_INIT_JPG
                                        & SDL_image.IMG_InitFlags.IMG_INIT_PNG
                                        & SDL_image.IMG_InitFlags.IMG_INIT_TIF
                                        & SDL_image.IMG_InitFlags.IMG_INIT_WEBP;

            if ((SDL_image.IMG_Init(flags) & (int)flags) != (int)flags)
            {
                throw new Exception("SDL_image failed to intialiise.");
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

            //Init texture array
            LoadedTextures = new List<IntPtr>();
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
            foreach (var tex in LoadedTextures)
            {
                SDL.SDL_DestroyTexture(tex);
            }

            LoadedTextures.Clear();

            SDL.SDL_DestroyRenderer(Renderer);
            SDL.SDL_DestroyWindow(Handle);
            Handle = IntPtr.Zero;
            Renderer = IntPtr.Zero;

            SDL_image.IMG_Quit();
            SDL.SDL_Quit();
        }
    }
}

#endif