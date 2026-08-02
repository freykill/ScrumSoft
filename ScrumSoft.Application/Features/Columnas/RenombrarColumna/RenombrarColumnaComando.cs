using ScrumSoft.Application.Mediador;

namespace ScrumSoft.Application.Columnas
{
    public sealed record RenombrarColumnaComando : IPeticion<ColumnaDto>
    {
        public Guid IdProyecto { get; init; }

        public Guid IdColumna { get; init; }

        public required string Nombre { get; init; }
    }
}
