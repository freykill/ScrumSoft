using ScrumSoft.Application.Mediador;

namespace ScrumSoft.Application.Proyectos
{
    // El id sale del selector que alimenta GET /api/v1/usuarios.
    public sealed record AgregarMiembroComando : IPeticion<MiembroDto>
    {
        public Guid IdProyecto { get; init; }

        public required Guid IdUsuario { get; init; }
    }
}
