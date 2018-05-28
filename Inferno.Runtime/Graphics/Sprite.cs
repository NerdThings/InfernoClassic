using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inferno.Runtime.Graphics
{
    public class Sprite
    {
        #region Fields

        private int currentFrame { get; set; }
        public Texture2D[] Textures { get; private set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Vector2 Origin { get; set; }
        public float Image_Speed { get; set; }
        public bool SpriteSheet { get; private set; }
        private float AnimationTimer { get; set; }
        public float Rotation { get; set; }

        public Rectangle SourceRectangle
        {
            get
            {
                if (SpriteSheet)
                    return new Rectangle(currentFrame * Width, 0, Width, Height);
                else
                    return new Rectangle(0, 0, Width, Height);
            }
        }

        public Texture2D Texture
        {
            get
            {
                if (SpriteSheet)
                    return Textures[0];
                else
                    return Textures[currentFrame];
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
        public Sprite(Texture2D Texture, Vector2 Origin, int FrameWidth, int FrameHeight, float Image_Speed = 1, int StartingFrame = 0, float Rotation=0) : this(new[] { Texture }, Origin, FrameWidth, FrameHeight, Image_Speed, StartingFrame, true, Rotation) { }

        /// <summary>
        /// Create a Sprite using Texture Array
        /// </summary>
        /// <param name="Textures">Textures Array</param>
        /// <param name="FrameWidth">Draw Width</param>
        /// <param name="FrameHeight">Draw Height</param>
        /// <param name="Image_Speed">Image Speed (in secs)</param>
        /// <param name="StartingFrame">Starting Frame (0 = first)</param>
        /// <param name="SpriteSheet">Whether or not this is a sprite sheet</param>
        public Sprite(Texture2D[] Textures, Vector2 Origin, int FrameWidth, int FrameHeight, float Image_Speed = 1, int StartingFrame = 0, bool SpriteSheet = true, float Rotation=0)
        {
            this.Textures = Textures;
            this.Origin = Origin;
            this.Width = FrameWidth;
            this.Height = FrameHeight;
            this.Image_Speed = Image_Speed;
            this.currentFrame = StartingFrame;
            this.SpriteSheet = SpriteSheet;
            this.Rotation = Rotation;
        }

        #endregion

        #region Runtime

        public void Update(GameTime gameTime)
        {
            AnimationTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (AnimationTimer > Image_Speed)
            {
                AnimationTimer = 0f;
                currentFrame++;
                if (currentFrame * Width >= Texture.Width)
                {
                    currentFrame = 0;
                }
            }
        }

        #endregion
    }
}
