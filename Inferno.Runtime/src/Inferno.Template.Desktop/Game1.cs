using Inferno.Runtime;
using Inferno.Runtime.Core;
using Inferno.Runtime.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Inferno.Template.Desktop
{
    public class Game1 : Runtime.Game
    {
        public Game1() : base(1280, 768)
        {
            Window.AllowUserResizing = true;
        }

        protected override void LoadContent()
        {
            int g1 = AddState(new HelloWorld(this));
            SetState(g1);

            base.LoadContent();
        }
    }

    public class HelloWorld : State
    {
        public HelloWorld(Runtime.Game parent) : base(parent)
        {
            Label label0 = new Label("HelloWorld", this, new Vector2(30, 30), "Hello World.");
            AddInstance(label0);
        }
    }

    public class Label : Instance
    {
        private string Text;
        private SpriteFont Font;

        public Label(string Name, State ParentState, Vector2 Position, string Text) : base(Name, ParentState, Position, 0, null, false, true)
        {
            Font = Runtime.Game.ContentManager.Load<SpriteFont>("DefaultFont");
            this.Text = Text;
        }

        protected override void Draw()
        {
            Drawing.Set_Font(Font);
            Drawing.Set_Color(Color.Black);
            Drawing.Draw_Text(Position, Text);
            base.Draw();
        }
    }
}
