using ScrumSoft.Domain.Common;

namespace ScrumSoft.Domain.Entities
{
    public sealed class ProyectoUsuario : BaseEntity
    {
        private ProyectoUsuario() { } // Requerido por EF Core

        internal ProyectoUsuario(Guid idProyecto, Guid idUsuario, DateTimeOffset fechaAsignacion)
        {
            IdProyecto = idProyecto;
            IdUsuario = idUsuario;
            FechaAsignacion = fechaAsignacion;
        }

        public Guid IdProyecto { get; private set; }

        public Guid IdUsuario { get; private set; }

        public DateTimeOffset FechaAsignacion { get; private set; }

        public Proyecto? Proyecto { get; private set; }

        public Usuario? Usuario { get; private set; }
    }
}
