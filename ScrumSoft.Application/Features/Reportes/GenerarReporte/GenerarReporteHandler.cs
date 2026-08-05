using ScrumSoft.Application.Common;
using ScrumSoft.Application.Mediador;
using ScrumSoft.Application.Ports;
using ScrumSoft.Domain.Common;

namespace ScrumSoft.Application.Reportes
{
    // Arma el reporte una sola vez y lo entrega al exportador que corresponda.
    // La consulta y la estructura son unicas: solo cambia quien las recibe.
    public sealed class GenerarReporteHandler(
        AccesoAProyectos acceso,
        ITareaRepository tareas,
        IUsuarioRepository usuarios,
        IEnumerable<IExportadorDeReporte> exportadores,
        IClock reloj) : IManejador<GenerarReporteConsulta, ArchivoDeReporte>
    {
        private const string SinAsignar = "-";

        public async Task<ArchivoDeReporte> ManejarAsync(
            GenerarReporteConsulta peticion,
            CancellationToken cancelacion)
        {
            ArgumentNullException.ThrowIfNull(peticion);

            var exportador = exportadores.FirstOrDefault(e => e.Formato == peticion.Formato)
                ?? throw new DomainException($"No hay un exportador registrado para el formato {peticion.Formato}.");

            var reporte = await ArmarAsync(peticion, cancelacion).ConfigureAwait(false);

            return new ArchivoDeReporte
            {
                Contenido = exportador.Exportar(reporte),
                NombreDeArchivo = $"reporte-{Sanear(reporte.Nombre)}{exportador.Extension}",
                TipoDeContenido = exportador.TipoDeContenido
            };
        }

        private async Task<ReporteProyectoDto> ArmarAsync(
            GenerarReporteConsulta peticion,
            CancellationToken cancelacion)
        {
            var proyecto = await acceso
                .ObtenerConAccesoAsync(peticion.IdProyecto, cancelacion)
                .ConfigureAwait(false);

            // La misma consulta y los mismos filtros que usa el tablero: por eso el
            // archivo descargado coincide con lo que el usuario tiene en pantalla.
            var todas = await tareas
                .ListarPorProyectoAsync(
                    peticion.IdProyecto,
                    peticion.IdResponsable,
                    peticion.Prioridad,
                    peticion.Texto,
                    cancelacion)
                .ConfigureAwait(false);

            // Los responsables se traen de una sola vez, no uno por tarea.
            var idsResponsables = todas
                .Where(t => t.IdResponsable is not null)
                .Select(t => t.IdResponsable!.Value)
                .Distinct()
                .ToList();

            var responsables = idsResponsables.Count == 0
                ? []
                : await usuarios.ListarPorIdsAsync(idsResponsables, cancelacion).ConfigureAwait(false);

            var nombrePorUsuario = responsables.ToDictionary(u => u.Id, u => u.Nombre);
            var nombrePorColumna = proyecto.Columnas.ToDictionary(c => c.Id, c => c.Nombre);
            var ordenPorColumna = proyecto.Columnas.ToDictionary(c => c.Id, c => c.Orden);

            var filas = todas
                .OrderBy(t => ordenPorColumna.TryGetValue(t.IdColumna, out var orden) ? orden : int.MaxValue)
                .ThenBy(t => t.Orden)
                .Select(t => new FilaDeReporteDto
                {
                    Titulo = t.Titulo,
                    Columna = nombrePorColumna.TryGetValue(t.IdColumna, out var nombre) ? nombre : SinAsignar,
                    Responsable = t.IdResponsable is { } id && nombrePorUsuario.TryGetValue(id, out var quien)
                        ? quien
                        : SinAsignar,
                    Prioridad = t.Prioridad
                })
                .ToList();

            return new ReporteProyectoDto
            {
                IdProyecto = proyecto.Id,
                Nombre = proyecto.Nombre,
                Descripcion = proyecto.Descripcion,
                FechaInicio = proyecto.FechaInicio,
                FechaFinPrevista = proyecto.FechaFinPrevista,
                EstadoProyecto = proyecto.EstadoProyecto,
                FechaGeneracion = reloj.UtcNow,
                Tareas = filas
            };
        }

        private static string Sanear(string nombre)
        {
            var invalidos = Path.GetInvalidFileNameChars();
            var limpio = new string([.. nombre.Where(c => !invalidos.Contains(c))]);

            return string.IsNullOrWhiteSpace(limpio) ? "proyecto" : limpio.Trim().Replace(' ', '-');
        }
    }
}
