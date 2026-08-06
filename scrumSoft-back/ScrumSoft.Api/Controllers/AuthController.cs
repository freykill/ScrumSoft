using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ScrumSoft.Application.Auth;
using ScrumSoft.Application.Mediador;

namespace ScrumSoft.Api.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public sealed class AuthController : ControllerBase
    {
        // El login es el unico endpoint anonimo y BCrypt no basta como freno:
        // hace lenta cada verificacion, pero no limita cuantas se intentan.
        public const string PoliticaDeLimiteDeLogin = "intentos-de-login";

        /// <summary>Valida las credenciales y devuelve un token JWT.</summary>
        /// <remarks>Usuarios de prueba: admin@scrumsoft.com / Admin123*</remarks>
        [HttpPost("login")]
        [AllowAnonymous]
        [EnableRateLimiting(PoliticaDeLimiteDeLogin)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<ActionResult<SesionDto>> Login([FromServices] IMediador mediador,
            [FromBody] CredencialesComando comando,
            CancellationToken cancelacion)
        {
            var dto = await mediador.EnviarAsync(comando, cancelacion);
            return Ok(dto);
        }
            
    }
}
