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
        private static Game Me { get; set; }
        public static SpriteBatch SpriteBatch { get; set; }
        public static ContentManager ContentManager { get; set; }

        public static RenderTarget2D BaseRenderTarget { get; set; }

        public static GraphicsDevice Graphics
        {
            get
            {
                return Me.GraphicsDevice;
            }
        }

        private GraphicsDeviceManager _GraphicsDeviceManager;

        public List<State> States { get; set; }
        public int CurrentState { get; set; }

        public int VirtualWidth { get; set; }
        public int VirtualHeight { get; set; }

        /// <summary>
        /// The back color to be displayed if things are out of bounds
        /// </summary>
        public Color BackColor = Color.Black;

        public int WindowWidth
        {
            get
            {
                return Window.ClientBounds.Width;
            }
        }

        public int WindowHeight
        {
            get
            {
                return Window.ClientBounds.Height;
            }
        }

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

        public Game() : this(1280, 768) { }

        public Game(int IntendedWidth, int IntendedHeight, bool vsync = false) : base()
        {
            _GraphicsDeviceManager = new GraphicsDeviceManager(this);
            IsMouseVisible = true;
            Content.RootDirectory = "Content";
            ContentManager = Content;
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
            _GraphicsDeviceManager.ApplyChanges();

            CurrentState = -1;
            States = new List<State>();
        }

        #region State Management

        protected int AddState(State state)
        {
            States.Add(state);
            return States.IndexOf(state);
        }

        public void SetState(int state)
        {
            CurrentState = state;

            if (CurrentState != -1)
            {
                States[CurrentState].InvokeOnStateLoad(this);
            }
        }

        [System.Obsolete("It is undecided on whether or not this will be removed, so we are marking it obsolete.")]
        public void SetState(State state)
        {
            SetState(States.IndexOf(state));
        }

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

        #region Runtime

        protected override void Draw(GameTime gameTime)
        {
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
