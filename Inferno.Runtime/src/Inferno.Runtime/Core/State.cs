using Inferno.Runtime.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inferno.Runtime.Core
{
    /// <summary>
    /// A State is effectivley a game screen
    /// </summary>
    public class State
    {
        #region Fields

        public Instance[] Instances;
        public int Width = 0;
        public int Height = 0;
        public Game ParentGame;
        public Camera Camera;

        #endregion

        #region Constructors

        public State(Game parent) : this(parent, null) { }

        public State(Game parent, Instance[] instances) : this(parent, instances, parent.VirtualWidth, parent.VirtualHeight) { }

        public State(Game parent, Instance[] instances, int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;

            if (instances != null)
                Instances = instances;
            else
                Instances = new Instance[0];

            ParentGame = parent;

            Camera = new Camera(ParentGame, this);
        }

        #endregion

        #region Instance Management

        public ref Instance GetInstance(int id)
        {
            return ref Instances[id];
        }

        public Instance[] GetInstanceChildren(int id)
        {
            Instance[] ret = { };

            for (int i = 0; i < Instances.Length && Instances[i].ParentId == id; i++)
            {
                int pos = ret.Length + 1;
                Array.Resize<Instance>(ref ret, pos + 1);
                ret[pos] = Instances[i];
            }

            return ret;
        }

        public int GetInstanceId(Instance instance)
        {
            for (int i = 0; i < Instances.Length && Instances[i] == instance; i++)
            {
                return i;
            }
            return -1;
        }

        public Instance[] GetInstances()
        {
            Instance[] ret = new Instance[Instances.Length];
            Array.Copy(Instances, ret, Instances.Length);
            return ret;
        }

        public int AddInstance(Instance instance)
        {
            int pos = Instances.Length;
            Array.Resize(ref Instances, pos + 1);
            Instances[pos] = instance;
            return pos;    
        }

        #endregion

        #region Runtime

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, Camera.TranslationMatrix);

            Drawing.Set_Color(Color.White);
            Drawing.Draw_Rectangle(new Vector2(0, 0), Width, Height);

            foreach (Instance i in Instances)
            {
                if (i.Draws)
                    i.Runtime_Draw(spriteBatch);
            }

            spriteBatch.End();
        }

        public void BeginUpdate()
        {
            foreach (Instance i in Instances)
            {
                if (i.Updates)
                    i.Runtime_BeginUpdate();
            }
        }

        public void Update(GameTime gameTime)
        {
            OnStateUpdate?.Invoke(this, new EventArgs());
            foreach (Instance i in Instances)
            {
                if (i.Updates)
                    i.Runtime_Update(gameTime);
            }
        }

        public void EndUpdate()
        {
            foreach (Instance i in Instances)
            {
                if (i.Updates)
                    i.Runtime_EndUpdate();
            }
        }

        public event EventHandler OnStateUpdate;
        public event EventHandler OnStateLoad;

        public void InvokeOnStateLoad(object sender)
        {
            OnStateLoad?.Invoke(sender, new EventArgs());
        }

        #endregion
    }
}
