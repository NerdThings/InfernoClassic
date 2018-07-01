using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inferno.Runtime.Graphics
{
    /// <summary>
    /// A Sprite is the core visual component
    /// </summary>
    public class Sprite
    {
        #region Fields
        
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
        public bool SpriteSheet { get; private set; }

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
        public Rectangle SourceRectangle
        {
            get
            {
                if (SpriteSheet)
                    return new Rectangle(CurrentFrame * Width, 0, Width, Height);
                else
                    return new Rectangle(0, 0, Width, Height);
            }
        }

        /// <summary>
        /// Get the current texture
        /// </summary>
        public Texture2D Texture
        {
            get
            {
                if (SpriteSheet)
                    return Textures[0];
                else
                    return Textures[CurrentFrame];
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a non-animated Sprite
        /// </summary>
        /// <param name="Texture">Texture for the Sprite</param>
        public Sprite(Texture2D Texture, Vector2 Origin) : this(new[] { Texture }, Origin, Texture.Width, Texture.Height) { }

        /// <summary>
        /// Create an animated Sprite using a Sprite Sheet
        /// </summary>
        /// <param name="Texture">Sprite Sheet</param>
        /// <param name="FrameWidth">Frame Width</param>
        /// <param name="FrameHeight">Frame Height</param>
        /// <param name="Image_Speed">Animation Speed (in secs)</param>
        /// <param name="StartingFrame">Starting Frame (0 = first)</param>
        public Sprite(Texture2D Texture, Vector2 Origin, int FrameWidth, int FrameHeight, float ImageSpeed = 1, int StartingFrame = 0, float Rotation=0) : this(new[] { Texture }, Origin, FrameWidth, FrameHeight, ImageSpeed, StartingFrame, true, Rotation) { }

        /// <summary>
        /// Create a Sprite using Texture Array
        /// </summary>
        /// <param name="Textures">Textures Array</param>
        /// <param name="FrameWidth">Draw Width</param>
        /// <param name="FrameHeight">Draw Height</param>
        /// <param name="Image_Speed">Image Speed (in secs)</param>
        /// <param name="StartingFrame">Starting Frame (0 = first)</param>
        /// <param name="SpriteSheet">Whether or not this is a sprite sheet</param>
        public Sprite(Texture2D[] Textures, Vector2 Origin, int FrameWidth, int FrameHeight, float ImageSpeed = 1, int StartingFrame = 0, bool SpriteSheet = true, float Rotation=0)
        {
            this.Textures = Textures;
            this.Origin = Origin;
            this.Width = FrameWidth;
            this.Height = FrameHeight;
            this.ImageSpeed = ImageSpeed;
            this.CurrentFrame = StartingFrame;
            this.SpriteSheet = SpriteSheet;
            this.Rotation = Rotation;
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
            //Initialize a texture
            Texture2D texture = new Texture2D(Game.Graphics, width, height);

            //The array holds the color for each pixel in the texture
            Color[] data = new Color[width * height];

            //Fill the texture data
            for (int x = 0; x < width * height; x++)
            {
                data[x] = color;
            }

            //Set the texture data
            texture.SetData(data);

            //Return as a new sprite
            return new Sprite(texture, new Vector2(0, 0));
        }

        #endregion

        #region Runtime

        /// <summary>
        /// Update method for updating animation frames
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            //Increment the timer
            AnimationTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            //If we meet our goal, increment
            if (AnimationTimer > ImageSpeed)
            {
                //Reset timer
                AnimationTimer = 0f;

                //Increase current frame
                CurrentFrame++;

                //Reset frame if out of range
                if (CurrentFrame * Width >= Texture.Width)
                    CurrentFrame = 0;
            }
        }

        #endregion
    }
}
