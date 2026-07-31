namespace ScrumSoft.Domain.Usuarios
{
    /// <summary>Nivel de permisos de un usuario dentro de la aplicacion.</summary>
    public enum RolUsuario
    {
        /// <summary>Participa en los proyectos a los que pertenece.</summary>
        Miembro,

        /// <summary>Administra usuarios y proyectos sin restriccion.</summary>
        Administrador
    }
}
