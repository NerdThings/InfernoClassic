#if DESKTOP

using System;
using System.IO;
using System.Reflection;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using SDL2;

namespace Inferno
{
    /// <summary>
    /// Desktop Specific GameWindow code
    /// </summary>
    public class PlatformGameWindow
    {
        internal IntPtr Handle { get; set; }
        internal IntPtr OpenGlContext { get; set; }
        internal int ProgramId { get; set; }

        public ContextHandle GetContextHandle()
        {
            return new ContextHandle(SDL.SDL_GL_GetCurrentContext());
        }

        public PlatformGameWindow(string title, int width, int height)
        {
            #region SDL Init

            //Create window using specified settings
            Handle = SDL.SDL_CreateWindow(title, SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, width, height, SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL);

            if (Handle == IntPtr.Zero)
                throw new Exception("Window could not be created. " + SDL.SDL_GetError());

            #endregion

            #region OpenGL

            //Set OpenGL Options
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MAJOR_VERSION, 3);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MINOR_VERSION, 1);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_PROFILE_MASK, 0);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_DOUBLEBUFFER, 1);

            //Get a GL Context
            OpenGlContext = SDL.SDL_GL_CreateContext(Handle);

            if (OpenGlContext == IntPtr.Zero)
                throw new Exception("Unable to create OpenGL Context. " + SDL.SDL_GetError());

            if (SDL.SDL_GL_SetSwapInterval(1) < 0)
                throw new Exception("Unable to set VSync. " + SDL.SDL_GetError());

            var c = new GraphicsContext(GetContextHandle(), SDL.SDL_GL_GetProcAddress, GetContextHandle);

            //Set initial viewports
            GL.Viewport(0, 0, Width, Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, Width, Height, 0, -1, 1);

            //Enables
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.ShadeModel(ShadingModel.Smooth);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            #endregion
        }

        public bool VSync
        {
            get => SDL.SDL_GL_GetSwapInterval() == 1 ? true : false;
            set
            {
                if (SDL.SDL_GL_SetSwapInterval(value ? 1 : 0) < 0)
                    throw new Exception("Unable to set VSync. " + SDL.SDL_GetError());
            }
        }

        public bool Fullscreen
        {
            get
            {
                var flags = (SDL.SDL_WindowFlags)SDL.SDL_GetWindowFlags(Handle);
                return (flags & SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN) != 0;
            }
            set => SDL.SDL_SetWindowFullscreen(Handle, value ? (uint)SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN : 0);
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

                if (value.X == -1) value.X = SDL.SDL_WINDOWPOS_UNDEFINED;
                if (value.Y == -1) value.Y = SDL.SDL_WINDOWPOS_UNDEFINED;

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

        public void Exit()
        {
            SDL.SDL_DestroyWindow(Handle);
            Handle = IntPtr.Zero;
        }
    }
}

#endif