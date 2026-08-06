using ScrumSoft.Application.Mediador;

namespace ScrumSoft.Application.Columnas
{
    public sealed record EliminarColumnaComando : IPeticion
    {
        public required Guid IdProyecto { get; init; }

        public required Guid IdColumna { get; init; }
    }
}
