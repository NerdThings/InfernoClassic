using System;
using System.Collections.Generic;
using System.Linq;
using Inferno.Graphics;
using Inferno.UI;
using Camera = Inferno.Graphics.Camera;

namespace Inferno
{
    /// <summary>
    /// Spatial Hashing config
    /// </summary>
    public enum SpatialMode
    {
        /// <summary>
        /// Collisions will work everywhere
        /// </summary>
        Regular,
        
        /// <summary>
        /// Collisions will only work within the safearea
        /// </summary>
        SafeArea
    }

    /// <summary>
    /// The update config
    /// </summary>
    public enum UpdateMode
    {
        /// <summary>
        /// Standard Update Scheme, every Instance gets updated every frame if it is marked as updatable
        /// </summary>
        Regular,

        /// <summary>
        /// Only Instances within the safearea will recieve updates
        /// </summary>
        SafeArea
    }

    /// <summary>
    /// The draw config
    /// </summary>
    public enum DrawMode
    {
        /// <summary>
        /// Standard Draw Scheme, draw every Instance
        /// </summary>
        Regular,

        /// <summary>
        /// Enhanced Draw Scheme, checks if the instance would be visible before drawing
        /// </summary>
        DrawCheck,

        /// <summary>
        /// Only instances within the safearea will be drawn
        /// </summary>
        SafeArea
    }

    /// <summary>
    /// GameState's contain Instances and controls them
    /// </summary>
    public class GameState
    {
        #region Private Fields

        /// <summary>
        /// All instances within the state
        /// </summary>
        private readonly List<Instance> _instances;

        /// <summary>
        /// The spatial hashing dictionary
        /// </summary>
        private readonly Dictionary<int, List<Instance>> _spatialDictionary;

        #endregion

        #region Public Fields

        /// <summary>
        /// The width of the state
        /// </summary>
        public int Width;

        /// <summary>
        /// The height of the state
        /// </summary>
        public int Height;

        /// <summary>
        /// The depth to draw the background at
        /// </summary>
        public float BackgroundDepth = -99f;

        /// <summary>
        /// The size of each spatial "block"
        /// </summary>
        public readonly int SpaceSize;

        /// <summary>
        /// The game that the state belongs to
        /// </summary>
        public readonly Game ParentGame;

        /// <summary>
        /// The state background image
        /// </summary>
        public Sprite Background;

        /// <summary>
        /// Whether or not the safe zone is enabled
        /// </summary>
        public bool SafeZoneEnabled;
        
        /// <summary>
        /// The safe zone
        /// </summary>
        public Rectangle SafeZone = new Rectangle(0, 0, -1, -1);

        /// <summary>
        /// The Update config
        /// </summary>
        public UpdateMode UpdateMode = UpdateMode.Regular;

        /// <summary>
        /// The draw config
        /// </summary>
        public DrawMode DrawMode = DrawMode.Regular;

        /// <summary>
        /// The spatial hashing config
        /// </summary>
        public SpatialMode SpatialMode = SpatialMode.Regular;

        /// <summary>
        /// The game translation camera
        /// </summary>
        public Camera Camera;

        /// <summary>
        /// The state user interface
        /// </summary>
        public UserInterface UserInterface;

        #endregion

        #region Properties

        /// <summary>
        /// The state bounds
        /// </summary>
        public Rectangle Bounds => new Rectangle(0, 0, Width, Height);

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new game state
        /// </summary>
        /// <param name="parentGame">Game that the state belongs to</param>
        public GameState(Game parentGame) 
            : this(parentGame, parentGame.VirtualWidth, parentGame.VirtualHeight, Color.White)
        {
        }

        /// <summary>
        /// Create a new game state
        /// </summary>
        /// <param name="parentGame">Game that the state belongs to</param>
        /// <param name="stateWidth">Width of the state</param>
        /// <param name="stateHeight">Height of the state</param>
        /// <param name="stateBackgroundColor">The state background color</param>
        /// <param name="spaceSize">The size for each spatial "block"</param>
        public GameState(Game parentGame, int stateWidth, int stateHeight, Color stateBackgroundColor, int spaceSize = 32)
           : this(parentGame, stateWidth, stateHeight, Sprite.FromColor(stateBackgroundColor, stateWidth, stateHeight), spaceSize)
        {
        }

        /// <summary>
        /// Create a new game state
        /// </summary>
        /// <param name="parentGame">Game that the state belongs to</param>
        /// <param name="stateWidth">Width of the state</param>
        /// <param name="stateHeight">Height of the state</param>
        /// <param name="stateBackground">The state background image</param>
        /// <param name="spaceSize">The size for each spatial "block"</param>
        public GameState(Game parentGame, int stateWidth, int stateHeight, Sprite stateBackground, int spaceSize = 32)
        {
            ParentGame = parentGame;
            Width = stateWidth;
            Height = stateHeight;
            Background = stateBackground;
            SpaceSize = spaceSize;

            _instances = new List<Instance>();
            Camera = new Camera(this);
            UserInterface = new UserInterface(this);

            //Calculate the size of the array
            var cols = Width / SpaceSize;
            var rows = Height / SpaceSize;

            _spatialDictionary = new Dictionary<int, List<Instance>>(cols * rows);
        }

        #endregion

        #region Instance Management

        /// <summary>
        /// Add an instance to the state
        /// </summary>
        /// <param name="instance">Instance to add</param>
        public void AddInstance(Instance instance)
        {
            lock (_instances)
            {
                _instances.Add(instance);
            }
        }

        /// <summary>
        /// Remove an instance from the state
        /// </summary>
        /// <param name="instance">Instance to remove</param>
        public void RemoveInstance(Instance instance)
        {
            lock (_instances)
            {
                _instances.Remove(instance);
            }
        }

        /// <summary>
        /// Remove all instances from the state
        /// </summary>
        public void ClearInstances()
        {
            lock (_instances)
            {
                _instances.Clear();
            }
        }

        /// <summary>
        /// Get instances at the given position
        /// </summary>
        /// <param name="position">Position</param>
        /// <param name="boundBySafearea">Search only within the safe zone</param>
        /// <returns></returns>
        public List<Instance> GetInstancesAt(Vector2 position, bool boundBySafearea = false)
        {
            var ret = new List<Instance>();

            lock (_instances)
            {
                if (boundBySafearea)
                {
                    ret.AddRange(_instances.Where(instance => instance != null)
                        .Where(instance => instance.Position == position)
                        .Where(instance => SafeZone.Contains(instance.Position)));
                }
                else
                {
                    ret.AddRange(_instances.Where(instance => instance != null)
                        .Where(instance => instance.Position == position));
                }
            }

            return ret;
        }

        #endregion

        #region Runtime

        /// <summary>
        /// Draw the state
        /// </summary>
        /// <param name="renderer"></param>
        internal void Draw(Renderer renderer)
        {
            //renderer.Begin(RenderSortMode.Depth, Camera.TranslationMatrix);
            //if (Background != null)
            //    renderer.Draw(Background, new Vector2(0, 0), Color.White, BackgroundDepth);
            //renderer.End();

            //Begin rendering with camera
            renderer.Begin(RenderSortMode.Depth, Camera.TranslationMatrix);

            //Draw the background
            

            //Draw event
            OnDraw?.Invoke(this, new StateOnDrawEventArgs(renderer));

            //Draw all instances
            lock (_instances)
            {
                foreach (var instance in _instances.Where(instance => instance != null)
                    .Where(instance => instance.Draws))
                {
                    //TODO: Draw check (Camera)

                    //Safezone check
                    if (DrawMode == DrawMode.SafeArea && SafeZoneEnabled && !SafeZone.Contains(instance.Position))
                        continue;

                    instance.Draw(renderer);
                }
            }

            renderer.End();

            //Now draw User Interface without the camera translation
            //renderer.Begin(RenderSortMode.Depth);
            //UserInterface.Draw(renderer);
            //renderer.End();
        }

        /// <summary>
        /// Run BeginUpdate for children
        /// </summary>
        internal void BeginUpdate()
        {
            //Check the safezone is configured before updating
            if (SafeZoneEnabled)
                if (SafeZone == new Rectangle(0, 0, -1, -1))
                    throw new Exception("SafeZoneEnabled is true, but the SafeZone has not set.");

            //Reconfigure spatial hashes
            Spatial_Initialise();

            //Update User Interface
            UserInterface.Update();

            //BeginUpdate for all instances that update
            lock (_instances)
            {
                foreach (var instance in _instances.Where(instance => instance != null)
                    .Where(instance => instance.Updates))
                {
                    //Safezone check
                    if (UpdateMode == UpdateMode.SafeArea && SafeZoneEnabled && !SafeZone.Contains(instance.Position))
                        continue;

                    instance.BeginUpdate();
                }
            }
        }

        /// <summary>
        /// Run Update for children
        /// </summary>
        internal void Update()
        {
            //Update event
            OnUpdate?.Invoke(this, EventArgs.Empty);

            //Update all instances that update
            lock (_instances)
            {
                foreach (var instance in _instances.Where(instance => instance != null))
                {
                    //Safezone check
                    if (UpdateMode == UpdateMode.SafeArea && SafeZoneEnabled && !SafeZone.Contains(instance.Position))
                        continue;

                    //Update instance sprite
                    instance.Sprite?.Update();

                    //Update the instance if it updates
                    if (instance.Updates)
                        instance.Update();
                }
            }
        }

        /// <summary>
        /// Run EndUpdate for children
        /// </summary>
        internal void EndUpdate()
        {
            //Update all instances that update
            lock (_instances)
            {
                foreach (var instance in _instances.Where(instance => instance != null)
                    .Where(instance => instance.Updates))
                {
                    //Safezone check
                    if (UpdateMode == UpdateMode.SafeArea && SafeZoneEnabled && !SafeZone.Contains(instance.Position))
                        continue;

                    instance.EndUpdate();
                }
            }
        }

        #endregion

        #region Spatial Hashing

        /// <summary>
        /// Initialise the spatial hashing map for this game phase
        /// </summary>
        private void Spatial_Initialise()
        {
            //Check Config
            if (SpatialMode == SpatialMode.SafeArea && !SafeZoneEnabled)
                throw new Exception("SpatialMode.SafeArea requires SafeZoneEnabled to be true");

            lock (_spatialDictionary)
            {
                //Calculate the size of the array
                var cols = Width / SpaceSize;
                var rows = Height / SpaceSize;

                _spatialDictionary.Clear();

                //Fill the array with lists
                for (var i = 0; i < cols * rows; i++)
                {
                    _spatialDictionary.Add(i, new List<Instance>());
                }

                //Register all instances into spaces
                lock (_instances)
                {
                    //The where statements check the instance is not null and in the room
                    foreach (var instance in _instances.Where(instance => instance != null)
                        .Where(instance => Bounds.Intersects(instance.Bounds)))
                    {
                        if (SpatialMode == SpatialMode.SafeArea && SafeZoneEnabled &&
                            !SafeZone.Contains(instance.Position))
                            continue;

                        Spatial_RegisterInstance(instance);
                    }
                }
            }
        }

        /// <summary>
        /// Get all the spaces that contain the instance
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        private List<int> Spatial_GetSpaces(Instance instance)
        {
            var spacesIn = new List<int>();
            
            var min = new Vector2(instance.Bounds.Top, instance.Bounds.Left);
            var max = new Vector2(instance.Bounds.Bottom, instance.Bounds.Right);

            //TopLeft
            Spatial_AddToSpace(min, spacesIn);
            //TopRight
            Spatial_AddToSpace(new Vector2(max.X, min.X), spacesIn);
            //BottomRight
            Spatial_AddToSpace(max, spacesIn);
            //BottomLeft
            Spatial_AddToSpace(new Vector2(min.X, max.Y), spacesIn);

            return spacesIn;
        }

        /// <summary>
        /// Register an instance into the spatial dictionary
        /// </summary>
        /// <param name="instance">Instance to register</param>
        private void Spatial_RegisterInstance(Instance instance)
        {
            lock (_spatialDictionary)
            {
                var spaces = Spatial_GetSpaces(instance);
                foreach (var space in spaces)
                {
                    _spatialDictionary[space].Add(instance);
                }
            }
        }

        /// <summary>
        /// Register a vector into a list of space ids
        /// </summary>
        /// <param name="position">Position to get id of</param>
        /// <param name="spacesList">List of ids to add to</param>
        private void Spatial_AddToSpace(Vector2 position, List<int> spacesList)
        {
            var width = (float)Width / SpaceSize;

            var cellPosition = (int)(
                (Math.Floor(position.X / SpaceSize)) +
                Math.Floor(position.Y / SpaceSize)
                * width);

            lock (_spatialDictionary)
            {
                if (!spacesList.Contains(cellPosition) && cellPosition >= 0 && cellPosition < _spatialDictionary.Count)
                    spacesList.Add(cellPosition);
            }
        }

        /// <summary>
        /// Get instances near the given instance
        /// </summary>
        /// <param name="instance">Instance to look around</param>
        /// <returns>All instances within one spatial "block" of the given instance</returns>
        public List<Instance> GetNearbyInstances(Instance instance)
        {
            var ret = new List<Instance>();

            var spaces = Spatial_GetSpaces(instance);
            lock (_spatialDictionary)
            {
                foreach (var space in spaces)
                {
                    ret.AddRange(_spatialDictionary[space].Where(inst => inst != null)
                        .Where(inst => !ret.Contains(inst)));
                }
            }

            return ret;
        }

        #endregion

        #region Events

        /// <summary>
        /// On state loaded
        /// </summary>
        public EventHandler OnLoad;

        /// <summary>
        /// On state unloaded
        /// </summary>
        public EventHandler OnUnLoad;

        /// <summary>
        /// On state drawn
        /// </summary>
        public EventHandler<StateOnDrawEventArgs> OnDraw;

        /// <summary>
        /// On state updated
        /// </summary>
        public EventHandler OnUpdate;

        public class StateOnDrawEventArgs : EventArgs
        {
            public Renderer Renderer;

            public StateOnDrawEventArgs(Renderer renderer)
            {
                Renderer = renderer;
            }
        }

        #endregion
    }
}