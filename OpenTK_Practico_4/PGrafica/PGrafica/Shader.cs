using System;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace PGrafica
{
    public class Shader : IDisposable
    {
        public int Handle { get; }

        public Shader(string vert = "shader.vert", string frag = "shader.frag")
        {
            CrearShadersPorDefecto(vert, frag);

            string vSrc = File.ReadAllText(vert);
            string fSrc = File.ReadAllText(frag);

            int v = Compilar(ShaderType.VertexShader, vSrc);
            int f = Compilar(ShaderType.FragmentShader, fSrc);

            Handle = GL.CreateProgram();
            GL.AttachShader(Handle, v);
            GL.AttachShader(Handle, f);
            GL.LinkProgram(Handle);

            GL.DetachShader(Handle, v);
            GL.DetachShader(Handle, f);
            GL.DeleteShader(v);
            GL.DeleteShader(f);
        }

        private static void CrearShadersPorDefecto(string vert, string frag)
        {
            if (!File.Exists(vert))
            {
                File.WriteAllText(vert,
@"#version 330 core
layout(location = 0) in vec3 aPosition;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    gl_Position = projection * view * model * vec4(aPosition, 1.0);
}");
            }

            if (!File.Exists(frag))
            {
                File.WriteAllText(frag,
@"#version 330 core
out vec4 FragColor;
void main() { FragColor = vec4(0.0, 0.4, 1.0, 1.0); }");
            }
        }

        private static int Compilar(ShaderType tipo, string codigo)
        {
            int id = GL.CreateShader(tipo);
            GL.ShaderSource(id, codigo);
            GL.CompileShader(id);
            GL.GetShader(id, ShaderParameter.CompileStatus, out int ok);
            if (ok == 0) throw new Exception(GL.GetShaderInfoLog(id));
            return id;
        }

        public void Usar() => GL.UseProgram(Handle);

        public void EstablecerMatriz4(string nombre, Matrix4 m)
        {
            int loc = GL.GetUniformLocation(Handle, nombre);
            GL.UniformMatrix4(loc, false, ref m);
        }

        public void Dispose() => GL.DeleteProgram(Handle);
    }
}
