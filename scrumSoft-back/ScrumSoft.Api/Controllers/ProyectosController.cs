using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScrumSoft.Application.Common;
using ScrumSoft.Application.Mediador;
using ScrumSoft.Application.Proyectos;
using ScrumSoft.Application.Tablero;
using ScrumSoft.Domain.Enums;

namespace ScrumSoft.Api.Controllers
{
    // Los manejadores se piden con [FromServices] en cada accion en lugar de por
    // constructor: asi cada endpoint declara exactamente lo que usa.
    [ApiController]
    [Route("api/v1/proyectos")]
    [Authorize]
    public sealed class ProyectosController : ControllerBase
    {
        /// <summary>Lista los proyectos del usuario, paginados y con filtro por nombre.</summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<ProyectoDto>>> Listar(
            [FromServices] IMediador mediador,
            [FromQuery] ListarProyectosConsulta consulta,
            CancellationToken cancelacion) =>
            Ok(await mediador.EnviarAsync(consulta, cancelacion));

        /// <summary>Devuelve el tablero completo: columnas en orden con sus tareas.</summary>
        /// <remarks>
        /// Los filtros son opcionales y se resuelven en el servidor. El reporte acepta
        /// los mismos dos, asi que descargarlo con el tablero filtrado da el mismo contenido.
        /// </remarks>
        [HttpGet("{idProyecto:guid}/tablero")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TableroDto>> Tablero(
            [FromServices] IMediador mediador,
            Guid idProyecto,
            CancellationToken cancelacion,
            [FromQuery] Guid? idResponsable = null,
            [FromQuery] Prioridad? prioridad = null,
            [FromQuery] string? texto = null) =>
            Ok(await mediador.EnviarAsync(
                new ObtenerTableroConsulta
                {
                    IdProyecto = idProyecto,
                    IdResponsable = idResponsable,
                    Prioridad = prioridad,
                    Texto = texto
                },
                cancelacion));

        /// <summary>Crea un proyecto con su flujo de trabajo inicial.</summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProyectoDto>> Crear(
            [FromServices] IMediador mediador,
            [FromBody] CrearProyectoComando comando,
            CancellationToken cancelacion)
        {
            var dto = await mediador.EnviarAsync(comando with {  }, cancelacion);

            return CreatedAtAction(nameof(Tablero), new { idProyecto = dto.Id }, dto);
        }

        /// <summary>Edita los datos generales del proyecto.</summary>
        [HttpPut("{idProyecto:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProyectoDto>> Actualizar(
            [FromServices] IMediador mediador,
            Guid idProyecto,
            [FromBody] ActualizarProyectoComando comando,
            CancellationToken cancelacion) =>
            // El id de la ruta manda sobre el del cuerpo: evita que alguien
            // edite un proyecto distinto al que pidio.
            Ok(await mediador.EnviarAsync(comando with { Id = idProyecto }, cancelacion));

        /// <summary>Elimina el proyecto de forma logica.</summary>
        [HttpDelete("{idProyecto:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Eliminar(
            [FromServices] IMediador mediador,
            Guid idProyecto,
            CancellationToken cancelacion)
        {
            await mediador.EnviarAsync(new EliminarProyectoComando { Id = idProyecto }, cancelacion);

            return NoContent();
        }
    }
}
