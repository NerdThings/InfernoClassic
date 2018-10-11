using System;
using System.Collections.Generic;
using System.Diagnostics;
using Inferno.Core;
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
        public List<State> States;

        /// <summary>
        /// The current state id
        /// </summary>
        public int CurrentStateId;

        /// <summary>
        /// The current visible state
        /// </summary>
        public State CurrentState => States[CurrentStateId];

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
        /// The Graphics Manager
        /// </summary>
        public GraphicsManager GraphicsManager;

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
        /// <param name="fullscreen">Whether or not the game will start in fullscreen</param>
        /// <param name="vsync">Whether or not VSync is enabled</param>
        public Game(int intendedWidth, int intendedHeight, string title = "Created with Inferno", bool fullscreen = false, bool vsync = true)
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
            States = new List<State>();

            //Create Graphics Manager
            GraphicsManager = new GraphicsManager();

            //Platform game
            PlatformGame = new PlatformGame();

            //Create GameWindow
            Window = new GameWindow(title, intendedWidth, intendedHeight);

            //Link Graphics Manager and Window
            GraphicsManager.Setup(Window);

            //Create Renderer
            Renderer = new Renderer(GraphicsManager);

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
            
            var fps = new Stopwatch();

            while (_running)
            {
                //Start timer
                fps.Start();

                //Logic
                Update();

                //Window events
                _running = PlatformGame.RunEvents();

                //Hang, not time to update again yet
                while (fps.ElapsedTicks < 1000 / FramesPerSecond) {}
                fps.Stop();
            }
            
            UnloadContent();

            Window.Exit();

            Dispose();
        }

#endregion

        #region Window Management Stuffs

        /// <summary>
        /// Set the game into fullscreen mode
        /// </summary>
        public void Fullscreen()
        {
            //TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set the game into windowed mode
        /// </summary>
        public void Windowed()
        {
            //TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Enable vertical retrace syncing
        /// </summary>
        public void EnableVSync()
        {
            //TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Disable vertical retrace syncing
        /// </summary>
        public void DisableVSync()
        {
            //TODO
            throw new NotImplementedException();
        }

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
        protected int AddState(State state)
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
                States[CurrentStateId].InvokeOnStateUnLoad(this);

            //Update state ID
            CurrentStateId = state;

            //Load the new state
            if (CurrentStateId != -1)
                States[CurrentStateId].InvokeOnStateLoad(this);
        }

        /// <summary>
        /// Set the game state using a State instance
        /// </summary>
        /// <param name="state">State to jump to</param>
        public void SetState(State state)
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
            //Dispose drawer
            Drawing.Dispose();

            //Dispose render target
            _baseRenderTarget?.Dispose();
            _baseRenderTarget = null;

            //Dispose Renderer
            Renderer.Dispose();

            //Dispose graphics manager
            GraphicsManager.Dispose();
        }

        /// <summary>
        /// Load all game content
        /// </summary>
        protected virtual void LoadContent()
        {
            //Init drawer
            Drawing.Config();
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
                States[CurrentStateId].InvokeOnStateUnLoad(this);

            //Disable state
            CurrentStateId = -1;
        }

        #endregion

        #region Auto pause

        protected virtual void OnActivated()
        {
            //Unpause if window becomes active
            if (FocusPause)
                Paused = false;
        }

        protected virtual void OnDeactivated()
        {
            //Pause if window becomes inactive
            if (FocusPause)
                Paused = true;
        }

        protected virtual void OnResize(Rectangle newBounds)
        {
            _baseRenderTarget.Dispose();
            _baseRenderTarget = new RenderTarget(newBounds.Width, newBounds.Height);
        }

        internal void TriggerOnResize() { OnResize(Window.Bounds); }
        internal void TriggerOnActivated() { OnActivated(); }
        internal void TriggerOnDeativated() { OnDeactivated(); }

        #endregion

        #region Runtime

        internal void InvokeDraw()
        {
            Draw();
        }

        protected void Draw()
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

            //Calculate view sizes and bar sizes
            if (outputAspect <= preferredAspect)
            {
                viewHeight = (int)((Window.Width / preferredAspect) + 0.5f);
                barheight = (Window.Height - viewHeight) / 2;
            }
            else
            {
                viewWidth = (int)((Window.Height * preferredAspect) + 0.5f);
                barwidth = (Window.Width - viewWidth) / 2;
            }

            GraphicsManager.Clear(Color.Black);

            //Set render target
            GraphicsManager.SetRenderTarget(_baseRenderTarget);

            //Clear target
            GraphicsManager.Clear(Color.White);

            //Draw state
            if (CurrentStateId != -1)
                States[CurrentStateId]?.Draw(Renderer);

            //Reset target ready for scaling
            GraphicsManager.SetRenderTarget(null);

            //Draw a quad to get the draw buffer to the back buffer
            Renderer.Begin();//(SpriteSortMode.Immediate, BlendState.Opaque);
            Renderer.Draw(_baseRenderTarget, new Rectangle(barwidth, barheight, viewWidth, viewHeight), Color.White);
            Renderer.End();
        }

        protected void Update()
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
