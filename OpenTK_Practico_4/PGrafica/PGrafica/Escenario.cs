using System.Collections.Generic;
using System.Text.Json;
using OpenTK.Mathematics;

namespace PGrafica
{
    public class Escenario : IRenderizable
    {
        public List<Objeto3D> Objetos { get; } = new();

        public Objeto3D CrearObjetoEn(Vector3 pos)
        {
            var o = new Objeto3D(pos);
            Objetos.Add(o);
            return o;
        }

        public void AgregarObjeto(Objeto3D o) => Objetos.Add(o);
        public void QuitarObjeto(Objeto3D o) => Objetos.Remove(o);

        public void Dibujar(Shader shader)
        {
            foreach (var o in Objetos) o.Dibujar(shader);
        }

        //Serializacion completa a JSON
        public string ToJson(bool indent = true) =>
            JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = indent });

        public static Escenario? FromJson(string json) =>
            JsonSerializer.Deserialize<Escenario>(json);
    }
}
