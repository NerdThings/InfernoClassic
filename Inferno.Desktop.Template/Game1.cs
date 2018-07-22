namespace Inferno.Desktop.Template
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Runtime.Game
    {
        int State1;
        //Replace with the dimensions for your game resolution
        public Game1() : base(1280, 768)
        {
            //Allow your game to be resized
            Window.AllowUserResizing = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            State1 = AddState(new State1(this));
            SetState(State1);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
    }    
}
