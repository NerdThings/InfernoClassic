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

        private int currentFrame;
        public readonly Texture2D[] Textures;
        public int Width;
        public int Height;
        public Vector2 Origin;
        public float Image_Speed;
        public readonly bool SpriteSheet;
        private float AnimationTimer;

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
        public Sprite(Texture2D Texture, Vector2 Origin, int FrameWidth, int FrameHeight, float Image_Speed = 1, int StartingFrame = 0) : this(new[] { Texture }, Origin, FrameWidth, FrameHeight, Image_Speed, StartingFrame, true) { }

        /// <summary>
        /// Create a Sprite using Texture Array
        /// </summary>
        /// <param name="Textures">Textures Array</param>
        /// <param name="FrameWidth">Draw Width</param>
        /// <param name="FrameHeight">Draw Height</param>
        /// <param name="Image_Speed">Image Speed (in secs)</param>
        /// <param name="StartingFrame">Starting Frame (0 = first)</param>
        /// <param name="SpriteSheet">Whether or not this is a sprite sheet</param>
        public Sprite(Texture2D[] Textures, Vector2 Origin, int FrameWidth, int FrameHeight, float Image_Speed = 1, int StartingFrame = 0, bool SpriteSheet = true)
        {
            this.Textures = Textures;
            this.Origin = Origin;
            this.Width = FrameWidth;
            this.Height = FrameHeight;
            this.Image_Speed = Image_Speed;
            this.currentFrame = StartingFrame;
            this.SpriteSheet = SpriteSheet;
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
