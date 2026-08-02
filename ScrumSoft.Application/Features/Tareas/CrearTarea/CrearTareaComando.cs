using ScrumSoft.Application.Mediador;
using ScrumSoft.Domain.Enums;

namespace ScrumSoft.Application.Tareas
{
    public sealed record CrearTareaComando : IPeticion<TareaDto>
    {
        public required Guid IdProyecto { get; init; }

        public required Guid IdColumna { get; init; }

        public required string Titulo { get; init; }

        public string? Descripcion { get; init; }

        public required Prioridad Prioridad { get; init; }

        public Guid? IdResponsable { get; init; }
    }
}
