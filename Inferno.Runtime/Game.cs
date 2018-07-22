﻿using System;
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
        public int CurrentStateID;

        /// <summary>
        /// The current visible state
        /// </summary>
        public State CurrentState
        {
            get
            {
                return States[CurrentStateID];
            }
        }

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
        /// <param name="IntendedWidth">The intended width of the game</param>
        /// <param name="IntendedHeight">The intended height of the game</param>
        /// <param name="fullscreen">Whether or not the game will start in fullscreen</param>
        /// <param name="vsync">Whether or not VSync is enabled</param>
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
            CurrentStateID = -1;
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
            if (CurrentStateID != -1)
                States[CurrentStateID].InvokeOnStateUnLoad(this);

            //Update state ID
            CurrentStateID = state;

            //Load the new state
            if (CurrentStateID != -1)
                States[CurrentStateID].InvokeOnStateLoad(this);
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
            foreach (State st in States)
            {
                if (st.GetType() == stateType)
                {
                    //Set the state
                    SetState(States.IndexOf(st));

                    //End the looping
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
                _GraphicsDeviceManager.Dispose();
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
            if (CurrentStateID != -1)
                States[CurrentStateID].InvokeOnStateUnLoad(this);

            //Disable state
            CurrentStateID = -1;

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

            //Set render target
            GraphicsDevice.SetRenderTarget(BaseRenderTarget);

            //Clear target
            GraphicsDevice.Clear(BackColor);

            //Draw state
            if (CurrentStateID != -1)
                States[CurrentStateID]?.Draw(SpriteBatch);

            //Reset target ready for scaling
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

            //Run updates
            if (CurrentStateID != -1)
            {
                States[CurrentStateID]?.BeginUpdate();
                States[CurrentStateID]?.Update(gameTime);
                States[CurrentStateID]?.EndUpdate();
            }

            base.Update(gameTime);
        }

        #endregion
    }
}
