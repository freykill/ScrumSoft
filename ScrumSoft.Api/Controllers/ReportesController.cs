using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScrumSoft.Application.Mediador;
using ScrumSoft.Application.Ports;
using ScrumSoft.Application.Reportes;
using ScrumSoft.Domain.Enums;

namespace ScrumSoft.Api.Controllers
{
    [ApiController]
    [Route("api/v1/proyectos/{idProyecto:guid}/reporte")]
    [Authorize]
    public sealed class ReportesController : ControllerBase
    {
        /// <summary>Descarga el reporte del proyecto en el formato indicado.</summary>
        /// <remarks>
        /// Los dos formatos salen de la misma consulta y la misma estructura de datos:
        /// solo cambia el exportador que la recibe. Acepta los mismos filtros que el
        /// tablero, para que el archivo coincida con lo que se esta viendo en pantalla.
        /// </remarks>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Descargar(
            [FromServices] IMediador mediador,
            Guid idProyecto,
            [FromQuery] FormatoDeReporte formato,
            CancellationToken cancelacion,
            [FromQuery] Guid? idResponsable = null,
            [FromQuery] Prioridad? prioridad = null)
        {
            var archivo = await mediador.EnviarAsync(
                new GenerarReporteConsulta
                {
                    IdProyecto = idProyecto,
                    Formato = formato,
                    IdResponsable = idResponsable,
                    Prioridad = prioridad
                },
                cancelacion);

            // File fija Content-Type y Content-Disposition, que es lo que necesita
            // el navegador para descargar con el nombre correcto.
            return File(archivo.Contenido, archivo.TipoDeContenido, archivo.NombreDeArchivo);
        }
    }
}
