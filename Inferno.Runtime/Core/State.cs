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
        //WARNING: THIS IS BEING REFRACTORED AND LOTS IS GOING TO CHANGE
        #region Fields

        public Instance[] Instances;
        public Dictionary<int, List<int>> Spaces;
        public int Width = 0;
        public int Height = 0;
        public int SpaceSize = 32;
        public Game ParentGame;
        public Camera Camera;
        public Color BackgroundColor = Color.White;

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(0, 0, Width, Height);
            }
        }

        #endregion

        #region NEW FIELDS

        public bool UseSpatialSafeZone;
        public Rectangle SpatialSafeZone;

        #endregion

        #region Constructors

        public State(Game parent) : this(parent, parent.VirtualWidth, parent.VirtualHeight) { }

        public State(Game parent, int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;

            Instances = new Instance[0];

            ParentGame = parent;

            //Create camera
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
            Drawing.Set_Color(BackgroundColor);
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
        private bool DidStateLoad = false;

        public void InvokeOnStateLoad(object sender)
        {
            if (!DidStateLoad)
            {
                OnStateLoad?.Invoke(sender, new EventArgs());
                DidStateLoad = true;
            }
        }

        #endregion

        #region Spatial Hashing

        protected void ConfigSpatial()
        {
            //Calculate the size of the table
            var Cols = Width / SpaceSize;
            var Rows = Height / SpaceSize;

            //Create the spaces array
            if (Spaces == null)
                Spaces = new Dictionary<int, List<int>>(Cols * Rows);

            //Clear the spaces array
            Spaces.Clear();

            //Fill the possible positions
            for (int i = 0; i < Cols * Rows; i++)
            {
                Spaces.Add(i, new List<int>());
            }

            //TODO: Come up with a better way of this loop, it is kinda clunky
            for (int i = 0; i < Instances.Length; i++)
            {
                //If the instance is outside of the room skip
                if (!Bounds.Intersects(Instances[i].Bounds))
                    continue;

                //If the instance is outside of the safe zone skip
                if (UseSpatialSafeZone)
                    if (!SpatialSafeZone.Intersects(Instances[i].Bounds))
                        continue;

                //Register the instance
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

        public List<int> GetIdForObj(int instance)
        {
            List<int> spacesIn = new List<int>();

            Instance obj = Instances[instance];

            Vector2 min = new Vector2(
                obj.Bounds.X - (obj.Bounds.Width/2),
                obj.Bounds.Y - (obj.Bounds.Height/2));
            Vector2 max = new Vector2(
                obj.Bounds.X + (obj.Bounds.Width/2),
               obj.Bounds.Y + (obj.Bounds.Height/2));

            float width = Width / SpaceSize;
            //TopLeft
            AddToSpace(min, width, spacesIn);
            //TopRight
            AddToSpace(new Vector2(max.X, min.X), width, spacesIn);
            //BottomRight
            AddToSpace(max, width, spacesIn);
            //BottomLeft
            AddToSpace(new Vector2(min.X, max.Y), width, spacesIn);

            return spacesIn;
        }

        public List<Instance> GetNearby(int obj)
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

            if (!spacestoaddto.Contains(cellPosition) && cellPosition >= 0 && cellPosition < Spaces.Count)
                spacestoaddto.Add(cellPosition);
        }

        #endregion
    }
}
