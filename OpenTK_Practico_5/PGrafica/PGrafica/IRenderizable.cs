namespace PGrafica
{
    public interface IRenderizable // Contrato, cualquier cosa que se pueda dibujar con un shader
    {
        void Dibujar(Shader shader);
    }
}
