using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScrumSoft.Application.Common;
using ScrumSoft.Application.Mediador;
using ScrumSoft.Application.Usuarios;
using ScrumSoft.Domain.Enums;

namespace ScrumSoft.Api.Controllers
{
    [ApiController]
    [Route("api/v1/usuarios")]
    [Authorize]
    public sealed class UsuariosController : ControllerBase
    {
        /// <summary>Lista usuarios para elegir a quien sumar a un proyecto.</summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PagedResult<UsuarioDto>>> Listar(
            [FromServices] IMediador mediador,
            [FromQuery] ListarUsuariosConsulta consulta,
            CancellationToken cancelacion) =>
            Ok(await mediador.EnviarAsync(consulta, cancelacion));

        /// <summary>Da de alta un usuario. Solo para administradores.</summary>
        [HttpPost]
        [Authorize(Roles = nameof(RolUsuario.Administrador))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<UsuarioDto>> Crear(
            [FromServices] IMediador mediador,
            [FromBody] CrearUsuarioComando comando,
            CancellationToken cancelacion)
        {
            var dto = await mediador.EnviarAsync(comando, cancelacion);

            return CreatedAtAction(nameof(Listar), new { }, dto);
        }

        /// <summary>Cambia el nombre y el rol de un usuario. Solo para administradores.</summary>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = nameof(RolUsuario.Administrador))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UsuarioDto>> Actualizar(
            [FromServices] IMediador mediador,
            Guid id,
            [FromBody] ActualizarUsuarioComando comando,
            CancellationToken cancelacion) =>
            // El id de la ruta manda sobre el del cuerpo, igual que en proyectos.
            Ok(await mediador.EnviarAsync(comando with { Id = id }, cancelacion));
    }
}
