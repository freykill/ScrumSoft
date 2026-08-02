using ScrumSoft.Application.Mediador;

namespace ScrumSoft.Application.Tablero
{
    public sealed record ObtenerTableroConsulta : IPeticion<TableroDto>
    {
        public required Guid IdProyecto { get; init; }
    }
}
