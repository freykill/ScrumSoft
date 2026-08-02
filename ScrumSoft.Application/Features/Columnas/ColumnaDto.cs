using ScrumSoft.Domain.Entities;

namespace ScrumSoft.Application.Columnas
{
    public sealed record ColumnaDto
    {
        public required Guid Id { get; init; }

        public required string Nombre { get; init; }

        public required int Orden { get; init; }

        public static ColumnaDto Desde(Columna columna)
        {
            ArgumentNullException.ThrowIfNull(columna);

            return new ColumnaDto
            {
                Id = columna.Id,
                Nombre = columna.Nombre,
                Orden = columna.Orden
            };
        }

        public static IReadOnlyList<ColumnaDto> Desde(IEnumerable<Columna> columnas)
        {
            ArgumentNullException.ThrowIfNull(columnas);

            return [.. columnas.Select(Desde)];
        }
    }
}
