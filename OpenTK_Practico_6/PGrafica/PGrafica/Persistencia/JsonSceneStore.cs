using System.Text.Json;

namespace PGrafica.Persistencia
{
    public static class JsonSceneStore
    {
        private static readonly JsonSerializerOptions Opt = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };
        /* ----------------------- Guardar ----------------------- */
        public static void Save(Escenario e, string path) =>
            File.WriteAllText(path, JsonSerializer.Serialize(e.ToDto(), Opt));
        public static void Save(Objeto3D o, string path) =>
            File.WriteAllText(path, JsonSerializer.Serialize(o.ToDto(), Opt));
        public static void Save(Cara c, string path) =>
            File.WriteAllText(path, JsonSerializer.Serialize(c.ToDto(), Opt));
        /* ----------------------- Cargar ------------------------ */
        public static object LoadAuto(string path)
        {
            string txt = File.ReadAllText(path).TrimStart();
            string low = txt.ToLowerInvariant();
            if (low.StartsWith("[") || low.Contains("\"objetos\""))
                return Escenario.FromDto(JsonSerializer.Deserialize<Escenario.Dto>(txt, Opt));
            if (low.Contains("\"partes\""))
                return Objeto3D.FromDto(JsonSerializer.Deserialize<Objeto3D.Dto>(txt, Opt));
            if (low.Contains("\"vertices\""))
                return Cara.FromMeshJson(path);
            throw new IOException("Formato JSON no reconocido.");
        }
        public static Cara LoadMesh(string path) => Cara.FromMeshJson(path);
    }
}

