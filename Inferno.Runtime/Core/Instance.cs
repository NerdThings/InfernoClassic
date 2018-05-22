using Inferno.Runtime.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inferno.Runtime.Core
{
    /// <summary>
    /// The base class of every game object
    /// </summary>
    public class Instance
    {
        #region Friendliness
        /// <summary>
        /// The state the instance is inside.
        /// </summary>
        public readonly State ParentState;

        /// <summary>
        /// The ID within the parent state
        /// </summary>
        public int Id
        {
            get
            {
                return Array.IndexOf(ParentState.Instances, this);
            }
        }

        /// <summary>
        /// A null instance
        /// </summary>
        public static Instance DefaultNull = null;

        /// <summary>
        /// A reference to the parent instance
        /// </summary>
        public ref Instance Parent
        {
            get
            {
                if (ParentId != -1)
                    return ref ParentState.GetInstance(ParentId);
                else
                    return ref DefaultNull;
            }
        }

        /// <summary>
        /// The ID of the Parent Instance
        /// </summary>
        public int ParentId;

        public Sprite Sprite;
        public Vector2 Position;
        public Rectangle CollisionMask;
        public float Depth;

        public Rectangle Bounds
        {
            get
            {
                if (Sprite == null)
                    return new Rectangle((int)Position.X, (int)Position.Y, 0, 0);
                else
                    return new Rectangle((int)(Position.X-Sprite.Origin.X), (int)(Position.Y - Sprite.Origin.Y), Sprite.Width, Sprite.Height);
            }
        }

        public bool InheritsParentEvents;

        #endregion

        #region Optimisation

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

        public Instance(State ParentState, Vector2 Position, float Depth = 0, Instance Parent = null, bool Updates = false, bool Draws = false)
        {
            this.ParentState = ParentState;
            this.Updates = Updates;
            this.Draws = Draws;
            this.Depth = Depth;
            if (Parent != null)
            {
                ParentId = this.ParentState.GetInstanceId(Parent);
            }

            if (Position == null)
                throw new ArgumentNullException();

            this.Position = Position;
        }

        #endregion

        #region Parenting

        public void UnsetParent()
        {
            ParentId = -1;
        }

        public void SetParent(Instance parent)
        {
            int id = ParentState.GetInstanceId(parent);
            ParentId = id;
        }

        #endregion

        #region Runtime

        public void Runtime_Draw(SpriteBatch spriteBatch)
        {
            Draw();
        }

        public void Runtime_BeginUpdate()
        {
            BeginUpdate();
        }

        public void Runtime_Update(GameTime gameTime)
        {
            Sprite?.Update(gameTime);
            Update(gameTime);
        }

        public void Runtime_EndUpdate()
        {
            EndUpdate();
        }

        #endregion

        #region Events

        //These events will be called at the correct time, like GameMaker

        /// <summary>
        /// This is where drawing will happen
        /// </summary>
        protected virtual void Draw()
        {
            Drawing.Draw_Instance(this);

            if (InheritsParentEvents)
                Parent?.Draw();
        }

        /// <summary>
        /// Called at the very start of a frame
        /// </summary>
        protected virtual void BeginUpdate()
        {
            if (InheritsParentEvents)
                Parent?.BeginUpdate();
        }

        /// <summary>
        /// Called after begin update
        /// </summary>
        /// <param name="gameTime"></param>
        protected virtual void Update(GameTime gameTime)
        {
            if (InheritsParentEvents)
                Parent?.Update(gameTime);
        }

        /// <summary>
        /// The event called after all updates are done
        /// </summary>
        protected virtual void EndUpdate()
        {
            if (InheritsParentEvents)
                Parent?.EndUpdate();
        }

        #endregion

        #region Collisions

        /// <summary>
        /// Is this instance colliding with ANYTHING
        /// </summary>
        /// <returns></returns>
        public bool IsColliding()
        {
            return IsColliding(null, Position);
        }
        
        /// <summary>
        /// Is this instance colliding with another instance of type
        /// </summary>
        /// <param name="InstanceType"></param>
        /// <returns></returns>
        public bool IsColliding(Type InstanceType)
        {
            return IsColliding(InstanceType, Position);
        }

        /// <summary>
        /// Would this instance collide with instance of type at position
        /// </summary>
        /// <param name="InstanceType"></param>
        /// <param name="Pos"></param>
        /// <returns></returns>
        public bool IsColliding(Type InstanceType, Vector2 Pos)
        {
            bool collides = false;

            List<Instance> Near;

            if (ParentState.UseSpatialHashing)
                Near = ParentState.GetNearby(Id);
            else
                Near = new List<Instance>(ParentState.Instances);

            Vector2 OrigPos = Position;

            Position = Pos;

            foreach (Instance inst in Near)
            {
                if ((inst.GetType() == InstanceType && InstanceType != null) && inst != this)
                {
                    if (inst.Sprite == null)
                        continue;

                    if (inst.Bounds.Intersects(Bounds))
                        collides = true;
                }
            }

            Position = OrigPos;

            return collides;
        }

        #endregion
    }
}