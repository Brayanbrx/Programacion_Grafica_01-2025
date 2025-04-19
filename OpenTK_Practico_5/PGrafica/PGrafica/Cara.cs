using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace PGrafica
{
    public class Cara : IDisposable // Reserva Recursos en la GPU que deben ser liberados Dispose
    {
        public List<Vector3> Vertices { get; } = new(); // lista de vertices, puntos 3D
        public List<int>     Indices { get; } = new();  // lista de indices que conectan vertices
        private int _vao, _vbo, _ebo; // Vertex Array, Vertex Buffer y Element Buffer
        private bool _inicializada;
        public Cara(IEnumerable<Vector3> vertices, IEnumerable<int> indices)
        {
            Vertices.AddRange(vertices);
            Indices.AddRange(indices);
        }
        internal void Dibujar(Shader shader)
        {
            if (!_inicializada) InicializarBuffers(); // Si aun no se subio la info al GPU
            GL.BindVertexArray(_vao);                 // Seleccion de Buffers
            GL.DrawElements(PrimitiveType.Triangles,  // Dibujamos los triangulos con los indices
                Indices.Count, DrawElementsType.UnsignedInt, 0);
        }
        public void InicializarBuffers()
        {
            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();
            _ebo = GL.GenBuffer();
            GL.BindVertexArray(_vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer,
                Vertices.Count * 3 * sizeof(float),
                Vertices.ToArray(),
                BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer,
                Indices.Count * sizeof(int),
                Indices.ToArray(),
                BufferUsageHint.StaticDraw);
            _inicializada = true;
        }

        public void Dispose() // Liberar recursos
        {
            if (!_inicializada) return;
            GL.DeleteVertexArray(_vao);
            GL.DeleteBuffer(_vbo);
            GL.DeleteBuffer(_ebo);
            _inicializada = false;
        }
    }
}
