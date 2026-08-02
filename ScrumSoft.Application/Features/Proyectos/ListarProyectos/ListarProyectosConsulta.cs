using ScrumSoft.Application.Common;
using ScrumSoft.Application.Mediador;

namespace ScrumSoft.Application.Proyectos
{
    public sealed record ListarProyectosConsulta : IPeticion<PagedResult<ProyectoDto>>
    {
        public string? Nombre { get; init; }

        public int Pagina { get; init; } = 1;

        public int TamanoPagina { get; init; } = 10;
    }
}
