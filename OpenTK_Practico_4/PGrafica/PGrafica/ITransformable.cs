using OpenTK.Mathematics;

namespace PGrafica
{
    //Contrato, admite traslacion, rotacion y escalado
    public interface ITransformable
    {
        void Trasladar(Vector3 delta);
        void Rotar(Vector3 axis, float angleDeg);
        void Escalar(Vector3 factor);
    }
}
