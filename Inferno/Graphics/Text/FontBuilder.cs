using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using Inferno.Content;

namespace Inferno.Graphics.Text
{
    /// <summary>
    /// A font bitmap builder.
    /// This will take a System.Drawing.Font and turn it into a Texture2D for use with Inferno.Graphics.Text.Font
    /// </summary>
    internal class FontBuilder
    {

        public static int GlyphsPerLine = 16;
        public static int GlyphLineCount = 16;

        public static Font CreateFontFromName(string name, int fontSize = 12, bool antialiasing = true)
        {
            return CreateFont(name, "", fontSize, antialiasing);
        }

        public static Font CreateFontFromFile(string filename, int fontSize = 12, bool antialiasing = true)
        {
            return CreateFont("", filename, fontSize, antialiasing);
        }

        private static Font CreateFont(string name, string filename = "", int fontSize = 12, bool antialiasing = true)
        {
            System.Drawing.Font font;
            if (!string.IsNullOrWhiteSpace(filename))
            {
                var collection = new PrivateFontCollection();
                collection.AddFontFile(filename);
                var fontFamily = new FontFamily(Path.GetFileNameWithoutExtension(filename), collection);
                font = new System.Drawing.Font(fontFamily, fontSize);
            }
            else
            {
                font = new System.Drawing.Font(name, fontSize);
            }

            //Measure each character to get glyph dimensions

            var sizeMap = new Vector2[GlyphLineCount * GlyphsPerLine];
            var coordMap = new Vector2[GlyphLineCount * GlyphsPerLine]; // Coord map for later

            using (var tmbBitmap = new Bitmap(1, 1))
            {
                using (var g = System.Drawing.Graphics.FromImage(tmbBitmap))
                {
                    var i = 0;
                    for (var p = 0; p < GlyphLineCount; p++)
                    {
                        for (var n = 0; n < GlyphsPerLine; n++)
                        {
                            var c = (char)(n + p * GlyphsPerLine);
                            var size = g.MeasureString(c.ToString(), font, 10, StringFormat.GenericTypographic);

                            sizeMap[i] = new Vector2(size.Width + 1, size.Height + 2);
                            i++;
                        }
                    }
                }
            }

            //Find max width and height
            var maxHeight = (int)sizeMap[0].Y;
            for (var i = 1; i < sizeMap.Length; i++)
            {
                if ((int)sizeMap[i].Y > maxHeight)
                    maxHeight = (int)sizeMap[i].Y;
            }

            var maxWidth = (int)sizeMap[0].X;
            for (var i = 1; i < sizeMap.Length; i++)
            {
                if ((int)sizeMap[i].X > maxWidth)
                    maxWidth = (int)sizeMap[i].X;
            }

            var bitmapWidth = GlyphsPerLine * maxWidth;
            var bitmapHeight = GlyphLineCount * maxHeight + maxHeight;

            var bitmap = new Bitmap(bitmapWidth, bitmapHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (var g = System.Drawing.Graphics.FromImage(bitmap))
            {
                if (antialiasing)
                {
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                }
                else
                {
                    g.SmoothingMode = SmoothingMode.None;
                    g.TextRenderingHint = TextRenderingHint.SingleBitPerPixel;
                }

                //Build bitmap and calculate coordinate map
                var i = 0;
                var heightSoFar = 0;
                for (var p = 0; p < GlyphLineCount; p++)
                {
                    var widthSoFar = 0;
                    for (var n = 0; n < GlyphsPerLine; n++)
                    {
                        var x = widthSoFar;
                        var y = heightSoFar;

                        var c = (char)(n + p * GlyphsPerLine);
                        g.DrawString(c.ToString(), font, Brushes.White,
                            x, y, StringFormat.GenericTypographic);

                        coordMap[i] = new Vector2(x, y);

                        widthSoFar += maxWidth;
                        i++;
                    }

                    if (i < sizeMap.Length)
                        heightSoFar += maxHeight + 1;
                }
            }

            var ret = new Font(ContentLoader.Texture2DFromBitmap(bitmap), sizeMap, coordMap, font.Height, maxWidth/4);
            bitmap.Dispose();
            return ret;
        }
    }
}
