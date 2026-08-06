namespace ScrumSoft.Application.Common
{
    // Se pidio algo que no existe. La Api la traduce a un 404.
    public sealed class RecursoNoEncontradoException : Exception
    {
        public RecursoNoEncontradoException(string mensaje) : base(mensaje)
        {
        }

        public RecursoNoEncontradoException(string recurso, Guid id)
            : base($"No se encontro {recurso} con id {id}.")
        {
        }
    }
}
