namespace Inferno.Graphics
{
    /// <summary>
    /// A view camera
    /// </summary>
    public class Camera
    {
        #region Private Fields

        /// <summary>
        /// The parent game state.
        /// </summary>
        private readonly GameState _parentState;

        #endregion

        #region Properties

        /// <summary>
        /// The current center position
        /// </summary>
        public Vector2 Position { get; set; }
        
        /// <summary>
        /// The scale factor of the view
        /// </summary>
        public float Zoom { get; set; }

        /// <summary>
        /// Rotation in degrees
        /// </summary>
        public float Rotation { get; set; }

        /// <summary>
        /// The width of the visible viewport
        /// </summary>
        public int ViewportWidth => _parentState.ParentGame.VirtualWidth;

        /// <summary>
        /// The height of the visible viewport
        /// </summary>
        public int ViewportHeight => _parentState.ParentGame.VirtualHeight;

        /// <summary>
        /// The center of the viewport
        /// </summary>
        public Vector2 ViewportCenter => new Vector2(ViewportWidth * 0.5f, ViewportHeight * 0.5f);

        /// <summary>
        /// The translation matrix used with the draw calls associated with the state
        /// </summary>
        public Matrix TranslationMatrix => Matrix.CreateTranslation(-(int)Position.X,
                                               -(int)Position.Y, 0) *
                                           Matrix.CreateRotationZ(Rotation) *
                                           Matrix.CreateScale(Zoom, Zoom, 1) *
                                           Matrix.CreateTranslation(new Vector3(ViewportCenter, 0));

        /// <summary>
        /// The viewport world boundary
        /// </summary>
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

        #endregion

        #region Constructors
        
        /// <summary>
        /// Create a new Camera.
        /// </summary>
        /// <param name="parentState"></param>
        /// <param name="zoom"></param>
        internal Camera(GameState parentState, float zoom = 1f)
        {
            _parentState = parentState;
            Zoom = zoom;
            Position = ViewportCenter;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Center the camera on a position.
        /// </summary>
        /// <param name="position"></param>
        public void CenterOn(Vector2 position)
        {
            //TODO: Calculate if the outside will be visible
            Position = new Vector2(position.X, position.Y);
        }

        /// <summary>
        /// Convert a world coordinate to a screen coordinate
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        public Vector2 WorldToScreen(Vector2 worldPosition)
        {
            return Vector2.Transform(worldPosition, TranslationMatrix);
        }

        /// <summary>
        /// Convert a screen coordinate to a world coordinate
        /// </summary>
        /// <param name="screenPosition"></param>
        /// <returns></returns>
        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            return Vector2.Transform(screenPosition, Matrix.Invert(TranslationMatrix));
        }

        /// <summary>
        /// Check if a rectangle will be visible on the screen
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public bool Drawable(Rectangle rect)
        {
            var rectTopLeft = new Vector2(rect.Left, rect.Top);
            var rectBottomLeft = new Vector2(rect.Left, rect.Bottom);
            var rectTopRight = new Vector2(rect.Right, rect.Top);
            var rectBottomRight = new Vector2(rect.Right, rect.Bottom);

            if (ViewportWorldBoundry.Contains(rectTopLeft) || ViewportWorldBoundry.Contains(rectBottomLeft) ||
                ViewportWorldBoundry.Contains(rectTopRight) || ViewportWorldBoundry.Contains(rectBottomRight))
                return true;
            return false;
        }

        #endregion
    }
}