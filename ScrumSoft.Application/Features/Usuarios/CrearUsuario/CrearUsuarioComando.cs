using ScrumSoft.Application.Mediador;
using ScrumSoft.Domain.Enums;

namespace ScrumSoft.Application.Usuarios
{
    public sealed record CrearUsuarioComando : IPeticion<UsuarioDto>
    {
        public required string Nombre { get; init; }

        public required string CorreoElectronico { get; init; }

        // En claro solo durante esta peticion: se hashea antes de tocar la base.
        public required string Contrasena { get; init; }

        public RolUsuario Rol { get; init; } = RolUsuario.Miembro;
    }
}
