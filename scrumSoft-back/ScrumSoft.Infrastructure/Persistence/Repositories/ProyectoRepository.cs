using Microsoft.EntityFrameworkCore;
using ScrumSoft.Application.Common;
using ScrumSoft.Application.Ports;
using ScrumSoft.Domain.Entities;

namespace ScrumSoft.Infrastructure.Persistence.Repositories
{
    public sealed class ProyectoRepository(ScrumSoftDbContext contexto) : IProyectoRepository
    {
        public void Agregar(Proyecto proyecto)
        {
            ArgumentNullException.ThrowIfNull(proyecto);

            // Solo lo marca como nuevo en el rastreador. La insercion ocurre al guardar.
            contexto.Proyectos.Add(proyecto);
        }

        public Task<Proyecto?> ObtenerPorIdAsync(Guid id, CancellationToken cancelacion = default) =>
            contexto.Proyectos
                // Proyecto es raiz de agregado: no se puede operar sobre medio agregado.
                // Los nombres con guion bajo son los campos privados de la entidad.
                .Include("_columnas")
                .Include("_miembros")
                .FirstOrDefaultAsync(p => p.Id == id, cancelacion);

        public async Task<PagedResult<Proyecto>> ListarAsync(
            Guid idUsuario,
            string? filtroNombre,
            int pagina,
            int tamanoPagina,
            CancellationToken cancelacion = default)
        {
            // El filtro de acceso viaja al SQL: no se traen proyectos ajenos para
            // descartarlos despues en memoria.
            var consulta = contexto.Proyectos
                .AsNoTracking()
                .Where(p => contexto.ProyectoUsuarios
                    .Any(m => m.IdProyecto == p.Id && m.IdUsuario == idUsuario));

            if (!string.IsNullOrWhiteSpace(filtroNombre))
            {
                // ILike es el LIKE sin distinguir mayusculas de PostgreSQL.
                consulta = consulta.Where(p => EF.Functions.ILike(p.Nombre, $"%{filtroNombre}%"));
            }

            var total = await consulta.CountAsync(cancelacion).ConfigureAwait(false);

            var elementos = await consulta
                .OrderBy(p => p.Nombre)
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .Include("_columnas")
                .ToListAsync(cancelacion)
                .ConfigureAwait(false);

            return new PagedResult<Proyecto>(elementos, pagina, tamanoPagina, total);
        }
    }
}
