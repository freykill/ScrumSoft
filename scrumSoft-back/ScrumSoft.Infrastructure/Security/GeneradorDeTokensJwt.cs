using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using ScrumSoft.Application.Ports;
using ScrumSoft.Domain.Common;
using ScrumSoft.Domain.Entities;

namespace ScrumSoft.Infrastructure.Security
{
    // Adaptador de IGeneradorDeTokens. Application solo pide "dame un token para
    // este usuario": que se firme con HS256 y que dure una hora se decide aqui.
    public sealed class GeneradorDeTokensJwt(IOptions<OpcionesDeJwt> opciones, IClock reloj) : IGeneradorDeTokens
    {
        private readonly OpcionesDeJwt _opciones = opciones.Value;

        public TokenDeAcceso Generar(Usuario usuario)
        {
            ArgumentNullException.ThrowIfNull(usuario);

            var expiraEn = reloj.UtcNow.AddMinutes(_opciones.MinutosDeVigencia);

            // Lo que viaja dentro del token. Nada sensible: el token es legible
            // por cualquiera que lo tenga, la firma solo garantiza que no fue alterado.
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new(ClaimTypes.Name, usuario.Nombre),
                new(ClaimTypes.Email, usuario.CorreoElectronico),
                new(ClaimTypes.Role, usuario.Rol.ToString())
            };

            var clave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opciones.Clave));

            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _opciones.Emisor,
                Audience = _opciones.Audiencia,
                Expires = expiraEn.UtcDateTime,
                SigningCredentials = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256)
            };

            var valor = new JsonWebTokenHandler().CreateToken(descriptor);

            return new TokenDeAcceso(valor, expiraEn);
        }
    }
}
