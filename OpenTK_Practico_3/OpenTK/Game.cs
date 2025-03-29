using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.IO;

namespace OpenTK
{
    public class Game : GameWindow
    {
        private int vao, vbo, ebo;
        private Shader shader = null!;
        private Objeto objeto1 = null!;
        private Objeto objeto2 = null!;
        private Objeto objeto3 = null!;

        public Game(int width, int height, string title)
            : base(GameWindowSettings.Default, new NativeWindowSettings()
            {
                ClientSize = new Vector2i(width, height),
                Title = title
            })
        { }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0.85f, 0.85f, 0.85f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            CrearShaders();

            // Instancias en diferentes posiciones
            objeto1 = new Objeto(new Vector3(0.0f, 0.0f, 0.0f));
            objeto2 = new Objeto(new Vector3(3.0f, 1.0f, 0.0f));
            objeto3 = new Objeto(new Vector3(-3.0f, 1.0f, 0.0f));

            // Buffers
            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();
            ebo = GL.GenBuffer();

            GL.BindVertexArray(vao);

            // Índices iguales para todos
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, objeto1.Indices.Length * sizeof(int), objeto1.Indices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // Shader
            shader = new Shader("shader.vert", "shader.frag");
            shader.Use();

            // MATRICES de cámara
            Matrix4 view = Matrix4.LookAt(new Vector3(8f, 6f, 10f), new Vector3(0, 0, 0), Vector3.UnitY);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), Size.X / (float)Size.Y, 0.1f, 100f);

            shader.SetMatrix4("view", view);
            shader.SetMatrix4("projection", projection);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            shader.Use();
            GL.BindVertexArray(vao);

            RenderObjeto(objeto1);
            RenderObjeto(objeto2);
            RenderObjeto(objeto3);

            SwapBuffers();
        }

        private void RenderObjeto(Objeto obj)
        {
            // Enviar vértices del objeto actual
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, obj.Vertices.Length * sizeof(float), obj.Vertices, BufferUsageHint.DynamicDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            Matrix4 model = Matrix4.Identity;
            shader.SetMatrix4("model", model);

            GL.DrawElements(PrimitiveType.Triangles, obj.Indices.Length, DrawElementsType.UnsignedInt, 0);
        }



        private void CrearShaders()
        {
            if (!File.Exists("shader.vert"))
            {
                File.WriteAllText("shader.vert", @"
#version 330 core
layout(location = 0) in vec3 aPosition;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    gl_Position = projection * view * model * vec4(aPosition, 1.0);
}
");
            }

            if (!File.Exists("shader.frag"))
            {
                File.WriteAllText("shader.frag", @"
#version 330 core
out vec4 FragColor;

void main()
{
    FragColor = vec4(0.0, 0.4, 1.0, 1.0); // Azul
}
");
            }
        }




        protected override void OnUnload()
        {
            base.OnUnload();
            GL.DeleteVertexArray(vao);
            GL.DeleteBuffer(vbo);
            GL.DeleteBuffer(ebo);
            shader.Dispose();
        }
    }
}
