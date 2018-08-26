﻿using Inferno.Runtime.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Inferno.Runtime.Core
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
        public readonly State ParentState;

        /// <summary>
        /// The ID within the parent state
        /// </summary>
        public int Id => ParentState.GetInstanceId(this);

        /// <summary>
        /// A reference to the parent instance
        /// </summary>
        public Instance Parent => ParentId != -1 ? ParentState.GetInstance(ParentId) : null;

        /// <summary>
        /// The ID of the Parent Instance
        /// </summary>
        public int ParentId;

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
                if (Locked)
                    throw new Exception("You can not modify a locked Instance.");

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
                if (Locked)
                    throw new Exception("You can not modify a locked Instance.");

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
        /// Whether or not the instance accepts changes
        /// WARNING: Misuse will render the Instance useless
        /// Locking will prevent cloning, changes and destruction
        /// Lock can not be lifted
        /// It is recommended if you use this to have your own implementations to prevent custom field modifications.
        /// 
        /// PS. Not finished yet
        /// </summary>
        protected bool Locked;

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
        public Instance(State parentState, Vector2 position, float depth = 0, Instance parent = null, bool updates = false, bool draws = false)
        {
            ParentState = parentState;
            Updates = updates;
            Draws = draws;
            Depth = depth;
            if (parent != null)
            {
                ParentId = ParentState.GetInstanceId(parent);
            }

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
            if (Locked)
                throw new Exception("You can not modify a locked Instance.");

            ParentId = -1;
        }

        /// <summary>
        /// Set the parent
        /// </summary>
        /// <param name="parent">The new Parent of this Instance</param>
        public void SetParent(Instance parent)
        {
            SetParent(ParentState.GetInstanceId(parent));
        }

        /// <summary>
        /// Set the parent
        /// </summary>
        /// <param name="parent">The id of the new Parent of this Instance</param>
        public void SetParent(int parent)
        {
            if (Locked)
                throw new Exception("You can not modify a locked Instance.");

            ParentId = parent;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Clone an instance with all current properties etc.
        /// </summary>
        /// <returns>The clone ID</returns>
        public int Clone()
        {
            if (Locked)
                throw new Exception("You can not modify a locked Instance.");

            //Just makes this call easier for Instances
            return ParentState.AddInstance(this);
        }

        /// <summary>
        /// Remove an instance from it's state
        /// </summary>
        public void Remove()
        {
            if (Locked)
                throw new Exception("You can not modify a locked Instance.");

            //Just makes this call easier for Instances
            ParentState.RemoveInstance(this);
        }

        #endregion

        #region Runtime

        /// <summary>
        /// This is where drawing will happen
        /// </summary>
        /// <param name="spriteBatch">The spritebatch</param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (!InheritsParentEvents) //If not inheriting, draw (to stop redrawing accidentally)
                Drawing.Draw_Instance(this);
            else
                Parent?.Draw(spriteBatch);
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
        /// <param name="gameTime"></param>
        public virtual void Update(GameTime gameTime)
        {
            if (InheritsParentEvents)
                Parent?.Update(gameTime);
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
            var near = ParentState.GetNearby(Id);

            //Create my temporary bounds
            var tmp = new Rectangle((int)(pos.X - Sprite.Origin.X), (int)(pos.Y - Sprite.Origin.Y), Bounds.Width, Bounds.Height);

            //Scan
            foreach (var inst in near)
            {
                //Skip invalid instances
                if ((inst.GetType() != instanceType) || inst == this)
                    continue;

                //Check if we are touching it
                if (!inst.Bounds.Touching(tmp) && !inst.Bounds.Intersects(tmp)) continue;
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