using ScrumSoft.Application.Common;
using ScrumSoft.Application.Mediador;
using ScrumSoft.Application.Ports;

namespace ScrumSoft.Application.Auth
{
    public sealed class IniciarSesionHandler(
        IUsuarioRepository usuarios,
        IHasheadorDeContrasenas hasheador,
        IGeneradorDeTokens generadorDeTokens) : IManejador<CredencialesComando, SesionDto>
    {
        public async Task<SesionDto> ManejarAsync(
            CredencialesComando peticion,
            CancellationToken cancelacion)
        {
            ArgumentNullException.ThrowIfNull(peticion);

            var usuario = await usuarios
                .ObtenerPorCorreoAsync(peticion.CorreoElectronico.Trim().ToLowerInvariant(), cancelacion)
                .ConfigureAwait(false);

            // Mismo mensaje si el correo no existe o si la contrasena es incorrecta:
            // distinguirlos permitiria averiguar que correos estan registrados.
            if (usuario is null || !hasheador.Verificar(peticion.Contrasena, usuario.PasswordHash))
                throw new AccesoDenegadoException("Correo o contrasena incorrectos.");

            if (!usuario.EstaActiva())
                throw new AccesoDenegadoException("La cuenta no esta activa.");

            var token = generadorDeTokens.Generar(usuario);

            return new SesionDto
            {
                Token = token.Valor,
                ExpiraEn = token.ExpiraEn,
                IdUsuario = usuario.Id,
                Nombre = usuario.Nombre,
                Rol = usuario.Rol
            };
        }
    }
}
