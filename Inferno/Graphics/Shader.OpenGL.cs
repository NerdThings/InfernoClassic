#if OPENGL

using OpenTK.Graphics.OpenGL;

namespace Inferno.Graphics
{
    internal class OpenGLShader
    {
        public int ShaderId;
        private Shader _shader;
        private string _source;

        public OpenGLShader(Shader shader, string source)
        {
            _shader = shader;
            _source = source;
        }

        public void Compile()
        {
            ShaderId = GL.CreateShader(_shader.Type == ShaderType.Fragment
                ? OpenTK.Graphics.OpenGL.ShaderType.FragmentShader
                : OpenTK.Graphics.OpenGL.ShaderType.VertexShader);

            GL.ShaderSource(ShaderId, _source);

            GL.CompileShader(ShaderId);
        }
    }
}

#endif