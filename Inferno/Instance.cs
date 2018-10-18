using System;
using System.Configuration;
using System.Linq;
using Inferno.Graphics;
using OpenTK.Input;

namespace Inferno
{
    /// <summary>
    /// Not used yet...
    /// </summary>
    public enum CollisionMode
    {
        /// <summary>
        /// Collision checking using the Bounds rectangle
        /// </summary>
        BoundingRectangle,
        
        /// <summary>
        /// Collision checking using pixel data from sprites
        /// </summary>
        PerPixel
    }
    
    /// <summary>
    /// A game object
    /// </summary>
    public class Instance : IDisposable
    {
        #region Public Fields

        /// <summary>
        /// The state that owns the instance
        /// </summary>
        public readonly GameState ParentState;

        /// <summary>
        /// The instance's sprite
        /// </summary>
        public Sprite Sprite;

        /// <summary>
        /// The depth that the instance will be drawn at
        /// </summary>
        public float Depth;

        /// <summary>
        /// The instance's parent
        /// </summary>
        public Instance Parent;

        /// <summary>
        /// Whether or not the instance will inherit it's parent's events
        /// </summary>
        public bool InheritsParentEvents = true;

        /// <summary>
        /// Whether or not the instance updates
        /// </summary>
        public bool Updates;

        /// <summary>
        /// Whether or not the instance draws
        /// </summary>
        public bool Draws;

        #endregion

        #region Private fields

        /// <summary>
        /// The private position
        /// </summary>
        private Vector2 _position;
        
        /// <summary>
        /// The private width
        /// </summary>
        private int _width;

        /// <summary>
        /// The private height
        /// </summary>
        private int _height;

        #endregion

        #region Properties

        //TODO: Collision option of Pixel Perfect...

        /// <summary>
        /// The bounding box of the instance
        /// </summary>
        public Rectangle Bounds => Sprite == null
            ? new Rectangle((int)Position.X, (int)Position.Y, Width, Height)
            : new Rectangle((int)(Position.X - Sprite.Origin.X), (int)(Position.Y - Sprite.Origin.Y), Width, Height);
        
        /// <summary>
        /// The instance's position
        /// </summary>
        public Vector2 Position
        {
            get => _position;
            set
            {
                //Save bounds for spatial recalculation
                var oldBounds = Bounds;
                
                //Update value
                _position = value;
                
                //Update spatial hash
                ParentState.Spatial_MoveInstance(oldBounds, Bounds, this);
            }
        }

        /// <summary>
        /// The X Position of the Instance
        /// </summary>
        public float X
        {
            get => Position.X;
            set => Position = new Vector2(value, Position.Y);
        }
        
        /// <summary>
        /// The Y Position of the Instance
        /// </summary>
        public float Y
        {
            get => Position.Y;
            set => Position = new Vector2(Position.X, value);
        }

        /// <summary>
        /// The width of the instance
        /// </summary>
        public int Width
        {
            get => Sprite?.Width ?? _width;
            set
            {
                //Save bounds for spatial recalculation
                var oldBounds = Bounds;
                
                //Update value
                if (Sprite == null)
                    _width = value;
                else
                    Sprite.Width = value;
                
                //Update spatial hash
                ParentState.Spatial_MoveInstance(oldBounds, Bounds, this);
            }
        }

        /// <summary>
        /// The height of the instance
        /// </summary>
        public int Height
        {
            get => Sprite?.Height ?? _width;
            set
            {
                //Save bounds for spatial recalculation
                var oldBounds = Bounds;
                
                //Update value
                if (Sprite == null)
                    _height = value;
                else
                    Sprite.Height = value;


                //Update spatial hash
                ParentState.Spatial_MoveInstance(oldBounds, Bounds, this);
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <param name="parentState">The state that owns the instance</param>
        public Instance(GameState parentState)
            : this(parentState, Vector2.Zero)
        {
        }

        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <param name="parentState">The state that owns the instance</param>
        /// <param name="position">The instance position</param>
        /// <param name="depth">The depth to draw the instance</param>
        /// <param name="updates">Whether or not the instance updates</param>
        /// <param name="draws">Whether or not the instance draws</param>
        /// <param name="parent">The instance that this inherits from</param>

        public Instance(GameState parentState, Vector2 position, float depth = 0f,
            bool updates = false, bool draws = false, Instance parent = null)
        {
            ParentState = parentState;
            Position = position;
            Depth = depth;
            Parent = parent;
            Updates = updates;
            Draws = draws;
        }

        #endregion

        #region Public Methods

        #region Parenting

        /// <summary>
        /// Set the instance's parent
        /// </summary>
        /// <param name="parent"></param>
        public void SetParent(Instance parent)
        {
            Parent = parent;
        }

        /// <summary>
        /// Remove the instance's parent
        /// </summary>
        public void RemoveParent()
        {
            Parent = null;
        }

        #endregion

        #region Management

        /// <summary>
        /// Clone the instance
        /// </summary>
        /// <returns>A clone of the instance</returns>
        /// <exception cref="NotImplementedException">Currently not implemented</exception>
        public Instance Clone()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Destroy the instance.
        /// Alias for Dispose().
        /// </summary>
        public void Destroy()
        {
            Dispose();
        }

        /// <summary>
        /// Dispose the instance
        /// </summary>
        public void Dispose()
        {
            ParentState.RemoveInstance(this);
            Updates = false;
            Draws = false;
            Sprite.Dispose();
            Sprite = null;
        }

        #endregion

        #region Runtime

        /// <summary>
        /// Draw the instance
        /// </summary>
        /// <param name="renderer">The game renderer</param>
        public virtual void Draw(Renderer renderer)
        {
            //Don't draw self if we inherit to prevent drawing multiple times
            if (!InheritsParentEvents || Parent == null)
                renderer.Draw(this);
            else
                Parent.Draw(renderer);
        }

        /// <summary>
        /// BeginUpdate
        /// </summary>
        public virtual void BeginUpdate()
        {
            if (InheritsParentEvents)
                Parent?.BeginUpdate();
        }

        /// <summary>
        /// Update
        /// </summary>
        public virtual void Update()
        {
            if (InheritsParentEvents)
                Parent?.Update();
        }

        /// <summary>
        /// EndUpdate
        /// </summary>
        public virtual void EndUpdate()
        {
            if (InheritsParentEvents)
                Parent?.EndUpdate();
        }

        #endregion

        #region Collisions

        /// <summary>
        /// Whether or not the instance is touching anything
        /// </summary>
        /// <param name="instanceType">A specific type to check for</param>
        /// <returns>Whether or not it is touching anything (or the specified type)</returns>
        public bool Colliding(Type instanceType = null)
        {
            return Colliding(Position, instanceType);
        }

        /// <summary>
        /// Whether or not the instance is touching anything
        /// </summary>
        /// <param name="pos">The position to check at (For pre-movement checks)</param>
        /// <param name="instanceType">A specific type to check for</param>
        /// <returns>Whether or not it is touching anything (or the specified type)</returns>
        public bool Colliding(Vector2 pos, Type instanceType = null)
        {
            //If the type is null, set it to Instance
            if (instanceType == null)
                instanceType = typeof(Instance);

            //Keep position for resetting
            var origPos = Position;

            //Move to the check position for checking collision
            //Using _position so we don't update the spatial map, because we dont need to
            _position = pos;

            //Build a list of everything nearby
            var near = ParentState.GetNearbyInstances(this);

            //Search for collision
            foreach (var inst in near)
            {
                //Skip invalid instances
                if ((inst.GetType().IsInstanceOfType(instanceType)) || inst == this)
                    continue;

                //Check if we are colliding with it
                if (!inst.Bounds.Touching(Bounds))
                    continue;

                //Reset my position and return
                _position = origPos;
                return true;
            }

            //Reset my position
            _position = origPos;

            //Return false
            return false;
        }

        #endregion

        #endregion
    }
}