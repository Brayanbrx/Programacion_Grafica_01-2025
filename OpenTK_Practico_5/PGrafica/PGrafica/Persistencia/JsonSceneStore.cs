using System;
using System.IO;
using System.Text.Json;

namespace PGrafica.Persistencia
{
    public static class JsonSceneStore
    {
        private static readonly JsonSerializerOptions Opt = new() { WriteIndented = true };
        public static void SaveObjeto(Objeto3D o, string path) // Convierte Objeto3D a DTO
            => File.WriteAllText(path, JsonSerializer.Serialize(
                   Objeto3DMapper.ToDto(o), Opt));
        public static Objeto3D LoadObjeto(string path)
        {
            string txt = File.ReadAllText(path).TrimStart();  
            if (txt.StartsWith('{') && txt.Contains("\"pos\"")) // Objeto3DDto
            {
                var dto = JsonSerializer.Deserialize<Objeto3DDto>(txt, Opt)
                          ?? throw new IOException("JSON inválido (objeto)");
                return Objeto3DMapper.FromDto(dto);
            }
            if (txt.StartsWith('{') && txt.Contains("\"vertices\"")) // Solo Vertices e Indices
            {
                var cara = Carga.CargarCara(path);      // ya inicializa buffers
                var parte = new Parte();
                parte.AgregarCara(cara);
                var obj = new Objeto3D();
                obj.AgregarParte(parte);
                return obj;                             // posición (0,0,0), escala 1
            }
            throw new IOException("Formato de archivo no reconocido.");
        }
        public static void SaveEscenario(Escenario e, string path)
        {
            var lista = e.Objetos.Select(Objeto3DMapper.ToDto).ToList();
            File.WriteAllText(path, JsonSerializer.Serialize(lista, Opt));
        }
        public static Escenario LoadEscenario(string path)
        {
            string txt = File.ReadAllText(path).TrimStart();
            if (!txt.StartsWith('['))
                throw new IOException("El archivo seleccionado contiene un solo objeto; usa «Importar objeto».");
            var listaDto = JsonSerializer.Deserialize<List<Objeto3DDto>>(txt, Opt)
                           ?? throw new IOException("JSON inválido");
            var esc = new Escenario();
            foreach (var dto in listaDto)
                esc.AgregarObjeto(Objeto3DMapper.FromDto(dto));
            return esc;
        }
    }
}
