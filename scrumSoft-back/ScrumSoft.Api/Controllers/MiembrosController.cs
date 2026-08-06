using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScrumSoft.Application.Mediador;
using ScrumSoft.Application.Proyectos;

namespace ScrumSoft.Api.Controllers
{
    // Ruta anidada bajo el proyecto, igual que las columnas: una membresia no
    // existe fuera del proyecto al que pertenece.
    [ApiController]
    [Route("api/v1/proyectos/{idProyecto:guid}/miembros")]
    [Authorize]
    public sealed class MiembrosController : ControllerBase
    {
        /// <summary>Lista el equipo del proyecto.</summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IReadOnlyList<MiembroDto>>> Listar(
            [FromServices] IMediador mediador,
            Guid idProyecto,
            CancellationToken cancelacion) =>
            Ok(await mediador.EnviarAsync(
                new ListarMiembrosConsulta { IdProyecto = idProyecto },
                cancelacion));

        /// <summary>Suma un usuario al equipo. Repetirlo no cambia nada.</summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MiembroDto>> Agregar(
            [FromServices] IMediador mediador,
            Guid idProyecto,
            [FromBody] AgregarMiembroComando comando,
            CancellationToken cancelacion) =>
            Ok(await mediador.EnviarAsync(comando with { IdProyecto = idProyecto }, cancelacion));

        /// <summary>Saca a un usuario del equipo. Falla con 400 si es el ultimo.</summary>
        [HttpDelete("{idUsuario:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Quitar(
            [FromServices] IMediador mediador,
            Guid idProyecto,
            Guid idUsuario,
            CancellationToken cancelacion)
        {
            await mediador.EnviarAsync(
                new QuitarMiembroComando { IdProyecto = idProyecto, IdUsuario = idUsuario },
                cancelacion);

            return NoContent();
        }
    }
}
