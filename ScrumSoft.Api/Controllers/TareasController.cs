using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScrumSoft.Application.Mediador;
using ScrumSoft.Application.Tareas;

namespace ScrumSoft.Api.Controllers
{
    [ApiController]
    [Route("api/v1/proyectos/{idProyecto:guid}/tareas")]
    [Authorize]
    public sealed class TareasController : ControllerBase
    {
        /// <summary>Crea una tarea al final de la columna indicada.</summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<TareaDto>> Crear(
            [FromServices] IMediador mediador,
            Guid idProyecto,
            [FromBody] CrearTareaComando comando,
            CancellationToken cancelacion) =>
            Ok(await mediador.EnviarAsync(comando with { IdProyecto = idProyecto }, cancelacion));

        /// <summary>Edita titulo, descripcion, prioridad y responsable.</summary>
        [HttpPut("{idTarea:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TareaDto>> Actualizar(
            [FromServices] IMediador mediador,
            Guid idProyecto,
            Guid idTarea,
            [FromBody] ActualizarTareaComando comando,
            CancellationToken cancelacion) =>
            Ok(await mediador.EnviarAsync(
                comando with { IdProyecto = idProyecto, IdTarea = idTarea },
                cancelacion));

        /// <summary>Mueve la tarea entre columnas o dentro de la misma. Es el arrastre del tablero.</summary>
        /// <remarks>
        /// El cliente informa entre que dos tarjetas se solto; el servidor calcula la posicion.
        /// IdTareaAnterior nulo significa "al inicio"; IdTareaSiguiente nulo, "al final".
        /// </remarks>
        [HttpPut("{idTarea:guid}/mover")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TareaDto>> Mover(
            [FromServices] IMediador mediador,
            Guid idProyecto,
            Guid idTarea,
            [FromBody] MoverTareaComando comando,
            CancellationToken cancelacion) =>
            Ok(await mediador.EnviarAsync(
                comando with { IdProyecto = idProyecto, IdTarea = idTarea },
                cancelacion));

        /// <summary>Elimina la tarea de forma logica.</summary>
        [HttpDelete("{idTarea:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Eliminar(
            [FromServices] IMediador mediador,
            Guid idProyecto,
            Guid idTarea,
            CancellationToken cancelacion)
        {
            await mediador.EnviarAsync(
                new EliminarTareaComando { IdProyecto = idProyecto, IdTarea = idTarea },
                cancelacion);

            return NoContent();
        }
    }
}
