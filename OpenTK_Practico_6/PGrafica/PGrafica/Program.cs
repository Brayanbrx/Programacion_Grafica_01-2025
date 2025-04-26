namespace PGrafica
{
    internal static class Program
    {
        static void Main()
        {
            using var juego = new Game(800, 600, "NoOo La Politcia");
            juego.Run();
        }
    }
}
