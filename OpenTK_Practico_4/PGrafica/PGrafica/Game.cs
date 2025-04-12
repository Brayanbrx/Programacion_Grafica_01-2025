using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace PGrafica
{
    public class Game : GameWindow
    {
        private Escenario _escena = null!;
        private Shader _shader = null!;

        public Game(int w, int h, string titulo)
            : base(GameWindowSettings.Default,
                   new NativeWindowSettings { ClientSize = new Vector2i(w, h), Title = titulo })
        { }

        protected override void OnLoad()
        {
            GL.ClearColor(0.85f, 0.85f, 0.85f, 1f);
            GL.Enable(EnableCap.DepthTest);
            _shader = new Shader("shader.vert", "shader.frag");
            _shader.Usar();

            Matrix4 vista = Matrix4.LookAt(new Vector3(8f, 6f, 10f), Vector3.Zero, Vector3.UnitY);
            Matrix4 proy = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(45f), Size.X / (float)Size.Y, 0.1f, 100f);

            _shader.EstablecerMatriz4("view", vista);
            _shader.EstablecerMatriz4("projection", proy);

            _escena = new Escenario();
            _escena.CrearObjetoEn(new Vector3(0, 0, 0)).AgregarParte(Carga.CargarParte("Models/u.json"));
            _escena.CrearObjetoEn(new Vector3(3, 1, 0)).AgregarParte(Carga.CargarParte("Models/u.json"));
            _escena.CrearObjetoEn(new Vector3(-3, 1, 0)).AgregarParte(Carga.CargarParte("Models/u.json"));
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            _shader.Usar();
            _escena.Dibujar(_shader);
            SwapBuffers();
        }

        protected override void OnUnload()
        {
            _shader.Dispose();
            base.OnUnload();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            var input = KeyboardState;
            float vel = 60f * (float)e.Time;   // grados por segundo

            if (input.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Left))
                _escena.Objetos[0].Rotar(Vector3.UnitY, vel);  // izquierda → rota +
            if (input.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Right))
                _escena.Objetos[0].Rotar(Vector3.UnitY, -vel);  // derecha → rota -
        }

    }
}
