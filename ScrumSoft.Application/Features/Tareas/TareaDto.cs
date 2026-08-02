using ScrumSoft.Domain.Entities;
using ScrumSoft.Domain.Enums;

namespace ScrumSoft.Application.Tareas
{
    public sealed record TareaDto
    {
        public required Guid Id { get; init; }

        public required string Titulo { get; init; }

        public string? Descripcion { get; init; }

        public required Prioridad Prioridad { get; init; }

        public Guid? IdResponsable { get; init; }

        public required Guid IdColumna { get; init; }

        public required int Orden { get; init; }

        public required DateTimeOffset FechaCreacion { get; init; }

        public static TareaDto Desde(Tarea tarea)
        {
            ArgumentNullException.ThrowIfNull(tarea);

            return new TareaDto
            {
                Id = tarea.Id,
                Titulo = tarea.Titulo,
                Descripcion = tarea.Descripcion,
                Prioridad = tarea.Prioridad,
                IdResponsable = tarea.IdResponsable,
                IdColumna = tarea.IdColumna,
                Orden = tarea.Orden,
                FechaCreacion = tarea.FechaCreacion
            };
        }
    }
}
