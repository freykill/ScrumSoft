using ScrumSoft.Domain.Enums;
using ScrumSoft.Domain.Common;

namespace ScrumSoft.Domain.Entities
{
    public sealed class Proyecto : BaseEntity
    {
        private readonly List<Columna> _columnas = [];
        private readonly List<ProyectoUsuario> _miembros = [];

        private Proyecto() { } // Requerido por EF Core

        public string Nombre { get; private set; } = null!;

        public string? Descripcion { get; private set; }

        public DateOnly FechaInicio { get; private set; }

        public DateOnly? FechaFinPrevista { get; private set; }

        public EstadoProyecto EstadoProyecto { get; private set; }

        public IReadOnlyList<Columna> Columnas =>
            [.. _columnas.Where(c => c.EstaActiva()).OrderBy(c => c.Orden)];

        public IReadOnlyList<ProyectoUsuario> Miembros =>
            [.. _miembros.Where(m => m.EstaActiva())];

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

        public void CambiarEstadoProyecto(EstadoProyecto estado) => EstadoProyecto = estado;

        public void Iniciar() => EstadoProyecto = EstadoProyecto.EnProgreso;

        public void Completar() => EstadoProyecto = EstadoProyecto.Completado;

        // ----------------------------------------------------------------
        // Columnas
        // ----------------------------------------------------------------

        public Columna AgregarColumna(string nombre)
        {
            var activas = _columnas.Where(c => c.EstaActiva()).ToList();

            var orden = activas.Count == 0
                ? CalculadoraDeOrden.Salto
                : activas.Max(c => c.Orden) + CalculadoraDeOrden.Salto;

            var columna = new Columna(Id, nombre, orden);
            _columnas.Add(columna);
            return columna;
        }

        public Columna RenombrarColumna(Guid idColumna, string nombre)
        {
            var columna = BuscarColumna(idColumna);
            columna.Renombrar(nombre);
            return columna;
        }

        public void ReordenarColumnas(IReadOnlyList<Guid> idsEnOrden)
        {
            ArgumentNullException.ThrowIfNull(idsEnOrden);

            var activas = _columnas.Where(c => c.EstaActiva()).ToList();

            if (idsEnOrden.Count != activas.Count)
                throw new DomainException("La lista recibida no coincide con las columnas del tablero.");

            if (idsEnOrden.Distinct().Count() != idsEnOrden.Count)
                throw new DomainException("La lista de columnas tiene identificadores repetidos.");

            for (var i = 0; i < idsEnOrden.Count; i++)
                BuscarColumna(idsEnOrden[i]).MoverA(CalculadoraDeOrden.PosicionEn(i));
        }

        public void EliminarColumna(Guid idColumna, bool contieneTareas)
        {
            var columna = BuscarColumna(idColumna);

            if (contieneTareas)
                throw new DomainException("No se puede eliminar una columna que contiene tareas.");

            columna.MarcarComoEliminada();
        }

        // ----------------------------------------------------------------
        // Miembros
        // ----------------------------------------------------------------

        public void AgregarMiembro(Guid idUsuario, DateTimeOffset ahora)
        {
            if (idUsuario == Guid.Empty)
                throw new DomainException("El usuario indicado no es valido.");

            var existente = _miembros.FirstOrDefault(m => m.IdUsuario == idUsuario);

            if (existente is null)
                _miembros.Add(new ProyectoUsuario(Id, idUsuario, ahora));
            else
                existente.Activar();
        }

        public void QuitarMiembro(Guid idUsuario)
        {
            var miembro = _miembros.FirstOrDefault(m => m.IdUsuario == idUsuario && m.EstaActiva());

            if (miembro is null)
                return;

            // Un proyecto sin miembros no aparece en la lista de nadie: quedaria
            // vivo en la base pero inalcanzable desde la aplicacion.
            if (Miembros.Count == 1)
                throw new DomainException("El proyecto debe conservar al menos un miembro.");

            miembro.MarcarComoEliminada();
        }

        public bool EsMiembro(Guid idUsuario) =>
            _miembros.Any(m => m.IdUsuario == idUsuario && m.EstaActiva());

        // ----------------------------------------------------------------

        private Columna BuscarColumna(Guid idColumna) =>
            _columnas.FirstOrDefault(c => c.Id == idColumna && c.EstaActiva())
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
