using ScrumSoft.Domain.Common;

namespace ScrumSoft.Domain.Entities
{
    public sealed class Columna : BaseEntity
    {
        private Columna() { } // Requerido por EF Core

        internal Columna(Guid idProyecto, string nombre, int orden)
        {
            IdProyecto = idProyecto;
            Nombre = Validar(nombre);
            Orden = orden;
        }

        public Guid IdProyecto { get; private set; }

        public string Nombre { get; private set; } = null!;

        public int Orden { get; private set; }

        public Proyecto? Proyecto { get; private set; }

        internal void Renombrar(string nombre) => Nombre = Validar(nombre);

        internal void MoverA(int orden) => Orden = orden;

        private static string Validar(string nombre) =>
            string.IsNullOrWhiteSpace(nombre)
                ? throw new DomainException("El nombre de la columna es obligatorio.")
                : nombre.Trim();
    }
}
