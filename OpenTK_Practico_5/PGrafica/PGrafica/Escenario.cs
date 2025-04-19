using OpenTK.Mathematics; // Contiene Vector3, Matrix4 y otros

namespace PGrafica
{
    public class Escenario : IRenderizable
    {
        public List<Objeto3D> Objetos { get; } = new();
        public Objeto3D CrearObjetoEn(Vector3 pos) //posicion especifica en el escenario
        {
            var o = new Objeto3D(pos);
            Objetos.Add(o);
            return o;
        }
        public void AgregarObjeto(Objeto3D o) => Objetos.Add(o);
        public bool QuitarObjeto(int idx)
        {
            if (idx < 0 || idx >= Objetos.Count) return false;
            Objetos.RemoveAt(idx);
            return true;
        }
        public void Limpiar() => Objetos.Clear();
        public void Dibujar(Shader shader)
        {
            foreach (var o in Objetos) o.Dibujar(shader);
        }
    }
}
