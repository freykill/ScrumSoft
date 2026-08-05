using ScrumSoft.Application.Mediador;

namespace ScrumSoft.Application.Proyectos
{
    public sealed record QuitarMiembroComando : IPeticion<Unidad>
    {
        public Guid IdProyecto { get; init; }

        public Guid IdUsuario { get; init; }
    }
}
