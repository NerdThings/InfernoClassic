using Inferno.Runtime.Core;
using Inferno.Runtime.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Inferno.Runtime.Tests.Windows
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public Game1() : base(1280, 768)
        {
            Window.AllowUserResizing = true;
        }

        protected override void LoadContent()
        {
            int g1 = AddState(new G1(this));
            SetState(g1);

            base.LoadContent();
        }
    }

    public class G1 : State
    {
        int Player;
        public G1(Game parent) : base(parent)
        {
            Instance inst = new Instance(this, Vector2.Zero);

            AddInstance(inst);

            Sprite wall = new Sprite(Game.ContentManager.Load<Texture2D>("Test_Wall"), new Vector2(0, 0));

            for (int i = 0; i < 4; i++)
                AddInstance(new Wall(this, new Vector2(i*16, 30), wall));

            Player = AddInstance(new Player(this, new Vector2(80, 80)));

            Camera.Zoom = 2f;

            OnStateUpdate += UpdateAction;
        }

        public void UpdateAction(object sender, EventArgs e)
        {
            Camera.CenterOn(GetInstance(Player).Position);
        }
    }

    public class Wall : Instance
    {
        public Wall(State parentState, Vector2 Position, Sprite sprite) : base(parentState, Position, 1, null, false, true)
        {
            Sprite = sprite;
        }
    }

    public class Player : Instance
    {
        public Player(State parentState, Vector2 Position) : base(parentState, Position, 1, null, true, true)
        {
            Sprite = new Sprite(Game.ContentManager.Load<Texture2D>("Test_Sprite"), new Vector2(8, 8));
        }

        protected override void Draw()
        {
            Drawing.Set_Color(Color.Red);
            Drawing.Draw_Rectangle(Bounds, true);

            MouseState ms = Inferno.Runtime.Input.Mouse.GetMouseState(ParentState);
            Drawing.Set_Color(Color.Blue);
            Drawing.Draw_Circle(new Vector2(ms.X, ms.Y), 16);

            base.Draw();
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState kbdstate = Keyboard.GetState();

            float vsp = 0;
            float hsp = 0;

            if (kbdstate.IsKeyDown(Keys.W))
            {
                vsp -= 1;
            }
            if (kbdstate.IsKeyDown(Keys.S))
            {
                vsp += 1;
            }
            if (kbdstate.IsKeyDown(Keys.A))
            {
                hsp -= 1;
            }
            if (kbdstate.IsKeyDown(Keys.D))
            {
                hsp += 1;
            }

            if (IsColliding(typeof(Wall), new Vector2(Position.X+hsp, Position.Y)) || Position.X + hsp < 0 || Position.X + hsp > ParentState.Width)
            {
                while (!IsColliding(typeof(Wall), new Vector2(Position.X+Math.Sign(hsp), Position.Y)) && Position.X+Math.Sign(hsp) > 0 && Position.X + Math.Sign(hsp) < ParentState.Width)
                {
                    Position.X += Math.Sign(hsp);
                }
                hsp = 0;
            }
            Position.X += hsp;

            if (IsColliding(typeof(Wall), new Vector2(Position.X, Position.Y+vsp)) || Position.Y + vsp < 0 || Position.Y + vsp > ParentState.Height)
            {
                while (!IsColliding(typeof(Wall), new Vector2(Position.X, Position.Y + Math.Sign(vsp))) && Position.Y + Math.Sign(vsp) > 0 && Position.Y + Math.Sign(vsp) < ParentState.Height)
                {
                    Position.Y += Math.Sign(vsp);
                }
                vsp = 0;
            }
            Position.Y += vsp;

            base.Update(gameTime);
        }
    }

    public class CircleDrawingThing : Instance
    {
        public CircleDrawingThing(State parentState, Vector2 Position, Instance parent = null, bool updates = true, bool draws = true) : base(parentState, Position, 0, parent, updates, draws)
        {
            
        }

        protected override void Draw()
        {
            Drawing.Set_Alpha(1);
            Drawing.Set_Color(Color.Black);
            Drawing.Draw_Circle(Position, 80, true, 1, 0);
            Drawing.Draw_Rectangle(new Vector2(Position.X + 200, Position.Y + 200), 25, 25, false, 1, 0);
            base.Draw();
        }

        protected override void Update(GameTime gameTime)
        {
            //ParentState.GetInstanceChildren(ParentId);

            //Position = new Vector2(Position.X + 1, Position.Y + 1);

            base.Update(gameTime);
        }
    }
}
