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
        #region Fields
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
                return ParentState.GetInstanceId(this);
            }
        }

        /// <summary>
        /// A null instance
        /// </summary>
        private static Instance DefaultNull = null;

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
        public Rectangle Bounds
        {
            get
            {
                if (Sprite == null)
                    return new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
                return new Rectangle((int)Position.X - (int)Sprite.Origin.X, (int)Position.Y - (int)Sprite.Origin.Y, Width, Height);
            }
        }

        /// <summary>
        /// The width of the instance
        /// </summary>
        public int Width
        {
            get
            {
                if (Sprite == null)
                    return _Width;
                return Sprite.Width;
            }
            set
            {
                if (Sprite == null)
                    _Width = value;
                else
                    Sprite.Width = value;
            }
        }

        /// <summary>
        /// The value of the width when a Sprite is not present
        /// </summary>
        private int _Width = 0;

        /// <summary>
        /// The height of the Instance
        /// </summary>
        public int Height
        {
            get
            {
                if (Sprite == null)
                    return _Height;
                return Sprite.Height;
            }
            set
            {
                if (Sprite == null)
                    _Height = value;
                else
                    Sprite.Height = value;
            }
        }

        /// <summary>
        /// The value of the width when a Sprite is not present
        /// </summary>
        private int _Height = 0;

        /// <summary>
        /// Whether or not this Instance calls it's parents events
        /// </summary>
        public bool InheritsParentEvents;

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
        /// <param name="ParentState">The Parent State of the Instance</param>
        /// <param name="Position">The Position of the Instance</param>
        /// <param name="Depth">The Depth of the Instance</param>
        /// <param name="Parent">The Parent of the Instance</param>
        /// <param name="Updates">Whether or not the Instance has Update code</param>
        /// <param name="Draws">Whether or not the Instance has Draw code</param>
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

        /// <summary>
        /// Un set the parent id
        /// </summary>
        public void UnsetParent()
        {
            ParentId = -1;
        }

        /// <summary>
        /// Set the parent
        /// </summary>
        /// <param name="parent">The new Parent of this Instance</param>
        public void SetParent(Instance parent)
        {
            int id = ParentState.GetInstanceId(parent);
            ParentId = id;
        }

        #endregion

        #region Runtime

        /// <summary>
        /// TO BE CALLED BY ENGINE ONLY
        /// </summary>
        public void Runtime_Draw(SpriteBatch spriteBatch)
        {
            Draw();
        }

        /// <summary>
        /// TO BE CALLED BY ENGINE ONLY
        /// </summary>
        public void Runtime_BeginUpdate()
        {
            BeginUpdate();
        }

        /// <summary>
        /// TO BE CALLED BY ENGINE ONLY
        /// </summary>
        public void Runtime_Update(GameTime gameTime)
        {
            //Call sprite update (For animations)
            Sprite?.Update(gameTime);
            Update(gameTime);
        }

        /// <summary>
        /// TO BE CALLED BY ENGINE ONLY
        /// </summary>
        public void Runtime_EndUpdate()
        {
            EndUpdate();
        }

        #endregion

        #region Events

        /// <summary>
        /// This is where drawing will happen
        /// </summary>
        protected virtual void Draw()
        {
            if (!InheritsParentEvents) //If not inheriting, draw (to stop redrawing accidentally)
                Drawing.Draw_Instance(this);
            else
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
        /// <param name="InstanceType">The Instance Type we are seeking</param>
        /// <param name="Pos">The position of the current instance</param>
        /// <returns>Whether or not this instance at Pos is touching any Instances of Type</returns>
        public bool Touching(Type InstanceType, Vector2 Pos)
        {
            //Error if we have a null argument
            if (InstanceType == null)
                throw new ArgumentNullException("InstanceType cannot be null.");

            //List of instances near this Instance
            List<Instance> Near;

            //Keep the original position for resetting
            Vector2 OrigPos = Position;

            //Set this instance's position to the target position for checking
            Position = Pos;

            //Fill the near list
            Near = ParentState.GetNearby(Id);

            //Create my temporary bounds
            Rectangle tmp = new Rectangle((int)(Pos.X - Sprite.Origin.X), (int)(Pos.Y - Sprite.Origin.Y), Bounds.Width, Bounds.Height);

            //Scan
            foreach (Instance inst in Near)
            {
                //Skip invalid instances
                if ((inst.GetType() != InstanceType) || inst == this)
                    continue;

                //Check if we are touching it
                if (inst.Bounds.Touching(tmp)
                    || inst.Bounds.Intersects(tmp))
                {
                    //Reset my position and return
                    Position = OrigPos;
                    return true;
                }
            }

            //Reset my position
            Position = OrigPos;

            //Return false
            return false;
        }

        #endregion
    }

    /// <summary>
    /// Rectangle touching extensions
    /// </summary>
    public static class RectangleTouches
    {
        /// <summary>
        /// Determine if 2 rectangles are touching on the left
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public static bool TouchingLeft(this Rectangle r1, Rectangle r2)
        {
            return r1.Right > r2.Left &&
                   r1.Left < r2.Left &&
                   r1.Bottom > r2.Top &&
                   r1.Top < r2.Bottom;
        }

        /// <summary>
        /// Determine if 2 rectangles are touching on the right
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public static bool TouchingRight(this Rectangle r1, Rectangle r2)
        {
            return r1.Left < r2.Right &&
                   r1.Right > r2.Right &&
                   r1.Bottom > r2.Top &&
                   r1.Top < r2.Bottom;
        }

        /// <summary>
        /// Determine if 2 rectangles are touching on the top
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public static bool TouchingTop(this Rectangle r1, Rectangle r2)
        {
            return r1.Bottom > r2.Top &&
                   r1.Top < r2.Top &&
                   r1.Right > r2.Left &&
                   r1.Left < r2.Right;
        }

        /// <summary>
        /// Determine if 2 rectangles are touching on the bottom
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public static bool TouchingBottom(this Rectangle r1, Rectangle r2)
        {
            return r1.Top < r2.Bottom &&
                   r1.Bottom > r2.Bottom &&
                   r1.Right > r2.Left &&
                   r1.Left < r2.Right;
        }

        /// <summary>
        /// Determine if 2 rectangles are touching
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public static bool Touching(this Rectangle r1, Rectangle r2)
        {
            //Implemented because we don't wanna write this every time
            return r1.TouchingTop(r2) || r1.TouchingBottom(r2) || r1.TouchingLeft(r2) || r1.TouchingRight(r2);
        }
    }
}