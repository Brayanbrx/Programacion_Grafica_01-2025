using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace PGrafica
{
    public class Cara : ITransformable, IDisposable
    {
        public List<Vector3> Vertices { get; } = new(); // lista de vertices
        public List<int> Indices { get; } = new(); // lista de indices

        private int _vao, _vbo, _ebo; //identificadores de buffers
        private bool _inicializada;

        public Cara() { }
        public Cara(IEnumerable<Vector3> vertices, IEnumerable<int> indices)
        {
            Vertices.AddRange(vertices);
            Indices.AddRange(indices);
        }

        // Transformaciones 
        public void Trasladar(Vector3 delta)
        {
            for (int i = 0; i < Vertices.Count; i++) Vertices[i] += delta;
        }
        public void Rotar(Vector3 axis, float deg)
        {
            Matrix4 rot = Matrix4.CreateFromAxisAngle(axis, MathHelper.DegreesToRadians(deg));
            for (int i = 0; i < Vertices.Count; i++)
                Vertices[i] = Vector3.TransformPosition(Vertices[i], rot);
        }
        public void Escalar(Vector3 factor)
        {
            for (int i = 0; i < Vertices.Count; i++) Vertices[i] *= factor;
        }

        internal void Dibujar(Shader shader)
        {
            if (!_inicializada) InicializarBuffers();

            // el VAO ya tiene los atributos; el 'model' llegó desde Objeto3D
            GL.BindVertexArray(_vao);
            GL.DrawElements(PrimitiveType.Triangles, Indices.Count, DrawElementsType.UnsignedInt, 0);
        }

        // Creamos VAO / VBO / EBO y subimos datos a la GPU
        private void InicializarBuffers()
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

        public void Dispose()
        {
            if (!_inicializada) return;
            GL.DeleteVertexArray(_vao);
            GL.DeleteBuffer(_vbo);
            GL.DeleteBuffer(_ebo);
            _inicializada = false;
        }
    }
}
