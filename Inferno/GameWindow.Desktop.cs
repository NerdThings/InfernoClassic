﻿#if DESKTOP

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Reflection;
using Inferno.Graphics;
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
            Handle = SDL.SDL_CreateWindow(title, SDL2.SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, width, height, SDL2.SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL);

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

            var context = GetContextHandle();
            var c = new GraphicsContext(GetContextHandle(), SDL.SDL_GL_GetProcAddress, GetContextHandle);

            //Init Program
            ProgramId = GL.CreateProgram();

            #region Shaders

            #region Vertex

            //Create vertex shader
            var vertexShader = GL.CreateShader(ShaderType.VertexShader);
            string vertexShaderSource = "";

            //Get fragment source
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Inferno.OpenGL.vert"))
            {
                using (var reader = new StreamReader(stream))
                {
                    vertexShaderSource = reader.ReadToEnd();
                }
            }

            //Set vertex source
            GL.ShaderSource(vertexShader, vertexShaderSource);

            //Compile shader source
            GL.CompileShader(vertexShader);

            //Attach to program
            GL.AttachShader(ProgramId, vertexShader);

            #endregion

            #region Fragment

            //Create fragment shader
            var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            string fragmentShaderSource = "";

            //Get fragment source
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Inferno.OpenGL.frag"))
            {
                using (var reader = new StreamReader(stream))
                {
                    fragmentShaderSource = reader.ReadToEnd();
                }
            }

            //Set fragment source
            GL.ShaderSource(fragmentShader, fragmentShaderSource);

            //Compile fragment source
            GL.CompileShader(fragmentShader);

            //Attach to program
            GL.AttachShader(ProgramId, fragmentShader);

            #endregion
            
            #endregion

            //Link Program
            GL.LinkProgram(ProgramId);

            //Get vertex attribute location
            var vetrexAttribLocation = GL.GetAttribLocation(ProgramId, "LVertexPos2D");

            //Check it's existance
            if (vetrexAttribLocation == -1)
            {
                //throw new Exception("LVertexPos2D is not a valid glsl program variable!");
            }

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

        
        public bool AllowResize
        {
            get
            {
                var flags = (SDL2.SDL.SDL_WindowFlags)SDL2.SDL.SDL_GetWindowFlags(Handle);
                return (flags & SDL2.SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE) != 0;
            }
            set => SDL.SDL_SetWindowResizable(Handle, value ? SDL2.SDL.SDL_bool.SDL_TRUE : SDL2.SDL.SDL_bool.SDL_FALSE);
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
            set => SDL.SDL_SetHint(SDL2.SDL.SDL_HINT_WINDOWS_NO_CLOSE_ON_ALT_F4, value ? "0" : "1");
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