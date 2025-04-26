
using OpenTK.Mathematics;

namespace PGrafica
{
    public class Parte
    {
        public List<Cara> Caras { get; } = new();
        public Vector3 Posicion { get; set; } = Vector3.Zero;
        public Vector3 RotacionEuler { get; set; } = Vector3.Zero;
        public Vector3 FactorEscala { get; set; } = Vector3.One;
        public Parte() { }
        public Parte(IEnumerable<Cara> cs) => Caras.AddRange(cs);
        public Parte(Cara c) => Caras.Add(c);
        public void AgregarCara(Cara c) => Caras.Add(c);
        public void QuitarCara(Cara c) => Caras.Remove(c);
        public void Dibujar(Shader s)
        { foreach (var c in Caras) c.Dibujar(s); }
        public readonly record struct Dto(
            float[]? pos, float[]? rot, float[]? scl,
            List<Cara.Dto> caras
        );
        public Dto ToDto() => new(
            new[] { Posicion.X, Posicion.Y, Posicion.Z },
            new[] { RotacionEuler.X, RotacionEuler.Y, RotacionEuler.Z },
            new[] { FactorEscala.X, FactorEscala.Y, FactorEscala.Z },
            Caras.Select(c => c.ToDto()).ToList()
        );
        public static Parte FromDto(Dto d)
        {
            var p = new Parte
            {
                Posicion = d.pos is { Length: 3 } ? new Vector3(d.pos[0], d.pos[1], d.pos[2])
                                                       : Vector3.Zero,
                RotacionEuler = d.rot is { Length: 3 } ? new Vector3(d.rot[0], d.rot[1], d.rot[2])
                                                       : Vector3.Zero,
                FactorEscala = d.scl is { Length: 3 } ? new Vector3(d.scl[0], d.scl[1], d.scl[2])
                                                       : Vector3.One
            };
            if (d.caras is not null)
                foreach (var cDto in d.caras)
                    p.AgregarCara(Cara.FromDto(cDto));
            return p;
        }

    }
}

