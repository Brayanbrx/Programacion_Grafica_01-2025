namespace PGrafica
{
    // Contrato, cualquier cosa que se pueda dibujar con un shader
    public interface IRenderizable
    {
        void Dibujar(Shader shader);
    }
}
