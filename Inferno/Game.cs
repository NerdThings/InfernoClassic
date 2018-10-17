using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        #region Fields

        public GameWindow Window;

        public static Renderer Renderer;

        /// <summary>
        /// A list of all the game States
        /// </summary>
        public List<GameState> States;

        /// <summary>
        /// The current state id
        /// </summary>
        public int CurrentStateId;

        /// <summary>
        /// The current visible state
        /// </summary>
        public GameState CurrentState => States[CurrentStateId];

        /// <summary>
        /// The target width
        /// </summary>
        public int VirtualWidth;

        /// <summary>
        /// The target height
        /// </summary>
        public int VirtualHeight;

        /// <summary>
        /// Whether or not the game is running
        /// </summary>
        public bool Paused;

        /// <summary>
        /// Whether or not the game should auto pause when window focus is lost
        /// </summary>
        public bool FocusPause = true;

        /// <summary>
        /// The back color to be displayed if things are out of bounds
        /// </summary>
        public Color BackColor = Color.Black;

        /// <summary>
        /// The number of frames displayed per second.
        /// </summary>
        public int FramesPerSecond = 30;

        /// <summary>
        /// The Graphics Device
        /// </summary>
        public GraphicsDevice GraphicsDevice;

        /// <summary>
        /// A private static reference to myself
        /// </summary>
        internal static Game Instance;

        internal PlatformGame PlatformGame;

        internal List<Key> Keys;

        private RenderTarget _baseRenderTarget;

        #endregion

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
            CurrentStateId = -1;
            States = new List<GameState>();

            //Create Graphics Manager
            GraphicsDevice = new GraphicsDevice();

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

        #region Runtime

        private bool _running = true;

#if WINDOWS_UWP
        public void Run(CanvasControl canvasControl)
#else
        public void Run()
#endif
        {

#if WINDOWS_UWP

            //Very untidy method
            Window.PlatformWindow.SetCanvas(canvasControl);

#endif

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
                _running = PlatformGame.RunEvents();
            }
            
            UnloadContent();

            OnExit?.Invoke(this, EventArgs.Empty); //Extra for those who may not unload content here

            Window.Exit();

            Dispose();
        }

#endregion

        #region Window Management Stuffs

        /// <summary>
        /// Set the game into fullscreen mode
        /// </summary>
        [Obsolete("This is unnecessary, use Window.Fullscreen = value instead.")]
        public void Fullscreen()
        {
            Window.Fullscreen = true;
        }

        /// <summary>
        /// Set the game into windowed mode
        /// </summary>
        [Obsolete("This is unnecessary, use Window.Fullscreen = value instead.")]
        public void Windowed()
        {
            Window.Fullscreen = false;
        }

        /// <summary>
        /// Enable vertical retrace syncing
        /// </summary>
        [Obsolete("This is unnecessary, use Window.VSync = value instead.")]
        public void EnableVSync()
        {
            Window.VSync = true;
        }

        /// <summary>
        /// Disable vertical retrace syncing
        /// </summary>
        [Obsolete("This is unnecessary, use Window.VSync = value instead.")]
        public void DisableVSync()
        {
            Window.VSync = false;
        }

        /// <summary>
        /// Resize the window and the logical game size
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

        #region State Management

        /// <summary>
        /// Add a new state into the game
        /// </summary>
        /// <param name="state">The State to add</param>
        /// <returns>The State ID</returns>
        protected int AddState(GameState state)
        {
            States.Add(state);
            return States.IndexOf(state);
        }

        /// <summary>
        /// Set the game state using an ID
        /// </summary>
        /// <param name="state">The state ID to set</param>
        public void SetState(int state)
        {
            //Unload the current state if there's one already open
            if (CurrentStateId != -1)
                States[CurrentStateId].OnUnLoad?.Invoke(this, EventArgs.Empty);

            //Update state ID
            CurrentStateId = state;

            //Load the new state
            if (CurrentStateId != -1)
                States[CurrentStateId].OnLoad?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Set the game state using a State instance
        /// </summary>
        /// <param name="state">State to jump to</param>
        public void SetState(GameState state)
        {
            //Set the state with the discovered ID
            SetState(States.IndexOf(state));
        }
        
        /// <summary>
        /// Set the game state using a State type
        /// </summary>
        /// <param name="stateType">State Type to jump to</param>
        public void SetState(Type stateType)
        {
            //Find state by type
            foreach (var st in States)
            {
                if (st.GetType() != stateType) continue;

                //Set the state
                SetState(States.IndexOf(st));

                //End the looping
                return;
            }
        }

        #endregion

        #region Game Management
        
        /// <summary>
        /// Initialise the game
        /// </summary>
        protected virtual void Initialize()
        {
        }

        public void Dispose()
        {
            //Dispose render target
            _baseRenderTarget?.Dispose();
            _baseRenderTarget = null;

            //Dispose Renderer
            Renderer.Dispose();

            //Dispose graphics manager
            GraphicsDevice.Dispose();
        }

        /// <summary>
        /// Load all game content
        /// </summary>
        protected virtual void LoadContent()
        {
        }

        /// <summary>
        /// Exit the game
        /// </summary>
        public void Exit()
        {
            //Kill the main game loop, allowing shutdown to begin
            _running = false;
        }

        protected virtual void UnloadContent()
        {
            //Unload the current state if there's one already open
            if (CurrentStateId != -1)
                States[CurrentStateId].OnUnLoad?.Invoke(this, EventArgs.Empty);

            //Disable state
            CurrentStateId = -1;
        }

        #endregion

        #region Events

        public EventHandler OnActivated;
        public EventHandler OnDeactivated;
        public EventHandler<OnResizeEventArgs> OnResize;

        public class OnResizeEventArgs : EventArgs
        {
            public Rectangle NewBounds;

            public OnResizeEventArgs(Rectangle newBounds)
            {
                NewBounds = newBounds;
            }
        }

        public EventHandler OnExit;

        #endregion

        #region Runtime

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
            if (CurrentStateId != -1)
                States[CurrentStateId]?.Draw(Renderer);

            //Reset target ready for scaling
            GraphicsDevice.SetRenderTarget(null);

            //Draw a quad to get the draw buffer to the back buffer
            Renderer.Begin();
            Renderer.Draw(_baseRenderTarget, new Rectangle(barwidth, barheight, viewWidth, viewHeight), Color.White);
            Renderer.End();
        }

        private void Update()
        {
            //Don't run if paused
            if (Paused)
                return;

            //Run updates
            if (CurrentStateId == -1)
                return;

            States[CurrentStateId]?.BeginUpdate();
            States[CurrentStateId]?.Update();
            States[CurrentStateId]?.EndUpdate();
        }

        #endregion
    }
}
