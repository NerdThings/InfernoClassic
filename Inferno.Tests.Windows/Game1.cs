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
        public Game1() : base(1280, 768)
        {
            Window.AllowResize = true;
            //Window.Fullscreen(true);

            BackColor = Color.White;
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

        public UserInterface UI;

        public G1(Game parent) : base(parent, 1024*2, 768*2)
        {
            OnStateUpdate += UpdateAction;
            OnStateDraw += DrawAction;
            OnStateUnLoad += OnUnload;

            UI = new UserInterface(this);

            var wall = new Sprite(new Texture2D("Test_Wall.png"), new Vector2(0, 0));
            fnt = Font.CreateFont("Times New Roman", 24);

            var test = new Texture2D("Test_Sprite.png");
            test.Dispose();

            for (var i = 0; i < 8; i++)
            {
                AddInstance(new Wall(this, new Vector2(i * 16, 12), wall));
                AddInstance(new Wall(this, new Vector2(i * 16, 52), wall));
            }

            Player = AddInstance(new Player(this, new Vector2(80, 80)));

            var btn = new Button(new Vector2(20, 20), this, "Hello World", fnt);
            btn.ControlClicked += delegate { Console.WriteLine("CLICKED"); };

            UserInterface.AddControl(btn);

            Camera.Zoom = 5f;
            //Camera.Rotation = 0.788f;

            UseSpatialSafeZone = true;
            SpatialSafeZone = new Rectangle(0, 0, 256, 256);

            //Background = Sprite.FromColor(Color.Blue, 1024, 1024);

            //MessageBox.Show("Game", "Game has been created. " + Camera.TranslationMatrix.M11);
        }

        private void OnUnload(object sender, EventArgs e)
        {
            fnt?.Dispose();
            TestTexture?.Dispose();
        }

        public void DrawAction(object sender, OnStateDrawEventArgs e)
        {
           
            e.Renderer.DrawText("Hello World", new Vector2(100, 100), fnt, Color.Black);
            //e.Renderer.Draw(fnt.Texture, Color.Black, 1f, new Vector2(100, 100), null, Vector2.Zero);

            var s = Mouse.GetState(this);

            e.Renderer.DrawLine(new Vector2(0, 50), new Vector2(500, 75), Color.Orange, 10, 3f);
            e.Renderer.DrawRectangle(new Rectangle(0, 0, Width, Height), Color.White, 0f, true);

            e.Renderer.DrawText("Hello World", new Vector2(50,20), fnt, Color.Blue);

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

            e.Renderer.DrawRectangle(new Rectangle(50, 50, 20, 20), Color.HotPink, 1f, true, 2);
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

            var p = GetInstance(Player);
            Camera.CenterOn(GetInstance(Player).Position);
            SpatialSafeZone = new Rectangle((int)p.Position.X - 128, (int)p.Position.Y - 128, 256, 256);

            UserInterface.Update();
        }
    }

    public class Wall : Instance
    {
        public Wall(State parentState, Vector2 position, Sprite sprite) : base(parentState, position, 0, null, false, true)
        {
            Sprite = sprite;
        }
    }

    public class Player : Instance
    {
        public Player(State parentState, Vector2 position) : base(parentState, position, 1, null, true, true)
        {
            Sprite = new Sprite(new Texture2D("Test_Sprite.png"), new Vector2(8, 8), 16, 16, 15f);
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
