using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Inferno.Content;
using Inferno.Formats.Graphics.Text;

namespace Inferno.Graphics.Text
{
    /// <summary>
    /// A Text Font
    /// </summary>
    public class Font : IDisposable
    {
        /// <summary>
        /// This is the texture containing all the glyphs
        /// </summary>
        public Texture2D Texture;

        public List<Vector4> CharMap;

        /// <summary>
        /// The amount of pixels between each line of text
        /// </summary>
        public int LineHeight;

        /// <summary>
        /// The number of pixels for a space character
        /// </summary>
        public int SpaceSize;

        /// <summary>
        /// Constructor for font loading
        /// </summary>
        internal Font()
        {

        }

        internal Font(Texture2D texture, IEnumerable<Vector4> charMap, int lineHeight, int spaceSize)
        {
            Texture = texture;
            CharMap = charMap.ToList();
            LineHeight = lineHeight;
            SpaceSize = spaceSize;
        }

        /// <summary>
        /// Create an Inferno font from a system font
        /// </summary>
        /// <param name="fontname"></param>
        /// <param name="ptSize"></param>
        /// <param name="antialiasing"></param>
        /// <returns></returns>
        [Obsolete("This will be removed once we fully remove FontBuilder from the core engine.")]
        public static Font CreateFont(string fontname, int ptSize = 12, bool antialiasing = true)
        {
            return FontBuilder.CreateFontFromName(fontname, ptSize, antialiasing);
        }

        /// <summary>
        /// Create an Inferno font from a font file
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="ptSize"></param>
        /// <param name="antialiasing"></param>
        /// <returns></returns>
        public static Font CreateFontFromFile(string filename, int ptSize = 12, bool antialiasing = true)
        {
            return FontBuilder.CreateFontFromFile(filename, ptSize, antialiasing);
        }

        public static Font FromFontFormat(FontFormat fontFormat)
        {
            //Build char map
            var charMap = new List<Vector4>();

            for (var i = 0; i < fontFormat.CharMap.Length; i += 4)
            {
                charMap.Add(new Vector4(fontFormat.CharMap[i], fontFormat.CharMap[i + 1], fontFormat.CharMap[i + 2],
                    fontFormat.CharMap[i + 3]));
            }

            //Build font color data
            var colorData = new Color[fontFormat.Width * fontFormat.Height];

            for (var i = 0; i < fontFormat.TextureData.Length; i++)
            {
                colorData[i] = new Color(fontFormat.TextureData[i]);
            }

            return new Font()
            {
                CharMap = charMap,
                Texture = new Texture2D(fontFormat.Width, fontFormat.Height, colorData),
                LineHeight = fontFormat.LineHeight,
                SpaceSize = fontFormat.SpaceSize
            };
        }

        /// <summary>
        /// Fetches the "source rectangle" for the renderer
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        internal Rectangle GetRectangleForChar(char c)
        {
            var i = (int)c;

            return new Rectangle((int)CharMap[i].X, (int)CharMap[i].Y, (int)CharMap[i].Z, (int)CharMap[i].W);
        }

        /// <summary>
        /// Measure how big a string would be in this font
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public Vector2 MeasureString(string text)
        {
            var width = 0; //Total width
            var height = 0; //Max height

            //Find total width
            var currentWidth = 0;
            foreach (var c in text)
            {
                switch (c)
                {
                    case '\n':
                        height += LineHeight;
                        width += currentWidth;
                        currentWidth = 0;
                        continue;
                    case ' ':
                        width += SpaceSize;
                        continue;
                }
                
                var size = GetRectangleForChar(c);
                currentWidth += size.Width;

                if (size.Height > height)
                    height = size.Height;
            }

            if (currentWidth > width)
                width = currentWidth;

            return new Vector2(width, height);
        }

        public void Dispose()
        {
            Texture?.Dispose();
            Texture = null;
            CharMap = null;
        }

        #region File Stuff

        

        #endregion
    }
}
