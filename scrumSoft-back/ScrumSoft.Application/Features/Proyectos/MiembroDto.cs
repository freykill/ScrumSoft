using ScrumSoft.Domain.Entities;
using ScrumSoft.Domain.Enums;

namespace ScrumSoft.Application.Proyectos
{
    // La membresia vista desde el proyecto: quien es la persona y desde cuando esta.
    // El Id es el del usuario, no el de la fila de union: al front le sirve el primero.
    public sealed record MiembroDto
    {
        public required Guid IdUsuario { get; init; }

        public required string Nombre { get; init; }

        public required string CorreoElectronico { get; init; }

        public required RolUsuario Rol { get; init; }

        public required DateTimeOffset FechaAsignacion { get; init; }

        public static MiembroDto Desde(ProyectoUsuario membresia, Usuario usuario)
        {
            ArgumentNullException.ThrowIfNull(membresia);
            ArgumentNullException.ThrowIfNull(usuario);

            return new MiembroDto
            {
                IdUsuario = usuario.Id,
                Nombre = usuario.Nombre,
                CorreoElectronico = usuario.CorreoElectronico,
                Rol = usuario.Rol,
                FechaAsignacion = membresia.FechaAsignacion
            };
        }
    }
}
