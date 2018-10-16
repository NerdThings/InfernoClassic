﻿using Inferno.Core;
using Inferno.Graphics;
using System;
using Inferno.Graphics.Text;
using Inferno.Input;
using Inferno.UI;
using Inferno.UI.Controls;

namespace Inferno.Runtime.Tests.Windows
{
    /// <inheritdoc />
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public Game1() : base(1024, 768, "Inferno Tests", 240, false, false)
        {
            Window.AllowResize = true;
            //Window.Fullscreen(true);
            Window.ShowCursor = false;

            BackColor = Color.White;
        }

        protected override void LoadContent()
        {
            var g1 = AddState(new G1(this));
            SetState(g1);

            base.LoadContent();
        }
    }

    public class G1 : GameState
    {
        public Player Player;

        public Texture2D TestTexture;

        public Font fnt;

        public Label Zoom;

        public Cursor cur;

        public G1(Game parent) : base(parent, 1024*2, 768*2, Color.White)
        {
            OnUpdate += UpdateAction;
            OnDraw += DrawAction;
            OnUnLoad += OnUnload;

            Mouse.KeyPressed += (sender, args) =>
            {
                if (args.Key == Key.Space)
                {
                    ParentGame.Window.VSync = !ParentGame.Window.VSync;
                }
            };

            var wall = new Sprite(new Texture2D("Test_Wall.png"), new Vector2(0, 0));
            fnt = Font.CreateFont("Papyrus", 24); //Lol, meme font

            var test = new Texture2D("Test_Sprite.png");
            test.Dispose();

            //Colordata test
            var test2 = new Texture2D(2, 2, new []{Color.Green, Color.Yellow, Color.Black, Color.Blue});
            var tst = test2.GetData();
            test2.Dispose();

            for (var i = 0; i < 8; i++)
            {
                AddInstance(new Wall(this, new Vector2(i * 16, 12), wall));
                AddInstance(new Wall(this, new Vector2(i * 16, 52), wall));
            }

            Player = new Player(this, new Vector2(80, 80));
            AddInstance(Player);

            var btn = new Button(new Vector2(20, 100), "Hello", fnt);
            btn.ControlClicked += delegate { Console.WriteLine("CLICKED"); };

            Zoom = new Label(new Vector2(10, 10), "Zoom: 0", fnt);

            UserInterface.AddControl(btn);
            UserInterface.AddControl(Zoom);
            var cursor = new Sprite(new Texture2D("Cursor.png"), new Vector2(0, 0))
            {
                Width = 64,
                Height = 64
            };
            cur = new Cursor(cursor);
            UserInterface.AddControl(cur);

            Camera.Zoom = 0.7f;
            //Camera.Rotation = 0.788f;

            SafeZoneEnabled = true;
            SafeZone = new Rectangle(0, 0, 256, 256);
        }

        private void OnUnload(object sender, EventArgs e)
        {
            fnt?.Dispose();
            TestTexture?.Dispose();
        }

        public void DrawAction(object sender, StateOnDrawEventArgs e)
        {
            var s = Mouse.GetState(this);           

            e.Renderer.DrawLine(new Vector2(0, 50), new Vector2(500, 75), Color.Orange, 10, 3f);
            e.Renderer.DrawText("[ a+b+c+d\n+e+f+g  §§", new Vector2(0,0), fnt, Color.Blue);

            if (s.LeftButton == ButtonState.Pressed)
                e.Renderer.DrawText("sdhfdsahfhsdaj", new Vector2(s.X, s.Y), fnt, Color.Black);
            else
                e.Renderer.DrawText("sdhfdsahfhsdaj", new Vector2(s.X, s.Y), fnt, Color.Blue);

            //Mouse crosshairs
            e.Renderer.DrawLine(new Vector2(s.X - 5, s.Y), new Vector2(s.X + 5, s.Y), Color.Red);
            e.Renderer.DrawLine(new Vector2(s.X, s.Y - 5), new Vector2(s.X, s.Y + 5), Color.Red);

            //Camera center crosshairs
            e.Renderer.DrawLine(new Vector2(Camera.Position.X - 10, Camera.Position.Y), new Vector2(Camera.Position.X + 10, Camera.Position.Y), Color.Red);
            e.Renderer.DrawLine(new Vector2(Camera.Position.X, Camera.Position.Y - 10), new Vector2(Camera.Position.X, Camera.Position.Y + 10), Color.Red);

            e.Renderer.DrawText("Henlo", new Vector2(100, 120), fnt, Color.Blue, 0f, new Vector2(0, 0), 45);
            e.Renderer.DrawText("Henlo", new Vector2(100, 100), fnt, Color.Black, 0);

            //e.Renderer.DrawRectangle(new Rectangle(50, 50, 20, 20), Color.HotPink, 1f, true, 2);
        }

        public void UpdateAction(object sender, EventArgs e)
        {
            var k = Keyboard.GetState();

            if (k.IsKeyDown(Key.Up))
                Camera.Zoom += 0.01f;
            if (k.IsKeyDown(Key.Down))
                Camera.Zoom -= 0.01f;

            Camera.CenterOn(new Vector2(0, 0));

            var s = Mouse.GetState(this);

            Camera.CenterOn(Player.Position);
            SafeZone = new Rectangle((int)Player.Position.X - 128, (int)Player.Position.Y - 128, 256, 256);

            Zoom.Text = "Zoom: " + Camera.Zoom;
        }
    }

    public class Wall : Instance
    {
        public Wall(GameState parentState, Vector2 position, Sprite sprite) : base(parentState, position, 0, null, false, true)
        {
            Sprite = sprite;
        }
    }

    public class Player : Instance
    {
        public Player(GameState parentState, Vector2 position) : base(parentState, position, 1, null, true, true)
        {
            Sprite = new Sprite(new Texture2D("Test_Sprite.png"), new Vector2(8, 8), 16, 16, 60f);
        }

        public override void Draw(Renderer renderer)
        {
            var ms = Mouse.GetState(ParentState);

            base.Draw(renderer);
        }

        public override void Update()
        {
            var kbdstate = Keyboard.GetState();


            float vsp = 0;
            float hsp = 0;

            if (kbdstate.IsKeyDown(Key.W))
            {
                vsp -= 2;
            }
            if (kbdstate.IsKeyDown(Key.S))
            {
                vsp += 2;
            }
            if (kbdstate.IsKeyDown(Key.A))
            {
                hsp -= 2;
            }
            if (kbdstate.IsKeyDown(Key.D))
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

            base.Update();
        }
    }
}
