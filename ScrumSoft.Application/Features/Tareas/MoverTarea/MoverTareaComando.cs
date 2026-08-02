using ScrumSoft.Application.Mediador;

namespace ScrumSoft.Application.Tareas
{
    // El frontend informa entre que dos tarjetas se solto la tarea.
    // La posicion numerica la calcula el servidor, nunca el navegador.
    public sealed record MoverTareaComando : IPeticion<TareaDto>
    {
        public required Guid IdProyecto { get; init; }

        public required Guid IdTarea { get; init; }

        // Puede ser la misma columna de origen si solo se reordena.
        public required Guid IdColumnaDestino { get; init; }

        // Tarea que queda justo encima. Null si se solto al inicio de la columna.
        public Guid? IdTareaAnterior { get; init; }

        // Tarea que queda justo debajo. Null si se solto al final de la columna.
        public Guid? IdTareaSiguiente { get; init; }
    }
}
