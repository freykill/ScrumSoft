namespace ScrumSoft.Domain.Common
{
    /// <summary>
    /// Entidad que registra cuando fue creada y modificada.
    /// Un interceptor de EF Core rellena estas fechas al guardar, no el codigo de negocio.
    /// </summary>
    public interface IAuditable
    {
        /// <summary>Momento de creacion, en UTC.</summary>
        DateTimeOffset FechaCreacion { get; }

        /// <summary>Momento de la ultima modificacion, en UTC. Nulo si nunca se modifico.</summary>
        DateTimeOffset? FechaActualizacion { get; }

        /// <summary>Fija la fecha de creacion. La invoca el interceptor de persistencia.</summary>
        /// <param name="ahora">Momento actual, provisto por <see cref="IClock"/>.</param>
        void RegistrarCreacion(DateTimeOffset ahora);

        /// <summary>Fija la fecha de modificacion. La invoca el interceptor de persistencia.</summary>
        /// <param name="ahora">Momento actual, provisto por <see cref="IClock"/>.</param>
        void RegistrarActualizacion(DateTimeOffset ahora);
    }
}
