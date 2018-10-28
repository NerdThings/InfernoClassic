#if OPENGL

using OpenTK.Graphics.OpenGL;

namespace Inferno.Graphics
{
    public partial class Shader
    {
        public int ShaderId;

        public void Compile()
        {
            ShaderId = GL.CreateShader(Type == ShaderType.Fragment
                ? OpenTK.Graphics.OpenGL.ShaderType.FragmentShader
                : OpenTK.Graphics.OpenGL.ShaderType.VertexShader);

            GL.ShaderSource(ShaderId, GLSLSource);

            GL.CompileShader(ShaderId);
        }

        public void Dispose()
        {
            //TODO
        }
    }
}

#endif