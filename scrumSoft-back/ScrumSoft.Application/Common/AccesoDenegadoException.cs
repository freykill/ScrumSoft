namespace ScrumSoft.Application.Common
{
    // El usuario esta autenticado pero no tiene permiso sobre este recurso.
    // La Api la traduce a un 403.
    public sealed class AccesoDenegadoException : Exception
    {
        public AccesoDenegadoException()
            : base("No tiene acceso a este recurso.")
        {
        }

        public AccesoDenegadoException(string mensaje) : base(mensaje)
        {
        }
    }
}
