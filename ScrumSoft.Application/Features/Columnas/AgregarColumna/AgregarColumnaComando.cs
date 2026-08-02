using ScrumSoft.Application.Mediador;

namespace ScrumSoft.Application.Columnas
{
    public sealed record AgregarColumnaComando : IPeticion<ColumnaDto>
    {
        public Guid IdProyecto { get; init; }

        public required string Nombre { get; init; }
    }
}
