using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScrumSoft.Application.Mediador;
using ScrumSoft.Application.Ports;
using ScrumSoft.Application.Reportes;

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
        /// solo cambia el exportador que la recibe.
        /// </remarks>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Descargar(
            [FromServices] IMediador mediador,
            Guid idProyecto,
            [FromQuery] FormatoDeReporte formato,
            CancellationToken cancelacion)
        {
            var archivo = await mediador.EnviarAsync(
                new GenerarReporteConsulta { IdProyecto = idProyecto, Formato = formato },
                cancelacion);

            // File fija Content-Type y Content-Disposition, que es lo que necesita
            // el navegador para descargar con el nombre correcto.
            return File(archivo.Contenido, archivo.TipoDeContenido, archivo.NombreDeArchivo);
        }
    }
}
