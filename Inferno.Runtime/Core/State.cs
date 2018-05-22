using Inferno.Runtime.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Inferno.Runtime.Core
{
    /// <summary>
    /// A State is effectivley a game screen
    /// </summary>
    public class State
    {
        #region Fields

        public Instance[] Instances;
        public Dictionary<int, List<int>> Spaces;
        public int Width = 0;
        public int Height = 0;
        public int SpaceSize = 32;
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

            //Init spatial stuff
            ConfigSpatial();
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
            //Reconfig spatial
            ConfigSpatial();

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

        #region Spatial Hashing

        protected void ConfigSpatial()
        {
            if (Spaces != null)
                Spaces.Clear();

            var Cols = Width / SpaceSize;
            var Rows = Height / SpaceSize;

            if (Spaces == null)
                Spaces = new Dictionary<int, List<int>>(Cols * Rows);

            for (int i = 0; i < Cols * Rows; i++)
            {
                Spaces.Add(i, new List<int>());
            }

            for (int i = 0; i < Instances.Length; i++)
            {
                RegisterInstanceInSpace(i);
            }
        }

        protected void RegisterInstanceInSpace(Instance obj)
        {
            RegisterInstanceInSpace(Array.IndexOf(Instances, obj));
        }

        protected void RegisterInstanceInSpace(int obj)
        {
            List<int> cellIds = GetIdForObj(obj);
            foreach (var item in cellIds)
            {
                Spaces[item].Add(obj);
            }
        }

        private List<int> GetIdForObj(int instance)
        {
            List<int> spacesIn = new List<int>();

            Instance obj = Instances[instance];

            float width = Width / SpaceSize;
            //TopLeft
            AddToSpace(new Vector2(obj.Bounds.Left, obj.Bounds.Top), width, spacesIn);
            //TopRight
            AddToSpace(new Vector2(obj.Bounds.Right, obj.Bounds.Top), width, spacesIn);
            //BottomRight
            AddToSpace(new Vector2(obj.Bounds.Right, obj.Bounds.Bottom), width, spacesIn);
            //BottomLeft
            AddToSpace(new Vector2(obj.Bounds.Left, obj.Bounds.Bottom), width, spacesIn);

            return spacesIn;
        }

        internal List<Instance> GetNearby(int obj)
        {
            List<Instance> objects = new List<Instance>();
            List<int> spaceIds = GetIdForObj(obj);
            foreach (var item in spaceIds)
            {
                foreach (int inst in Spaces[item])
                {
                    if (!objects.Contains(Instances[inst]))
                        objects.Add(Instances[inst]);
                }
            }
            return objects;
        }

        private void AddToSpace(Vector2 vector, float width, List<int> spacestoaddto)
        {
            int cellPosition = (int)(
                       (Math.Floor(vector.X / SpaceSize)) +
                       (Math.Floor(vector.Y / SpaceSize))
                       * width
            );

            if (!spacestoaddto.Contains(cellPosition) && cellPosition >= 0)
                spacestoaddto.Add(cellPosition);
        }

        #endregion
    }
}
