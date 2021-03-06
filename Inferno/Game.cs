﻿using System;
using System.Collections.Generic;
using Inferno.Audio;
using Inferno.Graphics;
using Inferno.Input;

#if WINDOWS_UWP

using Microsoft.Graphics.Canvas.UI.Xaml;
   
#endif

namespace Inferno
{
    /// <summary>
    /// The entire Game.
    /// This contains everything in the game world.
    /// </summary>
    public class Game : IDisposable
    {
        #region Public Fields

        /// <summary>
        /// The game audio device
        /// </summary>
        public AudioDevice AudioDevice;

        /// <summary>
        /// The back color to be displayed if things are out of bounds
        /// </summary>
        public Color BackColor = Color.Black;
        
        /// <summary>
        /// Whether or not the game should auto pause when window focus is lost
        /// </summary>
        public bool FocusPause = true;
        
        /// <summary>
        /// The number of frames displayed per second.
        /// </summary>
        public int FramesPerSecond;
        
        /// <summary>
        /// The Graphics Device
        /// </summary>
        public GraphicsDevice GraphicsDevice;
        
        /// <summary>
        /// Whether or not the game is running
        /// </summary>
        public bool Paused;
        
        /// <summary>
        /// The Game renderer
        /// </summary>
        public static Renderer Renderer;
        
        /// <summary>
        /// The target height
        /// </summary>
        public int VirtualHeight;
        
        /// <summary>
        /// The target width
        /// </summary>
        public int VirtualWidth;
        
        /// <summary>
        /// The Game window
        /// </summary>
        public readonly GameWindow Window;

        #endregion
        
        #region Private Fields
        
        /// <summary>
        /// The base render target used for scaling
        /// </summary>
        private RenderTarget _baseRenderTarget;
        
        /// <summary>
        /// The game running state
        /// </summary>
        private bool _running = true;
        
        /// <summary>
        /// The current game state
        /// </summary>
        private GameState _currentState;
        
        #endregion
        
        #region Internal Fields
        
        /// <summary>
        /// A private static reference to myself
        /// </summary>
        internal static Game Instance;
        
        /// <summary>
        /// The list of currently pressed keys
        /// </summary>
        internal readonly List<Key> Keys;

        /// <summary>
        /// The PlatformGame that manages platform specific actions
        /// </summary>
        internal readonly PlatformGame PlatformGame;
        
        #endregion
        
        #region Constructors
        
        /// <inheritdoc />
        /// <summary>
        /// Create a new game with the default size of 1280x768
        /// </summary>
        public Game() : this(1280, 768) { }

        /// <summary>
        /// Create a new game
        /// </summary>
        /// <param name="intendedWidth">The intended width of the game</param>
        /// <param name="intendedHeight">The intended height of the game</param>
        /// <param name="title">The window title</param>
        /// <param name="fps">The number of updates to be called every second</param>
        /// <param name="fullscreen">Whether or not the game will start in fullscreen</param>
        /// <param name="vsync">Whether or not VSync is enabled</param>
        public Game(int intendedWidth, int intendedHeight, string title = "Created with Inferno", int fps = 30, bool fullscreen = false, bool vsync = true)
        {
            //Set my "Me" reference
            Instance = this;

            //Scaling
            VirtualWidth = intendedWidth;
            VirtualHeight = intendedHeight;
           
            //Create Keys Array
            Keys = new List<Key>();

            //Configure states
            _currentState = null;

            //Create Graphics Device
            GraphicsDevice = new GraphicsDevice();

            //Create Audio Device
            AudioDevice = new AudioDevice();

            //Register events for focus pause
            OnDeactivated += (sender, e) =>
            {
                if (!FocusPause)
                    return;
                Paused = true;
                AudioDevice.PauseAll();
            };

            OnActivated += (sender, e) =>
            {
                if (!FocusPause)
                    return;
                Paused = false;
                AudioDevice.ResumeAll();
            };

            //Platform game
            PlatformGame = new PlatformGame(this);

            //Create GameWindow
            Window = new GameWindow(this, GraphicsDevice, title, intendedWidth, intendedHeight);
            
            //Fps and fullscreen
            FramesPerSecond = fps;
            Window.Fullscreen = fullscreen;

            //Attach window to device
            GraphicsDevice.AttachWindow(Window);

            //Create Renderer
            Renderer = new Renderer(GraphicsDevice);

            //Create render target
            _baseRenderTarget = new RenderTarget(VirtualWidth, VirtualHeight);
        }
        
        #endregion
        
        #region Public Methods
        
        #region Game
        
        /// <summary>
        /// Exit the game.
        /// </summary>
        public void Exit()
        {
            //Kill the main game loop, allowing shutdown to begin
            _running = false;
        }
        
        /// <summary>
        /// Resize the game
        /// </summary>
        /// <param name="width">New width</param>
        /// <param name="height">New height</param>
        public void Resize(int width, int height)
        {
            VirtualWidth = width;
            VirtualHeight = height;
            Window.Width = width;
            Window.Height = height;

            _baseRenderTarget.Dispose();
            _baseRenderTarget = new RenderTarget(width, height);
        }
        
        #endregion
        
        #region Runtime
        
        /// <summary>
        /// Run the draw calls for one frame
        /// </summary>
        private void Draw()
        {
            //Don't run if paused
            if (Paused)
                return;

            //Grab dimensions
            var viewWidth = Window.Width;
            var viewHeight = Window.Height;

            //Calculate ratios
            var outputAspect = Window.Width / (float)Window.Height;
            var preferredAspect = VirtualWidth / (float)VirtualHeight;

            //Init bar dimensions
            var barwidth = 0;
            var barheight = 0;

            if (outputAspect <= preferredAspect)
            {
                viewHeight = (int)(Window.Width / preferredAspect + 0.5f);
                barheight = (Window.Height - viewHeight) / 2;
            }
            else
            {
                viewWidth = (int)(Window.Height * preferredAspect + 0.5f);
                barwidth = (Window.Width - viewWidth) / 2;
            }

            //Clear the outside of the logical game window (For black bars etc.)
            GraphicsDevice.Clear(Color.Black);

            //Set render target
            GraphicsDevice.SetRenderTarget(_baseRenderTarget);

            //Clear target
            GraphicsDevice.Clear(Color.Black);

            //Draw state
            _currentState?.Draw(Renderer);

            //Reset target ready for scaling
            GraphicsDevice.SetRenderTarget(null);

            //Draw a quad to get the draw buffer to the back buffer
            Renderer.Begin();
            Renderer.Draw(_baseRenderTarget, new Rectangle(barwidth, barheight, viewWidth, viewHeight), Color.White);
            Renderer.End();
        }
        
        /// <summary>
        /// Launch the game
        /// </summary>
        public void Run()
        {
            LoadContent();
            Initialize();

            var previous = Environment.TickCount;
            var lag = 0f;
            while (_running)
            {
                var current = Environment.TickCount;
                var delta = current - previous;
                previous = current;
                lag += delta;
                
                while (lag >= 1000f / FramesPerSecond)
                {
                    //Logic
                    Update();
                    lag -= 1000f / FramesPerSecond;
                }

                //Begin Draw
                GraphicsDevice.BeginDraw();

                //Draw Game
                Draw();

                //End Draw
                GraphicsDevice.EndDraw();

                //Window events
                if (!PlatformGame.RunEvents())
                    _running = false;
            }
            
            UnloadContent();

            OnExit?.Invoke(this, EventArgs.Empty); //Extra for those who may not unload content here

            Window.Exit();

            Dispose();
        }
        
        /// <summary>
        /// Run one logical frame
        /// </summary>
        private void Update()
        {
            //Don't run if paused
            if (Paused)
                return;

            //Run updates
            _currentState?.BeginUpdate();
            _currentState?.Update();
            _currentState?.EndUpdate();

            //Update audio device
            AudioDevice.Update();
        }
        
        #endregion
        
        #region State Management

        /// <summary>
        /// Set the game state using an ID
        /// </summary>
        /// <param name="state">The state to set</param>
        public void SetState(GameState state)
        {
            //Unload the current state
            _currentState?.OnUnLoad?.Invoke(this, EventArgs.Empty);

            //Set state
            _currentState = state;

            //Load the new state
            _currentState?.OnLoad?.Invoke(this, EventArgs.Empty);
        }
        

        #endregion
        
        #region Other
        
        public void Dispose()
        {
            //Dispose render target
            _baseRenderTarget?.Dispose();
            _baseRenderTarget = null;

            //Dispose Renderer
            Renderer.Dispose();

            //Dispose graphics device
            GraphicsDevice.Dispose();

            //Dispose audio device
            AudioDevice.Dispose();
        }
        
        #endregion
        
        #region Virtual
        
        /// <summary>
        /// Initialise the game
        /// </summary>
        protected virtual void Initialize()
        {
        }

        /// <summary>
        /// Load all game content
        /// </summary>
        protected virtual void LoadContent()
        {
        }
        
        protected virtual void UnloadContent()
        {
            //Unload the current state if there's one already open
            _currentState?.OnUnLoad?.Invoke(this, EventArgs.Empty);

            //Unset state
            _currentState = null;
        }
        
        #endregion
        
        #endregion

        #region Events

        public EventHandler OnActivated;
        public EventHandler OnDeactivated;
        public EventHandler OnExit;
        public EventHandler<OnResizeEventArgs> OnResize;
        
        #region EventArgs
        
        public class OnResizeEventArgs : EventArgs
        {
            public Rectangle NewBounds;

            public OnResizeEventArgs(Rectangle newBounds)
            {
                NewBounds = newBounds;
            }
        }
        
        #endregion

        #endregion
    }
}
