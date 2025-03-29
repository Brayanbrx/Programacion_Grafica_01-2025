// === Clase Objeto.cs ===
using OpenTK.Mathematics;

namespace OpenTK
{
    internal class Objeto
    {
        public Vector3 Position { get; set; }
        public float[] Vertices { get; private set; }
        public int[] Indices { get; private set; }

        public Objeto(Vector3 position)
        {
            Position = position;
            GenerarVertices();
            GenerarIndices();
        }

        private void GenerarVertices()
        {
            float px = Position.X;
            float py = Position.Y;
            float pz = Position.Z;

            Vertices = new float[] {
                // Columna izquierda
                -1.0f+px, 0.0f+py,  0.3f+pz,  -0.7f+px, 0.0f+py,  0.3f+pz,  -0.7f+px, 2.0f+py,  0.3f+pz,  -1.0f+px, 2.0f+py,  0.3f+pz,
                -1.0f+px, 0.0f+py, -0.3f+pz,  -0.7f+px, 0.0f+py, -0.3f+pz,  -0.7f+px, 2.0f+py, -0.3f+pz,  -1.0f+px, 2.0f+py, -0.3f+pz,

                // Columna derecha
                 0.7f+px, 0.0f+py,  0.3f+pz,   1.0f+px, 0.0f+py,  0.3f+pz,   1.0f+px, 2.0f+py,  0.3f+pz,   0.7f+px, 2.0f+py,  0.3f+pz,
                 0.7f+px, 0.0f+py, -0.3f+pz,   1.0f+px, 0.0f+py, -0.3f+pz,   1.0f+px, 2.0f+py, -0.3f+pz,   0.7f+px, 2.0f+py, -0.3f+pz,

                // Base inferior
                -0.7f+px, 0.0f+py,  0.3f+pz,   0.7f+px, 0.0f+py,  0.3f+pz,   0.7f+px, 0.3f+py,  0.3f+pz,  -0.7f+px, 0.3f+py,  0.3f+pz,
                -0.7f+px, 0.0f+py, -0.3f+pz,   0.7f+px, 0.0f+py, -0.3f+pz,   0.7f+px, 0.3f+py, -0.3f+pz,  -0.7f+px, 0.3f+py, -0.3f+pz
            };
        }

        private void GenerarIndices()
        {
            Indices = new int[] {
                // Columna izquierda (6 caras)
                0, 1, 2,  2, 3, 0,
                1, 5, 6,  6, 2, 1,
                5, 4, 7,  7, 6, 5,
                4, 0, 3,  3, 7, 4,
                3, 2, 6,  6, 7, 3,
                0, 1, 5,  5, 4, 0,

                // Columna derecha (6 caras)
                8, 9, 10,  10, 11, 8,
                9, 13, 14,  14, 10, 9,
                13, 12, 15,  15, 14, 13,
                12, 8, 11,  11, 15, 12,
                11, 10, 14,  14, 15, 11,
                8, 9, 13,  13, 12, 8,

                // Base inferior (6 caras)
                16, 17, 18,  18, 19, 16,
                17, 21, 22,  22, 18, 17,
                21, 20, 23,  23, 22, 21,
                20, 16, 19,  19, 23, 20,
                19, 18, 22,  22, 23, 19,
                16, 17, 21,  21, 20, 16
            };
        }
    }
}
