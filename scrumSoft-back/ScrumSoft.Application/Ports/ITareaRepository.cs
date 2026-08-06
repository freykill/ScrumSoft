using ScrumSoft.Domain.Entities;
using ScrumSoft.Domain.Enums;

namespace ScrumSoft.Application.Ports
{
    public interface ITareaRepository
    {
        void Agregar(Tarea tarea);

        Task<Tarea?> ObtenerPorIdAsync(Guid id, CancellationToken cancelacion = default);

        // Ordenadas por su posicion. Sirve para calcular vecinos al arrastrar.
        Task<IReadOnlyList<Tarea>> ListarPorColumnaAsync(Guid idColumna, CancellationToken cancelacion = default);

        // Todas las tareas del proyecto en una sola consulta: alimenta el tablero y el reporte.
        // Los filtros viven aqui y no en cada caso de uso, para que el tablero filtrado
        // y el reporte descargado desde ese tablero devuelvan exactamente lo mismo.
        Task<IReadOnlyList<Tarea>> ListarPorProyectoAsync(
            Guid idProyecto,
            Guid? idResponsable = null,
            Prioridad? prioridad = null,
            string? texto = null,
            CancellationToken cancelacion = default);

        // El dato que el dominio necesita para decidir si una columna se puede eliminar.
        Task<bool> ExisteAlgunaEnColumnaAsync(Guid idColumna, CancellationToken cancelacion = default);
    }
}
