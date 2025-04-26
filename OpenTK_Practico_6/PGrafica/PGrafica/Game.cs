using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using PGrafica.Persistencia;
using VecTK = OpenTK.Mathematics.Vector3;
using VecN = System.Numerics.Vector3;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace PGrafica
{
    public class Game : GameWindow
    {
        private Escenario _escena = null!;
        private Shader _shader = null!;
        private ImGuiController? _imgui = null!;
        private int _objSel = 0;  // Objeto seleccionado
        private int _modoTransform = 0; // 0 = Objeto, 1 = Escenario
        /* -------------------- cámara -------------------- */
        private VecTK _camPos = new(8f, 6f, 10f);
        private VecTK _camTarget = VecTK.Zero;
        private VecTK _camUp = VecTK.UnitY;
        private float _camSpeed = 5f;
        /* -------------------- guardado / carga -------------------- */
        private static readonly string SaveDir = Path.Combine(AppContext.BaseDirectory, "Saves");
        private string _saveName = "escena.json";
        private List<string> _files = new();
        private int _fileSelIdx = -1;
        /* -------------------- estado -------------------- */
        private string _statusMsg = "";
        private float _statusTTL = 0f;
        public Game(int width, int height, string title)
            : base(
                  GameWindowSettings.Default,
                  new NativeWindowSettings
                  {
                      Size = new Vector2i(width, height),
                      Title = title,
                      API = ContextAPI.OpenGL,
                      APIVersion = new Version(3, 3)
                  })
        { }
        protected override void OnLoad()
        {
            base.OnLoad();
            Directory.CreateDirectory(SaveDir);
            GL.ClearColor(0.85f, 0.85f, 0.85f, 1f);
            GL.Enable(EnableCap.DepthTest);
            _shader = new Shader("shader.vert", "shader.frag");
            _shader.Usar();
            SetViewProjection();
            _escena = new Escenario();
            _escena = JsonSceneStore.LoadAuto(Path.Combine(SaveDir, "escena.json"))
            as Escenario ?? new Escenario();
            //_escena.CrearObjetoEn(new VecTK(0, 0, 0))
            //    .AgregarParte(new Parte(JsonSceneStore.LoadMesh("Models/u.json")));
            //_escena.CrearObjetoEn(new VecTK(3, 1, 0))
            //       .AgregarParte(new Parte(JsonSceneStore.LoadMesh("Models/u.json")));
            //_escena.CrearObjetoEn(new VecTK(-3, 1, 0))
            //       .AgregarParte(new Parte(JsonSceneStore.LoadMesh("Models/u.json")));
            _imgui = new ImGuiController(ClientSize.X, ClientSize.Y);
            ImGui.StyleColorsDark();
            RefreshFileList();
        }
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
            _imgui.WindowResized(e.Width, e.Height);
        }
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            float delta = (float)e.Time;
            /* Movimiento de cámara ------------------------------- */
            Vector3 forward = Vector3.Normalize(_camTarget - _camPos);
            Vector3 right = Vector3.Normalize(Vector3.Cross(forward, _camUp));
            if (KeyboardState.IsKeyDown(Keys.W)) _camPos += forward * _camSpeed * delta;
            if (KeyboardState.IsKeyDown(Keys.S)) _camPos -= forward * _camSpeed * delta;
            if (KeyboardState.IsKeyDown(Keys.A)) _camPos -= right * _camSpeed * delta;
            if (KeyboardState.IsKeyDown(Keys.D)) _camPos += right * _camSpeed * delta;
            if (KeyboardState.IsKeyDown(Keys.Q)) _camPos += _camUp * _camSpeed * delta;
            if (KeyboardState.IsKeyDown(Keys.E)) _camPos -= _camUp * _camSpeed * delta;
        }
        protected override void OnUnload()
        {
            _shader.Dispose();
            _imgui.Dispose();
            base.OnUnload();
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            /* -------- lógica de mensajes -------- */
            _statusTTL = Math.Max(0f, _statusTTL - (float)e.Time);
            /* ----  Render de la escena -------- */
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            _shader.Usar();
            _shader.EstablecerMatriz4("view",
                Matrix4.LookAt(_camPos, _camTarget, _camUp));
            // Siempre aplicamos la transformación global del escenario
            _escena.Dibujar(_shader, true);
            /* ----  Interfaz ImGui ------------- */
            _imgui.Update(this, (float)e.Time);
            ImGui.Begin("Transformaciones");
            /* 2.1 Selector de modo */
            ImGui.Text("Modo de transformación:");
            ImGui.RadioButton("Objeto", ref _modoTransform, 0); ImGui.SameLine();
            ImGui.RadioButton("Escenario", ref _modoTransform, 1);
            ImGui.Separator();
            /* 2.2 Transformaciones */
            if (_modoTransform == 0) ShowObjetoControls();
            else ShowEscenarioControls();
            /* 2.3 Botón limpiar escena */
            if (ImGui.Button("Limpiar Escena"))
            {
                _escena.Limpiar();
                _objSel = 0;
                ShowStatus("Escena limpiada");
            }
            /* 2.4 Sección Guardar / Cargar */
            ShowSaveLoadSection();
            ImGui.End();
            _imgui.Render();
            SwapBuffers();
        }

        private void SetViewProjection()
        {
            var vista = Matrix4.LookAt(_camPos, _camTarget, _camUp);
            var proy = Matrix4.CreatePerspectiveFieldOfView(
                            MathHelper.DegreesToRadians(45f),
                            Size.X / (float)Size.Y, 0.1f, 100f);
            _shader.EstablecerMatriz4("view", vista);
            _shader.EstablecerMatriz4("projection", proy);
        }
        private void RefreshFileList()
        {
            if (!Directory.Exists(SaveDir)) Directory.CreateDirectory(SaveDir);
            _files = Directory.GetFiles(SaveDir, "*.json")
                              .Select(Path.GetFileNameWithoutExtension)
                              .OrderBy(n => n)
                              .ToList();
            _fileSelIdx = -1;
        }
        private void ShowObjetoControls()
        {
            if (_escena.Objetos.Count == 0) return;
            /* Combo para elegir objeto -------------------------------- */
            if (ImGui.BeginCombo("Seleccionar Objeto", $"Objeto {_objSel}"))
            {
                for (int i = 0; i < _escena.Objetos.Count; i++)
                {
                    bool sel = (i == _objSel);
                    if (ImGui.Selectable($"Objeto {i}", sel)) _objSel = i;
                    if (sel) ImGui.SetItemDefaultFocus();
                }
                ImGui.EndCombo();
            }
            /* Sliders -------------------------------------------------- */
            var obj = _escena.Objetos[_objSel];
            VecN pos = new(obj.Posicion.X, obj.Posicion.Y, obj.Posicion.Z);
            VecN rot = new(obj.RotacionEuler.X, obj.RotacionEuler.Y, obj.RotacionEuler.Z);
            VecN scl = new(obj.FactorEscala.X, obj.FactorEscala.Y, obj.FactorEscala.Z);
            if (ImGui.SliderFloat3("Posición Objeto", ref pos, -10f, 10f))
                obj.Posicion = new VecTK(pos.X, pos.Y, pos.Z);
            if (ImGui.SliderFloat3("Rotación Objeto", ref rot, -180f, 180f))
                obj.RotacionEuler = new VecTK(rot.X, rot.Y, rot.Z);
            if (ImGui.SliderFloat3("Escala Objeto", ref scl, 0.1f, 5f))
                obj.FactorEscala = new VecTK(scl.X, scl.Y, scl.Z);
            /* Botón eliminar ------------------------------------------ */
            if (ImGui.Button("Eliminar Objeto"))
            {
                _escena.QuitarObjeto(_objSel);
                _objSel = _escena.Objetos.Count > 0
                          ? Math.Clamp(_objSel, 0, _escena.Objetos.Count - 1)
                          : 0;
                ShowStatus("Objeto eliminado");
            }
        }
        private void ShowEscenarioControls()
        {
            VecN escPos = new(_escena.Posicion.X, _escena.Posicion.Y, _escena.Posicion.Z);
            VecN escRot = new(_escena.RotacionEuler.X, _escena.RotacionEuler.Y, _escena.RotacionEuler.Z);
            VecN escScl = new(_escena.FactorEscala.X, _escena.FactorEscala.Y, _escena.FactorEscala.Z);
            if (ImGui.SliderFloat3("Posición Escenario", ref escPos, -10f, 10f))
                _escena.Posicion = new VecTK(escPos.X, escPos.Y, escPos.Z);
            if (ImGui.SliderFloat3("Rotación Escenario", ref escRot, -180f, 180f))
                _escena.RotacionEuler = new VecTK(escRot.X, escRot.Y, escRot.Z);
            if (ImGui.SliderFloat3("Escala Escenario", ref escScl, 0.1f, 5f))
                _escena.FactorEscala = new VecTK(escScl.X, escScl.Y, escScl.Z);
        }

        private void ShowSaveLoadSection()
        {
            ImGui.Separator();
            ImGui.Text("Guardar (Enter):");
            ImGui.SetNextItemWidth(-1);
            if (ImGui.InputTextWithHint("##save", "(nombre json)", ref _saveName, 64,
                                        ImGuiInputTextFlags.EnterReturnsTrue)
                && _saveName.Trim().Length > 0)
            {
                string path = Path.Combine(SaveDir, _saveName.Trim() + ".json");
                if (_modoTransform == 1)
                {
                    /* Guardar escenario */
                    JsonSceneStore.Save(_escena, path);
                    ShowStatus($"Escenario guardado: {_saveName.Trim()}");
                }
                else if (_escena.Objetos.Count > 0)
                {
                    /* Guardar objeto seleccionado */
                    JsonSceneStore.Save(_escena.Objetos[_objSel], path);
                    ShowStatus($"Objeto guardado: {_saveName.Trim()}");
                }
                RefreshFileList();
            }
            ImGui.Separator();
            ImGui.Text("Archivos en 'Saves':");
            if (ImGui.BeginListBox("##files", new System.Numerics.Vector2(-1, 120)))
            {
                for (int i = 0; i < _files.Count; i++)
                {
                    bool sel = (i == _fileSelIdx);
                    if (ImGui.Selectable(_files[i], sel)) _fileSelIdx = i;
                    if (sel) ImGui.SetItemDefaultFocus();
                }
                ImGui.EndListBox();
            }
            if (_fileSelIdx >= 0)
            {
                string sel = _files[_fileSelIdx];
                string selPath = Path.Combine(SaveDir, sel + ".json");
                if (ImGui.Button("Cargar JSON"))
                {
                    try
                    {
                        object loaded = JsonSceneStore.LoadAuto(selPath);
                        switch (loaded)
                        {
                            case Escenario esc:
                                _escena = esc;
                                ShowStatus($"Escena cargada: {sel}");
                                break;
                            case Objeto3D obj:
                                _escena.AgregarObjeto(obj);
                                ShowStatus($"Objeto importado: {sel}");
                                break;
                            case Cara cara:
                                var parte = new Parte();
                                parte.AgregarCara(cara);
                                _escena.CrearObjetoEn(VecTK.Zero).AgregarParte(parte);
                                ShowStatus($"Malla importada: {sel}");
                                break;
                            default:
                                ShowStatus("Tipo de JSON desconocido", 3f);
                                break;
                        }
                    }
                    catch (IOException ex)
                    {
                        ShowStatus(ex.Message, 3f);
                    }
                }
            }
            /* Mensaje de estado */
            if (_statusTTL > 0f)
            {
                ImGui.Spacing();
                ImGui.TextColored(new System.Numerics.Vector4(0, 1, 0,1), _statusMsg);
            }
        }
        private void ShowStatus(string msg, float ttl = 2f)
        {
            _statusMsg = msg;
            _statusTTL = ttl;
        }
    }
}
