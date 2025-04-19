using OpenTK.Mathematics;

namespace PGrafica
{
    public interface ITransformable //Contrato, admite traslacion, rotacion y escalado
    {
        void Trasladar(Vector3 delta);
        void Rotar(Vector3 axis, float angleDeg);
        void Escalar(Vector3 factor);
    }
}
