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

            Sprite wall = new Sprite(Game.ContentManager.Load<Texture2D>("Test_Wall"), new Vector2(0, 0));

            for (int i = 0; i < 8; i++)
            {
                AddInstance(new Wall(this, new Vector2(i * 16, 12), wall));
                AddInstance(new Wall(this, new Vector2(i * 16, 52), wall));
            }

            Player = AddInstance(new Player(this, new Vector2(80, 80)));

            Camera.Zoom = 4f;

            UseSpatialSafeZone = true;
            SpatialSafeZone = new Rectangle(0, 0, 256, 256);

            OnStateUpdate += UpdateAction;
        }

        public void UpdateAction(object sender, EventArgs e)
        {
            Instance p = GetInstance(Player);
            Camera.CenterOn(GetInstance(Player).Position);
            SpatialSafeZone = new Rectangle((int)p.Position.X - 128, (int)p.Position.Y - 128, 256, 256);
        }
    }

    public class Wall : Instance
    {
        public Wall(State parentState, Vector2 Position, Sprite sprite) : base(parentState, Position, 0, null, false, true)
        {
            Sprite = sprite;
        }

        protected override void Draw()
        {
            Drawing.Set_Color(Color.Red);
            Drawing.Draw_Rectangle(Bounds, true);
            //base.Draw();
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
            if (Touching(typeof(Wall), new Vector2(Position.X, Position.Y)))
                Drawing.Set_Color(Color.Red);
            else
                Drawing.Set_Color(Color.Blue);
            Drawing.Draw_Rectangle(Bounds, true);

            MouseState ms = Inferno.Runtime.Input.Mouse.GetMouseState(ParentState);
            Drawing.Set_Color(Color.Blue);
            Drawing.Draw_Circle(new Vector2(ms.X, ms.Y), 16, false, 1, 0);

            for (int xx = 0; xx < ParentState.Width; xx+=ParentState.SpaceSize)
            {
                Drawing.Draw_Line(new Vector2(xx, 0), new Vector2(xx, ParentState.Height));
            }

            for (int yy = 0; yy < ParentState.Height; yy += ParentState.SpaceSize)
            {
                Drawing.Draw_Line(new Vector2(0, yy), new Vector2(ParentState.Width, yy));
            }

            Drawing.Set_Color(Color.Blue);
            Drawing.Draw_Rectangle(new Rectangle(0, 0, ParentState.Width, ParentState.Height), true, 4);

            base.Draw();
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState kbdstate = Keyboard.GetState();

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

            base.Update(gameTime);
        }
    }
}
