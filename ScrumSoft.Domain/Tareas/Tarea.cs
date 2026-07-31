using ScrumSoft.Domain.Common;

namespace ScrumSoft.Domain.Tareas
{
    /// <summary>Unidad de trabajo del tablero. Vive en una columna y tiene una posicion dentro de ella.</summary>
    public sealed class Tarea : Entity
    {
        /// <summary>Titulo visible en la tarjeta.</summary>
        public string Titulo { get; private set; } = null!;

        /// <summary>Detalle opcional.</summary>
        public string? Descripcion { get; private set; }

        /// <summary>Nivel de urgencia.</summary>
        public Prioridad Prioridad { get; private set; }

        /// <summary>Usuario asignado. Nulo si nadie la tomo todavia.</summary>
        public Guid? IdResponsable { get; private set; }

        /// <summary>Columna en la que se encuentra.</summary>
        public Guid IdColumna { get; private set; }

        /// <summary>Posicion dentro de su columna.</summary>
        public int Orden { get; private set; }

        private Tarea() { } // Requerido por EF Core

        /// <summary>Crea una tarea validada en la columna y posicion indicadas.</summary>
        /// <param name="titulo">Titulo de la tarjeta. Obligatorio.</param>
        /// <param name="descripcion">Detalle opcional.</param>
        /// <param name="prioridad">Nivel de urgencia.</param>
        /// <param name="idColumna">Columna donde nace la tarea.</param>
        /// <param name="orden">Posicion calculada por <see cref="CalculadoraDeOrden"/>.</param>
        /// <param name="idResponsable">Usuario asignado, opcional.</param>
        /// <returns>La tarea creada.</returns>
        public static Tarea Crear(
            string titulo,
            string? descripcion,
            Prioridad prioridad,
            Guid idColumna,
            int orden,
            Guid? idResponsable = null)
        {
            if (string.IsNullOrWhiteSpace(titulo))
                throw new DomainException("El titulo de la tarea es obligatorio.");

            if (idColumna == Guid.Empty)
                throw new DomainException("La tarea debe pertenecer a una columna.");

            return new Tarea
            {
                Titulo = titulo.Trim(),
                Descripcion = descripcion?.Trim(),
                Prioridad = prioridad,
                IdColumna = idColumna,
                Orden = orden,
                IdResponsable = idResponsable
            };
        }

        /// <summary>Mueve la tarea a una columna y posicion nuevas. Es el arrastre del tablero.</summary>
        /// <param name="idColumna">Columna destino. Puede ser la misma si solo se reordena.</param>
        /// <param name="orden">Nueva posicion, calculada por <see cref="CalculadoraDeOrden"/>.</param>
        public void MoverA(Guid idColumna, int orden)
        {
            if (idColumna == Guid.Empty)
                throw new DomainException("La tarea debe pertenecer a una columna.");

            IdColumna = idColumna;
            Orden = orden;
        }

        /// <summary>Cambia titulo, descripcion y prioridad.</summary>
        /// <param name="titulo">Nuevo titulo. Obligatorio.</param>
        /// <param name="descripcion">Nuevo detalle, opcional.</param>
        /// <param name="prioridad">Nuevo nivel de urgencia.</param>
        public void Actualizar(string titulo, string? descripcion, Prioridad prioridad)
        {
            if (string.IsNullOrWhiteSpace(titulo))
                throw new DomainException("El titulo de la tarea es obligatorio.");

            Titulo = titulo.Trim();
            Descripcion = descripcion?.Trim();
            Prioridad = prioridad;
        }

        /// <summary>Asigna o quita el responsable. Pasar null la deja sin asignar.</summary>
        /// <param name="idResponsable">Usuario asignado, o null para dejarla libre.</param>
        public void AsignarResponsable(Guid? idResponsable) => IdResponsable = idResponsable;
    }
}
