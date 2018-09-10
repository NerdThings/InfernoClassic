using Inferno.Runtime.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Color = Inferno.Runtime.Graphics.Color;

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
        public int Width;

        /// <summary>
        /// The State height
        /// </summary>
        public int Height;

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
        public Rectangle Bounds => new Rectangle(0, 0, Width, Height);

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
            get => Instances[id];
            set => Instances[id] = value;
        }

        #endregion

        #region Constructors

        /// <inheritdoc />
        /// <summary>
        /// Create a new Game State
        /// </summary>
        /// <param name="parent">The Game the State belongs to</param>
        public State(Game parent) : this(parent, parent.VirtualWidth, parent.VirtualHeight) { }

        /// <summary>
        /// Create a new Game State
        /// </summary>
        /// <param name="parent">The Game the State belongs to</param>
        /// <param name="width">The Width of the State</param>
        /// <param name="height">The Height of the State</param>
        /// <param name="background">The background to be applied to the State</param>
        public State(Game parent, int width, int height, Sprite background = null)
        {
            Width = width;
            Height = 0;
            Height = height;

            if (background == null)
                background = Sprite.FromColor(Color.White, width, height);

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
        /// <param name="width">The Width of the State</param>
        /// <param name="height">The Height of the State</param>
        /// <param name="backgroundColor">The background color to be applied to the State</param>
        public State(Game parent, int width, int height, Color backgroundColor)
        {
            Width = width;
            Height = 0;
            Height = height;

            Background = Sprite.FromColor(backgroundColor, width, height);

            Instances = new Instance[0];

            ParentGame = parent;

            //Create camera
            Camera = new Camera(this);

            //Init spatial stuff
            ConfigSpatial();
        }

        #endregion

        #region Instance Management

        private readonly List<int> _reusableIDs = new List<int>();

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

            for (var i = 0; i < Instances.Length && Instances[i].ParentId == id; i++)
            {
                var pos = ret.Length + 1;
                Array.Resize(ref ret, pos + 1);
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
            var ret = new Instance[Instances.Length];
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
            if (_reusableIDs.Count > 0)
            {
                //Reuse an ID
                var id = _reusableIDs[0];
                Instances[id] = instance;
                _reusableIDs.Remove(id);
                return id;
            }
            
            //Don't reuse an ID
            var pos = Instances.Length;
            Array.Resize(ref Instances, pos + 1);
            Instances[pos] = instance;
            return pos;    
        }

        /// <summary>
        /// Remove all instances at the specified position
        /// </summary>
        /// <param name="position">Position of instances to remove</param>
        /// <param name="boundBySafearea">Whether or not we are only able to remove in the safe area</param>
        public void RemoveInstances(Vector2 position, bool boundBySafearea = false)
        {
            foreach (var instance in GetInstancesAt(position, boundBySafearea))
            {
                RemoveInstance(instance);
            }
        }

        /// <summary>
        /// Remove every instance within the state
        /// </summary>
        public void RemoveAllInstances()
        {
            foreach (var instance in Instances)
            {
                RemoveInstance(instance);
            }
        }

        /// <summary>
        /// Get all instances under the specified position
        /// </summary>
        /// <param name="position">Position to check for</param>
        /// <param name="boundBySafearea">If this is true, this can only fetch stuff within the safe area</param>
        /// <returns></returns>
        public List<Instance> GetInstancesAt(Vector2 position, bool boundBySafearea = false)
        {
            var ret = new List<Instance>();

            if (!boundBySafearea)
            {
                //Cycle through everything (Slow if there are loads of instances)
                ret.AddRange(Instances.Where(instance => instance != null).Where(instance => instance.Position == position));
            }
            else
            {
                //This will only remove stuff IF it is within the safearea (Quicker, but not good)
                var space = GetSpaceForVector(position);

                if (space < 0 || space >= Spaces.Count) return ret;
                foreach (var instance in Spaces[space])
                {
                    if (instance <= 0 || instance >= Instances.Length) continue;
                    if (Instances[instance].Position == position)
                    {
                        ret.Add(Instances[instance]);
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Get the number of instances at the specified position
        /// </summary>
        /// <param name="position">Position to count instances at</param>
        /// <param name="boundBySafeArea">If this is true, this can only count stuff within the safe area</param>
        /// <returns></returns>
        public int CountInstancesAt(Vector2 position, bool boundBySafeArea = false)
        {
            return GetInstancesAt(position, boundBySafeArea).Count;
        }

        /// <summary>
        /// Remove an instance using an instance object
        /// </summary>
        /// <param name="instance">Instance to remove</param>
        public void RemoveInstance(Instance instance)
        {
            RemoveInstance(Array.IndexOf(Instances, instance));
        }

        /// <summary>
        /// Remove an instance with a specific ID
        /// </summary>
        /// <param name="id">ID of the instance to remove</param>
        public void RemoveInstance(int id)
        {
            Instances[id] = null;
            _reusableIDs.Add(id);
        }

        #endregion

        #region Runtime
        
        /// <summary>
        /// Draw a frame of the state
        /// </summary>
        /// <param name="renderer">The spritebatch</param>
        public void Draw(Renderer renderer)
        {
            //TODO: Proper renderer settings
            renderer.Begin();//(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Camera.TranslationMatrix.Monogame);

            //Draw the State background
            Drawing.Set_Color(Color.White);
            Drawing.Draw_Sprite(new Vector2(0, 0), Background);

            //Invoke OnStateDraw
            OnStateDraw?.Invoke(this, new EventArgs());

            //Draw all instances
            foreach (var i in Instances)
            {
                if (i == null)
                    continue;

                //Drawing Check
                if (DrawingCheck)
                    if (!Camera.Drawable(i.Bounds))
                        continue;

                //Only draw if drawable
                if (i.Draws)
                    i.Draw(renderer);
            }

            //End the draw
            renderer.End();
        }

        /// <summary>
        /// Begin updating the state
        /// </summary>
        public void BeginUpdate()
        {
            //Reconfig spatial
            ConfigSpatial();

            foreach (var i in Instances)
            {
                if (i == null)
                    continue;

                //Skip if outside safe area
                if (UseSpatialSafeZone && UpdateMode == UpdateMode.SafeArea)
                    if (!SpatialSafeZone.Intersects(Instances[Array.IndexOf(Instances, i)].Bounds))
                        continue;

                //Check the instance can run Update
                if (i.Updates)
                    i.BeginUpdate();
            }
        }

        /// <summary>
        /// Update the state
        /// </summary>
        /// <param name="delta">The time since last update</param>
        public void Update(float delta)
        {
            //Invoke OnStateUpdate
            OnStateUpdate?.Invoke(this, new EventArgs());

            //Call Update for every instance
            foreach (var i in Instances)
            {
                if (i == null)
                    continue;

                //Skip if outside safe area
                if (UseSpatialSafeZone && UpdateMode == UpdateMode.SafeArea)
                    if (!SpatialSafeZone.Intersects(Instances[Array.IndexOf(Instances, i)].Bounds))
                        continue;

                //Check the instance can run Update
                if (!i.Updates)
                    continue;

                i.Update(delta);
                i.Sprite?.Update(delta);
            }
        }

        /// <summary>
        /// End update of the state
        /// </summary>
        public void EndUpdate()
        {
            //Run end update for every instance
            foreach (var i in Instances)
            {
                if (i == null)
                    continue;

                //Skip if outside safe area
                if (UseSpatialSafeZone && UpdateMode == UpdateMode.SafeArea)
                    if (!SpatialSafeZone.Intersects(Instances[Array.IndexOf(Instances, i)].Bounds))
                        continue;

                //Check the instance can run Update
                if (i.Updates)
                    i.EndUpdate();
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
        private bool _didStateLoad;
        
        /// <summary>
        /// Invoke the state load event
        /// </summary>
        /// <param name="sender">Who is calling the event</param>
        public void InvokeOnStateLoad(object sender)
        {
            //Check if the state has loaded
            if (!_didStateLoad)
            {
                //Load the state
                OnStateLoad?.Invoke(sender, new EventArgs());
                //Set the state as loaded
                _didStateLoad = true;
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
            if (_didStateLoad)
            {
                //Load the state
                OnStateUnLoad?.Invoke(sender, new EventArgs());
                //Set the state as loaded
                _didStateLoad = false;
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
                throw new Exception("SpatialMode.SafeArea requires UseSpatialSafeZone to be true");

            //Calculate the size of the table
            var cols = Width / SpaceSize;
            var rows = Height / SpaceSize;

            //Create the spaces array
            if (Spaces == null)
                Spaces = new Dictionary<int, List<int>>(cols * rows);

            //Clear the spaces array if State size is changed
            Spaces.Clear();

            //Fill the possible positions
            for (var i = 0; i < cols * rows; i++)
            {
                Spaces.Add(i, new List<int>());
            }

            //Register all instances into spaces
            for (var i = 0; i < Instances.Length; i++)
            {
                if (Instances[i] == null)
                    continue;

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
            var cellIds = GetIdForObj(obj);
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
            var spacesIn = new List<int>();

            var obj = Instances[instance];

            var min = new Vector2(
                obj.Bounds.X - (obj.Bounds.Width/2),
                obj.Bounds.Y - (obj.Bounds.Height/2));

            var max = new Vector2(
                obj.Bounds.X + (obj.Bounds.Width/2),
               obj.Bounds.Y + (obj.Bounds.Height/2));

            var width = (float)Width / SpaceSize;
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
            var objects = new List<Instance>();

            if (SpatialMode == SpatialMode.Regular)
            {
                var spaceIds = GetIdForObj(obj);
                foreach (var item in spaceIds)
                {
                    foreach (var inst in Spaces[item])
                    {
                        if (Instances[inst] == null)
                            continue;

                        if (!objects.Contains(Instances[inst]))
                            objects.Add(Instances[inst]);
                    }
                }
            }
            else if (SpatialMode == SpatialMode.SafeArea && UseSpatialSafeZone)
            {
                for (var item = 0; item < Spaces.Count - 1; item++)
                {
                    foreach (var inst in Spaces[item])
                    {
                        if (Instances[inst] == null)
                            continue;

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
        private void AddToSpace(Vector2 vector, float width, ICollection<int> spacestoaddto)
        {
            var cellPosition = (int)(
                       (Math.Floor(vector.X / SpaceSize)) +
                       Math.Floor(vector.Y / SpaceSize)
                       * width
            );

            if (!spacestoaddto.Contains(cellPosition) && cellPosition >= 0 && cellPosition < Spaces.Count)
                spacestoaddto.Add(cellPosition);
        }

        public int GetSpaceForVector(Vector2 vector)
        {
            return (int)(
                       Math.Floor(vector.X / SpaceSize) +
                       (Math.Floor(vector.Y / SpaceSize))
                       * ((float)Width / SpaceSize));
        }

        #endregion
    }
}
