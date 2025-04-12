using System.Collections.Generic;
using OpenTK.Mathematics;

namespace PGrafica
{
    public class Parte
    {
        public List<Cara> Caras { get; } = new();
        public Parte() { }
        public Parte(IEnumerable<Cara> caras) => Caras.AddRange(caras);

        public void AgregarCara(Cara c) => Caras.Add(c);
        public void QuitarCara(Cara c) => Caras.Remove(c);

        internal void Dibujar(Shader shader)
        {
            foreach (var c in Caras) c.Dibujar(shader);
        }
    }
}
