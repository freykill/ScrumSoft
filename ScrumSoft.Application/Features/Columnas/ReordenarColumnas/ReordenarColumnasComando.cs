using ScrumSoft.Application.Mediador;

namespace ScrumSoft.Application.Columnas
{
    public sealed record ReordenarColumnasComando : IPeticion<IReadOnlyList<ColumnaDto>>
    {
        public required Guid IdProyecto { get; init; }

        // Ids de todas las columnas activas del proyecto, en el orden deseado.
        public required IReadOnlyList<Guid> IdsEnOrden { get; init; }
    }
}
