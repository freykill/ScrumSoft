namespace ScrumSoft.Domain.Common
{
    /// <summary>Base de toda entidad. Aporta identidad, igualdad por Id, auditoria y estado de registro.</summary>
    public abstract class Entity : IAuditable
    {
        /// <summary>Clave primaria.</summary>
        public Guid Id { get; protected set; } = Guid.NewGuid();

        /// <summary>Estado de la fila. Toda entidad nace activa.</summary>
        public EstadoRegistro Estado { get; private set; } = EstadoRegistro.Activo;

        /// <inheritdoc />
        public DateTimeOffset FechaCreacion { get; private set; }

        /// <inheritdoc />
        public DateTimeOffset? FechaActualizacion { get; private set; }

        /// <inheritdoc />
        public void RegistrarCreacion(DateTimeOffset ahora) => FechaCreacion = ahora;

        /// <inheritdoc />
        public void RegistrarActualizacion(DateTimeOffset ahora) => FechaActualizacion = ahora;

        /// <summary>Devuelve la fila a circulacion.</summary>
        public void Activar() => Estado = EstadoRegistro.Activo;

        /// <summary>Saca la fila de circulacion sin eliminarla. Sigue siendo consultable.</summary>
        public void Desactivar() => Estado = EstadoRegistro.Inactivo;

        /// <summary>Borra la fila logicamente. Deja de aparecer en las consultas.</summary>
        public void MarcarComoEliminada() => Estado = EstadoRegistro.Eliminado;

        /// <summary>Dos entidades son la misma si son del mismo tipo y tienen el mismo Id.</summary>
        public override bool Equals(object? obj) =>
            obj is Entity otra && GetType() == otra.GetType() && Id == otra.Id;

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(GetType(), Id);
    }
}
