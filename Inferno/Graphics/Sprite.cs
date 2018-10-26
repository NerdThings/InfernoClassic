using System;
using Inferno.Content;

namespace Inferno.Graphics
{
    /// <summary>
    /// A Sprite is the core visual component
    /// </summary>
    public class Sprite : IDisposable
    {
        #region Properties
        
        /// <summary>
        /// The array of textures contained by the Sprite
        /// </summary>
        public Texture2D[] Textures { get; private set; }

        /// <summary>
        /// The Sprite draw Width
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// The Sprite draw Height
        /// </summary>
        public int Height { get; set; }

        public int FrameWidth { get; set; }
        public int FrameHeight { get; set; }

        /// <summary>
        /// The sprite draw origin
        /// </summary>
        public Vector2 Origin { get; set; }

        /// <summary>
        /// The speed of the animation
        /// </summary>
        public float ImageSpeed { get; set; }

        /// <summary>
        /// Whether or not this is a sprite sheet (One texture with more than 1 sprite)
        /// </summary>
        public bool SpriteSheet { get; set; }

        /// <summary>
        /// The rotation to draw at
        /// </summary>
        public float Rotation { get; set; }

        /// <summary>
        /// The current animation frame OR the current texture id in array
        /// </summary>
        private int CurrentFrame { get; set; }

        /// <summary>
        /// The timing system for animations
        /// </summary>
        private float AnimationTimer { get; set; }

        /// <summary>
        /// Get the rectangle that will be drawn of the current texture
        /// </summary>
        public Rectangle SourceRectangle => SpriteSheet ? new Rectangle(CurrentFrame * FrameWidth, 0, FrameWidth, FrameHeight) : new Rectangle(0, 0, Texture.Width, Texture.Height);
        //TODO: Multi-row spritesheets

        /// <summary>
        /// Get the current texture
        /// </summary>
        public Texture2D Texture => SpriteSheet ? Textures[0] : Textures[CurrentFrame];

        /// <summary>
        /// Whether or not the texture is animated
        /// </summary>
        public bool IsAnimated => (FrameHeight != Texture.Height && FrameWidth != Texture.Width);

        #endregion

        #region Constructors

        public Sprite(string filename, Vector2 origin) : this(ContentLoader.Texture2DFromFile(filename), origin)
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Create a non-animated Sprite
        /// </summary>
        /// <param name="texture">Texture for the Sprite</param>
        /// <param name="origin"></param>
        public Sprite(Texture2D texture, Vector2 origin) : this(new[] { texture }, origin, texture.Width, texture.Height) { }

        /// <inheritdoc />
        /// <summary>
        /// Create an animated Sprite using a Sprite Sheet
        /// </summary>
        /// <param name="texture">Sprite Sheet</param>
        /// <param name="origin"></param>
        /// <param name="frameWidth">Frame Width</param>
        /// <param name="frameHeight">Frame Height</param>
        /// <param name="imageSpeed">Animation Speed (in secs)</param>
        /// <param name="startingFrame">Starting Frame (0 = first)</param>
        /// <param name="rotation">The rotation of the sprite</param>
        public Sprite(Texture2D texture, Vector2 origin, int frameWidth, int frameHeight, float imageSpeed = 30, int startingFrame = 0, float rotation = 0) : this(new[] { texture }, origin, frameWidth, frameHeight, imageSpeed, startingFrame, true, rotation) { }

        /// <summary>
        /// Create a Sprite using Texture Array
        /// </summary>
        /// <param name="textures">Textures Array</param>
        /// <param name="origin">The origin of the texture</param>
        /// <param name="frameWidth">Draw Width</param>
        /// <param name="frameHeight">Draw Height</param>
        /// <param name="imageSpeed">Image Speed (in frames)</param>
        /// <param name="startingFrame">Starting Frame (0 = first)</param>
        /// <param name="spriteSheet">Whether or not this is a sprite sheet</param>
        /// <param name="rotation">The rotation of the sprite</param>
        public Sprite(Texture2D[] textures, Vector2 origin, int frameWidth, int frameHeight, float imageSpeed = 30, int startingFrame = 0, bool spriteSheet = true, float rotation = 0)
        {
            Textures = textures;
            Origin = origin;
            Width = frameWidth;
            Height = frameHeight;
            FrameWidth = frameWidth;
            FrameHeight = frameHeight;
            ImageSpeed = imageSpeed;
            CurrentFrame = startingFrame;
            SpriteSheet = spriteSheet;
            Rotation = rotation;
        }

        #endregion

        #region Special

        /// <summary>
        /// Create a sprite from a color
        /// </summary>
        /// <param name="color"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Sprite FromColor(Color color, int width, int height)
        {
            //The array holds the color for each pixel in the texture
            var data = new Color[width * height];

            //Fill the texture data
            for (var x = 0; x < width * height; x++)
            {
                data[x] = color;
            }

            //Create the texture
            var texture = new Texture2D(width, height, data);

            //Return as a new sprite
            return new Sprite(texture, new Vector2(0, 0));
        }

        #endregion

        #region Runtime

        /// <summary>
        /// Update method for updating animation frames
        /// </summary>
        public void Update()
        {
            //Increment the timer
            AnimationTimer += 1;

            //If we meet our goal, increment
            if (!(AnimationTimer > ImageSpeed))
                return;
            //Reset timer
            AnimationTimer = 0f;

            //Increase current frame
            CurrentFrame++;

            //Reset frame if out of range
            if (CurrentFrame * Width >= Texture.Width)
                CurrentFrame = 0;
        }

        #endregion
        
        #region Public Methods

        public void Dispose()
        {
            for (var i = 0; i < Textures.Length; i++)
            {
                Textures[i].Dispose();
                Textures[i] = null;
            }
        }
        
        #endregion
    }
}
