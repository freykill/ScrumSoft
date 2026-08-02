using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScrumSoft.Application.Columnas;
using ScrumSoft.Application.Mediador;

namespace ScrumSoft.Api.Controllers
{
    // Ruta anidada bajo el proyecto: una columna no existe fuera de su tablero.
    [ApiController]
    [Route("api/v1/proyectos/{idProyecto:guid}/columnas")]
    [Authorize]
    public sealed class ColumnasController : ControllerBase
    {
        /// <summary>Agrega una columna al final del tablero.</summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ColumnaDto>> Agregar(
            [FromServices] IMediador mediador,
            Guid idProyecto,
            [FromBody] AgregarColumnaComando comando,
            CancellationToken cancelacion) =>
            Ok(await mediador.EnviarAsync(comando with { IdProyecto = idProyecto }, cancelacion));

        /// <summary>Cambia el nombre de una columna.</summary>
        [HttpPut("{idColumna:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ColumnaDto>> Renombrar(
            [FromServices] IMediador mediador,
            Guid idProyecto,
            Guid idColumna,
            [FromBody] RenombrarColumnaComando comando,
            CancellationToken cancelacion) =>
            Ok(await mediador.EnviarAsync(
                comando with { IdProyecto = idProyecto, IdColumna = idColumna },
                cancelacion));

        /// <summary>Reordena el tablero segun la secuencia de ids recibida.</summary>
        [HttpPut("orden")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IReadOnlyList<ColumnaDto>>> Reordenar(
            [FromServices] IMediador mediador,
            Guid idProyecto,
            [FromBody] ReordenarColumnasComando comando,
            CancellationToken cancelacion) =>
            Ok(await mediador.EnviarAsync(comando with { IdProyecto = idProyecto }, cancelacion));

        /// <summary>Elimina una columna. Falla con 400 si contiene tareas.</summary>
        [HttpDelete("{idColumna:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Eliminar(
            [FromServices] IMediador mediador,
            Guid idProyecto,
            Guid idColumna,
            CancellationToken cancelacion)
        {
            await mediador.EnviarAsync(
                new EliminarColumnaComando { IdProyecto = idProyecto, IdColumna = idColumna },
                cancelacion);

            return NoContent();
        }
    }
}
