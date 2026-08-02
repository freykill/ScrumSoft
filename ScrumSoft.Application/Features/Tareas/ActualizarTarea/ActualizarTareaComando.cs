using ScrumSoft.Application.Mediador;
using ScrumSoft.Domain.Enums;

namespace ScrumSoft.Application.Tareas
{
    public sealed record ActualizarTareaComando : IPeticion<TareaDto>
    {
        public required Guid IdProyecto { get; init; }

        public required Guid IdTarea { get; init; }

        public required string Titulo { get; init; }

        public string? Descripcion { get; init; }

        public required Prioridad Prioridad { get; init; }

        // Null deja la tarea sin responsable.
        public Guid? IdResponsable { get; init; }
    }
}
