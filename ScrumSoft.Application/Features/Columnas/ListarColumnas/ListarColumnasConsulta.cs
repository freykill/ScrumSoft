using ScrumSoft.Application.Mediador;

namespace ScrumSoft.Application.Columnas
{
    public sealed record ListarColumnasConsulta : IPeticion<IReadOnlyList<ColumnaDto>>
    {
        public required Guid IdProyecto { get; init; }
    }
}
