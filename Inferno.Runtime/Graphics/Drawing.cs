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
        /// The current circle precision (Number of lines making up a circle)
        /// </summary>
        private static int _circlePrecision = 32;

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
        /// Update circle precision
        /// </summary>
        /// <param name="precision">The number of lines drawn per circle</param>
        public static void Set_CirclePrecision(int precision)
        {
            _circlePrecision = precision;
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

        #region Shapes

        /// <summary>
        /// Draw a rectangle with a rectangle struct
        /// </summary>
        /// <param name="rect">The shape to draw</param>
        /// <param name="outline">Whether or not this is an outlined rectangle</param>
        /// <param name="lwidth">Width of the outlined rectangle</param>
        /// <param name="depth">Depth to draw at</param>
        public static void Draw_Rectangle(Rectangle rect, bool outline = false, int lwidth = 1, float depth = 0)
        {
            //Send struct data
            Draw_Rectangle(new Vector2(rect.X, rect.Y), rect.Width, rect.Height, outline, lwidth, depth);
        }

        /// <summary>
        /// Draw a rectangle
        /// </summary>
        /// <param name="position">Position of the rectangle</param>
        /// <param name="width">Width of the rectangle</param>
        /// <param name="height">Height of the rectangle</param>
        /// <param name="outline">Whether or not this is an outlined rectangle</param>
        /// <param name="lwidth">Line width of the outline</param>
        /// <param name="depth">The depth to draw at</param>
        public static void Draw_Rectangle(Vector2 position, int width, int height, bool outline = false, int lwidth = 1, float depth = 0)
        {
            //Don't try if game isn't initialised
            if (Game.Instance.GraphicsManager == null)
                return;

            //Build the rectangle
            var rectangle = new Rectangle((int)position.X, (int)position.Y, width, height);

            //TODO: depth
            Game.Renderer.DrawRectangle(rectangle, _currentColor * _alpha, !outline);
        }

        /// <summary>
        /// Draw a vertex array
        /// </summary>
        /// <param name="vertex">Vertex array to draw</param>
        /// <param name="lineWidth">Width of each line</param>
        /// <param name="depth">Depth to draw at</param>
        public static void Draw_VertexArray(Vector2[] vertex, int lineWidth, float depth)
        {
            //Don't try if game isn't initialised
            if (Game.Instance.GraphicsManager == null)
                return;

            //Make sure the array isn't empty
            if (vertex.Length <= 0) return;
            //Draw each line inside the vertex array
            for (var i = 0; i < vertex.Length - 1; i++)
            {
                Draw_Line(vertex[i], vertex[i + 1], lineWidth, depth);
            }
            //Link back
            Draw_Line(vertex[vertex.Length - 1], vertex[0], lineWidth, depth);
        }

        /// <summary>
        /// Draw a circle
        /// </summary>
        /// <param name="position">Position of the circle</param>
        /// <param name="radius">The radius of the circle</param>
        /// <param name="outline">Whether or not this is a circle outline</param>
        /// <param name="lwidth">The thickness of the line for the outline</param>
        /// <param name="depth">The depth to draw at</param>
        public static void Draw_Circle(Vector2 position, int radius, bool outline = false, int lwidth = 1, float depth = 0)
        {
            //Don't try if game isn't initialised
            if (Game.Instance.GraphicsManager == null)
                return;

            //If this is an outlined circle
            if (outline)
            {
                //Build vertex array
                var vertex = new Vector2[_circlePrecision];

                //Get ready
                var increment = Math.PI * 2.0 / _circlePrecision;
                var theta = 0.0;

                //Build Vertex array content
                for (var i = 0; i < _circlePrecision; i++)
                {
                    vertex[i] = position + radius * new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta)); //TODO: Test that this still works
                    theta += increment;
                }

                //Draw array
                Draw_VertexArray(vertex, lwidth, depth);
            }
            else
            {
                Game.Renderer.DrawCircle(position, radius, _currentColor * _alpha);
            }
        }        

        /// <summary>
        /// Draw a line
        /// </summary>
        /// <param name="point1">Start point</param>
        /// <param name="point2">End point</param>
        /// <param name="lineWidth">Width of the line</param>
        /// <param name="depth">Depth to draw the line at</param>
        public static void Draw_Line(Vector2 point1, Vector2 point2, int lineWidth = 1, float depth = 0)
        {
            //Don't try if game isn't initialised
            if (Game.Instance.GraphicsManager == null)
                return;

            //TODO: lineWidth and depth
            Game.Renderer.DrawLine(point1, point2, _currentColor * _alpha);
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

            var destRect = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);

            if (texture != null)
                Game.Renderer.Draw(texture, _currentColor * _alpha, 0f, destRect, sourceRectangle, o, rotation);
        }

        #endregion
    }
}
