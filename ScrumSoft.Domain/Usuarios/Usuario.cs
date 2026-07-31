using ScrumSoft.Domain.Common;

namespace ScrumSoft.Domain.Usuarios
{
    /// <summary>Persona que accede a la aplicacion. El dominio nunca ve la contrasena en claro.</summary>
    public sealed class Usuario : Entity
    {
        /// <summary>Nombre para mostrar.</summary>
        public string Nombre { get; private set; } = null!;

        /// <summary>Correo con el que inicia sesion. Siempre en minusculas.</summary>
        public string CorreoElectronico { get; private set; } = null!;

        /// <summary>Contrasena ya hasheada. El hash lo produce un adaptador, no el dominio.</summary>
        public string PasswordHash { get; private set; } = null!;

        /// <summary>Nivel de permisos.</summary>
        public RolUsuario Rol { get; private set; }

        private Usuario() { } // Requerido por EF Core

        /// <summary>Registra un usuario validado.</summary>
        /// <param name="nombre">Nombre para mostrar. Obligatorio.</param>
        /// <param name="correoElectronico">Correo de acceso. Se normaliza a minusculas.</param>
        /// <param name="passwordHash">Contrasena ya hasheada por el adaptador correspondiente.</param>
        /// <param name="rol">Nivel de permisos.</param>
        /// <returns>El usuario creado.</returns>
        public static Usuario Crear(
            string nombre,
            string correoElectronico,
            string passwordHash,
            RolUsuario rol)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new DomainException("El nombre del usuario es obligatorio.");

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new DomainException("La contrasena es obligatoria.");

            return new Usuario
            {
                Nombre = nombre.Trim(),
                CorreoElectronico = NormalizarCorreo(correoElectronico),
                PasswordHash = passwordHash,
                Rol = rol
            };
        }

        /// <summary>Cambia el nombre para mostrar.</summary>
        /// <param name="nombre">Nuevo nombre. Obligatorio.</param>
        public void Renombrar(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new DomainException("El nombre del usuario es obligatorio.");

            Nombre = nombre.Trim();
        }

        /// <summary>Reemplaza la contrasena por un hash nuevo.</summary>
        /// <param name="passwordHash">Hash producido por el adaptador de contrasenas.</param>
        public void CambiarPassword(string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new DomainException("La contrasena es obligatoria.");

            PasswordHash = passwordHash;
        }

        /// <summary>Cambia el nivel de permisos.</summary>
        /// <param name="rol">Nuevo rol.</param>
        public void CambiarRol(RolUsuario rol) => Rol = rol;

        private static string NormalizarCorreo(string correo)
        {
            if (string.IsNullOrWhiteSpace(correo))
                throw new DomainException("El correo electronico es obligatorio.");

            var limpio = correo.Trim().ToLowerInvariant();
            var arroba = limpio.IndexOf('@');

            if (arroba <= 0 || arroba == limpio.Length - 1)
                throw new DomainException("El correo electronico no es valido.");

            return limpio;
        }
    }
}
