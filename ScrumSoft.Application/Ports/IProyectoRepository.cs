using ScrumSoft.Application.Common;
using ScrumSoft.Domain.Entities;

namespace ScrumSoft.Application.Ports
{
    public interface IProyectoRepository
    {
        void Agregar(Proyecto proyecto);

        // Trae el proyecto con sus columnas y sus miembros ya cargados: es una raiz
        // de agregado y no se puede operar sobre medio agregado.
        Task<Proyecto?> ObtenerPorIdAsync(Guid id, CancellationToken cancelacion = default);

        Task<PagedResult<Proyecto>> ListarAsync(
            Guid idUsuario,
            string? filtroNombre,
            int pagina,
            int tamanoPagina,
            CancellationToken cancelacion = default);
    }
}
