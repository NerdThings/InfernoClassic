namespace Inferno.Core
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
        public int ViewportWidth => _parentState.ParentGame.VirtualWidth;

        /// <summary>
        /// Viewport Height
        /// </summary>
        public int ViewportHeight => _parentState.ParentGame.VirtualHeight;

        /// <summary>
        /// Viewport Center
        /// </summary>
        public Vector2 ViewportCenter => new Vector2(ViewportWidth * 0.5f, ViewportHeight * 0.5f);

        /// <summary>
        /// Translation Martrix
        /// </summary>
        public Matrix TranslationMatrix => Matrix.CreateTranslation(-(int) Position.X,
                                               -(int) Position.Y, 0) *
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

            Zoom = zoom;

            Position = new Vector2(ViewportWidth / 2f, ViewportHeight / 2f);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Center the camera on a position
        /// </summary>
        /// <param name="position">Center Location</param>
        public void CenterOn(Vector2 position)
        {
            //TODO: Fix these calculations... again!
            Position = new Vector2(position.X, position.Y);

            if (position.X / Zoom < (ViewportWorldBoundry.Width / 2f) / Zoom)
            {
                Position = new Vector2(ViewportWorldBoundry.Width / 2f, Position.Y);
            }
            else if (position.X / Zoom > (_parentState.Width - ViewportWorldBoundry.Width / 2)/Zoom)
            {
                Position = new Vector2(_parentState.Width - ViewportWorldBoundry.Width / 2, Position.Y);
            }

            if (position.Y / Zoom < (ViewportWorldBoundry.Height / 2f) / Zoom)
            {
                Position = new Vector2(Position.X, ViewportWorldBoundry.Height / 2f);
            }
            else if (position.Y / Zoom > (_parentState.Height - ViewportWorldBoundry.Height / 2f) / Zoom)
            {
                Position = new Vector2(Position.X, _parentState.Height - ViewportWorldBoundry.Height / 2);
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
