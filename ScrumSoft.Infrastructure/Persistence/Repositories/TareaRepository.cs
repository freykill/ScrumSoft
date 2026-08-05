using Microsoft.EntityFrameworkCore;
using ScrumSoft.Application.Ports;
using ScrumSoft.Domain.Entities;
using ScrumSoft.Domain.Enums;

namespace ScrumSoft.Infrastructure.Persistence.Repositories
{
    public sealed class TareaRepository(ScrumSoftDbContext contexto) : ITareaRepository
    {
        public void Agregar(Tarea tarea)
        {
            ArgumentNullException.ThrowIfNull(tarea);

            contexto.Tareas.Add(tarea);
        }

        public Task<Tarea?> ObtenerPorIdAsync(Guid id, CancellationToken cancelacion = default) =>
            contexto.Tareas.FirstOrDefaultAsync(t => t.Id == id, cancelacion);

        // Sin AsNoTracking a proposito: al mover una tarea puede haber que renumerar
        // sus vecinas, y para eso EF tiene que estar rastreandolas.
        public async Task<IReadOnlyList<Tarea>> ListarPorColumnaAsync(
            Guid idColumna,
            CancellationToken cancelacion = default) =>
            await contexto.Tareas
                .Where(t => t.IdColumna == idColumna)
                .OrderBy(t => t.Orden)
                .ToListAsync(cancelacion)
                .ConfigureAwait(false);

        // Una sola consulta para todo el tablero, no una por columna.
        public async Task<IReadOnlyList<Tarea>> ListarPorProyectoAsync(
            Guid idProyecto,
            Guid? idResponsable = null,
            Prioridad? prioridad = null,
            CancellationToken cancelacion = default)
        {
            var consulta = contexto.Tareas
                .AsNoTracking()
                .Where(t => contexto.Columnas
                    .Any(c => c.Id == t.IdColumna && c.IdProyecto == idProyecto));

            // Los filtros viajan al SQL: no se traen todas las tareas para
            // descartarlas despues en memoria.
            if (idResponsable is { } responsable)
                consulta = consulta.Where(t => t.IdResponsable == responsable);

            if (prioridad is { } nivel)
                consulta = consulta.Where(t => t.Prioridad == nivel);

            return await consulta
                .OrderBy(t => t.Orden)
                .ToListAsync(cancelacion)
                .ConfigureAwait(false);
        }

        // AnyAsync genera un EXISTS: se detiene en la primera coincidencia
        // en vez de contarlas todas.
        public Task<bool> ExisteAlgunaEnColumnaAsync(
            Guid idColumna,
            CancellationToken cancelacion = default) =>
            contexto.Tareas.AnyAsync(t => t.IdColumna == idColumna, cancelacion);
    }
}
