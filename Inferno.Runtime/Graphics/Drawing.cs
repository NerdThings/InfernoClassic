using Inferno.Runtime.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

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
        private static Color CurrentColor = Color.Black;

        /// <summary>
        /// The current draw alpha
        /// </summary>
        private static float Alpha = 1f;

        /// <summary>
        /// The current circle precision (Number of lines making up a circle)
        /// </summary>
        private static int CirclePrecision = 32;

        /// <summary>
        /// The current draw font
        /// </summary>
        private static SpriteFont Font = null;

        #endregion

        #region Textures

        /// <summary>
        /// A blank texture to be used
        /// </summary>
        private static Texture2D BlankTexture;

        #endregion

        #region Options Config

        /// <summary>
        /// Update draw alpha
        /// </summary>
        /// <param name="alpha">The alpha to draw with</param>
        public static void Set_Alpha(float alpha)
        {
            Alpha = alpha;
        }

        /// <summary>
        /// Update draw color
        /// </summary>
        /// <param name="color">The color of which to draw in</param>
        public static void Set_Color(Color color)
        {
            CurrentColor = color;
        }

        /// <summary>
        /// Update circle precision
        /// </summary>
        /// <param name="precision">The number of lines drawn per circle</param>
        public static void Set_CirclePrecision(int precision)
        {
            CirclePrecision = precision;
        }

        /// <summary>
        /// Update draw font
        /// </summary>
        /// <param name="font">The font for the drawer to use</param>
        public static void Set_Font(SpriteFont font)
        {
            Font = font;
        }

        #endregion

        #region Config

        /// <summary>
        /// Configure the drawer.
        /// This is internally used.
        /// </summary>
        public static void Config()
        {
            BlankTexture = new Texture2D(Game.Graphics, 1, 1);
            BlankTexture.SetData(new[] { Color.White });
        }

        /// <summary>
        /// Dispose drawer components
        /// This is internally used.
        /// </summary>
        /// <param name="disposing"></param>
        public static void Dispose()
        {
            //Dispose of the textures
            BlankTexture.Dispose();
            BlankTexture = null;
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
        /// <param name="Position">Position of the rectangle</param>
        /// <param name="Width">Width of the rectangle</param>
        /// <param name="Height">Height of the rectangle</param>
        /// <param name="outline">Whether or not this is an outlined rectangle</param>
        /// <param name="lwidth">Line width of the outline</param>
        /// <param name="depth">The depth to draw at</param>
        public static void Draw_Rectangle(Vector2 Position, int Width, int Height, bool outline = false, int lwidth = 1, float depth = 0)
        {
            //Don't try if game isn't initialised
            if (Game.Graphics == null)
                return;

            //If we are drawing an outlined rectangle
            if (outline)
            {
                //Build the rectangle
                Rectangle rectangle = new Rectangle((int)Position.X, (int)Position.Y, Width, Height);

                //Build vertex array
                Vector2[] vertex = new Vector2[4];
                vertex[0] = new Vector2(rectangle.Left, rectangle.Top);
                vertex[1] = new Vector2(rectangle.Right, rectangle.Top);
                vertex[2] = new Vector2(rectangle.Right, rectangle.Bottom);
                vertex[3] = new Vector2(rectangle.Left, rectangle.Bottom);

                //Draw vertex array
                Draw_VertexArray(vertex, lwidth, depth);
            }
            else
            {
                //Draw the 1x1 blank texture with a set size and color
                Game.SpriteBatch.Draw(BlankTexture, Position, null, CurrentColor * Alpha,
                0f, Vector2.Zero, new Vector2(Width, Height),
                SpriteEffects.None, depth);

            }
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
            if (Game.Graphics == null)
                return;

            //Make sure the array isn't empty
            if (vertex.Length > 0)
            {
                //Draw each line inside the vertex array
                for (int i = 0; i < vertex.Length - 1; i++)
{
                    Draw_Line(vertex[i], vertex[i + 1], lineWidth, depth);
                }
                //Link back
                Draw_Line(vertex[vertex.Length - 1], vertex[0], lineWidth, depth);
            }
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
            if (Game.Graphics == null)
                return;

            //If this is an outlined circle
            if (outline)
            {
                //Build vertex array
                Vector2[] Vertex = new Vector2[CirclePrecision];

                //Get ready
                double increment = Math.PI * 2.0 / CirclePrecision;
                double theta = 0.0;

                //Build Vertex array content
                for (int i = 0; i < CirclePrecision; i++)
                {
                    Vertex[i] = position + radius * new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
                    theta += increment;
                }

                //Draw array
                Draw_VertexArray(Vertex, lwidth, depth);
            }
            else
            {
                Texture2D texture = new Texture2D(Game.Graphics, radius, radius);
                Color[] colorData = new Color[radius * radius];

                float diam = radius / 2f;
                float diamsq = diam * diam;

                for (int x = 0; x < radius; x++)
                {
                    for (int y = 0; y < radius; y++)
                    {
                        int index = x * radius + y;
                        Vector2 pos = new Vector2(x - diam, y - diam);
                        if (pos.LengthSquared() <= diamsq)
                        {
                            colorData[index] = CurrentColor;
                        }
                        else
                        {
                            colorData[index] = Color.Transparent;
                        }
                    }
                }

                texture.SetData(colorData);

                Game.SpriteBatch.Draw(texture, position, null, Color.White * Alpha, 0f, Vector2.Zero, 1f, SpriteEffects.None, depth);

                texture.Dispose();
                texture = null;
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
            //Don't try if game isn't initialised or if our texture isn't ready
            if (Game.Graphics == null || BlankTexture == null)
                return;

            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            float length = Vector2.Distance(point1, point2);

            Game.SpriteBatch.Draw(BlankTexture, point1, null, CurrentColor*Alpha,
            angle, Vector2.Zero, new Vector2(length, lineWidth),
            SpriteEffects.None, depth);
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
        /// <param name="Position">Position to draw at</param>
        /// <param name="Sprite">The sprite to draw</param>
        /// <param name="depth">The depth to draw at</param>
        public static void Draw_Sprite(Vector2 Position, Sprite Sprite, float depth = 0)
        {
            if (Sprite != null)
                Draw_Sprite(Position, Sprite, Sprite.SourceRectangle, depth);
        }

        /// <summary>
        /// Draw a sprite
        /// </summary>
        /// <param name="Position">The position to draw the sprite</param>
        /// <param name="Sprite">The sprite to draw</param>
        /// <param name="SourceRectangle">The source rectangle</param>
        /// <param name="depth">The depth to draw at</param>
        public static void Draw_Sprite(Vector2 Position, Sprite Sprite, Rectangle SourceRectangle, float depth = 0)
        {
            //Don't try if game isn't initialised
            if (Game.Graphics == null)
                return;

            if (Sprite != null && Position != null)
                Draw_Raw_Texture(Position, Sprite.Texture, SourceRectangle, Sprite.Rotation, Sprite.Origin, 1.0f, depth);
        }

        #endregion

        #region Text

        /// <summary>
        /// Draw text to the screen
        /// </summary>
        /// <param name="Position">Position to draw</param>
        /// <param name="Text">Text to draw</param>
        /// <param name="depth">Depth to draw at</param>
        public static void Draw_Text(Vector2 Position, string Text, float depth = 0)
        {
            //Throw error if font is null
            if (Font == null)
                throw new Exception("The Font may not be null.");

            //Draw the text
            Game.SpriteBatch.DrawString(Font, Text, Position, CurrentColor * Alpha, 0f, Vector2.Zero, 1f, SpriteEffects.None, depth);
        }

        #endregion

        #region Raw

        /// <summary>
        /// Draw a raw Texture2D
        /// </summary>
        /// <param name="Position">Position to draw</param>
        /// <param name="Texture">The texture to draw</param>
        /// <param name="destinationRectangle">The destination rectangle of the texture</param>
        /// <param name="rotation">The rotation to draw at</param>
        /// <param name="origin">The origin of the texture</param>
        /// <param name="scale">Scale of the texture</param>
        /// <param name="depth">The depth to draw at</param>
        public static void Draw_Raw_Texture(Vector2 Position, Texture2D Texture, Rectangle? destinationRectangle = null, float rotation = 0f, Vector2? origin = null, float scale = 1f, float depth = 0)
        {
            //Don't try if game isn't initialised
            if (Game.Graphics == null)
                return;

            //Configure the origin
            Vector2 o = new Vector2(0, 0);

            //Set origin if specified
            if (origin != null)
            {
                o.X = origin.Value.X;
                o.Y = origin.Value.Y;
            }

            //Check everything else is okay, then draw
            if (Position != null && Texture != null)
                Game.SpriteBatch.Draw(Texture, Position, destinationRectangle, Color.White * Alpha, rotation, o, scale, SpriteEffects.None, depth);
        }

        #endregion
    }
}
