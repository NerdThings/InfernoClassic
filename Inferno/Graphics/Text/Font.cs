using System;

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

        /// <summary>
        /// This is the glyph size map
        /// </summary>
        public Vector2[] _sizeMap;

        /// <summary>
        /// This is the glyph coordinate map.
        /// Why do we have this when we already have widths? To save processing time.
        /// </summary>
        public Vector2[] _coordMap;

        /// <summary>
        /// The amount of pixels between each line of text
        /// </summary>
        public int LineHeight;

        internal Font(Texture2D texture, Vector2[] sizeMap, Vector2[] coordMap, int lineHeight)
        {
            Texture = texture;
            _sizeMap = sizeMap;
            _coordMap = coordMap;
            LineHeight = lineHeight;
        }

        /// <summary>
        /// Create an Inferno font from a system font
        /// </summary>
        /// <param name="fontname"></param>
        /// <param name="ptSize"></param>
        /// <param name="antialiasing"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Fetches the "source rectangle" for the renderer
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        internal Rectangle GetRectangleForChar(char c)
        {
            var i = (int)c;

            return new Rectangle((int)_coordMap[i].X, (int)_coordMap[i].Y, (int)_sizeMap[i].X, (int)_sizeMap[i].Y);
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
            foreach (var c in text)
            {
                var size = GetRectangleForChar(c);
                width += size.Width;

                if (size.Height > height)
                    height = size.Height;
            }

            return new Vector2(width, height);
        }

        public void Dispose()
        {
            Texture.Dispose();
            Texture = null;
            _coordMap = null;
            _sizeMap = null;
        }
    }
}
