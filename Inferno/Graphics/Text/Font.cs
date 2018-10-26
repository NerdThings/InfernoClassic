using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Inferno.Content;

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

        public const string Header = "INFERNOFONT";
        public const string Version = "VERSION_1.1";

        public static Font FromStream(FileStream stream)
        {
            var file = new Font();
            using (var gzip = new GZipStream(stream, CompressionMode.Decompress))
            {
                using (var reader = new BinaryReader(gzip))
                {
                    //Get header
                    var header = reader.ReadString();
                    if (header != Header)
                        throw new Exception("Data is invalid.");

                    //Check version number
                    var version = reader.ReadString();
                    if (version != Version)
                        throw new Exception("File is of invalid version");

                    //Get image dimensions
                    var width = reader.ReadInt32();
                    var height = reader.ReadInt32();

                    //Read space size and line height
                    file.SpaceSize = reader.ReadInt32();
                    file.LineHeight = reader.ReadInt32();

                    //Get the image data
                    var image = Base64DecodeString(reader.ReadString());

                    //Split data by comma
                    var imageParts = image.Split(',');

                    //Array for colors
                    var colors = new Color[width * height];

                    //For every four integers, add to array
                    for (var i = 0; i < imageParts.Length; i++)
                    {
                        var c = StringToUint(imageParts[i]);

                        colors[i] = new Color(c);
                    }

                    //Create texture
                    file.Texture = new Texture2D(width, height, colors);

                    //Get the chardata
                    var data = Base64DecodeString(reader.ReadString());

                    //Create CharMap
                    file.CharMap = new List<Vector4>();

                    //Split data by comma
                    var dataParts = data.Split(',');

                    //Check data length
                    if (dataParts.Length % 4 != 0)
                        throw new Exception("Data is invalid.");

                    //For every four integers, add to array
                    for (var i = 0; i < dataParts.Length; i += 4)
                    {
                        var x = StringToFloat(dataParts[i]);
                        var y = StringToFloat(dataParts[i + 1]);
                        var z = StringToFloat(dataParts[i + 2]);
                        var w = StringToFloat(dataParts[i + 3]);

                        file.CharMap.Add(new Vector4(x, y, z, w));
                    }
                }
            }

            return file;
        }

        public void WriteOut(Stream stream)
        {
            using (var gzip = new GZipStream(stream, CompressionMode.Compress))
            {
                using (var writer = new BinaryWriter(gzip))
                {
                    //Write file headers
                    writer.Write(Header);
                    writer.Write(Version);

                    //Write texture dimensions
                    writer.Write(Texture.Width);
                    writer.Write(Texture.Height);
                    
                    //Write space size and line height
                    writer.Write(SpaceSize);
                    writer.Write(LineHeight);

                    //Build texture data
                    var colorData = Texture.GetData();
                    var colorArray = string.Join(",", colorData.Select(c=>c.PackedValue));

                    //Write texture data
                    writer.Write(Base64EncodeString(colorArray));

                    //Create char map data
                    var charMapArray = "";
                    foreach (var mapValue in CharMap)
                    {
                        charMapArray += mapValue.X + ",";
                        charMapArray += mapValue.Y + ",";
                        charMapArray += mapValue.Z + ",";
                        charMapArray += mapValue.W + ",";
                    }

                    charMapArray = charMapArray.TrimEnd(',');

                    //Write char map data
                    writer.Write(Base64EncodeString(charMapArray));
                }
            }
        }

        private static float StringToFloat(string str)
        {
            if (float.TryParse(str, out var n))
                return n;
            throw new Exception("Invalid data.");
        }

        private static uint StringToUint(string str)
        {
            if (uint.TryParse(str, out var n))
                return n;
            throw new Exception("Invalid data.");
        }

        private static string Base64EncodeString(string plainText)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
        }

        private static string Base64DecodeString(string base64EncodedData)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(base64EncodedData));
        }

        #endregion
    }
}
