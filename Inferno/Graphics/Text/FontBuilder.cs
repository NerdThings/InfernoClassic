#if DESKTOP

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using Inferno.Content;

namespace Inferno.Graphics.Text
{
    /// <summary>
    /// A font bitmap builder.
    /// This will take a System.Drawing.Font and turn it into a Texture2D for use with Inferno.Graphics.Text.Font.
    /// This is only available on desktop, for testing purposes only.
    /// </summary>
    public class FontBuilder
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

        public static Tuple<Bitmap, Vector4[], int, int> CreateFontStuff(string name, string filename = "", int fontSize = 12, bool antialiasing = true)
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

            var charMap = new Vector4[GlyphLineCount * GlyphsPerLine];

            using (var tmbBitmap = new Bitmap(1, 1))
            {
                using (var g = System.Drawing.Graphics.FromImage(tmbBitmap))
                {
                    var i = 0;
                    for (var p = 0; p < GlyphLineCount; p++)
                    {
                        for (var n = 0; n < GlyphsPerLine; n++)
                        {
                            var c = (char) (n + p * GlyphsPerLine);
                            var size = g.MeasureString(c.ToString(), font, 10, StringFormat.GenericTypographic);

                            charMap[i] = new Vector4(0, 0, size.Width + 1, size.Height + 2);
                            i++;
                        }
                    }
                }
            }

            //Find max width and height
            var maxHeight = (int) charMap[0].W;
            for (var i = 1; i < charMap.Length; i++)
            {
                if ((int) charMap[i].W > maxHeight)
                    maxHeight = (int) charMap[i].W;
            }

            var maxWidth = (int) charMap[0].Z;
            for (var i = 1; i < charMap.Length; i++)
            {
                if ((int) charMap[i].Z > maxWidth)
                    maxWidth = (int) charMap[i].Z;
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

                        var c = (char) (n + p * GlyphsPerLine);
                        g.DrawString(c.ToString(), font, Brushes.White,
                            x, y, StringFormat.GenericTypographic);

                        charMap[i].X = x;
                        charMap[i].Y = y;

                        widthSoFar += maxWidth;
                        i++;
                    }

                    if (i < charMap.Length)
                        heightSoFar += maxHeight + 1;
                }
            }

            return new Tuple<Bitmap, Vector4[], int, int>(bitmap, charMap, font.Height, maxWidth);
        }

        private static Font CreateFont(string name, string filename = "", int fontSize = 12, bool antialiasing = true)
        {
            var dat = CreateFontStuff(name, filename, fontSize, antialiasing);

            var ret = new Font(ContentLoader.Texture2DFromBitmap(dat.Item1), dat.Item2, dat.Item3, dat.Item4/4);
            dat.Item1.Dispose();
            return ret;
        }
    }
}

#endif