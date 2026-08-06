using ScrumSoft.Application.Common;
using ScrumSoft.Application.Mediador;
using ScrumSoft.Application.Ports;

namespace ScrumSoft.Application.Usuarios
{
    public sealed class ListarUsuariosHandler(IUsuarioRepository usuarios)
        : IManejador<ListarUsuariosConsulta, PagedResult<UsuarioDto>>
    {
        private const int TamanoMaximoDePagina = 100;

        public async Task<PagedResult<UsuarioDto>> ManejarAsync(
            ListarUsuariosConsulta peticion,
            CancellationToken cancelacion)
        {
            ArgumentNullException.ThrowIfNull(peticion);

            // Se acotan en vez de rechazarse, igual que en el listado de proyectos.
            var pagina = peticion.Pagina < 1 ? 1 : peticion.Pagina;
            var tamano = Math.Clamp(peticion.TamanoPagina, 1, TamanoMaximoDePagina);

            var filtro = string.IsNullOrWhiteSpace(peticion.Filtro) ? null : peticion.Filtro.Trim();

            var resultado = await usuarios
                .ListarAsync(filtro, pagina, tamano, cancelacion)
                .ConfigureAwait(false);

            var dtos = resultado.Elementos.Select(UsuarioDto.Desde).ToList();

            return new PagedResult<UsuarioDto>(
                dtos,
                resultado.Pagina,
                resultado.TamanoPagina,
                resultado.TotalElementos);
        }
    }
}
