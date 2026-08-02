using ScrumSoft.Application.Common;
using ScrumSoft.Application.Mediador;
using ScrumSoft.Application.Ports;

namespace ScrumSoft.Application.Proyectos
{
    public sealed class ListarProyectosHandler(
        IProyectoRepository proyectos,
        IUsuarioActual usuarioActual)
        : IManejador<ListarProyectosConsulta, PagedResult<ProyectoDto>>
    {
        private const int TamanoMaximoDePagina = 100;

        public async Task<PagedResult<ProyectoDto>> ManejarAsync(
            ListarProyectosConsulta peticion,
            CancellationToken cancelacion)
        {
            ArgumentNullException.ThrowIfNull(peticion);

            // Se corrigen en vez de rechazarse: una pagina 0 o un tamano de 5000
            // no son un error del usuario, son valores que hay que acotar.
            var pagina = peticion.Pagina < 1 ? 1 : peticion.Pagina;
            var tamano = Math.Clamp(peticion.TamanoPagina, 1, TamanoMaximoDePagina);

            var filtro = string.IsNullOrWhiteSpace(peticion.Nombre) ? null : peticion.Nombre.Trim();

            var resultado = await proyectos
                .ListarAsync(usuarioActual.Id, filtro, pagina, tamano, cancelacion)
                .ConfigureAwait(false);

            var dtos = resultado.Elementos.Select(ProyectoDto.Desde).ToList();

            return new PagedResult<ProyectoDto>(
                dtos,
                resultado.Pagina,
                resultado.TamanoPagina,
                resultado.TotalElementos);
        }
    }
}
