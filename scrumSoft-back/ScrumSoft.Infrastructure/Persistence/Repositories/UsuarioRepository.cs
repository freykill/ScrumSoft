using Microsoft.EntityFrameworkCore;
using ScrumSoft.Application.Common;
using ScrumSoft.Application.Ports;
using ScrumSoft.Domain.Entities;

namespace ScrumSoft.Infrastructure.Persistence.Repositories
{
    public sealed class UsuarioRepository(ScrumSoftDbContext contexto) : IUsuarioRepository
    {
        public void Agregar(Usuario usuario)
        {
            ArgumentNullException.ThrowIfNull(usuario);

            contexto.Usuarios.Add(usuario);
        }

        public async Task<PagedResult<Usuario>> ListarAsync(
            string? filtro,
            int pagina,
            int tamanoPagina,
            CancellationToken cancelacion = default)
        {
            var consulta = contexto.Usuarios.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                // Una sola cadena contra nombre y correo: el usuario no tiene por que
                // saber por cual de los dos esta buscando.
                consulta = consulta.Where(u =>
                    EF.Functions.ILike(u.Nombre, $"%{filtro}%") ||
                    EF.Functions.ILike(u.CorreoElectronico, $"%{filtro}%"));
            }

            var total = await consulta.CountAsync(cancelacion).ConfigureAwait(false);

            var elementos = await consulta
                .OrderBy(u => u.Nombre)
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToListAsync(cancelacion)
                .ConfigureAwait(false);

            return new PagedResult<Usuario>(elementos, pagina, tamanoPagina, total);
        }

        // El correo se guarda ya normalizado a minusculas por la entidad,
        // asi que la comparacion directa aprovecha el indice unico.
        public Task<Usuario?> ObtenerPorCorreoAsync(
            string correoElectronico,
            CancellationToken cancelacion = default) =>
            contexto.Usuarios
                .FirstOrDefaultAsync(u => u.CorreoElectronico == correoElectronico, cancelacion);

        public Task<Usuario?> ObtenerPorIdAsync(Guid id, CancellationToken cancelacion = default) =>
            contexto.Usuarios.FirstOrDefaultAsync(u => u.Id == id, cancelacion);

        // Una consulta con IN en vez de una por cada id.
        public async Task<IReadOnlyList<Usuario>> ListarPorIdsAsync(
            IReadOnlyCollection<Guid> ids,
            CancellationToken cancelacion = default)
        {
            ArgumentNullException.ThrowIfNull(ids);

            if (ids.Count == 0)
                return [];

            return await contexto.Usuarios
                .AsNoTracking()
                .Where(u => ids.Contains(u.Id))
                .ToListAsync(cancelacion)
                .ConfigureAwait(false);
        }
    }
}
