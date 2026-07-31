using ScrumSoft.Domain.Common;

namespace ScrumSoft.Domain.Proyectos
{
    /// <summary>Raiz del agregado: un proyecto, sus columnas y sus miembros.</summary>
    public sealed class Proyecto : Entity
    {
        private readonly List<Columna> _columnas = [];
        private readonly List<Guid> _miembros = [];

        /// <summary>Nombre del proyecto.</summary>
        public string Nombre { get; private set; } = null!;

        /// <summary>Descripcion libre, opcional.</summary>
        public string? Descripcion { get; private set; }

        /// <summary>Fecha de arranque.</summary>
        public DateOnly FechaInicio { get; private set; }

        /// <summary>Fecha de cierre estimada, opcional.</summary>
        public DateOnly? FechaFinPrevista { get; private set; }

        /// <summary>Estado del ciclo de vida del proyecto. No confundir con Estado, que es el de la fila.</summary>
        public EstadoProyecto EstadoProyecto { get; private set; }

        /// <summary>Columnas del tablero, ya ordenadas. Solo lectura desde fuera del agregado.</summary>
        public IReadOnlyList<Columna> Columnas => _columnas.OrderBy(c => c.Orden).ToList();

        /// <summary>Usuarios con acceso al proyecto. Solo lectura desde fuera del agregado.</summary>
        public IReadOnlyList<Guid> Miembros => _miembros.AsReadOnly();

        private Proyecto() { } // Requerido por EF Core

        /// <summary>Crea un proyecto validado, en estado de planificacion.</summary>
        /// <param name="nombre">Nombre del proyecto. Obligatorio.</param>
        /// <param name="descripcion">Descripcion opcional.</param>
        /// <param name="fechaInicio">Fecha de arranque.</param>
        /// <param name="fechaFinPrevista">Fecha de cierre estimada, opcional.</param>
        /// <returns>El proyecto creado.</returns>
        public static Proyecto Crear(
            string nombre,
            string? descripcion,
            DateOnly fechaInicio,
            DateOnly? fechaFinPrevista)
        {
            Validar(nombre, fechaInicio, fechaFinPrevista);

            return new Proyecto
            {
                Nombre = nombre.Trim(),
                Descripcion = descripcion?.Trim(),
                FechaInicio = fechaInicio,
                FechaFinPrevista = fechaFinPrevista,
                EstadoProyecto = EstadoProyecto.Planificacion
            };
        }

        /// <summary>Cambia los datos generales del proyecto.</summary>
        /// <param name="nombre">Nuevo nombre. Obligatorio.</param>
        /// <param name="descripcion">Nueva descripcion, opcional.</param>
        /// <param name="fechaInicio">Nueva fecha de arranque.</param>
        /// <param name="fechaFinPrevista">Nueva fecha de cierre estimada, opcional.</param>
        public void Actualizar(
            string nombre,
            string? descripcion,
            DateOnly fechaInicio,
            DateOnly? fechaFinPrevista)
        {
            Validar(nombre, fechaInicio, fechaFinPrevista);

            Nombre = nombre.Trim();
            Descripcion = descripcion?.Trim();
            FechaInicio = fechaInicio;
            FechaFinPrevista = fechaFinPrevista;
        }

        /// <summary>Cambia el estado del ciclo de vida del proyecto.</summary>
        /// <param name="estado">Nuevo estado.</param>
        public void CambiarEstadoProyecto(EstadoProyecto estado) => EstadoProyecto = estado;

        // ----------------------------------------------------------------
        // Columnas
        // ----------------------------------------------------------------

        /// <summary>Agrega una columna al final del tablero.</summary>
        /// <param name="nombre">Nombre de la columna. Obligatorio.</param>
        /// <returns>La columna creada.</returns>
        public Columna AgregarColumna(string nombre)
        {
            var orden = _columnas.Count == 0
                ? CalculadoraDeOrden.Salto
                : _columnas.Max(c => c.Orden) + CalculadoraDeOrden.Salto;

            var columna = new Columna(Id, nombre, orden);
            _columnas.Add(columna);
            return columna;
        }

        /// <summary>Cambia el nombre de una columna del tablero.</summary>
        /// <param name="idColumna">Columna a renombrar.</param>
        /// <param name="nombre">Nuevo nombre. Obligatorio.</param>
        public void RenombrarColumna(Guid idColumna, string nombre) =>
            BuscarColumna(idColumna).Renombrar(nombre);

        /// <summary>
        /// Reordena el tablero completo segun la secuencia de ids recibida.
        /// Las columnas son pocas, asi que se renumeran todas: es exacto y no acumula error.
        /// </summary>
        /// <param name="idsEnOrden">Ids de todas las columnas, en el orden deseado.</param>
        public void ReordenarColumnas(IReadOnlyList<Guid> idsEnOrden)
        {
            ArgumentNullException.ThrowIfNull(idsEnOrden);

            if (idsEnOrden.Count != _columnas.Count)
                throw new DomainException("La lista recibida no coincide con las columnas del tablero.");

            for (var i = 0; i < idsEnOrden.Count; i++)
                BuscarColumna(idsEnOrden[i]).MoverA(CalculadoraDeOrden.PosicionEn(i));
        }

        /// <summary>
        /// Elimina una columna del tablero. Quien llama debe informar si la columna
        /// contiene tareas, porque las tareas viven en otro agregado.
        /// </summary>
        /// <param name="idColumna">Columna a eliminar.</param>
        /// <param name="contieneTareas">True si la columna tiene tareas asociadas.</param>
        public void EliminarColumna(Guid idColumna, bool contieneTareas)
        {
            var columna = BuscarColumna(idColumna);

            if (contieneTareas)
                throw new DomainException("No se puede eliminar una columna que contiene tareas.");

            _columnas.Remove(columna);
        }

        // ----------------------------------------------------------------
        // Miembros
        // ----------------------------------------------------------------

        /// <summary>Da acceso al proyecto a un usuario. Si ya lo tenia, no hace nada.</summary>
        /// <param name="idUsuario">Usuario al que se le da acceso.</param>
        public void AgregarMiembro(Guid idUsuario)
        {
            if (idUsuario == Guid.Empty)
                throw new DomainException("El usuario indicado no es valido.");

            if (!_miembros.Contains(idUsuario))
                _miembros.Add(idUsuario);
        }

        /// <summary>Quita el acceso de un usuario al proyecto.</summary>
        /// <param name="idUsuario">Usuario al que se le retira el acceso.</param>
        public void QuitarMiembro(Guid idUsuario) => _miembros.Remove(idUsuario);

        /// <summary>Indica si un usuario tiene acceso al proyecto.</summary>
        /// <param name="idUsuario">Usuario a comprobar.</param>
        /// <returns>True si el usuario es miembro.</returns>
        public bool EsMiembro(Guid idUsuario) => _miembros.Contains(idUsuario);

        // ----------------------------------------------------------------

        private Columna BuscarColumna(Guid idColumna) =>
            _columnas.FirstOrDefault(c => c.Id == idColumna)
                ?? throw new DomainException("La columna no pertenece a este proyecto.");

        private static void Validar(string nombre, DateOnly fechaInicio, DateOnly? fechaFinPrevista)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new DomainException("El nombre del proyecto es obligatorio.");

            if (fechaFinPrevista is not null && fechaFinPrevista < fechaInicio)
                throw new DomainException("La fecha fin prevista no puede ser anterior a la de inicio.");
        }
    }
}
