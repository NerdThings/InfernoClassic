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
    //TODO: Refactor and clean
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
        /// Would this instance collide with instance of type at position
        /// </summary>
        /// <param name="InstanceType"></param>
        /// <param name="Pos"></param>
        /// <returns></returns>
        public bool Touching(Type InstanceType, Vector2 Pos)
        {
            if (InstanceType == null)
                throw new Exception("An instance type must be supplied");

            bool collides = false;

            List<Instance> Near;

            Near = ParentState.GetNearby(Id);

            Vector2 OrigPos = Position;

            Position = Pos;

            foreach (Instance inst in Near)
            {
                if ((inst.GetType() == InstanceType) && inst != this)
                {
                    if (inst.Bounds.TouchingTop(Bounds)
                        || inst.Bounds.TouchingBottom(Bounds)
                        || inst.Bounds.TouchingLeft(Bounds)
                        || inst.Bounds.TouchingRight(Bounds))
                        collides = true;
                }
            }

            Position = OrigPos;

            return collides;
        }

        public bool Intersecting(Type InstanceType, Vector2 Pos)
        {
            if (InstanceType == null)
                throw new Exception("An instance type must be supplied");

            bool collides = false;

            List<Instance> Near;

            Near = ParentState.GetNearby(Id);

            Vector2 OrigPos = Position;

            Position = Pos;

            foreach (Instance inst in Near)
            {
                if ((inst.GetType() == InstanceType) && inst != this)
                {
                    if (inst.Bounds.Intersects(Bounds))
                        collides = true;
                }
            }

            Position = OrigPos;

            return collides;
        }

        #endregion
    }

    public static class RectangleTouches
    {
        public static bool TouchingLeft(this Rectangle r1, Rectangle r2)
        {
            return r1.Right > r2.Left &&
                   r1.Left < r2.Left &&
                   r1.Bottom > r2.Top &&
                   r1.Top < r2.Bottom;
        }

        public static bool TouchingRight(this Rectangle r1, Rectangle r2)
        {
            return r1.Left < r2.Right &&
                   r1.Right > r2.Right &&
                   r1.Bottom > r2.Top &&
                   r1.Top < r2.Bottom;
        }

        public static bool TouchingTop(this Rectangle r1, Rectangle r2)
        {
            return r1.Bottom > r2.Top &&
                   r1.Top < r2.Top &&
                   r1.Right > r2.Left &&
                   r1.Left < r2.Right;
        }

        public static bool TouchingBottom(this Rectangle r1, Rectangle r2)
        {
            return r1.Top < r2.Bottom &&
                   r1.Bottom > r2.Bottom &&
                   r1.Right > r2.Left &&
                   r1.Left < r2.Right;
        }
    }
}