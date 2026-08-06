using ScrumSoft.Application.Common;
using ScrumSoft.Domain.Entities;

namespace ScrumSoft.Application.Ports
{
    public interface IUsuarioRepository
    {
        void Agregar(Usuario usuario);

        // Alimenta el selector de "agregar miembro": paginado porque la lista
        // completa de usuarios no cabe en un desplegable.
        Task<PagedResult<Usuario>> ListarAsync(
            string? filtro,
            int pagina,
            int tamanoPagina,
            CancellationToken cancelacion = default);

        // La consulta del inicio de sesion. El correo se compara en minusculas.
        Task<Usuario?> ObtenerPorCorreoAsync(string correoElectronico, CancellationToken cancelacion = default);

        Task<Usuario?> ObtenerPorIdAsync(Guid id, CancellationToken cancelacion = default);

        // Varios de una sola vez: evita consultar uno por uno al resolver
        // los responsables de una lista de tareas.
        Task<IReadOnlyList<Usuario>> ListarPorIdsAsync(
            IReadOnlyCollection<Guid> ids,
            CancellationToken cancelacion = default);
    }
}
