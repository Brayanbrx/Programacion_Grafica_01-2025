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
        public static void Save<T>(T data, string path)
        {
            object dto = data switch
            {
                Escenario esc => esc.ToDto(),
                Objeto3D obj => obj.ToDto(),
                Cara car => car.ToDto(),
                _ => throw new NotSupportedException(
                         $"No sé serializar objetos de tipo {typeof(T).Name}")
            };

            File.WriteAllText(path, JsonSerializer.Serialize(dto, Opt));
        }

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

