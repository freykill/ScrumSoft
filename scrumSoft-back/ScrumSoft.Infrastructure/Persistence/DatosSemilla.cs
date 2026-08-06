using ScrumSoft.Domain.Common;
using ScrumSoft.Domain.Enums;

namespace ScrumSoft.Infrastructure.Persistence
{
    // Datos que la migracion inserta al crear la base (requisito 6.2).
    //
    // Todo aqui es constante a proposito: HasData exige valores fijos. Si un Guid,
    // una fecha o un hash se generaran al vuelo, cada vez que se creara una migracion
    // EF veria valores distintos y creeria que el modelo cambio.
    //
    // Contrasenas en claro, documentadas en el README:
    //   admin@scrumsoft.com    Admin123*
    //   miembro@scrumsoft.com  Miembro123*
    public static class DatosSemilla
    {
        public static readonly Guid IdAdministrador = new("11111111-1111-1111-1111-111111111111");

        public static readonly Guid IdMiembro = new("22222222-2222-2222-2222-222222222222");

        public static readonly DateTimeOffset FechaDeSiembra =
            new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

        public static object[] Usuarios() =>
        [
            new
            {
                Id = IdAdministrador,
                Nombre = "Administrador",
                CorreoElectronico = "admin@scrumsoft.com",
                PasswordHash = "$2a$12$ARg7iJNOV843LqiS8ssGfOHp1tSWigIUANkA7soFy708ZExz7g1yO",
                Rol = RolUsuario.Administrador,
                Estado = EstadoRegistro.Activo,
                FechaCreacion = FechaDeSiembra,
                FechaActualizacion = (DateTimeOffset?)null
            },
            new
            {
                Id = IdMiembro,
                Nombre = "Miembro de prueba",
                CorreoElectronico = "miembro@scrumsoft.com",
                PasswordHash = "$2a$12$104Q7YvMTtAmNJxFxsTjl.N3BPTPtGnU1iVskwbfPp3gg4CQpekGa",
                Rol = RolUsuario.Miembro,
                Estado = EstadoRegistro.Activo,
                FechaCreacion = FechaDeSiembra,
                FechaActualizacion = (DateTimeOffset?)null
            }
        ];
    }
}
