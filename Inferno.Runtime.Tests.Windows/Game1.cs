using Inferno.Runtime.Core;
using Inferno.Runtime.Graphics;
using System;
using Inferno.Runtime.Graphics.Text;
using Inferno.Runtime.Input;
using Inferno.Runtime.UI;

namespace Inferno.Runtime.Tests.Windows
{
    /// <inheritdoc />
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public Game1() : base(1280, 768)
        {
            Window.AllowResize = true;

            BackColor = Color.Red;
        }

        protected override void LoadContent()
        {
            var g1 = AddState(new G1(this));
            SetState(g1);

            base.LoadContent();
        }
    }

    public class G1 : State
    {
        public int Player;

        public Texture2D TestTexture;

        public Font fnt;

        public G1(Game parent) : base(parent)
        {
            OnStateUpdate += UpdateAction;
            OnStateDraw += DrawAction;

            //var wall = new Sprite(Game.ContentManager.Load<Texture2D>("Test_Wall"), new Vector2(0, 0));

            for (var i = 0; i < 8; i++)
            {
                //AddInstance(new Wall(this, new Vector2(i * 16, 12), wall));
                //AddInstance(new Wall(this, new Vector2(i * 16, 52), wall));
            }

            Player = AddInstance(new Player(this, new Vector2(80, 80)));

            Camera.Zoom = 1f;

            UseSpatialSafeZone = true;
            SpatialSafeZone = new Rectangle(0, 0, 256, 256);

            TestTexture = new Texture2D("Test_Sprite.png");
            fnt = new Font("C:\\WINDOWS\\Fonts\\Arial.ttf", 12);

            Background = Sprite.FromColor(Color.Blue, 1024, 1024);

            //MessageBox.Show("Game", "Game has been created. " + Camera.TranslationMatrix.M11);
        }

        public void DrawAction(object sender, EventArgs e)
        {
            var s = Mouse.GetState(this);

            Game.Renderer.Draw(TestTexture, new Vector2(5, 5), Color.Blue);

            Game.Renderer.DrawText("Hello World", new Vector2(50,20), fnt, Color.Blue);

            if (s.LeftButton == ButtonState.Released)
                Game.Renderer.DrawText("sdhfdsahfhsdaj", new Vector2(s.X, s.Y), fnt, Color.Black);
            else
                Game.Renderer.DrawText("sdhfdsahfhsdaj", new Vector2(s.X, s.Y), fnt, Color.Blue);

            Game.Renderer.DrawRectangle(new Rectangle(50, 50, 20, 30), Color.Transparent);

            Drawing.Draw_Circle(new Vector2(80, 80), 20, false);
        }

        public void UpdateAction(object sender, EventArgs e)
        {
            var s = Mouse.GetState(this);

            var p = GetInstance(Player);
            Camera.CenterOn(GetInstance(Player).Position);
            SpatialSafeZone = new Rectangle((int)p.Position.X - 128, (int)p.Position.Y - 128, 256, 256);
        }
    }

    public class Wall : Instance
    {
        public Wall(State parentState, Vector2 position, Sprite sprite) : base(parentState, position, 0, null, false, true)
        {
            Sprite = sprite;
        }

        public override void Draw(Renderer renderer)
        {
            Drawing.Set_Color(Graphics.Color.Red);
            Drawing.Draw_Rectangle(Bounds, true);
            //base.Draw(renderer);
        }
    }

    public class Player : Instance
    {
        public Player(State parentState, Vector2 position) : base(parentState, position, 1, null, true, true)
        {
            Sprite = new Sprite(new Texture2D("Test_Sprite.png"), new Vector2(8, 8));
        }

        public override void Draw(Renderer renderer)
        {
            Drawing.Set_Color(Touching(typeof(Wall), new Vector2(Position.X, Position.Y))
                ? Graphics.Color.Red
                : Graphics.Color.Blue);
            Drawing.Draw_Rectangle(Bounds, true);

            var ms = Input.Mouse.GetState(ParentState);
            Drawing.Set_Color(Graphics.Color.Blue);
            Drawing.Draw_Circle(new Vector2(ms.X, ms.Y), 16);

            for (var xx = 0; xx < ParentState.Width; xx+=ParentState.SpaceSize)
            {
                Drawing.Draw_Line(new Vector2(xx, 0), new Vector2(xx, ParentState.Height));
            }

            for (var yy = 0; yy < ParentState.Height; yy += ParentState.SpaceSize)
            {
                Drawing.Draw_Line(new Vector2(0, yy), new Vector2(ParentState.Width, yy));
            }

            Drawing.Set_Color(Graphics.Color.Blue);
            Drawing.Draw_Rectangle(new Rectangle(0, 0, ParentState.Width, ParentState.Height), true, 4);

            base.Draw(renderer);
        }

        public override void Update(float delta)
        {
            /*var kbdstate = Keyboard.GetState();

            float vsp = 0;
            float hsp = 0;

            if (kbdstate.IsKeyDown(Keys.W))
            {
                vsp -= 2;
            }
            if (kbdstate.IsKeyDown(Keys.S))
            {
                vsp += 2;
            }
            if (kbdstate.IsKeyDown(Keys.A))
            {
                hsp -= 2;
            }
            if (kbdstate.IsKeyDown(Keys.D))
            {
                hsp += 2;
            }

            if (Touching(typeof(Wall), new Vector2(Position.X+hsp, Position.Y)) || Position.X + hsp < 0 || Position.X + hsp > ParentState.Width)
            {
                while (!Touching(typeof(Wall), new Vector2(Position.X+Math.Sign(hsp), Position.Y)) && Position.X+Math.Sign(hsp) > 0 && Position.X + Math.Sign(hsp) < ParentState.Width)
                {
                    Position.X += Math.Sign(hsp);
                }
                hsp = 0;
            }
            Position.X += hsp;

            if (Touching(typeof(Wall), new Vector2(Position.X, Position.Y+vsp)) || Position.Y + vsp < 0 || Position.Y + vsp > ParentState.Height)
            {
                while (!Touching(typeof(Wall), new Vector2(Position.X, Position.Y + Math.Sign(vsp))) && Position.Y + Math.Sign(vsp) > 0 && Position.Y + Math.Sign(vsp) < ParentState.Height)
                {
                    Position.Y += Math.Sign(vsp);
                }
                vsp = 0;
            }

            Position.Y += vsp;
            */

            Position.X += 0.01f;
            Position.Y += 0.01f;

            base.Update(delta);
        }
    }
}
