using ScrumSoft.Application.Mediador;

namespace ScrumSoft.Application.Columnas
{
    public sealed record RenombrarColumnaComando : IPeticion<ColumnaDto>
    {
        public required Guid IdProyecto { get; init; }

        public required Guid IdColumna { get; init; }

        public required string Nombre { get; init; }
    }
}
