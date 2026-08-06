namespace ScrumSoft.Domain.Common
{
    public abstract class BaseEntity : IAuditable
    {
        public Guid Id { get; protected set; } = Guid.NewGuid();

        public EstadoRegistro Estado { get; private set; } = EstadoRegistro.Activo;

        public DateTimeOffset FechaCreacion { get; private set; }

        public DateTimeOffset? FechaActualizacion { get; private set; }

        public void RegistrarCreacion(DateTimeOffset ahora) => FechaCreacion = ahora;

        public void RegistrarActualizacion(DateTimeOffset ahora) => FechaActualizacion = ahora;

        public void Activar() => Estado = EstadoRegistro.Activo;

        public void Desactivar() => Estado = EstadoRegistro.Inactivo;

        public void MarcarComoEliminada() => Estado = EstadoRegistro.Eliminado;

        public bool EstaActiva() => Estado == EstadoRegistro.Activo;

        public override bool Equals(object? obj) =>
            obj is BaseEntity otra && GetType() == otra.GetType() && Id == otra.Id;

        public override int GetHashCode() => HashCode.Combine(GetType(), Id);
    }
}
