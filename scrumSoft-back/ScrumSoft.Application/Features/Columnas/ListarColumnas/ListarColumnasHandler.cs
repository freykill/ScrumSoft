using ScrumSoft.Application.Common;
using ScrumSoft.Application.Mediador;

namespace ScrumSoft.Application.Columnas
{
    public sealed class ListarColumnasHandler(AccesoAProyectos acceso)
        : IManejador<ListarColumnasConsulta, IReadOnlyList<ColumnaDto>>
    {
        public async Task<IReadOnlyList<ColumnaDto>> ManejarAsync(
            ListarColumnasConsulta peticion,
            CancellationToken cancelacion)
        {
            ArgumentNullException.ThrowIfNull(peticion);

            var proyecto = await acceso
                .ObtenerConAccesoAsync(peticion.IdProyecto, cancelacion)
                .ConfigureAwait(false);

            // El agregado ya devuelve solo las columnas activas y ordenadas.
            return ColumnaDto.Desde(proyecto.Columnas);
        }
    }
}
