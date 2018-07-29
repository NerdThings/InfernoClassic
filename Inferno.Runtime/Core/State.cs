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
        protected Instance[] Instances;

        /// <summary>
        /// The Spatial Hashing dictionary
        /// </summary>
        protected Dictionary<int, List<int>> Spaces;

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
        /// Specified UpdateMode
        /// </summary>
        public UpdateMode UpdateMode = UpdateMode.Regular;

        /// <summary>
        /// Whether or not to check if something can be seen before drawing
        /// </summary>
        public bool DrawingCheck = false;

        #endregion

        #region Properties

        /// <summary>
        /// Get an Instance inside the State
        /// </summary>
        /// <param name="id">The Instance ID</param>
        /// <returns>The Instance</returns>
        public Instance this[int id]
        {
            get
            {
                return Instances[id];
            }
            set
            {
                Instances[id] = value;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new Game State
        /// </summary>
        /// <param name="parent">The Game the State belongs to</param>
        public State(Game parent) : this(parent, parent.VirtualWidth, parent.VirtualHeight, null) { }

        /// <summary>
        /// Create a new Game State
        /// </summary>
        /// <param name="parent">The Game the State belongs to</param>
        /// <param name="Width">The Width of the State</param>
        /// <param name="Height">The Height of the State</param>
        /// <param name="background">The background to be applied to the State</param>
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
            Camera = new Camera(this);

            //Init spatial stuff
            ConfigSpatial();
        }

        /// <summary>
        /// Create a new Game State
        /// </summary>
        /// <param name="parent">The Game the State belongs to</param>
        /// <param name="Width">The Width of the State</param>
        /// <param name="Height">The Height of the State</param>
        /// <param name="backgroundColor">The background color to be applied to the State</param>
        public State(Game parent, int Width, int Height, Color backgroundColor)
        {
            this.Width = Width;
            this.Height = Height;

            Background = Sprite.FromColor(backgroundColor, Width, Height);

            Instances = new Instance[0];

            ParentGame = parent;

            //Create camera
            Camera = new Camera(this);

            //Init spatial stuff
            ConfigSpatial();
        }

        #endregion

        #region Instance Management

        List<int> ReusableIDs = new List<int>();

        /// <summary>
        /// Get an instance by ID
        /// </summary>
        /// <param name="id">The Instance to find</param>
        /// <returns>The found Instance</returns>
        public ref Instance GetInstance(int id)
        {
            return ref Instances[id];
        }

        /// <summary>
        /// Get everything that states it is a children of the specified instance ID
        /// </summary>
        /// <param name="id">The ID of the instance</param>
        /// <returns>A list of instances which are children of the specified Instance ID</returns>
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
        /// <param name="instance">The Instance to find an ID for</param>
        /// <returns>The found ID</returns>
        public int GetInstanceId(Instance instance)
        {
            return Array.IndexOf(Instances, instance);
        }

        /// <summary>
        /// Get an array of present Instances
        /// </summary>
        /// <returns>All current Instances</returns>
        public Instance[] GetInstances()
        {
            Instance[] ret = new Instance[Instances.Length];
            Array.Copy(Instances, ret, Instances.Length);
            return ret;
        }

        /// <summary>
        /// Add an instance to the State
        /// </summary>
        /// <param name="instance">The instance to add</param>
        /// <returns>The instance reference ID</returns>
        public int AddInstance(Instance instance)
        {
            if (ReusableIDs.Count > 0)
            {
                //Reuse an ID
                int id = ReusableIDs[0];
                Instances[id] = instance;
                ReusableIDs.Remove(id);
                return id;
            }
            
            //Don't reuse an ID
            int pos = Instances.Length;
            Array.Resize(ref Instances, pos + 1);
            Instances[pos] = instance;
            return pos;    
        }

        //TODO: Add removing of Instances and ID reuse system

        /// <summary>
        /// Remove all instances at the specified position
        /// </summary>
        /// <param name="Position">Position of instances to remove</param>
        public void RemoveInstances(Vector2 Position)
        {
            int space = GetSpaceForVector(Position);

            if (space >= 0 && space < Spaces.Count)
            {
                foreach (int instance in Spaces[space])
                {
                    if (instance >= 0 && instance < Instances.Length)
                    {
                        Instance inst = null;
                        try
                        {
                            inst = Instances[instance];
                        }
                        catch
                        {
                            continue;
                        }

                        if (inst != null)
                        {
                            if (inst.Position == Position)
                            {
                                RemoveInstance(instance);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Remove an instance with a specific ID
        /// </summary>
        /// <param name="id">ID of the instance to remove</param>
        public void RemoveInstance(int id)
        {
            Instances[id] = null;
            ReusableIDs.Add(id);
        }

        #endregion

        #region Runtime
        
        /// <summary>
        /// Draw a frame of the state
        /// </summary>
        /// <param name="spriteBatch">The spritebatch</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            //TODO: Reenable depth soon
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Camera.TranslationMatrix);

            //Draw the State background
            Drawing.Set_Color(Color.White);
            Drawing.Draw_Sprite(new Vector2(0, 0), Background);

            //Invoke OnStateDraw
            OnStateDraw?.Invoke(this, new EventArgs());

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

        /// <summary>
        /// Begin updating the state
        /// </summary>
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

        /// <summary>
        /// Update the state
        /// </summary>
        /// <param name="gameTime">The gametime</param>
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

        /// <summary>
        /// End update of the state
        /// </summary>
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
        }
        
        /// <summary>
        /// Called when the state is updated
        /// </summary>
        public event EventHandler OnStateUpdate;

        /// <summary>
        /// Called when the state is drawn
        /// </summary>
        public event EventHandler OnStateDraw;

        /// <summary>
        /// Called when the state is loaded
        /// </summary>
        public event EventHandler OnStateLoad;

        /// <summary>
        /// Called when the state is unloaded
        /// </summary>
        public event EventHandler OnStateUnLoad;

        /// <summary>
        /// Whether or not the state loaded
        /// </summary>
        private bool DidStateLoad = false;
        
        /// <summary>
        /// Invoke the state load event
        /// </summary>
        /// <param name="sender">Who is calling the event</param>
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
            else
            {
                throw new Exception("Cannot load state before it has been unloaded.");
            }
        }

        /// <summary>
        /// Invoke the state unload event
        /// </summary>
        /// <param name="sender">Who is calling the event</param>
        public void InvokeOnStateUnLoad(object sender)
        {
            //Check if the state has loaded
            if (DidStateLoad)
            {
                //Load the state
                OnStateUnLoad?.Invoke(sender, new EventArgs());
                //Set the state as loaded
                DidStateLoad = false;
            }
            else
            {
                throw new Exception("Cannot unload state before it is loaded.");
            }
        }

        #endregion

        #region Spatial Hashing

        /// <summary>
        /// Configure the spatial spaces
        /// </summary>
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
            Spaces.Clear();

            //Fill the possible positions
            for (int i = 0; i < Cols * Rows; i++)
            {
                Spaces.Add(i, new List<int>());
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

        /// <summary>
        /// Register the instance in a space
        /// </summary>
        /// <param name="obj">Object to register</param>
        protected void RegisterInstanceInSpace(Instance obj)
        {
            RegisterInstanceInSpace(Array.IndexOf(Instances, obj));
        }

        /// <summary>
        /// Register an instance in a space by ID
        /// </summary>
        /// <param name="obj">Object to register</param>
        protected void RegisterInstanceInSpace(int obj)
        {
            List<int> cellIds = GetIdForObj(obj);
            foreach (var item in cellIds)
            {
                Spaces[item].Add(obj);
            }
        }

        /// <summary>
        /// Get a list of spaces that contains the instance
        /// </summary>
        /// <param name="instance">Instance to calculate spaces for</param>
        /// <returns>Spaces the Instance is inside</returns>
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

        /// <summary>
        /// Get all Instances near the specified instance
        /// </summary>
        /// <param name="obj">Object we are looking around</param>
        /// <returns>Instances near the checked Instance</returns>
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

        /// <summary>
        /// Calculate a space ID and add it to the list
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="width"></param>
        /// <param name="spacestoaddto"></param>
        private void AddToSpace(Vector2 vector, float width, List<int> spacestoaddto)
        {
            int cellPosition = (int)(
                       (System.Math.Floor(vector.X / SpaceSize)) +
                       (System.Math.Floor(vector.Y / SpaceSize))
                       * width
            );

            if (!spacestoaddto.Contains(cellPosition) && cellPosition >= 0 && cellPosition < Spaces.Count)
                spacestoaddto.Add(cellPosition);
        }

        public int GetSpaceForVector(Vector2 vector)
        {
            return (int)(
                       (System.Math.Floor(vector.X / SpaceSize)) +
                       (System.Math.Floor(vector.Y / SpaceSize))
                       * (Width / SpaceSize));
        }

        #endregion
    }
}
