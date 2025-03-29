using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using System;

namespace OpenTK
{
    /// <summary>
    /// Representa una ventana de juego que renderiza una figura usando OpenGL
    /// OpenTK para la gestion de la ventana y la representacion grafica
    /// </summary>
    public class Game : GameWindow
    {
        /// <summary>
        /// Identificador del VAO, encapsula los estados de los buffers de vertices
        /// </summary>
        private int VertexArrayObject;

        /// <summary>
        /// Identificador del VBO, almacena los datos de los vertices
        /// </summary>
        private int VertexBufferObject;

        /// <summary>
        /// Identificador del EBO, almacena los indices que indican como conectar los vertices
        /// </summary>
        private int ElementBufferObject;

        /// <summary>
        /// Instancia del Shader, usado para renderizar la figura
        /// </summary>
        private Shader shader;

        /// <summary>
        /// Posicion en donde se colocara la figura en forma de U (x,y,z)
        /// </summary>
        private Vector3 uPosition = new Vector3(2.0f, 0.0f, 2.0f);

        /// <summary>
        /// Lista de coordenadas de los vertices
        /// </summary>
        private float[] vertices = {
            // Columna izquierda
            -1.0f, 0.0f,  0.3f,  -0.7f, 0.0f,  0.3f,  -0.7f, 2.0f,  0.3f,  -1.0f, 2.0f,  0.3f,
            -1.0f, 0.0f, -0.3f,  -0.7f, 0.0f, -0.3f,  -0.7f, 2.0f, -0.3f,  -1.0f, 2.0f, -0.3f,

            // Columna derecha
             0.7f, 0.0f,  0.3f,   1.0f, 0.0f,  0.3f,   1.0f, 2.0f,  0.3f,   0.7f, 2.0f,  0.3f,
             0.7f, 0.0f, -0.3f,   1.0f, 0.0f, -0.3f,   1.0f, 2.0f, -0.3f,   0.7f, 2.0f, -0.3f,

            // Base inferior
            -0.7f, 0.0f,  0.3f,   0.7f, 0.0f,  0.3f,   0.7f, 0.3f,  0.3f,  -0.7f, 0.3f,  0.3f,
            -0.7f, 0.0f, -0.3f,   0.7f, 0.0f, -0.3f,   0.7f, 0.3f, -0.3f,  -0.7f, 0.3f, -0.3f
        };

        /// <summary>
        /// Lista de indices que definen como se conectan los vertices, se usa con el EBO
        /// </summary>
        private int[] indices = {
            // Columna izquierda (6 caras)
            0, 1, 2,  2, 3, 0, // cara frontal
            1, 5, 6,  6, 2, 1, // lateral derecho
            5, 4, 7,  7, 6, 5, // cara trasera
            4, 0, 3,  3, 7, 4, // lateral izquierdo
            3, 2, 6,  6, 7, 3, // tapa superior
            0, 1, 5,  5, 4, 0, // base inferior

            // Columna derecha (6 caras)
            8, 9, 10,  10, 11, 8, // cara frontal
            9, 13, 14,  14, 10, 9,
            13, 12, 15,  15, 14, 13,
            12, 8, 11,  11, 15, 12,
            11, 10, 14,  14, 15, 11,
            8, 9, 13,  13, 12, 8,

            // Base inferior (6 caras)
            16, 17, 18,  18, 19, 16, // cara frontal
            17, 21, 22,  22, 18, 17, // lateral derecho
            21, 20, 23,  23, 22, 21, // cara trasera
            20, 16, 19,  19, 23, 20, // lateral izquierdo
            19, 18, 22,  22, 23, 19, // tapa superior
            16, 17, 21,  21, 20, 16  // base inferior
        };

        /// <summary>
        /// Constructor. Inicializa la ventana con tamaño y titulo
        /// </summary>
        /// <param name="width">Ancho de ventana</param>
        /// <param name="height">Alto de ventana</param>
        /// <param name="title">Titulo de ventana</param>
        public Game(int width, int height, string title)
            : base(GameWindowSettings.Default, new NativeWindowSettings()
            {
                ClientSize = new Vector2i(width, height),
                Title = title
            })
        {
            shader = new Shader("shader.vert", "shader.frag");
        }

        /// <summary>
        /// Se ejecuta una vez cuando se carga la ventana.
        /// </summary>
        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(0.85f, 0.85f, 0.85f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            CreateBuffers();
            shader.Use();
            SetMatrices();
        }

        /// <summary>
        /// Se ejecuta en cada frame, para renderizar la escena.
        /// Limpia los buffers, utiliza el shader y dibuja los elementos
        /// </summary>
        /// <param name="e">Informacion del frame</param>
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            shader.Use();
            GL.BindVertexArray(VertexArrayObject);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            SwapBuffers();
        }

        /// <summary>
        /// Libera los recursos usados por OpenGL, cuando se cierra la ventana
        /// </summary>
        protected override void OnUnload()
        {
            base.OnUnload();
            GL.DeleteVertexArray(VertexArrayObject);
            GL.DeleteBuffer(VertexBufferObject);
            GL.DeleteBuffer(ElementBufferObject);
            shader.Dispose();
        }

        /// <summary>
        /// Crea los buffers VAO, VBO y EBO, configura los atributos de vertices
        /// </summary>
        private void CreateBuffers()
        {
            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);

            VertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            ElementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(int), indices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
        }

        /// <summary>
        /// Configura las matrices de modelo, vista y proyeccion.
        /// Son necesarias para transformar el objeto en el espacio 3D
        /// </summary>
        private void SetMatrices()
        {
            Matrix4 model = Matrix4.CreateTranslation(uPosition);
            Matrix4 view = Matrix4.LookAt(new Vector3(3.5f, 3.5f, 6.0f), Vector3.Zero, Vector3.UnitY);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), Size.X / (float)Size.Y, 0.1f, 100.0f);

            shader.SetMatrix4("model", model);
            shader.SetMatrix4("view", view);
            shader.SetMatrix4("projection", projection);
        }
    }
}
