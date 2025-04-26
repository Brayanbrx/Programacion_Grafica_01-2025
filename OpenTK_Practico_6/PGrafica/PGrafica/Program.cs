namespace PGrafica
{
    internal static class Program
    {
        static void Main()
        {
            using var juego = new Game(800, 600, "Instancias de U PGrafica");
            juego.Run();
        }
    }
}
