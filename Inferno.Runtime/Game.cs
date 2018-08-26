using System;
using System.Collections.Generic;
using Inferno.Runtime.Core;
using Inferno.Runtime.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace Inferno.Runtime
{
    /// <summary>
    /// The entire Game.
    /// This contains everything in the game world.
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        #region Fields

        /// <summary>
        /// The game's sprite batcher
        /// </summary>
        public static SpriteBatch SpriteBatch;

        /// <summary>
        /// The game's content manager
        /// </summary>
        public static ContentManager ContentManager;

        /// <summary>
        /// The scaling render target
        /// </summary>
        public static RenderTarget2D BaseRenderTarget;

        /// <summary>
        /// The game graphics device
        /// </summary>
        public static GraphicsDevice GraphicsDeviceInstance => _me.GraphicsDevice;

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
        public Graphics.Color BackColor = Graphics.Color.Black;

        /// <summary>
        /// A private static reference to myself
        /// </summary>
        private static Game _me;

        /// <summary>
        /// The graphics device manager
        /// </summary>
        private readonly GraphicsDeviceManager _graphicsDeviceManager;

        #endregion

        #region Properties

        /// <summary>
        /// The current window width
        /// </summary>
        public int WindowWidth => Window.ClientBounds.Width;

        /// <summary>
        /// The current window height
        /// </summary>
        public int WindowHeight => Window.ClientBounds.Height;

        /// <summary>
        /// The window title
        /// </summary>
        public string WindowTitle
        {
            get => Window.Title;

            set => Window.Title = value;
        }

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
        /// <param name="fullscreen">Whether or not the game will start in fullscreen</param>
        /// <param name="vsync">Whether or not VSync is enabled</param>
        public Game(int intendedWidth, int intendedHeight, bool fullscreen = false, bool vsync = true)
        {
            //Create graphics manager
            _graphicsDeviceManager = new GraphicsDeviceManager(this);

            //Make mouse visible
            IsMouseVisible = true;

            //Configure content
            Content.RootDirectory = "Content";
            ContentManager = Content;

            //Set my "Me" reference
            _me = this;

            //Scaling
            VirtualWidth = intendedWidth;
            VirtualHeight = intendedHeight;

            //Config TouchPanel (Bugged)
            TouchPanel.DisplayHeight = VirtualHeight;
            TouchPanel.DisplayWidth = VirtualWidth;
            TouchPanel.EnableMouseTouchPoint = true;

            //Config GraphicsDeviceInstance Device
            _graphicsDeviceManager.SynchronizeWithVerticalRetrace = vsync;
            _graphicsDeviceManager.PreferredBackBufferWidth = VirtualWidth;
            _graphicsDeviceManager.PreferredBackBufferHeight = VirtualHeight;
            _graphicsDeviceManager.IsFullScreen = fullscreen;
            _graphicsDeviceManager.ApplyChanges();

            //Configure states
            CurrentStateId = -1;
            States = new List<State>();
        }

        #region GraphicsDeviceInstance Manager Stuffs

        /// <summary>
        /// Set the game into fullscreen mode
        /// </summary>
        public void Fullscreen()
        {
            _graphicsDeviceManager.IsFullScreen = true;
            _graphicsDeviceManager.ApplyChanges();
        }

        /// <summary>
        /// Set the game into windowed mode
        /// </summary>
        public void Windowed()
        {
            _graphicsDeviceManager.IsFullScreen = false;
            _graphicsDeviceManager.ApplyChanges();
        }

        /// <summary>
        /// Enable vertical retrace syncing
        /// </summary>
        public void EnableVSync()
        {
            _graphicsDeviceManager.SynchronizeWithVerticalRetrace = true;
            _graphicsDeviceManager.ApplyChanges();
        }

        /// <summary>
        /// Disable vertical retrace syncing
        /// </summary>
        public void DisableVSync()
        {
            _graphicsDeviceManager.SynchronizeWithVerticalRetrace = false;
            _graphicsDeviceManager.ApplyChanges();
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

        protected override void Initialize()
        {
            //Create SpriteBatch
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            //Grab presentation parameters
            var pp = GraphicsDeviceInstance.PresentationParameters;

            //Set up our render target for scaling
            BaseRenderTarget = new RenderTarget2D(GraphicsDevice, VirtualWidth, VirtualHeight, false, SurfaceFormat.Color, DepthFormat.None, pp.MultiSampleCount, RenderTargetUsage.DiscardContents);

            base.Initialize();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //Dispose render target
                BaseRenderTarget.Dispose();
                BaseRenderTarget = null;

                //Dispose drawer
                Drawing.Dispose();

                //Dispose SpriteBatch
                SpriteBatch.Dispose();

                //Dispose graphics device manager
                _graphicsDeviceManager.Dispose();
            }

            base.Dispose(disposing);
        }

        protected override void LoadContent()
        {
            //Init drawer
            Drawing.Config();

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            //Unload the current state if there's one already open
            if (CurrentStateId != -1)
                States[CurrentStateId].InvokeOnStateUnLoad(this);

            //Disable state
            CurrentStateId = -1;

            base.UnloadContent();
        }

        #endregion

        #region Auto pause

        protected override void OnActivated(object sender, EventArgs args)
        {
            //Unpause if window becomes active
            if (FocusPause)
                Paused = false;

            base.OnActivated(sender, args);
        }


        protected override void OnDeactivated(object sender, EventArgs args)
        {
            //Pause if window becomes inactive
            if (FocusPause)
                Paused = true;

            base.OnDeactivated(sender, args);
        }

        #endregion

        #region Runtime

        protected override void Draw(GameTime gameTime)
        {
            //Don't run if paused
            if (Paused)
                return;

            //Grab dimensions
            var viewWidth = WindowWidth;
            var viewHeight = WindowHeight;

            //Calculate ratios
            var outputAspect = WindowWidth / (float)WindowHeight;
            var preferredAspect = VirtualWidth / (float)VirtualHeight;

            //Init bar dimensions
            var barwidth = 0;
            var barheight = 0;

            //Calculate view sizes and bar sizes
            if (outputAspect <= preferredAspect)
            {
                viewHeight = (int)((WindowWidth / preferredAspect) + 0.5f);
                barheight = (WindowHeight - viewHeight) / 2;
            }
            else
            {
                viewWidth = (int)((WindowHeight * preferredAspect) + 0.5f);
                barwidth = (WindowWidth - viewWidth) / 2;
            }

            //Set render target
            GraphicsDevice.SetRenderTarget(BaseRenderTarget);

            //Clear target
            GraphicsDevice.Clear(BackColor.Monogame);

            //Draw state
            if (CurrentStateId != -1)
                States[CurrentStateId]?.Draw(SpriteBatch);

            //Reset target ready for scaling
            GraphicsDevice.SetRenderTarget(null);

            //Draw a quad to get the draw buffer to the back buffer
            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            SpriteBatch.Draw(BaseRenderTarget, new Rectangle(barwidth, barheight, viewWidth, viewHeight).Monogame, Graphics.Color.White.Monogame);
            SpriteBatch.End();

            base.Draw(gameTime);
        }

        protected override void Update(GameTime gameTime)
        {
            //Don't run if paused
            if (Paused)
                return;

            //Run updates
            if (CurrentStateId != -1)
            {
                States[CurrentStateId]?.BeginUpdate();
                States[CurrentStateId]?.Update(gameTime);
                States[CurrentStateId]?.EndUpdate();
            }

            base.Update(gameTime);
        }

        #endregion
    }
}
