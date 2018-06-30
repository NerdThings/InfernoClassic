using Inferno.Runtime.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Inferno.Runtime.Core
{
    /// <summary>
    /// The SpatialMode options
    /// </summary>
    public enum SpatialMode
    {
        /// <summary>
        /// This will make GetNearby() to return objects in spaces that the object is inside
        /// </summary>
        Regular,
        /// <summary>
        /// Safe Area will trigger GetNearby() to return every object in spaces contained by the safe area, this ignores any parameters given
        /// </summary>
        SafeArea
    }

    /// <summary>
    /// The UpdateMode options
    /// </summary>
    public enum UpdateMode
    {
        /// <summary>
        /// Every instance has update called
        /// </summary>
        Regular,
        /// <summary>
        /// With SafeArea, BeginUpdate(), Update() and EndUpdate() will only be called on objects within the safe area, improving performance
        /// </summary>
        SafeArea
    }

    /// <summary>
    /// A State which contains an array of Instances and controls them, allowing full power over them
    /// </summary>
    public class State
    {
        #region Fields

        /// <summary>
        /// The Instances Array
        /// </summary>
        private Instance[] Instances;

        /// <summary>
        /// The Spatial Hashing dictionary
        /// </summary>
        private Dictionary<int, List<int>> Spaces;

        /// <summary>
        /// The State Width
        /// </summary>
        public int Width = 0;

        /// <summary>
        /// The State height
        /// </summary>
        public int Height = 0;

        /// <summary>
        /// The Size of each Spatial "Space"
        /// </summary>
        public int SpaceSize = 32;

        /// <summary>
        /// The game that the state belongs to
        /// </summary>
        public Game ParentGame;

        /// <summary>
        /// The states view camera
        /// </summary>
        public Camera Camera;

        /// <summary>
        /// The states background
        /// </summary>
        public Sprite Background;

        /// <summary>
        /// The state bounds
        /// </summary>
        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(0, 0, Width, Height);
            }
        }

        /// <summary>
        /// Whether or not the State has a Spatial Safe Zone
        /// </summary>
        public bool UseSpatialSafeZone;

        /// <summary>
        /// The specified safe zone
        /// </summary>
        public Rectangle SpatialSafeZone;

        /// <summary>
        /// Spatial Mode
        /// </summary>
        public SpatialMode SpatialMode = SpatialMode.Regular;

        /// <summary>
        /// The LastWidth used to determine a State size change
        /// </summary>
        private int LastWidth = 0;

        /// <summary>
        /// The LastHeight used to determine a State size change
        /// </summary>
        private int LastHeight = 0;

        /// <summary>
        /// Specified UpdateMode
        /// </summary>
        public UpdateMode UpdateMode = UpdateMode.Regular;

        /// <summary>
        /// Whether or not to check if something can be seen before drawing
        /// </summary>
        public bool DrawingCheck = false;

        #endregion

        #region Constructors

        public State(Game parent) : this(parent, parent.VirtualWidth, parent.VirtualHeight, null) { }

        public State(Game parent, int Width, int Height, Sprite background = null)
        {
            this.Width = Width;
            this.Height = Height;

            if (background == null)
                background = Sprite.FromColor(Color.White, Width, Height);

            Background = background;

            Instances = new Instance[0];

            ParentGame = parent;

            //Create camera
            Camera = new Camera(ParentGame, this);

            //Init spatial stuff
            ConfigSpatial();
        }

        #endregion

        #region Instance Management

        /// <summary>
        /// Get an instance by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ref Instance GetInstance(int id)
        {
            return ref Instances[id];
        }

        /// <summary>
        /// Get everything that states it is a children of the specified instance ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get the ID of an instance
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public int GetInstanceId(Instance instance)
        {
            return Array.IndexOf(Instances, instance);
        }

        /// <summary>
        /// Get an array of present Instances
        /// </summary>
        /// <returns></returns>
        public Instance[] GetInstances()
        {
            Instance[] ret = new Instance[Instances.Length];
            Array.Copy(Instances, ret, Instances.Length);
            return ret;
        }

        /// <summary>
        /// Add an instance to the State
        /// </summary>
        /// <param name="instance"></param>
        /// <returns>The instance reference ID</returns>
        public int AddInstance(Instance instance)
        {
            int pos = Instances.Length;
            Array.Resize(ref Instances, pos + 1);
            Instances[pos] = instance;
            return pos;    
        }

        //TODO: Add removing of Instances and ID reuse system

        #endregion

        #region Runtime

        public void Draw(SpriteBatch spriteBatch)
        {
            //TODO: Reenable depth soon
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Camera.TranslationMatrix);

            //Draw the State background
            Drawing.Set_Color(Color.White);
            Drawing.Draw_Sprite(new Vector2(0, 0), Background);

            //Draw all instances
            foreach (Instance i in Instances)
            {
                //Drawing Check
                if (DrawingCheck)
                    if (!Camera.Drawable(i.Bounds))
                        continue;

                //Only draw if drawable
                if (i.Draws)
                    i.Runtime_Draw(spriteBatch);
            }

            //End the draw
            spriteBatch.End();
        }

        public void BeginUpdate()
        {
            //Reconfig spatial
            ConfigSpatial();

            foreach (Instance i in Instances)
            {
                //Skip if outside safe area
                if (UseSpatialSafeZone && UpdateMode == UpdateMode.SafeArea)
                    if (!SpatialSafeZone.Intersects(Instances[Array.IndexOf(Instances, i)].Bounds))
                        continue;

                //Check the instance can run Update
                if (i.Updates)
                    i.Runtime_BeginUpdate();
            }
        }

        public void Update(GameTime gameTime)
        {
            //Invoke OnStateUpdate
            OnStateUpdate?.Invoke(this, new EventArgs());

            //Call Update for every instance
            foreach (Instance i in Instances)
            {
                //Skip if outside safe area
                if (UseSpatialSafeZone && UpdateMode == UpdateMode.SafeArea)
                    if (!SpatialSafeZone.Intersects(Instances[Array.IndexOf(Instances, i)].Bounds))
                        continue;

                //Check the instance can run Update
                if (i.Updates)
                    i.Runtime_Update(gameTime);
            }
        }

        public void EndUpdate()
        {
            //Run end update for every instance
            foreach (Instance i in Instances)
            {
                //Skip if outside safe area
                if (UseSpatialSafeZone && UpdateMode == UpdateMode.SafeArea)
                    if (!SpatialSafeZone.Intersects(Instances[Array.IndexOf(Instances, i)].Bounds))
                        continue;

                //Check the instance can run Update
                if (i.Updates)
                    i.Runtime_EndUpdate();
            }

            //Update Last Dimensions
            LastWidth = Width;
            LastHeight = Height;
        }

        public event EventHandler OnStateUpdate;
        public event EventHandler OnStateLoad;
        private bool DidStateLoad = false;

        public void InvokeOnStateLoad(object sender)
        {
            //Check if the state has loaded
            if (!DidStateLoad)
            {
                //Load the state
                OnStateLoad?.Invoke(sender, new EventArgs());
                //Set the state as loaded
                DidStateLoad = true;
            }
            //If the above didn't call, it is because of some weird bug, this is it's hotfix (but also just good practice).
        }

        #endregion

        #region Spatial Hashing

        protected void ConfigSpatial()
        {
            //Check config
            if (SpatialMode == SpatialMode.SafeArea && !UseSpatialSafeZone)
                throw new Exception("SpatialMode.SafeArea requires USeSpatialSafeZone to be true");

            //Calculate the size of the table
            var Cols = Width / SpaceSize;
            var Rows = Height / SpaceSize;

            //Create the spaces array
            if (Spaces == null)
                Spaces = new Dictionary<int, List<int>>(Cols * Rows);

            //Clear the spaces array if State size is changed
            if (LastWidth != Width || LastHeight != Height)
                Spaces.Clear();

            //Fill the possible positions
            for (int i = 0; i < Cols * Rows; i++)
            {
                if (LastWidth != Width || LastHeight != Height)
                    Spaces.Add(i, new List<int>());
                else
                    Spaces[i].Clear();
            }

            //Register all instances into spaces
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

            if (SpatialMode == SpatialMode.Regular)
            {
                List<int> spaceIds = GetIdForObj(obj);
                foreach (var item in spaceIds)
                {
                    foreach (int inst in Spaces[item])
                    {
                        if (!objects.Contains(Instances[inst]))
                            objects.Add(Instances[inst]);
                    }
                }
            }
            else if (SpatialMode == SpatialMode.SafeArea && UseSpatialSafeZone)
            {
                for (int item = 0; item < Spaces.Count - 1; item++)
                {
                    foreach (int inst in Spaces[item])
                    {
                        if (!objects.Contains(Instances[inst]))
                            objects.Add(Instances[inst]);
                    }
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
