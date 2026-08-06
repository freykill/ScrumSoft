using ScrumSoft.Application.Common;
using ScrumSoft.Application.Mediador;
using ScrumSoft.Application.Ports;
using ScrumSoft.Application.Tareas;

namespace ScrumSoft.Application.Tablero
{
    public sealed class ObtenerTableroHandler(
        AccesoAProyectos acceso,
        ITareaRepository tareas) : IManejador<ObtenerTableroConsulta, TableroDto>
    {
        public async Task<TableroDto> ManejarAsync(
            ObtenerTableroConsulta peticion,
            CancellationToken cancelacion)
        {
            ArgumentNullException.ThrowIfNull(peticion);

            var proyecto = await acceso
                .ObtenerConAccesoAsync(peticion.IdProyecto, cancelacion)
                .ConfigureAwait(false);

            // Una sola consulta para todas las tareas del proyecto, no una por columna.
            // Los filtros los aplica el repositorio en SQL, no este metodo en memoria.
            var todas = await tareas
                .ListarPorProyectoAsync(
                    peticion.IdProyecto,
                    peticion.IdResponsable,
                    peticion.Prioridad,
                    peticion.Texto,
                    cancelacion)
                .ConfigureAwait(false);

            var porColumna = todas
                .GroupBy(t => t.IdColumna)
                .ToDictionary(g => g.Key, g => (IReadOnlyList<TareaDto>)[.. g.OrderBy(t => t.Orden).Select(TareaDto.Desde)]);

            var columnas = proyecto.Columnas
                .Select(c => new ColumnaConTareasDto
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Orden = c.Orden,
                    Tareas = porColumna.TryGetValue(c.Id, out var lista) ? lista : []
                })
                .ToList();

            return new TableroDto
            {
                IdProyecto = proyecto.Id,
                NombreProyecto = proyecto.Nombre,
                Columnas = columnas
            };
        }
    }
}
