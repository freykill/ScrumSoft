using ScrumSoft.Domain.Enums;

namespace ScrumSoft.Application.Reportes
{
    public sealed record FilaDeReporteDto
    {
        public required string Titulo { get; init; }

        public required string Columna { get; init; }

        public required string Responsable { get; init; }

        public required Prioridad Prioridad { get; init; }
    }

    // Requisito 6.8: una sola estructura de transferencia alimenta el PDF y el Excel.
    // Ningun exportador la modifica.
    public sealed record ReporteProyectoDto
    {
        public required Guid IdProyecto { get; init; }

        public required string Nombre { get; init; }

        public string? Descripcion { get; init; }

        public required DateOnly FechaInicio { get; init; }

        public DateOnly? FechaFinPrevista { get; init; }

        public required EstadoProyecto EstadoProyecto { get; init; }

        public required DateTimeOffset FechaGeneracion { get; init; }

        public required IReadOnlyList<FilaDeReporteDto> Tareas { get; init; }
    }
}
