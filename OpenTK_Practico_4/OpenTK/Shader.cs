using System;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace OpenTK
{
    /// <summary>
    /// Representa un shader en OpenGL, se compila a partir de archivos de vertice y fragmento :B
    /// </summary>
    public class Shader
    {
        /// <summary>
        /// Identificador del shader en OpenGL
        /// </summary>
        public int Handle { get; private set; }

        /// <summary>
        /// Carga, compila y enlaza los shaders de vertices y fragmentos desde mis rutas
        /// Vertex Shader: Procesa cada vertice y determina su posicion en pantalla
        /// Fragment Shader: Procesa el color de cada pixel en pantalla
        /// </summary>
        /// <param name="vertexPath">Ruta del shader de vertices</param>
        /// <param name="fragmentPath">Ruta del shader de fragmentos</param>
        public Shader(string vertexPath, string fragmentPath)
        {
            // Leer el codigo fuente de los shaders desde los archivos
            string vertexShaderSource = File.ReadAllText(vertexPath);
            string fragmentShaderSource = File.ReadAllText(fragmentPath);

            // Crea y compila el shader de vertices
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSource);
            GL.CompileShader(vertexShader);
            CheckCompileErrors(vertexShader, "VERTEX");

            // Crea y compila el shader de fragmentos
            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderSource);
            GL.CompileShader(fragmentShader);
            CheckCompileErrors(fragmentShader, "FRAGMENT");

            // Crea y enlazamos el programa de shader
            Handle = GL.CreateProgram();
            GL.AttachShader(Handle, vertexShader);
            GL.AttachShader(Handle, fragmentShader);
            GL.LinkProgram(Handle);
            CheckCompileErrors(Handle, "PROGRAM");

            // Limpiamos los shaders individuales despues de enlazarlos
            GL.DetachShader(Handle, vertexShader);
            GL.DetachShader(Handle, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }

        /// <summary>
        /// Activa el shader para ser utilizado en la renderizacion
        /// </summary>
        public void Use()
        {
            GL.UseProgram(Handle);
        }

        /// <summary>
        /// Establece una matriz 4x4 en el shader como una variable uniforme
        /// </summary>
        /// <param name="name"> Nombre de la variable uniforme en el shader</param>
        /// <param name="matrix">Matriz 4x4 que se enviara al shader</param>
        public void SetMatrix4(string name, Matrix4 matrix)
        {
            int location = GL.GetUniformLocation(Handle, name);
            GL.UniformMatrix4(location, false, ref matrix);
        }

        /// <summary>
        /// Comprueba si hubo errores en compilacion o enlace del shader
        /// </summary>
        /// <param name="shader">Identificador del shader</param>
        /// <param name="type">Tipo de objeto a comprobar: "VERTEX", "FRAGMENT" o "PROGRAM"</param>
        private void CheckCompileErrors(int shader, string type)
        {
            if (type != "PROGRAM")
            {
                GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
                if (success == 0)
                {
                    string infoLog = GL.GetShaderInfoLog(shader);
                    Console.WriteLine($"ERROR::SHADER_COMPILATION_ERROR of type: {type}\n{infoLog}");
                }
            }
            else
            {
                GL.GetProgram(shader, GetProgramParameterName.LinkStatus, out int success);
                if (success == 0)
                {
                    string infoLog = GL.GetProgramInfoLog(shader);
                    Console.WriteLine($"ERROR::PROGRAM_LINKING_ERROR\n{infoLog}");
                }
            }
        }

        /// <summary>
        /// libera los recursos asociados con el shader eliminando el programa de OpenGL
        /// </summary>
        public void Dispose()
        {
            GL.DeleteProgram(Handle);
        }
    }
}
