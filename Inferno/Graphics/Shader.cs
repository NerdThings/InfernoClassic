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
#if DESKTOP
        internal OpenGLShader OpenGLShader;
#endif

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
#if DESKTOP
                    OpenGLShader = new OpenGLShader(this, source);
#endif
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
#if DESKTOP
                    OpenGLShader.Compile();
#endif
                    break;
                }
                default:
                    throw new Exception("The current platform type does not support compiling shaders.");
            }
        }
    }
}
