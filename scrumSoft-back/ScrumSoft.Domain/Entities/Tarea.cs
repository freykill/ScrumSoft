using ScrumSoft.Domain.Enums;
using ScrumSoft.Domain.Common;

namespace ScrumSoft.Domain.Entities
{
    public sealed class Tarea : BaseEntity
    {
        private Tarea() { } // Requerido por EF Core

        public string Titulo { get; private set; } = null!;

        public string? Descripcion { get; private set; }

        public Prioridad Prioridad { get; private set; }

        public Guid? IdResponsable { get; private set; }

        public Guid IdColumna { get; private set; }

        public int Orden { get; private set; }

        public Columna? Columna { get; private set; }

        public Usuario? Responsable { get; private set; }

        public static Tarea Crear(
            Guid idColumna,
            string titulo,
            string? descripcion,
            Prioridad prioridad,
            int orden,
            Guid? idResponsable = null)
        {
            if (idColumna == Guid.Empty)
                throw new DomainException("La tarea debe pertenecer a una columna.");

            return new Tarea
            {
                IdColumna = idColumna,
                Titulo = ValidarTitulo(titulo),
                Descripcion = descripcion?.Trim(),
                Prioridad = prioridad,
                Orden = orden,
                IdResponsable = idResponsable
            };
        }

        public void Actualizar(string titulo, string? descripcion, Prioridad prioridad)
        {
            Titulo = ValidarTitulo(titulo);
            Descripcion = descripcion?.Trim();
            Prioridad = prioridad;
        }

        public void MoverA(Guid idColumnaDestino, int orden)
        {
            if (idColumnaDestino == Guid.Empty)
                throw new DomainException("La tarea debe pertenecer a una columna.");

            IdColumna = idColumnaDestino;
            Orden = orden;
        }

        public void Reposicionar(int orden) => Orden = orden;

        public void Asignar(Guid idResponsable)
        {
            if (idResponsable == Guid.Empty)
                throw new DomainException("El usuario indicado no es valido.");

            IdResponsable = idResponsable;
        }

        public void Desasignar() => IdResponsable = null;

        public bool TieneResponsable() => IdResponsable is not null;

        private static string ValidarTitulo(string titulo) =>
            string.IsNullOrWhiteSpace(titulo)
                ? throw new DomainException("El titulo de la tarea es obligatorio.")
                : titulo.Trim();
    }
}
