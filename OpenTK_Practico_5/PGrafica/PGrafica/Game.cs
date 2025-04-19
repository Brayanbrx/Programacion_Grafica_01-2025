using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using ImGuiNET;
using PGrafica.Persistencia;

using VecTK = OpenTK.Mathematics.Vector3;
using VecN = System.Numerics.Vector3;
using OpenTK.Windowing.GraphicsLibraryFramework;


namespace PGrafica
{
    public class Game : GameWindow
    {
        // Atributos Principales
        private Escenario _escena = null!;
        private Shader _shader = null!;
        private ImGuiController? _imgui = null!;
        private int _objSel = 0;
        //  Manejo de JSON
        private static readonly string SaveDir = Path.Combine(AppContext.BaseDirectory, "Saves");
        private string _sceneName = "escena.json";
        private string _objectName = "objeto.json";
        private List<string> _files = new();
        private int _fileSelIdx = -1;
        private void RefreshFileList()
        {
            if (!Directory.Exists(SaveDir)) Directory.CreateDirectory(SaveDir);
            _files = Directory.GetFiles(SaveDir, "*.json")
                              .Select(f => Path.GetFileNameWithoutExtension(f))
                              .OrderBy(f => f).ToList();
            _fileSelIdx = -1;
        }
        // Mensajes Feedback
        private string _statusMsg = "";
        private float _statusTTL = 0f;
        private void ShowStatus(string msg, float ttl = 2f)
        {
            _statusMsg = msg;
            _statusTTL = ttl;
        }
        // Cámara
        private Vector3 _camPos = new Vector3(8f, 6f, 10f);
        private Vector3 _camTarget = Vector3.Zero;
        private Vector3 _camUp = Vector3.UnitY;
        private float _camSpeed = 5f;
        public Game(int w, int h, string titulo)
            : base(GameWindowSettings.Default,
                   new NativeWindowSettings { 
                       ClientSize = new Vector2i(w, h), 
                       Title = titulo,
                       API = ContextAPI.OpenGL,
                       APIVersion = new Version(3,3)
                   })
        { }
        protected override void OnLoad()
        {
            base.OnLoad();
            Directory.CreateDirectory(SaveDir);
            RefreshFileList();
            GL.ClearColor(0.85f, 0.85f, 0.85f, 1f);
            GL.Enable(EnableCap.DepthTest);
            _shader = new Shader("shader.vert", "shader.frag"); // Crea y Carga Shaders
            _shader.Usar();
            Matrix4 vista = Matrix4.LookAt(new VecTK(8f, 6f, 10f), VecTK.Zero, VecTK.UnitY);
            Matrix4 proy = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(45f), Size.X / (float)Size.Y, 0.1f, 100f);
            _shader.EstablecerMatriz4("view", vista);
            _shader.EstablecerMatriz4("projection", proy);
            _escena = new Escenario();
            _escena.CrearObjetoEn(new VecTK(0, 0, 0))
                .AgregarParte(Carga.CargarParte("Models/u.json"));
            _escena.CrearObjetoEn(new VecTK(3, 1, 0))
                .AgregarParte(Carga.CargarParte("Models/u.json"));
            _escena.CrearObjetoEn(new VecTK(-3, 1, 0))
                .AgregarParte(Carga.CargarParte("Models/u.json"));
            _imgui = new ImGuiController(ClientSize.X, ClientSize.Y);
            ImGui.StyleColorsDark();
        }
        protected override void OnResize(ResizeEventArgs e) // Adaptamos las dimensiones o responsivo :B
        {
            base.OnResize(e);
            GL.Viewport(0,0, e.Width, e.Height);
            _imgui.WindowResized(e.Width, e.Height);
        }
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            float delta = (float)e.Time;
            Vector3 forward = Vector3.Normalize(_camTarget - _camPos);
            Vector3 right = Vector3.Normalize(Vector3.Cross(forward, _camUp));  
            if (KeyboardState.IsKeyDown(Keys.W)) _camPos += forward * _camSpeed * delta; // Movimientos de Camara
            if (KeyboardState.IsKeyDown(Keys.S)) _camPos -= forward * _camSpeed * delta;
            if (KeyboardState.IsKeyDown(Keys.A)) _camPos -= right * _camSpeed * delta;
            if (KeyboardState.IsKeyDown(Keys.D)) _camPos += right * _camSpeed * delta;
            if (KeyboardState.IsKeyDown(Keys.Q)) _camPos += _camUp * _camSpeed * delta;
            if (KeyboardState.IsKeyDown(Keys.E)) _camPos -= _camUp * _camSpeed * delta; 
            if (KeyboardState.IsKeyPressed(Keys.Escape)) Close(); // Salir
            if (_statusTTL > 0f) _statusTTL -= delta; // Temporizador de status
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);  
            Matrix4 vista = Matrix4.LookAt(_camPos, _camTarget, _camUp); // Actualizar vista de cámara
            _shader.EstablecerMatriz4("view", vista);
            // Activar Shader y Dibujar
            _shader.Usar(); 
            _escena.Dibujar(_shader); 
            // Interfaz IMGUI
            _imgui.Update(this, (float)e.Time);  // recoge input + frame new
            ImGui.Begin("Transformaciones");
            if (ImGui.BeginCombo("Objeto", $"#{_objSel}")) // Seleccion de Objetos
            {
                for (int i = 0; i < _escena.Objetos.Count; i++)
                {
                    bool sel = i == _objSel;
                    if (ImGui.Selectable($"Objeto {i}", sel)) _objSel = i;
                    if (sel) ImGui.SetItemDefaultFocus();
                }
                ImGui.EndCombo();
            }
            if (_escena.Objetos.Count > 0)
            {
                var obj = _escena.Objetos[_objSel];
            // Sliders Para actualizar Traslacion, Rotacion y Escala
            VecN pos = new(obj.Posicion.X, obj.Posicion.Y, obj.Posicion.Z);
            VecN rot = new(obj.RotacionEuler.X, obj.RotacionEuler.Y, obj.RotacionEuler.Z);
            VecN scl = new(obj.FactorEscala.X, obj.FactorEscala.Y, obj.FactorEscala.Z);
            if (ImGui.SliderFloat3("Posición", ref pos, -10f, 10f))
                obj.Posicion = new VecTK(pos.X, pos.Y, pos.Z);
            if (ImGui.SliderFloat3("Rotación", ref rot, -180f, 180f))
                obj.RotacionEuler = new VecTK(rot.X, rot.Y, rot.Z);
            if (ImGui.SliderFloat3("Escala", ref scl, 0.1f, 5f))
                obj.FactorEscala = new VecTK(scl.X, scl.Y, scl.Z);
            }
            if (ImGui.Button("Eliminar objeto"))
            {
                _escena.QuitarObjeto(_objSel);
                _objSel = _escena.Objetos.Count == 0 ? -1 : Math.Clamp(_objSel, 0, _escena.Objetos.Count - 1);
            }
            if (ImGui.Button("Limpiar escena"))
            {
                _escena.Limpiar();
                _objSel = -1;     // sin seleccion
            }   
            ImGui.Separator();    // Guardar
            ImGui.Text("Guardar:");
            ImGui.SetNextItemWidth(-1);
            if (ImGui.InputTextWithHint("##escena", "(nombre escena)", ref _sceneName, 64,
                 ImGuiInputTextFlags.EnterReturnsTrue) && _sceneName.Trim().Length > 0)
            {
                string path = Path.Combine(SaveDir, _sceneName.Trim() + ".json");
                JsonSceneStore.SaveEscenario(_escena, path);
                ShowStatus($"Escena guardada: {_sceneName.Trim()}");
                RefreshFileList();
            }
            ImGui.SetNextItemWidth(-1);
            if (ImGui.InputTextWithHint("##obj", "(nombre objeto)", ref _objectName, 64,
                 ImGuiInputTextFlags.EnterReturnsTrue) && _objectName.Trim().Length > 0 && _escena.Objetos.Count > 0)
            {
                string path = Path.Combine(SaveDir, _objectName.Trim() + ".json");
                JsonSceneStore.SaveObjeto(_escena.Objetos[_objSel], path);
                ShowStatus($"Objeto guardado: {_objectName.Trim()}");
                RefreshFileList();
            }
            ImGui.Separator(); //  Archivos existentes
            ImGui.Text("Archivos en 'Saves':");
            if (ImGui.BeginListBox("##files", new System.Numerics.Vector2(-1, 120)))
            {
                for (int i = 0; i < _files.Count; i++)
                {
                    bool sel = i == _fileSelIdx;
                    if (ImGui.Selectable(_files[i], sel)) _fileSelIdx = i;
                    if (sel) ImGui.SetItemDefaultFocus();
                }
                ImGui.EndListBox();
            }
            if (_fileSelIdx >= 0)
            {
                string selected = _files[_fileSelIdx];
                string selPath = Path.Combine(SaveDir, selected + ".json");

                if (ImGui.Button("Cargar escena"))
                {
                    try
                    {
                        _escena = JsonSceneStore.LoadEscenario(selPath);
                        ShowStatus($"Escena cargada: {selected}");
                    }
                    catch (IOException ex)
                    {
                        ShowStatus(ex.Message, 3);
                    }
                }
                ImGui.SameLine();
                if (ImGui.Button("Importar objeto"))
                {
                    try
                    {
                        _escena.AgregarObjeto(JsonSceneStore.LoadObjeto(selPath));
                        if (_objSel == -1) _objSel = 0;
                        ShowStatus($"Objeto importado: {selected}");
                    }
                    catch (IOException ex)
                    {
                        ShowStatus(ex.Message, 3);
                    }
                }
            }
            if (_statusTTL > 0f) // Estado
            {
                ImGui.Spacing();
                ImGui.TextColored(new System.Numerics.Vector4(0, 1, 0, 1), _statusMsg);
            }
            ImGui.End();
            _imgui.Render();
            SwapBuffers();
        }
        protected override void OnUnload()
        {
            _shader.Dispose();
            _imgui.Dispose();
            base.OnUnload();
        }
    }
}
