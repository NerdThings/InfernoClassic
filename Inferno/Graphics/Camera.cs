namespace Inferno.Graphics
{
    /// <summary>
    /// A view camera
    /// </summary>
    public class Camera
    {
        #region Private Fields

        private readonly GameState _parentState;

        #endregion

        #region Properties

        public Vector2 Position { get; private set; }
        
        public float Zoom { get; set; }

        /// <summary>
        /// Rotation in degrees
        /// </summary>
        public float Rotation { get; set; }

        public int ViewportWidth => _parentState.ParentGame.VirtualWidth;

        public int ViewportHeight => _parentState.ParentGame.VirtualHeight;

        public Vector2 ViewportCenter => new Vector2(ViewportWidth * 0.5f, ViewportHeight * 0.5f);

        public Matrix TranslationMatrix => Matrix.CreateTranslation(-(int)Position.X,
                                               -(int)Position.Y, 0) *
                                           Matrix.CreateRotationZ(Rotation) *
                                           Matrix.CreateScale(Zoom, Zoom, 1) *
                                           Matrix.CreateTranslation(new Vector3(ViewportCenter, 0));

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

        internal Camera(GameState parentState)
            : this(parentState, 1f)
        {
        }

        internal Camera(GameState parentState, float zoom)
        {
            _parentState = parentState;
            Zoom = zoom;
            Position = ViewportCenter;
        }

        #endregion

        #region Public Methods

        public void CenterOn(Vector2 position)
        {
            //TODO: Calculate if the outside will be visible
            Position = new Vector2(position.X, position.Y);
        }

        public Vector2 WorldToScreen(Vector2 worldPosition)
        {
            return Vector2.Transform(worldPosition, TranslationMatrix);
        }

        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            return Vector2.Transform(screenPosition, Matrix.Invert(TranslationMatrix));
        }

        //TODO: Drawable check

        #endregion
    }
}