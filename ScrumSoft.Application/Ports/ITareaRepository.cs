using ScrumSoft.Domain.Entities;

namespace ScrumSoft.Application.Ports
{
    public interface ITareaRepository
    {
        void Agregar(Tarea tarea);

        Task<Tarea?> ObtenerPorIdAsync(Guid id, CancellationToken cancelacion = default);

        // Ordenadas por su posicion. Sirve para calcular vecinos al arrastrar.
        Task<IReadOnlyList<Tarea>> ListarPorColumnaAsync(Guid idColumna, CancellationToken cancelacion = default);

        // Todas las tareas del proyecto en una sola consulta: alimenta el tablero y el reporte.
        Task<IReadOnlyList<Tarea>> ListarPorProyectoAsync(Guid idProyecto, CancellationToken cancelacion = default);

        // El dato que el dominio necesita para decidir si una columna se puede eliminar.
        Task<bool> ExisteAlgunaEnColumnaAsync(Guid idColumna, CancellationToken cancelacion = default);
    }
}
