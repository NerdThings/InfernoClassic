using Microsoft.Xna.Framework;

namespace Inferno.Runtime.Core
{
    /// <summary>
    /// The View Camera for a State
    /// </summary>
    public sealed class Camera
    {
        #region Properties

        /// <summary>
        /// The Camera Position
        /// </summary>
        public Vector2 Position { get; private set; }

        /// <summary>
        /// The Camera Zoom
        /// </summary>
        public float Zoom { get; set; }

        /// <summary>
        /// The Camera Rotation
        /// </summary>
        public float Rotation { get; set; }

        /// <summary>
        /// Viewport Width
        /// </summary>
        public int ViewportWidth { get; set; }

        /// <summary>
        /// Viewport Height
        /// </summary>
        public int ViewportHeight { get; set; }

        /// <summary>
        /// Viewport Center
        /// </summary>
        public Vector2 ViewportCenter => new Vector2(ViewportWidth * 0.5f, ViewportHeight * 0.5f);

        /// <summary>
        /// Viewport Center
        /// </summary>
        [System.Obsolete("Access to the window size is not required, this will be removed in a future release.")]
        public Vector2 ViewCenter => new Vector2(_parentState.ParentGame.WindowWidth * 0.5f, _parentState.ParentGame.WindowHeight * 0.5f);

        /// <summary>
        /// Translation Martrix
        /// </summary>
        public Matrix TranslationMatrix => Matrix.CreateTranslation(-(int)Position.X,
                                               -(int)Position.Y, 0) *
                                           Matrix.CreateRotationZ(Rotation) *
                                           Matrix.CreateScale(Zoom, Zoom, 1) *
                                           Matrix.CreateTranslation(new Vector3(ViewportCenter, 0));

        /// <summary>
        /// Viewport boundary
        /// </summary>
        /// <returns>Viewport boundarys</returns>
        public Rectangle ViewportWorldBoundry
        {
            get
            {
                var viewPortCorner = ScreenToWorld(new Vector2(0, 0));
                var viewPortBottomCorner =
                   ScreenToWorld(new Vector2(ViewportWidth, ViewportHeight));

                return new Rectangle((int)viewPortCorner.X,
                   (int)viewPortCorner.Y,
                   (int)(viewPortBottomCorner.X - viewPortCorner.X),
                   (int)(viewPortBottomCorner.Y - viewPortCorner.Y));
            }
        }

        /// <summary>
        /// The state parenting this camera
        /// </summary>
        private readonly State _parentState;
        #endregion

        #region Constructors

        /// <inheritdoc />
        /// <summary>
        /// Create a Camera
        /// </summary>
        /// <param name="parentState">The parent state</param>
        public Camera(State parentState) : this(parentState, 1.0f) { }

        /// <summary>
        /// Create a Camara
        /// </summary>
        /// <param name="parentState">The parent state</param>
        /// <param name="zoom">Zoom</param>
        public Camera(State parentState, float zoom)
        {
            _parentState = parentState;

            ViewportHeight = _parentState.ParentGame.WindowHeight;
            ViewportWidth = _parentState.ParentGame.WindowWidth;

            Position = new Vector2(ViewportWidth / 2f, ViewportHeight / 2f);

            Zoom = zoom > 0.25f ? zoom : 1.0f;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Center the camera on a position
        /// </summary>
        /// <param name="position">Center Location</param>
        public void CenterOn(Vector2 position)
        {
            //This is where #8 occurs, i know why, just not fixing for now
            
            if (position.X < ViewportWorldBoundry.Width / 2f)
            {
                Position = new Vector2(ViewportWorldBoundry.Width / 2f, Position.Y);
            }
            else if (position.X > _parentState.Width - ViewportWorldBoundry.Width / 2)
            {
                Position = new Vector2(_parentState.Width - ViewportWorldBoundry.Width / 2, Position.Y);
            }
            else
            {
                Position = new Vector2(position.X, Position.Y);
            }

            if (position.Y < ViewportWorldBoundry.Height / 2f)
            {
                Position = new Vector2(Position.X, ViewportWorldBoundry.Height / 2f);
            }
            else if (position.Y > _parentState.Height - ViewportWorldBoundry.Height / 2f)
            {
                Position = new Vector2(Position.X, _parentState.Height - ViewportWorldBoundry.Height / 2);
            }
            else
            {
                Position = new Vector2(Position.X, position.Y);
            }
        }

        /// <summary>
        /// Convert World Coorinate to Screen Cooridnate
        /// </summary>
        /// <param name="worldPosition">World Position</param>
        /// <returns>Screen Position</returns>
        public Vector2 WorldToScreen(Vector2 worldPosition)
        {
            return Vector2.Transform(worldPosition, TranslationMatrix);
        }

        /// <summary>
        /// Convert Screen Coorinate to World Cooridnate
        /// </summary>
        /// <param name="screenPosition">Screen Position</param>
        /// <returns>World Position</returns>
        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            return Vector2.Transform(screenPosition, Matrix.Invert(TranslationMatrix));
        }

        /// <summary>
        /// Determine if it is in range
        /// </summary>
        /// <param name="bounds">Range to check</param>
        /// <returns>Whether or not the rectangle is within camera bounds</returns>
        public bool Drawable(Rectangle bounds)
        {
            if (bounds.X + bounds.Width < Position.X - ViewportWidth / 2f && bounds.X < Position.X - ViewportWidth / 2f)
            {
                return false;
            }

            if (bounds.X + bounds.Width > Position.X + ViewportWidth / 2f && bounds.X > Position.X + ViewportWidth / 2f)
            {
                return false;
            }

            if (bounds.Y + bounds.Height < Position.Y - ViewportHeight / 2f && bounds.Y < Position.Y - ViewportHeight / 2f)
            {
                return false;
            }

            return !(bounds.Y + bounds.Height > Position.Y + ViewportHeight / 2f) || !(bounds.Y > Position.Y + ViewportHeight / 2f);
        }

        #endregion
    }
}
