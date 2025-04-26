using System.Text.Json;
using System.Text.Json.Serialization;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace PGrafica
{
    public class Cara : IDisposable
    {
        public List<Vector3> Vertices { get; } = new(); //3D x,y,z
        public List<int> Indices { get; } = new();
        [JsonIgnore] private int _vao, _vbo, _ebo; // id de buffers en GPU confi, vertices, indices
        [JsonIgnore] private bool _inicializada; // flag
        public Cara() { }
        public Cara(IEnumerable<Vector3> v, IEnumerable<int> idx)
        {
            Vertices.AddRange(v); Indices.AddRange(idx);
        }
        public readonly record struct Dto(float[][] vertices, int[] indices); // Data Transfer Object
        public Dto ToDto() => new(Vertices.Select(p => new[] { p.X, p.Y, p.Z }).ToArray(), //Jsonfriendly xd
                                    Indices.ToArray());
        public static Cara FromDto(Dto d)
        {
            var verts = d.vertices.Select(a => new Vector3(a[0], a[1], a[2]));
            var c = new Cara(verts, d.indices); c.InicializarBuffers();
            return c;
        }
        public static Cara FromMeshJson(string path)
        {
            using JsonDocument doc = JsonDocument.Parse(File.ReadAllText(path));
            var root = doc.RootElement;
            if (!root.TryGetProperty("vertices", out var vArr) ||
                !root.TryGetProperty("indices", out var iArr))
                throw new IOException("JSON de malla sin 'vertices' o 'indices'");
            var verts = new Vector3[vArr.GetArrayLength()];
            for (int i = 0; i < verts.Length; i++)
            {
                var v = vArr[i];
                verts[i] = new Vector3(v[0].GetSingle(), v[1].GetSingle(), v[2].GetSingle());
            }
            int[] idx = iArr.EnumerateArray().Select(x => x.GetInt32()).ToArray();
            var cara = new Cara(verts, idx); cara.InicializarBuffers();
            return cara;
        }
        internal void Dibujar(Shader s)
        {
            if (!_inicializada) InicializarBuffers();
            GL.BindVertexArray(_vao);
            GL.DrawElements(PrimitiveType.Triangles, Indices.Count,
                            DrawElementsType.UnsignedInt, 0);
        }
        public void InicializarBuffers()
        {
            if (_inicializada) return;
            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();
            _ebo = GL.GenBuffer();
            GL.BindVertexArray(_vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer,
                          Vertices.Count * 3 * sizeof(float),
                          Vertices.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer,
                          Indices.Count * sizeof(int), Indices.ToArray(),
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

