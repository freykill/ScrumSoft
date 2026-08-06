using ScrumSoft.Application.Tareas;

namespace ScrumSoft.Application.Tablero
{
    public sealed record ColumnaConTareasDto
    {
        public required Guid Id { get; init; }

        public required string Nombre { get; init; }

        public required int Orden { get; init; }

        public required IReadOnlyList<TareaDto> Tareas { get; init; }
    }

    // El tablero completo en una sola respuesta: el frontend lo renderiza sin pedir nada mas.
    public sealed record TableroDto
    {
        public required Guid IdProyecto { get; init; }

        public required string NombreProyecto { get; init; }

        public required IReadOnlyList<ColumnaConTareasDto> Columnas { get; init; }
    }
}
