
namespace PGrafica
{
    public class Parte
    {
        public List<Cara> Caras { get; } = new();
        public Parte() { }
        public Parte(IEnumerable<Cara> cs) => Caras.AddRange(cs);
        public Parte(Cara c) => Caras.Add(c);
        public void AgregarCara(Cara c) => Caras.Add(c);
        public void QuitarCara(Cara c) => Caras.Remove(c);
        public void Dibujar(Shader s)
        { foreach (var c in Caras) c.Dibujar(s); }
        public readonly record struct Dto(List<Cara.Dto> caras);
        public Dto ToDto() => new(Caras.Select(c => c.ToDto()).ToList());
        public static Parte FromDto(Dto d)
        {
            var p = new Parte();
            foreach (var c in d.caras) p.AgregarCara(Cara.FromDto(c));
            return p;
        }
    }
}

