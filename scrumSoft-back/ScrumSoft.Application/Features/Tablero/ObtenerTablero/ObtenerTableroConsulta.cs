using ScrumSoft.Application.Mediador;
using ScrumSoft.Domain.Enums;

namespace ScrumSoft.Application.Tablero
{
    public sealed record ObtenerTableroConsulta : IPeticion<TableroDto>
    {
        public required Guid IdProyecto { get; init; }

        // Filtros opcionales. Nulos significa "sin filtrar": el tablero completo.
        // Las columnas se devuelven siempre, aunque queden vacias al filtrar.
        public Guid? IdResponsable { get; init; }

        public Prioridad? Prioridad { get; init; }

        // Busca en el titulo y en la descripcion de la tarea.
        public string? Texto { get; init; }
    }
}
