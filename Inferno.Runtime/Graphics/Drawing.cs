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

        private static Color CurrentColor = Color.Black;
        private static float Alpha = 1f;
        private static int CirclePrecision = 32;
        private static SpriteFont Font = null;

        #endregion

        #region Textures

        private static Texture2D BlankTexture;

        #endregion

        #region Options Config

        public static void Set_Alpha(float alpha)
        {
            Alpha = alpha;
        }

        public static void Set_Color(Color color)
        {
            CurrentColor = color;
        }

        public static void Set_CirclePrecision(int precision)
        {
            CirclePrecision = precision;
        }

        public static void Set_Font(SpriteFont font)
        {
            Font = font;
        }

        #endregion

        #region Config

        public static void Config()
        {
            Console.WriteLine("Config Drawer");
            BlankTexture = new Texture2D(Game.Graphics, 1, 1);
            BlankTexture.SetData(new[] { Color.White });
        }

        #endregion

        #region Shapes

        public static void Draw_Rectangle(Rectangle rect, bool outline = false, int lwidth = 1, float depth = 0)
        {
            Draw_Rectangle(new Vector2(rect.X, rect.Y), rect.Width, rect.Height, outline, lwidth, depth);
        }

        public static void Draw_Rectangle(Vector2 Position, int Width, int Height, bool outline = false, int lwidth = 1, float depth = 0)
        {
            if (Game.Graphics == null)
                return;

            if (outline)
            {
                Rectangle rectangle = new Rectangle((int)Position.X, (int)Position.Y, Width, Height);

                Vector2[] vertex = new Vector2[4];
                vertex[0] = new Vector2(rectangle.Left, rectangle.Top);
                vertex[1] = new Vector2(rectangle.Right, rectangle.Top);
                vertex[2] = new Vector2(rectangle.Right, rectangle.Bottom);
                vertex[3] = new Vector2(rectangle.Left, rectangle.Bottom);

                Draw_VertexArray(vertex, lwidth, depth);

                vertex = null;
            }
            else
            {
                Game.SpriteBatch.Draw(BlankTexture, Position, null, CurrentColor * Alpha,
                0f, Vector2.Zero, new Vector2(Width, Height),
                SpriteEffects.None, depth);

            }
        }

        public static void Draw_VertexArray(Vector2[] vertex, int lineWidth, float depth)
        {
            if (Game.Graphics == null)
                return;

            if (vertex.Length > 0)
            {
                for (int i = 0; i < vertex.Length - 1; i++)
{
                    Draw_Line(vertex[i], vertex[i + 1], lineWidth, depth);
                }
                Draw_Line(vertex[vertex.Length - 1], vertex[0], lineWidth, depth);
            }

            vertex = null;
        }

        public static void Draw_Circle(Vector2 position, int radius, bool outline = false, int lwidth = 1, float depth = 0)
        {
            if (Game.Graphics == null)
                return;

            if (outline)
            {
                Vector2[] Vertex = new Vector2[CirclePrecision];

                double increment = Math.PI * 2.0 / CirclePrecision;
                double theta = 0.0;

                for (int i = 0; i < CirclePrecision; i++)
                {
                    Vertex[i] = position + radius * new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
                    theta += increment;
                }

                Draw_VertexArray(Vertex, lwidth, depth);

                Vertex = null;
                GC.Collect();
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

                texture = null;
            }
        }        

        public static void Draw_Line(Vector2 point1, Vector2 point2, int lineWidth = 1, float depth = 0)
        {
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

        public static void Draw_Instance(Instance instance)
        {
            if (Game.Graphics == null)
                return;

            Draw_Sprite(instance.Position, instance.Sprite, instance.Depth);
        }

        public static void Draw_Sprite(Vector2 Position, Sprite Sprite, float depth = 0)
        {
            if (Game.Graphics == null)
                return;

            if (Sprite != null && Position != null)
                Game.SpriteBatch.Draw(Sprite.Texture, Position, Sprite.SourceRectangle, Color.White * Alpha, Sprite.Rotation, Sprite.Origin, 1.0f, SpriteEffects.None, depth);
        }

        #endregion

        #region Text

        public static void Draw_Text(Vector2 Position, string Text, float depth = 0)
        {
            if (Font == null)
                return;

            Game.SpriteBatch.DrawString(Font, Text, Position, CurrentColor * Alpha, 0f, Vector2.Zero, 1f, SpriteEffects.None, depth);
        }

        #endregion

        #region Raw

        public static void Draw_Raw_Texture(Vector2 Position, Texture2D Texture, float depth)
        {
            if (Game.Graphics == null)
                return;

            if (Position != null && Texture != null)
                Game.SpriteBatch.Draw(Texture, Position, null, Color.White * Alpha, 0f, Vector2.Zero, 1f, SpriteEffects.None, depth);
        }

        #endregion
    }
}
