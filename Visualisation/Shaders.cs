using System;
using OpenTK.Graphics.OpenGL;

namespace Visualisation
{
    class Shaders
    {
        readonly string VertexShaderSource =
            @"
        #version 330
 
        in vec3 vertex_position;
        in vec3 vertex_color;
        uniform mat4 lookAt;
        uniform mat4 perspective;
        out vec3 color;
 
        void main()
        {
          color = vertex_color;
          gl_Position = perspective * lookAt * vec4(vertex_position, 1.0);
        }";

        readonly string FragmentShaderSource =
            @"
        #version 330
 
        in vec3 color;
        out vec4 outColor;

        void main()
        {
            outColor = vec4(color, 1.0);
        }";

        int VertexShaderHandle, FragmentShaderHandle;
        public int ShaderProgramHandle;

        public Shaders()
        {
            CreateShaders();
        }

        void CreateShaders()
        {
            ShaderProgramHandle = GL.CreateProgram();

            VertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            FragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(VertexShaderHandle, VertexShaderSource);
            GL.ShaderSource(FragmentShaderHandle, FragmentShaderSource);

            GL.CompileShader(VertexShaderHandle);
            GL.CompileShader(FragmentShaderHandle);
            Console.WriteLine(GL.GetShaderInfoLog(VertexShaderHandle));
            Console.WriteLine(GL.GetShaderInfoLog(FragmentShaderHandle));

            GL.AttachShader(ShaderProgramHandle, VertexShaderHandle);
            GL.AttachShader(ShaderProgramHandle, FragmentShaderHandle);

            GL.BindAttribLocation(ShaderProgramHandle, 0, "vertex_position");
            GL.BindAttribLocation(ShaderProgramHandle, 1, "vertex_color");
            Console.WriteLine(GL.GetProgramInfoLog(ShaderProgramHandle));
            GL.LinkProgram(ShaderProgramHandle);
            Console.WriteLine(GL.GetProgramInfoLog(ShaderProgramHandle));
            GL.UseProgram(ShaderProgramHandle);
        }
    }
}