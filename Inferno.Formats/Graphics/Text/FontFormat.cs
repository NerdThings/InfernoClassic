using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace Inferno.Formats.Graphics.Text
{
    public class FontFormat
    {
        public int Width;
        public int Height;
        public int SpaceSize;
        public int LineHeight;
        public uint[] TextureData;
        public float[] CharMap;

        public const string Header = "INFERNOFONT";
        public const string Version = "VERSION_1.2";

        /// <summary>
        /// Update a font file from V1.1 to V1.2
        /// </summary>
        /// <param name="stream">Stream to update</param>
        /// <param name="outStream">Output stream</param>
        [Obsolete("This should only be used to upgrade old files then be removed, do not use this permanently.")]
        public static void Update1_1Stream(Stream stream, Stream outStream)
        {
            using (var gzipStream = new GZipStream(stream, CompressionMode.Decompress))
            {
                using (var reader = new BinaryReader(gzipStream))
                {
                    if (reader.ReadString() != Header)
                        throw new Exception("Invalid font file.");

                    if (reader.ReadString() != "VERSION_1.1")
                        throw new Exception("This font file cannot be upgraded.");

                    using (var gzipOutStream = new GZipStream(outStream, CompressionMode.Compress))
                    {
                        using (var writer = new BinaryWriter(gzipOutStream))
                        {
                            writer.Write(Header);
                            writer.Write(Version);

                            writer.Write(reader.ReadInt32()); //Width
                            writer.Write(reader.ReadInt32()); //Height
                            writer.Write(reader.ReadInt32()); //SpaceSize
                            writer.Write(reader.ReadInt32()); //LineHeight

                            //Convert image data
                            var colorCSV = Base64DecodeString(reader.ReadString()).Split(',');
                            var colorByteArray = new List<byte>();

                            foreach (var c in colorCSV)
                            {
                                var color = StringToUint(c);
                                colorByteArray.AddRange(BitConverter.GetBytes(color));
                            }

                            writer.Write(colorByteArray.ToArray().Length);
                            writer.Write(colorByteArray.ToArray());

                            //Convert char map
                            var charMapCSV = Base64DecodeString(reader.ReadString()).Split(',');
                            var charMapArray = new List<float>();

                            foreach (var component in charMapCSV)
                            {
                                var c = StringToFloat(component);
                                charMapArray.Add(c);
                            }

                            var charMap = charMapArray.SelectMany(BitConverter.GetBytes).ToArray();
                            writer.Write(charMap.Length);
                            writer.Write(charMap);
                        }
                    }
                }
            }
        }

        private static string Base64DecodeString(string base64EncodedData)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(base64EncodedData));
        }

        public static FontFormat FromStream(Stream stream)
        {
            var font = new FontFormat();
            using (var gzip = new GZipStream(stream, CompressionMode.Decompress))
            {
                using (var reader = new BinaryReader(gzip))
                {
                    //Get header
                    var header = reader.ReadString();
                    if (header != Header)
                        throw new Exception("This is not a vaild font file.");

                    //Check version number
                    var version = reader.ReadString();
                    if (version != Version)
                        throw new Exception("File is not the correct version.");

                    //Get image dimensions
                    font.Width = reader.ReadInt32();
                    font.Height = reader.ReadInt32();

                    //Read space size and line height
                    font.SpaceSize = reader.ReadInt32();
                    font.LineHeight = reader.ReadInt32();

                    //Get the image data
                    var imageBytes = reader.ReadInt32();
                    var texData = reader.ReadBytes(imageBytes); //byte[] textureData = packedColorArray.SelectMany(BitConverter.GetBytes).ToArray();

                    font.TextureData = new uint[texData.Length / sizeof(uint)];

                    for (var i = 0; i < texData.Length; i += sizeof(uint))
                    {
                        font.TextureData[i / sizeof(uint)] = BitConverter.ToUInt32(texData, i);
                    }

                    //Get the chardata
                    var dataLength = reader.ReadInt32();
                    var data = reader.ReadBytes(dataLength);

                    //Check data length
                    if ((data.Length / sizeof(float)) % 4 != 0)
                        throw new Exception("Data is invalid.");

                    font.CharMap = new float[data.Length / sizeof(float)];

                    for (var i = 0; i < dataLength; i += sizeof(float))
                    {
                        font.CharMap[i / sizeof(float)] = BitConverter.ToSingle(data, i);
                    }
                }
            }

            return font;
        }

        public void WriteOut(Stream stream)
        {
            using (var gzip = new GZipStream(stream, CompressionMode.Compress))
            {
                using (var writer = new BinaryWriter(gzip))
                {
                    //Write headers
                    writer.Write(Header);
                    writer.Write(Version);

                    //Write data
                    writer.Write(Width);
                    writer.Write(Height);
                    writer.Write(SpaceSize);
                    writer.Write(LineHeight);

                    var textureData = TextureData.SelectMany(BitConverter.GetBytes).ToArray();
                    writer.Write(textureData.Length);
                    writer.Write(textureData);

                    var charMap = CharMap.SelectMany(BitConverter.GetBytes).ToArray();
                    writer.Write(charMap.Length);
                    writer.Write(charMap);
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
    }
}
