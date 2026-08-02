using ScrumSoft.Application.Mediador;

namespace ScrumSoft.Application.Proyectos
{
    public sealed record CrearProyectoComando : IPeticion<ProyectoDto>
    {
        public required string Nombre { get; init; }

        public string? Descripcion { get; init; }

        public required DateOnly FechaInicio { get; init; }

        public DateOnly? FechaFinPrevista { get; init; }

        public IReadOnlyList<string>? Columnas { get; init; }
    }
}
