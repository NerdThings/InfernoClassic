using System;
using System.IO;
using System.IO.Compression;

namespace Inferno.Graphics
{
    public enum ShaderType
    {
        Vertex,
        Fragment
    }

    public partial class Shader : IDisposable
    {
        public ShaderType Type;
        public string GLSLSource;
        public string HLSLSource;

        internal Shader()
        {
        }

        public Shader(ShaderType type, string glslSource = "", string hlslSource = "")
        {
            Type = type;
            GLSLSource = glslSource;
            HLSLSource = hlslSource;
        }

        #region File Format

        public const string Header = "INFERNOSHADER";
        public const string Version = "VERSION_1";

        public void WriteOut(Stream stream)
        {
            using (var gzip = new GZipStream(stream, CompressionMode.Compress))
            {
                using (var writer = new BinaryWriter(gzip))
                {
                    writer.Write(Header);
                    writer.Write(Version);
                    writer.Write((byte) Type);
                    writer.Write(GLSLSource);
                    writer.Write(HLSLSource);
                }
            }
        }

        public static Shader FromStream(Stream stream)
        {
            var shader = new Shader();

            using (var gzip = new GZipStream(stream, CompressionMode.Decompress))
            {
                using (var reader = new BinaryReader(gzip))
                {
                    //Check file header
                    var header = new string(reader.ReadChars(Header.Length)); ;
                    if (header != Header)
                        throw new Exception("Invalid shader file.");

                    //Check file version
                    var version = new string(reader.ReadChars(Version.Length));
                    if (version != Version)
                        throw new Exception("Invalid shader file version.");

                    //Get shader type
                    shader.Type = (ShaderType)reader.ReadByte();

                    //Get glsl source
                    shader.GLSLSource = reader.ReadString();

                    //Get hlsl source
                    shader.HLSLSource = reader.ReadString();
                }
            }

            return shader;
        }

        #endregion
    }
}
