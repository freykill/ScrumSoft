using ScrumSoft.Application.Mediador;

namespace ScrumSoft.Application.Tareas
{
    public sealed record EliminarTareaComando : IPeticion
    {
        public required Guid IdProyecto { get; init; }

        public required Guid IdTarea { get; init; }
    }
}
