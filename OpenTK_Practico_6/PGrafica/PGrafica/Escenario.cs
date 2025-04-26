using OpenTK.Mathematics;

namespace PGrafica
{
    public class Escenario
    {
        public List<Objeto3D> Objetos { get; } = new();
        public Vector3 Posicion { get; set; } = Vector3.Zero;
        public Vector3 RotacionEuler { get; set; } = Vector3.Zero;
        public Vector3 FactorEscala { get; set; } = Vector3.One;
        public Objeto3D CrearObjetoEn(Vector3 pos)
        {
            var o = new Objeto3D(pos); 
            Objetos.Add(o); 
            return o; 
        }
        public void AgregarObjeto(Objeto3D o) => Objetos.Add(o);
        public bool QuitarObjeto(int idx)
        {
            if (idx < 0 || idx >= Objetos.Count)
                return false;
            Objetos.RemoveAt(idx);
                return true;
        }
        public void Limpiar() => Objetos.Clear();
        public readonly record struct Dto(
            float[] pos, float[] rot, float[] scl,
            List<Objeto3D.Dto> objetos);
        public Dto ToDto() => new(
            new[] { Posicion.X, Posicion.Y, Posicion.Z },
            new[] { RotacionEuler.X, RotacionEuler.Y, RotacionEuler.Z },
            new[] { FactorEscala.X, FactorEscala.Y, FactorEscala.Z },
            Objetos.Select(o => o.ToDto()).ToList()
        );
        public static Escenario FromDto(Dto d)
        {
            var e = new Escenario
            {
                Posicion = new Vector3(d.pos[0], d.pos[1], d.pos[2]),
                RotacionEuler = new Vector3(d.rot[0], d.rot[1], d.rot[2]),
                FactorEscala = new Vector3(d.scl[0], d.scl[1], d.scl[2])
            };
            var objs = d.objetos ?? new List<Objeto3D.Dto>();  
            foreach (var oDto in objs) e.AgregarObjeto(Objeto3D.FromDto(oDto));
            return e;
        }
        public void Dibujar(Shader s, bool aplicarTransformEscenario = false)
        {
            Matrix4 escenarioModel = Matrix4.Identity;
            if (aplicarTransformEscenario)
            {
                escenarioModel =
                    Matrix4.CreateScale(FactorEscala) *
                    Matrix4.CreateRotationX(MathHelper.DegreesToRadians(RotacionEuler.X)) *
                    Matrix4.CreateRotationY(MathHelper.DegreesToRadians(RotacionEuler.Y)) *
                    Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(RotacionEuler.Z)) *
                    Matrix4.CreateTranslation(Posicion);
            }
            foreach (var o in Objetos) o.Dibujar(s, escenarioModel);
        }
    }
}


