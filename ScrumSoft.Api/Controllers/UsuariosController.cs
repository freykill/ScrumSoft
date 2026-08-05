using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScrumSoft.Application.Common;
using ScrumSoft.Application.Mediador;
using ScrumSoft.Application.Usuarios;

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
    }
}
