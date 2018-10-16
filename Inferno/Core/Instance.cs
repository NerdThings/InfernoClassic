using Inferno.Graphics;
using System;

namespace Inferno.Core
{
    /// <summary>
    /// The base class of every game object
    /// </summary>
    public class Instance
    {
        #region Fields
        /// <summary>
        /// The state the instance is inside.
        /// </summary>
        public readonly GameState ParentState;

        /// <summary>
        /// The sprite of the Instance
        /// </summary>
        public Sprite Sprite;

        /// <summary>
        /// The Position of the instance in the world
        /// </summary>
        public Vector2 Position;

        /// <summary>
        /// The depth of the instance
        /// </summary>
        public float Depth;

        /// <summary>
        /// The bounds of the instance
        /// </summary>
        public Rectangle Bounds => Sprite == null ? new Rectangle((int)Position.X, (int)Position.Y, Width, Height) : new Rectangle((int)Position.X - (int)Sprite.Origin.X, (int)Position.Y - (int)Sprite.Origin.Y, Width, Height);

        /// <summary>
        /// The width of the instance
        /// </summary>
        public int Width
        {
            get => Sprite?.Width ?? _width;
            set
            {
                if (Sprite == null)
                    _width = value;
                else
                    Sprite.Width = value;
            }
        }

        /// <summary>
        /// The value of the width when a Sprite is not present
        /// </summary>
        private int _width;

        /// <summary>
        /// The height of the Instance
        /// </summary>
        public int Height
        {
            get => Sprite?.Height ?? _height;
            set
            {
                if (Sprite == null)
                    _height = value;
                else
                    Sprite.Height = value;
            }
        }

        /// <summary>
        /// The value of the width when a Sprite is not present
        /// </summary>
        private int _height;

        /// <summary>
        /// Whether or not this Instance calls it's parents events
        /// </summary>
        public bool InheritsParentEvents;

        /// <summary>
        /// The Instance parent
        /// </summary>
        public Instance Parent { get; set; }

        #endregion

        #region Optimisation (Added to stop unrequired update cycles)

        /// <summary>
        /// Whether or not Update should be called for this Instance
        /// </summary>
        public bool Updates;

        /// <summary>
        /// Whether or not Draw should be called for this Instance
        /// </summary>
        public bool Draws;

        #endregion

        #region Constructor

        /// <summary>
        /// Create a new game Instance
        /// </summary>
        /// <param name="parentState">The Parent State of the Instance</param>
        /// <param name="position">The Position of the Instance</param>
        /// <param name="depth">The Depth of the Instance</param>
        /// <param name="parent">The Parent of the Instance</param>
        /// <param name="updates">Whether or not the Instance has Update code</param>
        /// <param name="draws">Whether or not the Instance has Draw code</param>
        public Instance(GameState parentState, Vector2 position, float depth = 0, Instance parent = null, bool updates = false, bool draws = false)
        {
            ParentState = parentState;
            Updates = updates;
            Draws = draws;
            Depth = depth;
            Parent = parent;

            if (position == null)
                throw new ArgumentNullException();

            Position = position;
        }

        #endregion

        #region Parenting

        /// <summary>
        /// Remove the current parent
        /// </summary>
        public void RemoveParent()
        {
            Parent = null;
        }

        /// <summary>
        /// Set the parent
        /// </summary>
        /// <param name="parent">The new Parent of this Instance</param>
        public void SetParent(Instance parent)
        {
            Parent = parent;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Clone an instance with all current properties etc.
        /// </summary>
        /// <returns>The clone ID</returns>
        public Instance Clone()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Remove an instance from it's state
        /// </summary>
        public void Remove()
        {
            //Just makes this call easier for Instances
            ParentState.RemoveInstance(this);
        }

        #endregion

        #region Runtime

        /// <summary>
        /// This is where drawing will happen
        /// </summary>
        /// <param name="renderer">The spritebatch</param>
        public virtual void Draw(Renderer renderer)
        {
            if (!InheritsParentEvents) //If not inheriting, draw (to stop redrawing accidentally)
            {
                renderer.Draw(this);
            }
            else
                Parent?.Draw(renderer);
        }

        /// <summary>
        /// Called at the very start of a frame
        /// </summary>
        public virtual void BeginUpdate()
        {
            if (InheritsParentEvents)
                Parent?.BeginUpdate();
        }

        /// <summary>
        /// Called after begin update
        /// </summary>
        public virtual void Update()
        {
            if (InheritsParentEvents)
                Parent?.Update();
        }

        /// <summary>
        /// The event called after all updates are done
        /// </summary>
        public virtual void EndUpdate()
        {
            if (InheritsParentEvents)
                Parent?.EndUpdate();
        }

        #endregion

        #region Collisions

        /// <summary>
        /// Would this instance collide with instance of type at position
        /// </summary>
        /// <param name="instanceType">The Instance Type we are seeking</param>
        /// <param name="pos">The position of the current instance</param>
        /// <returns>Whether or not this instance at Pos is touching any Instances of Type</returns>
        public bool Touching(Type instanceType, Vector2 pos)
        {
            //Error if we have a null argument
            if (instanceType == null)
                throw new ArgumentNullException();

            //Keep the original position for resetting
            var origPos = Position;

            //Set this instance's position to the target position for checking
            Position = pos;

            //Build a near list
            var near = ParentState.GetNearbyInstances(this);

            //Create my temporary bounds
            Rectangle tmp;

            if (Sprite != null)
                tmp = new Rectangle((int) (pos.X - Sprite.Origin.X), (int) (pos.Y - Sprite.Origin.Y), Bounds.Width, Bounds.Height);
            else
                tmp = new Rectangle((int) (pos.X), (int) (pos.Y), Bounds.Width, Bounds.Height);

            //Scan
            foreach (var inst in near)
            {
                //Skip invalid instances
                if ((inst.GetType() != instanceType) || inst == this)
                    continue;

                //Check if we are touching it
                if (!inst.Bounds.Touching(tmp) && !inst.Bounds.Intersects(tmp))
                    continue;

                //Reset my position and return
                Position = origPos;
                return true;
            }

            //Reset my position
            Position = origPos;

            //Return false
            return false;
        }

        #endregion
    }
}