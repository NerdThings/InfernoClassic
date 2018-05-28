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

        private GraphicsDeviceManager _GraphicsDeviceManager { get; set; }

        public List<State> States { get; set; }
        public int CurrentState { get; set; }

        public int VirtualWidth { get; set; }
        public int VirtualHeight { get; set;  }

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

        public Game() : this(1280, 768) { }

        public Game(int IntendedWidth, int IntendedHeight) : base()
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
            _GraphicsDeviceManager.SynchronizeWithVerticalRetrace = true;
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
                    SetState(st);
                    return;
                }
            }
        }

        #endregion

        #region Game Management

        protected override void Initialize()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            PresentationParameters pp = Graphics.PresentationParameters;

            BaseRenderTarget = new RenderTarget2D(GraphicsDevice, VirtualWidth, VirtualHeight, false, SurfaceFormat.Color, DepthFormat.None, pp.MultiSampleCount, RenderTargetUsage.DiscardContents);

            //Config Drawer
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
            base.UnloadContent();
        }

        #endregion

        #region Runtime

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(BaseRenderTarget);
            GraphicsDevice.Clear(Color.Black);

            if (CurrentState != -1)
                States[CurrentState]?.Draw(SpriteBatch);

            // draw render target
            float outputAspect = Window.ClientBounds.Width / (float)Window.ClientBounds.Height;
            float preferredAspect = VirtualWidth / (float)VirtualHeight;

            Rectangle dst;

            if (outputAspect <= preferredAspect)
            {
                // output is taller than it is wider, bars on top/bottom
                int presentHeight = (int)((Window.ClientBounds.Width / preferredAspect) + 0.5f);
                int barHeight = (Window.ClientBounds.Height - presentHeight) / 2;

                dst = new Rectangle(0, barHeight, Window.ClientBounds.Width, presentHeight);
            }
            else
            {
                // output is wider than it is tall, bars left/right
                int presentWidth = (int)((Window.ClientBounds.Height * preferredAspect) + 0.5f);
                int barWidth = (Window.ClientBounds.Width - presentWidth) / 2;

                dst = new Rectangle(barWidth, 0, presentWidth, Window.ClientBounds.Height);
            }

            GraphicsDevice.SetRenderTarget(null);

            // clear to get black bars
            GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 1.0f, 0);

            // draw a quad to get the draw buffer to the back buffer
            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            SpriteBatch.Draw(BaseRenderTarget, dst, Color.White);
            SpriteBatch.End();

            base.Draw(gameTime);
        }

        protected override void Update(GameTime gameTime)
        {
            if (CurrentState != -1)
                States[CurrentState]?.BeginUpdate();

            if (CurrentState != -1)
                States[CurrentState]?.Update(gameTime);

            if (CurrentState != -1)
                States[CurrentState]?.EndUpdate();

            base.Update(gameTime);
        }

        #endregion
    }
}
