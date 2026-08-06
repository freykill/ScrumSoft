namespace ScrumSoft.Domain.Common
{
    public sealed class DomainException(string mensaje) : Exception(mensaje);
}
