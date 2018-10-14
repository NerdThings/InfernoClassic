using System;
using System.Collections.Generic;
using System.Text;

namespace Inferno.Graphics
{
    public enum ShaderType
    {
        Fragment, //Pixel Shader in DX?
        Vertex
    }

    public enum ShaderLanguage
    {
        /// <summary>
        /// GLSL Shader Language (OpenGL)
        /// </summary>
        GLSL,

        /// <summary>
        /// HLSL Shader Language (Direct X)
        /// </summary>
        HLSL
    }

    public class Shader
    {
        internal ShaderType Type;
        internal OpenGLShader OpenGLShader;

        public Shader(ShaderType type)
        {
            Type = type;
        }

        public void SetSource(ShaderLanguage language, string source)
        {
            switch (language)
            {
                case ShaderLanguage.GLSL:
                {
                    OpenGLShader = new OpenGLShader(this, source);
                    break;
                }
                default:
                    throw new Exception("Unsupported Shader Language Provided.");
            }
        }

        public void Compile()
        {
            switch (GraphicsDevice.PlatformType)
            {
                case PlatformType.OpenGL:
                {
                    OpenGLShader.Compile();
                    break;
                }
                default:
                    throw new Exception("The current platform type does not support compiling shaders.");
            }
        }
    }
}
