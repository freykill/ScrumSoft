using ScrumSoft.Application.Mediador;

namespace ScrumSoft.Application.Proyectos
{
    public sealed record ListarMiembrosConsulta : IPeticion<IReadOnlyList<MiembroDto>>
    {
        public required Guid IdProyecto { get; init; }
    }
}
