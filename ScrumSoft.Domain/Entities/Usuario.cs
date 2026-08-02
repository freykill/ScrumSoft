using ScrumSoft.Domain.Enums;
using ScrumSoft.Domain.Common;

namespace ScrumSoft.Domain.Entities
{
    public sealed class Usuario : BaseEntity
    {
        private readonly List<ProyectoUsuario> _proyectos = [];

        private Usuario() { } // Requerido por EF Core

        public string Nombre { get; private set; } = null!;

        public string CorreoElectronico { get; private set; } = null!;

        public string PasswordHash { get; private set; } = null!;

        public RolUsuario Rol { get; private set; }

        public IReadOnlyList<ProyectoUsuario> Proyectos => _proyectos.AsReadOnly();

        public static Usuario Crear(
            string nombre,
            string correoElectronico,
            string passwordHash,
            RolUsuario rol) =>
            new()
            {
                Nombre = ValidarNombre(nombre),
                CorreoElectronico = NormalizarCorreo(correoElectronico),
                PasswordHash = ValidarHash(passwordHash),
                Rol = rol
            };

        public void Renombrar(string nombre) => Nombre = ValidarNombre(nombre);

        public void CambiarPassword(string passwordHash) => PasswordHash = ValidarHash(passwordHash);

        public void CambiarRol(RolUsuario rol) => Rol = rol;

        public bool EsAdministrador() => Rol == RolUsuario.Administrador;

        private static string ValidarNombre(string nombre) =>
            string.IsNullOrWhiteSpace(nombre)
                ? throw new DomainException("El nombre del usuario es obligatorio.")
                : nombre.Trim();

        private static string ValidarHash(string passwordHash) =>
            string.IsNullOrWhiteSpace(passwordHash)
                ? throw new DomainException("La contrasena es obligatoria.")
                : passwordHash;

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
