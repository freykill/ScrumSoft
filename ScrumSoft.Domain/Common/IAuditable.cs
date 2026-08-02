namespace ScrumSoft.Domain.Common
{
    public interface IAuditable
    {
        DateTimeOffset FechaCreacion { get; }

        DateTimeOffset? FechaActualizacion { get; }

        void RegistrarCreacion(DateTimeOffset ahora);

        void RegistrarActualizacion(DateTimeOffset ahora);
    }
}
