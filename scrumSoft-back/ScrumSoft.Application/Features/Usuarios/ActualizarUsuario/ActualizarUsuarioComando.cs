using ScrumSoft.Application.Mediador;
using ScrumSoft.Domain.Enums;

namespace ScrumSoft.Application.Usuarios
{
    // El correo no se edita: es la llave del inicio de sesion y cambiarlo
    // dejaria fuera al usuario sin avisarle.
    public sealed record ActualizarUsuarioComando : IPeticion<UsuarioDto>
    {
        public Guid Id { get; init; }

        public required string Nombre { get; init; }

        public required RolUsuario Rol { get; init; }
    }
}
