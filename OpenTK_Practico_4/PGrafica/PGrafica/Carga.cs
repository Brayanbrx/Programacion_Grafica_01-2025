using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using OpenTK.Mathematics;

namespace PGrafica
{
    internal static class Carga
    {
        // Lee un JSON con vertices e indices y crea Caras, Partes
        private class MallaDTO      // solo para deserializar
        {
            public float[][]? vertices { get; set; }
            public int[]? indices { get; set; }
        }

        public static Cara CargarCara(string rutaJson)
        {
            string json = File.ReadAllText(rutaJson);
            var dto = JsonSerializer.Deserialize<MallaDTO>(json)
                          ?? throw new IOException("JSON vacío");

            if (dto.vertices == null || dto.indices == null)
                throw new IOException("Faltan campos en el JSON");

            // convierte float[][] → Vector3[]
            var verts = new Vector3[dto.vertices.Length];
            for (int i = 0; i < verts.Length; i++)
            {
                var v = dto.vertices[i];
                verts[i] = new Vector3(v[0], v[1], v[2]);
            }

            return new Cara(verts, dto.indices);
        }

        public static Parte CargarParte(string jsonPath)
        {
            var cara = CargarCara(jsonPath);
            var parte = new Parte();
            parte.AgregarCara(cara);
            return parte;
        }
    }
}
