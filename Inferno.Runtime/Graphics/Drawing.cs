using Inferno.Runtime.Core;
using System;
using Inferno.Runtime.Graphics.Text;

namespace Inferno.Runtime.Graphics
{
    /// <summary>
    /// Contains all drawing tools like rectangles, strings etc.
    /// </summary>
    public static class Drawing
    {
        #region Options

        /// <summary>
        /// The current draw color
        /// </summary>
        private static Color _currentColor = Color.Black;

        /// <summary>
        /// The current draw alpha
        /// </summary>
        private static float _alpha = 1f;

        /// <summary>
        /// The current draw font
        /// </summary>
        private static Font _font;

        #endregion

        #region Textures

        #endregion

        #region Options Config

        /// <summary>
        /// Update draw alpha
        /// </summary>
        /// <param name="alpha">The alpha to draw with</param>
        public static void Set_Alpha(float alpha)
        {
            _alpha = alpha;
        }

        /// <summary>
        /// Update draw color
        /// </summary>
        /// <param name="color">The color of which to draw in</param>
        public static void Set_Color(Color color)
        {
            _currentColor = color;
        }

        /// <summary>
        /// Update draw font
        /// </summary>
        /// <param name="font">The font for the drawer to use</param>
        public static void Set_Font(Font font)
        {
            _font = font;
        }

        #endregion

        #region Config

        /// <summary>
        /// Configure the drawer.
        /// This is internally used.
        /// </summary>
        public static void Config()
        {
            
        }

        /// <summary>
        /// Dispose drawer components
        /// This is internally used.
        /// </summary>
        public static void Dispose()
        {
            
        }

        #endregion

        #region Textures

        /// <summary>
        /// Draw an instance
        /// </summary>
        /// <param name="instance">Instance to draw</param>
        public static void Draw_Instance(Instance instance)
        {
            //Draw the sprite using instance data
            Draw_Sprite(instance.Position, instance.Sprite, instance.Depth);
        }

        /// <summary>
        /// Draw a sprite
        /// </summary>
        /// <param name="position">Position to draw at</param>
        /// <param name="sprite">The sprite to draw</param>
        /// <param name="depth">The depth to draw at</param>
        public static void Draw_Sprite(Vector2 position, Sprite sprite, float depth = 0)
        {
            if (sprite != null)
                Draw_Sprite(position, sprite, sprite.SourceRectangle, depth);
        }

        /// <summary>
        /// Draw a sprite
        /// </summary>
        /// <param name="position">The position to draw the sprite</param>
        /// <param name="sprite">The sprite to draw</param>
        /// <param name="sourceRectangle">The source rectangle</param>
        /// <param name="depth">The depth to draw at</param>
        public static void Draw_Sprite(Vector2 position, Sprite sprite, Rectangle sourceRectangle, float depth = 0)
        {
            //Don't try if game isn't initialised
            if (Game.Instance.GraphicsManager == null)
                return;

            if (sprite != null)
                Draw_Raw_Texture(position, sprite.Texture, sourceRectangle, sprite.Rotation, sprite.Origin, 1.0f, depth);
        }

        #endregion

        #region Text

        /// <summary>
        /// Draw text to the screen
        /// </summary>
        /// <param name="position">Position to draw</param>
        /// <param name="text">Text to draw</param>
        /// <param name="depth">Depth to draw at</param>
        public static void Draw_Text(Vector2 position, string text, float depth = 0)
        {
            //Throw error if font is null
            if (_font == null)
                throw new Exception("The Font may not be null.");

            //Draw the text
            Game.Renderer.DrawText(text, position, _font, _currentColor * _alpha);
        }

        #endregion

        #region Raw

        /// <summary>
        /// Draw a raw Texture2D
        /// </summary>
        /// <param name="position">Position to draw</param>
        /// <param name="texture">The texture to draw</param>
        /// <param name="sourceRectangle">The source rectangle of the texture</param>
        /// <param name="rotation">The rotation to draw at</param>
        /// <param name="origin">The origin of the texture</param>
        /// <param name="scale">Scale of the texture</param>
        /// <param name="depth">The depth to draw at</param>
        public static void Draw_Raw_Texture(Vector2 position, Texture2D texture, Rectangle? sourceRectangle = null, double rotation = 0, Vector2? origin = null, float scale = 1f, float depth = 0)
        {
            //Don't try if game isn't initialised
            if (Game.Instance.GraphicsManager == null)
                return;

            //Configure the origin
            var o = new Vector2(0, 0);

            //Set origin if specified
            if (origin != null)
            {
                o.X = origin.Value.X;
                o.Y = origin.Value.Y;
            }

            if (texture != null)
                Game.Renderer.Draw(texture, _currentColor * _alpha, 0f, position, sourceRectangle, o, rotation);
        }

        #endregion
    }
}
