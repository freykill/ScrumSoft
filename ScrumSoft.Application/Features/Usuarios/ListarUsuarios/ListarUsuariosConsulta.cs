using ScrumSoft.Application.Common;
using ScrumSoft.Application.Mediador;

namespace ScrumSoft.Application.Usuarios
{
    public sealed record ListarUsuariosConsulta : IPeticion<PagedResult<UsuarioDto>>
    {
        // Busca por nombre o por correo con la misma cadena: es lo que se
        // escribe en el cuadro de busqueda del selector.
        public string? Filtro { get; init; }

        public int Pagina { get; init; } = 1;

        public int TamanoPagina { get; init; } = 10;
    }
}
