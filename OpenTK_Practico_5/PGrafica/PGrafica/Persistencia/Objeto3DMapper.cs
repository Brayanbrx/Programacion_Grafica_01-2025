using OpenTK.Mathematics;

namespace PGrafica.Persistencia
{
    internal static class Objeto3DMapper
    {
        public static Objeto3DDto ToDto(Objeto3D o) => new(
            pos: new[] { o.Posicion.X, o.Posicion.Y, o.Posicion.Z },
            rot: new[] { o.RotacionEuler.X, o.RotacionEuler.Y, o.RotacionEuler.Z },
            scl: new[] { o.FactorEscala.X, o.FactorEscala.Y, o.FactorEscala.Z },
            partes: o.Partes.Select(p => new ParteDto(
                p.Caras.Select(c => new CaraDto(
                    c.Vertices.Select(v => new[] { v.X, v.Y, v.Z }).ToArray(),
                    c.Indices.ToArray()
                )).ToList()
            )).ToList()
        );

        public static Objeto3D FromDto(Objeto3DDto d)
        {
            var o = new Objeto3D
            {
                Posicion = new Vector3(d.pos[0], d.pos[1], d.pos[2]),
                RotacionEuler = new Vector3(d.rot[0], d.rot[1], d.rot[2]),
                FactorEscala = new Vector3(d.scl[0], d.scl[1], d.scl[2])
            };

            foreach (var pDto in d.partes)
            {
                var parte = new Parte();
                foreach (var cDto in pDto.caras)
                {
                    var verts = cDto.vertices
                                    .Select(a => new Vector3(a[0], a[1], a[2]))
                                    .ToArray();
                    var cara = new Cara(verts, cDto.indices);
                    cara.InicializarBuffers();
                    parte.AgregarCara(cara);
                }
                o.AgregarParte(parte);
            }
            return o;
        }
    }
}
