using System;
using System.Collections.Generic;
using Inferno.Runtime.Core;
using Inferno.Runtime.Graphics;

namespace Inferno.Runtime
{
    /// <summary>
    /// The entire Game.
    /// This contains everything in the game world.
    /// </summary>
    public class Game : IDisposable
    {
        #region Fields

        public GameWindow Window;

        //TODO: Decide whether or not to make this static
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
        /// A private static reference to myself
        /// </summary>
        internal static Game Instance;

        #endregion

        #region Properties

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

            //TODO: Config Graphics config

            //Configure states
            CurrentStateId = -1;
            States = new List<State>();

            //Create GameWindow
            Window = new GameWindow(title, intendedWidth, intendedHeight);
        }

        #region Runtime

        private bool _running = false;

        public void Run()
        {
            Initialize();
            _running = true;
            while (_running)
            {
            }
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

        protected virtual void Initialize()
        {
            //Create Renderer
            Renderer = new Renderer();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            //Dispose Renderer
            Renderer.Dispose();

            //Dispose drawer
            Drawing.Dispose();

            //TODO: Work out if we will ever need these
            //Dispose render target
            //BaseRenderTarget.Dispose();
            //BaseRenderTarget = null;

            //Dispose SpriteBatch
            //SpriteBatch.Dispose();

            //Dispose graphics device manager
            //_graphicsDeviceManager.Dispose();
        }

        protected virtual void LoadContent()
        {
            //Init drawer
            Drawing.Config();
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

        //TODO: Caller
        protected virtual void OnActivated(object sender, EventArgs args)
        {
            //Unpause if window becomes active
            if (FocusPause)
                Paused = false;
        }

        //TODO: Caller
        protected virtual void OnDeactivated(object sender, EventArgs args)
        {
            //Pause if window becomes inactive
            if (FocusPause)
                Paused = true;
        }

        #endregion

        #region Runtime

        protected void Draw(float delta)
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

            //TODO: Render targets

            //Set render target
            //GraphicsDevice.SetRenderTarget(BaseRenderTarget);

            //Clear target
            //GraphicsDevice.Clear(BackColor.Monogame);

            //Draw state
            if (CurrentStateId != -1)
                States[CurrentStateId]?.Draw(Renderer);

            //Reset target ready for scaling
            //GraphicsDevice.SetRenderTarget(null);

            //Draw a quad to get the draw buffer to the back buffer
            Renderer.Begin();//(SpriteSortMode.Immediate, BlendState.Opaque);
            //Renderer.Draw(BaseRenderTarget, new Rectangle(barwidth, barheight, viewWidth, viewHeight), Graphics.Color.White);
            Renderer.End();
        }

        protected void Update(float delta)
        {
            //Don't run if paused
            if (Paused)
                return;

            //Run updates
            if (CurrentStateId == -1)
                return;
            States[CurrentStateId]?.BeginUpdate();
            States[CurrentStateId]?.Update(delta);
            States[CurrentStateId]?.EndUpdate();
        }

        #endregion
    }
}
