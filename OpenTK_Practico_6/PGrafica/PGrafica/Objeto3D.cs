using OpenTK.Mathematics;

namespace PGrafica
{
    public class Objeto3D
    {
        public List<Parte> Partes { get; } = new();
        public Vector3 Posicion { get; set; } = Vector3.Zero;
        public Vector3 RotacionEuler { get; set; } = Vector3.Zero;
        public Vector3 FactorEscala { get; set; } = Vector3.One;
        public void AgregarParte(Parte p) => Partes.Add(p);
        public void QuitarParte(Parte p) => Partes.Remove(p);
        public Objeto3D() : this(Vector3.Zero) { }
        public Objeto3D(Vector3 pos, IEnumerable<Parte>? partes = null)
        {
            Posicion = pos;
            if (partes != null) Partes.AddRange(partes);
        }
        public readonly record struct Dto(float[] pos, float[] rot, float[] scl,
                                          List<Parte.Dto> partes);
        public Dto ToDto() => new(
            new[] { Posicion.X, Posicion.Y, Posicion.Z },
            new[] { RotacionEuler.X, RotacionEuler.Y, RotacionEuler.Z },
            new[] { FactorEscala.X, FactorEscala.Y, FactorEscala.Z },
            Partes.Select(p => p.ToDto()).ToList()
        );
        public static Objeto3D FromDto(Dto d)
        {
            var o = new Objeto3D
            {
                Posicion = new Vector3(d.pos[0], d.pos[1], d.pos[2]),
                RotacionEuler = new Vector3(d.rot[0], d.rot[1], d.rot[2]),
                FactorEscala = new Vector3(d.scl[0], d.scl[1], d.scl[2])
            };
            var partes = d.partes ?? new List<Parte.Dto>();
            foreach (var pDto in partes)
                o.AgregarParte(Parte.FromDto(pDto));
            return o;
        }
        public void Dibujar(Shader s, Matrix4 parentModel)
        {
            /* Matriz del objeto */
            Matrix4 objModel =
                Matrix4.CreateScale(FactorEscala) *
                Matrix4.CreateRotationX(MathHelper.DegreesToRadians(RotacionEuler.X)) *
                Matrix4.CreateRotationY(MathHelper.DegreesToRadians(RotacionEuler.Y)) *
                Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(RotacionEuler.Z)) *
                Matrix4.CreateTranslation(Posicion);

            /* Cada parte aplica su propia transformación */
            foreach (var parte in Partes)
            {
                Matrix4 parteModel =
                    Matrix4.CreateScale(parte.FactorEscala) *
                    Matrix4.CreateRotationX(MathHelper.DegreesToRadians(parte.RotacionEuler.X)) *
                    Matrix4.CreateRotationY(MathHelper.DegreesToRadians(parte.RotacionEuler.Y)) *
                    Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(parte.RotacionEuler.Z)) *
                    Matrix4.CreateTranslation(parte.Posicion);

                s.EstablecerMatriz4("model", parentModel * objModel * parteModel);
                parte.Dibujar(s);
            }
        }
    }
}

