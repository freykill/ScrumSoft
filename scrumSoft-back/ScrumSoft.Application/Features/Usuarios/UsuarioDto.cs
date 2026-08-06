using ScrumSoft.Domain.Entities;
using ScrumSoft.Domain.Enums;

namespace ScrumSoft.Application.Usuarios
{
    // Solo lo necesario para pintar un selector. El hash de la contrasena
    // nunca sale de la capa de infraestructura.
    public sealed record UsuarioDto
    {
        public required Guid Id { get; init; }

        public required string Nombre { get; init; }

        public required string CorreoElectronico { get; init; }

        public required RolUsuario Rol { get; init; }

        public static UsuarioDto Desde(Usuario usuario)
        {
            ArgumentNullException.ThrowIfNull(usuario);

            return new UsuarioDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                CorreoElectronico = usuario.CorreoElectronico,
                Rol = usuario.Rol
            };
        }
    }
}
