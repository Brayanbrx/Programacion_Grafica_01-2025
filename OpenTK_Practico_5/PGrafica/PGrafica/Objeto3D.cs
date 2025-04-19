using OpenTK.Mathematics;

namespace PGrafica
{
    public class Objeto3D : IRenderizable
    {
        public List<Parte> Partes     { get; } = new(); 
        public Vector3 Posicion       { get; set; } // Coordenadas o punto de Origen
        public Vector3 RotacionEuler  { get; set; } // Rotacion respecto a X,Y, y Z
        public Vector3 FactorEscala   { get; set; } = Vector3.One; // Tamaño original

        public Objeto3D() : this(Vector3.Zero) { } // Origen por defecto
        public Objeto3D(Vector3 pos, IEnumerable<Parte>? partes = null)
        {
            Posicion = pos;
            if (partes != null) Partes.AddRange(partes);
        }
        public void AgregarParte(Parte p) => Partes.Add(p);
        public void QuitarParte(Parte p)  => Partes.Remove(p);
      /*public void Trasladar(Vector3 d) => Posicion += d;
        public void Rotar(Vector3 axis, float deg) => RotacionEuler += axis * deg;
        public void Escalar(Vector3 f) => FactorEscala *= f; */
        public void Dibujar(Shader shader)
        {
        Matrix4 modelo =
        Matrix4.CreateScale(FactorEscala) *
        Matrix4.CreateRotationX(MathHelper.DegreesToRadians(RotacionEuler.X)) *
        Matrix4.CreateRotationY(MathHelper.DegreesToRadians(RotacionEuler.Y)) *
        Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(RotacionEuler.Z)) *
        Matrix4.CreateTranslation(Posicion);
            shader.EstablecerMatriz4("model", modelo);
            foreach (var parte in Partes) parte.Dibujar(shader);
        }
    }
}
