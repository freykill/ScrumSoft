using ScrumSoft.Application.Mediador;

namespace ScrumSoft.Application.Proyectos
{
    public sealed record EliminarProyectoComando : IPeticion
    {
        public required Guid Id { get; init; }
    }
}
