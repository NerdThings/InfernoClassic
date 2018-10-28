using System;
using Inferno.Graphics;

namespace Inferno
{
    /// <summary>
    /// The type of collision that an Instance uses
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
        /// Whether or not the Instance is affected by gravity
        /// </summary>
        public bool AffectedByGravity = false;
        
        /// <summary>
        /// The current collision mode
        /// </summary>
        public CollisionMode CollisionMode = CollisionMode.BoundingRectangle;
        
        /// <summary>
        /// The Collision Rectangle.
        /// Uses coordinates relative to the texture.
        /// If null, the instance dimensions will be used
        /// </summary>
        public Rectangle? CollisionRectangle;
        
        /// <summary>
        /// The depth that the instance will be drawn at
        /// </summary>
        public float Depth;

        /// <summary>
        /// Whether or not the instance draws
        /// </summary>
        public bool Draws;
        
        /// <summary>
        /// Whether or not the instance will inherit it's parent's events
        /// </summary>
        public bool InheritsParentEvents = true;
        
        /// <summary>
        /// The instance's parent
        /// </summary>
        public Instance Parent;

        /// <summary>
        /// The state that owns the instance
        /// </summary>
        public readonly GameState ParentState;

        /// <summary>
        /// The roughness of the Instance used for friction
        /// </summary>
        public float Roughness = 0f;
        
        /// <summary>
        /// The instance's sprite
        /// </summary>
        public Sprite Sprite;

        /// <summary>
        /// Whether or not the instance updates
        /// </summary>
        public bool Updates;

        /// <summary>
        /// The current velocity of the instance
        /// </summary>
        public Vector2 Velocity;

        #endregion

        #region Private fields

        /// <summary>
        /// The private collision mask
        /// </summary>
        private Sprite _collisionMask;
        
        /// <summary>
        /// The private height
        /// </summary>
        private int _height;
        
        /// <summary>
        /// The private position
        /// </summary>
        private Vector2 _position;
        
        /// <summary>
        /// The private width
        /// </summary>
        private int _width;

        #endregion

        #region Properties

        /// <summary>
        /// The bounding box of the instance
        /// </summary>
        public Rectangle Bounds
        {
            get
            {
                if (CollisionRectangle.HasValue)
                {
                    var rect = CollisionRectangle.Value;
                    if (Sprite == null)
                        return new Rectangle((int) (Position.X + rect.X), (int) (Position.Y + rect.Y), rect.Width,
                            rect.Height);

                    return new Rectangle((int) (Position.X - Sprite.Origin.X + rect.X),
                        (int) (Position.Y - Sprite.Origin.Y + rect.Y), rect.Width, rect.Height);
                }

                if (Sprite == null)
                    return new Rectangle((int) Position.X, (int) Position.Y, Width, Height);

                return new Rectangle((int) (Position.X - Sprite.Origin.X), (int) (Position.Y - Sprite.Origin.Y),
                    Width, Height);
            }
        }
        
        /// <summary>
        /// The sprite collision mask
        /// </summary>
        public Sprite CollisionMask
        {
            get => _collisionMask ?? Sprite;
            set => _collisionMask = value;
        }
        
        /// <summary>
        /// The height of the instance
        /// </summary>
        public int Height
        {
            get => Sprite?.Height ?? _height;
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

        /// <summary>
        /// Where the Instance will be next frame
        /// </summary>
        public Vector2 NextPosition => Position + Velocity;
        
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
        /// Remove the instance's parent
        /// </summary>
        public void RemoveParent()
        {
            Parent = null;
        }

        /// <summary>
        /// Set the instance's parent
        /// </summary>
        /// <param name="parent"></param>
        public void SetParent(Instance parent)
        {
            Parent = parent;
        }
        
        #endregion

        #region Management

        /// <summary>
        /// Clone the instance
        /// </summary>
        /// <returns>A clone of the instance</returns>
        public Instance Clone()
        {
            return new Instance(ParentState)
            {
                AffectedByGravity = AffectedByGravity,
                CollisionMode = CollisionMode,
                CollisionRectangle = CollisionRectangle,
                Depth = Depth,
                Draws = Draws,
                InheritsParentEvents = InheritsParentEvents,
                Parent = Parent,
                Roughness = Roughness,
                Sprite = Sprite,
                Updates = Updates,
                Velocity = Velocity,
                _collisionMask = _collisionMask,
                _height = _height,
                _width = _width
            };
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
        /// BeginUpdate
        /// </summary>
        public virtual void BeginUpdate()
        {
            if (InheritsParentEvents)
                Parent?.BeginUpdate();
        }

        /// <summary>
        /// Draw the instance
        /// </summary>
        /// <param name="renderer">The game renderer</param>
        public virtual void Draw(Renderer renderer)
        {
            //Don't draw self if we inherit to prevent drawing multiple times
            if ((!InheritsParentEvents || Parent == null) && Sprite != null)
                renderer.Draw(Sprite.Texture, Color.White, Depth, new Rectangle((int) Position.X, (int) Position.Y, Width, Height), Sprite.SourceRectangle, Sprite.Origin, Sprite.Rotation);
            else
                Parent?.Draw(renderer);
        }
        
        /// <summary>
        /// EndUpdate
        /// </summary>
        public virtual void EndUpdate()
        {
            if (InheritsParentEvents)
                Parent?.EndUpdate();

            //Increase position by velocity
            Position += Velocity;
        }

        /// <summary>
        /// Update
        /// </summary>
        public virtual void Update()
        {
            if (InheritsParentEvents)
                Parent?.Update();
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
            //Check that our own collision mask is valid
            if (CollisionMask != null)
                if (CollisionMask.IsAnimated)
                    throw new Exception("An instance collision mask cannot be animated.");
            
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
                if (!CollisionCheck(CollisionMask, inst.CollisionMask, Bounds, inst.Bounds, CollisionMode, inst.CollisionMode))
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
        
        #region Collision Checker
        
        /// <summary>
        /// Check for a pixel collision between two sprites
        /// </summary>
        /// <param name="spriteA"></param>
        /// <param name="spriteB"></param>
        /// <param name="boundsA"></param>
        /// <param name="boundsB"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private bool BothPerPixelCheck(Sprite spriteA, Sprite spriteB, Rectangle boundsA, Rectangle boundsB)
        {
            var colorDataA = spriteA.Texture.GetData();
            var colorDataB = spriteB.Texture.GetData();
            var sourceA = spriteA.SourceRectangle;
            var sourceB = spriteB.SourceRectangle;
            
            //Check for animations
            if (Settings.AttemptToPerPixelCheckAnimation)
                if (spriteA.IsAnimated || spriteB.IsAnimated)
                   throw new Exception(
                        "An attempt to per pixel check an animated sprite has been made, disable this exception by setting Settings.AttemptToPerPixelCheckAnimation to false.");

            var left = Math.Max(boundsA.X, boundsB.X);
            var top = Math.Max(boundsA.Y, boundsB.Y);
            var width = Math.Min(boundsA.Right, boundsB.Right) - left;
            var height = Math.Min(boundsA.Bottom, boundsB.Bottom) - top;

            for (var x = left; x < left + width; x++)
            {
                for (var y = top; y < top + height; y++)
                {
                    var colorAx = x - boundsA.X + sourceA.X;
                    var colorAy = y - boundsA.Y + sourceA.Y;
                    var colorAi = colorAy * spriteA.Texture.Width + colorAx;
                    var colorBx = x - boundsB.X + sourceB.X;
                    var colorBy = y - boundsB.Y + sourceB.Y;
                    var colorBi = colorBy * spriteB.Texture.Width + colorBx;

                    var colorA = colorDataA[colorAi];
                    var colorB = colorDataB[colorBi];

                    if (colorA.A > 0 && colorB.A > 0)
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// A very advanced check to see if a sprite is colliding with another.
        /// This has support for pixel to rectangle and pixel to pixel checks
        /// </summary>
        /// <param name="spriteA">First sprite</param>
        /// <param name="spriteB">Second sprite</param>
        /// <param name="boundsA">First bounds</param>
        /// <param name="boundsB">Second bounds</param>
        /// <param name="collisionModeA">First collision mode</param>
        /// <param name="collisionModeB">Second collision mode</param>
        private bool CollisionCheck(Sprite spriteA, Sprite spriteB, Rectangle boundsA, Rectangle boundsB,
            CollisionMode collisionModeA, CollisionMode collisionModeB)
        {
            //Simple rectangle check
            if (collisionModeA == CollisionMode.BoundingRectangle && collisionModeB == CollisionMode.BoundingRectangle)
            {
                return boundsA.Intersects(boundsB);
            }

            //Pixel to pixel check
            if (collisionModeA == CollisionMode.PerPixel && collisionModeB == CollisionMode.PerPixel)
            {
                return BothPerPixelCheck(spriteA, spriteB, boundsA, boundsB);
            }

            //Pixel to rectangle check
            if (collisionModeA == CollisionMode.PerPixel && collisionModeB == CollisionMode.BoundingRectangle)
            {
                return PixelToRectangleCheck(spriteA, boundsA, boundsB);
            }
            
            //Rectangle to pixel check
            if (collisionModeA == CollisionMode.BoundingRectangle && collisionModeB == CollisionMode.PerPixel)
            {
                return PixelToRectangleCheck(spriteB, boundsB, boundsA);
            }

            //Somehow we slipped the net, return false
            return false;
        }

        /// <summary>
        /// Check for a collision between a sprite and a rectangle
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="boundsA"></param>
        /// <param name="boundsB"></param>
        /// <returns></returns>
        private bool PixelToRectangleCheck(Sprite sprite, Rectangle boundsA, Rectangle boundsB)
        {
            var colorData = sprite.Texture.GetData();
            var source = sprite.SourceRectangle;
            
            var left = Math.Max(boundsA.X, boundsB.X);
            var top = Math.Max(boundsA.Y, boundsB.Y);
            var width = Math.Min(boundsA.Right, boundsB.Right) - left;
            var height = Math.Min(boundsA.Bottom, boundsB.Bottom) - top;
            
            //Check for animations
            if (Settings.AttemptToPerPixelCheckAnimation)
                if (sprite.IsAnimated)
                    throw new Exception(
                        "An attempt to per pixel check an animated sprite has been made, disable this exception by setting Settings.AttemptToPerPixelCheckAnimation to false.");
            
            for (var x = left; x < left + width; x++)
            {
                for (var y = top; y < top + height; y++)
                {
                    var colorX = x - boundsA.X + source.X;
                    var colorY = y - boundsA.Y + source.Y;
                    var colorI = colorY * sprite.Texture.Width + colorX;

                    var color = colorData[colorI];

                    if (color.A > 0 && boundsB.Contains(new Vector2(x, y)))
                        return true;
                }
            }

            return false;
        }
        
        #endregion

        #endregion
    }
}