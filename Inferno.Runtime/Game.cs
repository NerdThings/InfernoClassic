using System;
using System.Collections.Generic;
using System.Text;
using Inferno.Runtime.Core;
using Inferno.Runtime.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace Inferno.Runtime
{
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
        public static GraphicsDevice Graphics
        {
            get
            {
                return Me.GraphicsDevice;
            }
        }

        /// <summary>
        /// A list of all the game States
        /// </summary>
        public List<State> States;

        /// <summary>
        /// The current state id
        /// </summary>
        public int CurrentState;

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
        /// The back color to be displayed if things are out of bounds
        /// </summary>
        public Color BackColor = Color.Black;

        /// <summary>
        /// A private static reference to myself
        /// </summary>
        private static Game Me;

        /// <summary>
        /// The graphics device manager
        /// </summary>
        private GraphicsDeviceManager _GraphicsDeviceManager;

        #endregion

        #region Properties

        /// <summary>
        /// The current window width
        /// </summary>
        public int WindowWidth
        {
            get
            {
                return Window.ClientBounds.Width;
            }
        }

        /// <summary>
        /// The current window height
        /// </summary>
        public int WindowHeight
        {
            get
            {
                return Window.ClientBounds.Height;
            }
        }

        /// <summary>
        /// The window title
        /// </summary>
        public string WindowTitle
        {
            get
            {
                return Window.Title;
            }

            set
            {
                Window.Title = value;
            }
        }

        #endregion

        /// <summary>
        /// Create a new game with the default size of 1280x768
        /// </summary>
        public Game() : this(1280, 768) { }

        /// <summary>
        /// Create a new game
        /// </summary>
        /// <param name="IntendedWidth"></param>
        /// <param name="IntendedHeight"></param>
        /// <param name="vsync"></param>
        public Game(int IntendedWidth, int IntendedHeight, bool fullscreen = false, bool vsync = true) : base()
        {
            //Create graphics manager
            _GraphicsDeviceManager = new GraphicsDeviceManager(this);

            //Make mouse visible
            IsMouseVisible = true;

            //Configure content
            Content.RootDirectory = "Content";
            ContentManager = Content;

            //Set my "Me" reference
            Me = this;

            //Scaling
            this.VirtualWidth = IntendedWidth;
            this.VirtualHeight = IntendedHeight;

            //Config TouchPanel (Bugged)
            TouchPanel.DisplayHeight = VirtualHeight;
            TouchPanel.DisplayWidth = VirtualWidth;
            TouchPanel.EnableMouseTouchPoint = true;

            //Config Graphics Device
            _GraphicsDeviceManager.SynchronizeWithVerticalRetrace = vsync;
            _GraphicsDeviceManager.PreferredBackBufferWidth = VirtualWidth;
            _GraphicsDeviceManager.PreferredBackBufferHeight = VirtualHeight;
            _GraphicsDeviceManager.IsFullScreen = fullscreen;
            _GraphicsDeviceManager.ApplyChanges();

            //Configure states
            CurrentState = -1;
            States = new List<State>();
        }

        #region Graphics Manager Stuffs

        /// <summary>
        /// Set the game into fullscreen mode
        /// </summary>
        public void Fullscreen()
        {
            _GraphicsDeviceManager.IsFullScreen = true;
            _GraphicsDeviceManager.ApplyChanges();
        }

        /// <summary>
        /// Set the game into windowed mode
        /// </summary>
        public void Windowed()
        {
            _GraphicsDeviceManager.IsFullScreen = false;
            _GraphicsDeviceManager.ApplyChanges();
        }

        /// <summary>
        /// Enable vertical retrace syncing
        /// </summary>
        public void EnableVSync()
        {
            _GraphicsDeviceManager.SynchronizeWithVerticalRetrace = true;
            _GraphicsDeviceManager.ApplyChanges();
        }

        /// <summary>
        /// Disable vertical retrace syncing
        /// </summary>
        public void DisableVSync()
        {
            _GraphicsDeviceManager.SynchronizeWithVerticalRetrace = false;
            _GraphicsDeviceManager.ApplyChanges();
        }

        #endregion

        #region State Management

        /// <summary>
        /// Add a new state into the game
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected int AddState(State state)
        {
            States.Add(state);
            return States.IndexOf(state);
        }

        /// <summary>
        /// Set the game state using an ID
        /// </summary>
        /// <param name="state"></param>
        public void SetState(int state)
        {
            CurrentState = state;

            if (CurrentState != -1)
            {
                States[CurrentState].InvokeOnStateLoad(this);
            }
        }

        /// <summary>
        /// Set the game state using a State instance
        /// </summary>
        /// <param name="state"></param>
        public void SetState(State state)
        {
            SetState(States.IndexOf(state));
        }
        
        /// <summary>
        /// Set the game state using a State type
        /// </summary>
        /// <param name="stateType"></param>
        public void SetState(Type stateType)
        {
            foreach (State st in States)
            {
                if (st.GetType() == stateType)
                {
                    SetState(States.IndexOf(st));
                    return;
                }
            }
        }

        #endregion

        #region Game Management

        protected override void Initialize()
        {
            //Create SpriteBatch
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            //Grab presentation parameters
            PresentationParameters pp = Graphics.PresentationParameters;

            //Set up our render target for scaling
            BaseRenderTarget = new RenderTarget2D(GraphicsDevice, VirtualWidth, VirtualHeight, false, SurfaceFormat.Color, DepthFormat.None, pp.MultiSampleCount, RenderTargetUsage.DiscardContents);

            //Config Drawjng
            Drawing.Config();

            base.Initialize();
        }

        protected override void Dispose(bool disposing)
        {
            //Destroy render target
            if (disposing)
            {
                BaseRenderTarget.Dispose();
                BaseRenderTarget = null;
            }

            base.Dispose(disposing);
        }

        protected override void LoadContent()
        {
            if (CurrentState != -1)
            {
                States[CurrentState].InvokeOnStateLoad(this);
            }
            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            //TODO: Properly unload
            base.UnloadContent();
        }

        #endregion

        #region Auto pause

        protected override void OnActivated(object sender, EventArgs args)
        {
            //Unpause if window becomes active
            Paused = false;
            base.OnActivated(sender, args);
        }


        protected override void OnDeactivated(object sender, EventArgs args)
        {
            //Pause if window becomes inactive
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
            int viewWidth = WindowWidth;
            int viewHeight = WindowHeight;

            //Calculate ratios
            float outputAspect = WindowWidth / (float)WindowHeight;
            float preferredAspect = VirtualWidth / (float)VirtualHeight;

            //Init bar dimensions
            int barwidth = 0;
            int barheight = 0;

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

            //Draw game
            GraphicsDevice.SetRenderTarget(BaseRenderTarget);
            GraphicsDevice.Clear(BackColor);

            if (CurrentState != -1)
                States[CurrentState]?.Draw(SpriteBatch);

            GraphicsDevice.SetRenderTarget(null);

            //Draw a quad to get the draw buffer to the back buffer
            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            SpriteBatch.Draw(BaseRenderTarget, new Rectangle(barwidth, barheight, viewWidth, viewHeight), Color.White);
            SpriteBatch.End();

            base.Draw(gameTime);
        }

        protected override void Update(GameTime gameTime)
        {
            //Don't run if paused
            if (Paused)
                return;

            if (CurrentState != -1)
            {
                States[CurrentState]?.BeginUpdate();
                States[CurrentState]?.Update(gameTime);
                States[CurrentState]?.EndUpdate();
            }

            base.Update(gameTime);
        }

        #endregion
    }
}
