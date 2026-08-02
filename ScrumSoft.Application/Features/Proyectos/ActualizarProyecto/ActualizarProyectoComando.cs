using ScrumSoft.Application.Mediador;
using ScrumSoft.Domain.Enums;

namespace ScrumSoft.Application.Proyectos
{
    public sealed record ActualizarProyectoComando : IPeticion<ProyectoDto>
    {
        public required Guid Id { get; init; }

        public required string Nombre { get; init; }

        public string? Descripcion { get; init; }

        public required DateOnly FechaInicio { get; init; }

        public DateOnly? FechaFinPrevista { get; init; }

        public required EstadoProyecto EstadoProyecto { get; init; }
    }
}
