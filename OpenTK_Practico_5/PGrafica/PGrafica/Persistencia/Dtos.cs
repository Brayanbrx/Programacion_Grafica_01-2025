using System;

namespace PGrafica.Persistencia
{
    // Data Transfer Object, objeto plano, para transportar datos entre capas.
    public record CaraDto(float[][] vertices, int[] indices);
    public record ParteDto(List<CaraDto> caras);
    public record Objeto3DDto(float[] pos, float[] rot, float[] scl,
                              List<ParteDto> partes);
}
