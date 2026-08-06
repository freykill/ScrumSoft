using ScrumSoft.Application.Common;
using ScrumSoft.Application.Mediador;
using ScrumSoft.Application.Ports;
using ScrumSoft.Domain.Common;
using ScrumSoft.Domain.Entities;

namespace ScrumSoft.Application.Usuarios
{
    public sealed class CrearUsuarioHandler(
        IUsuarioRepository usuarios,
        IHasheadorDeContrasenas hasheador,
        IUnitOfWork unitOfWork) : IManejador<CrearUsuarioComando, UsuarioDto>
    {
        public async Task<UsuarioDto> ManejarAsync(
            CrearUsuarioComando peticion,
            CancellationToken cancelacion)
        {
            ArgumentNullException.ThrowIfNull(peticion);

            // La entidad normaliza el correo a minusculas; aqui se normaliza igual
            // para que la busqueda del duplicado compare lo mismo que se va a guardar.
            var correo = peticion.CorreoElectronico.Trim().ToLowerInvariant();

            // Sin esto el choque saldria como violacion del indice unico, o sea
            // un 500 en vez de un mensaje que el formulario pueda mostrar.
            var existente = await usuarios
                .ObtenerPorCorreoAsync(correo, cancelacion)
                .ConfigureAwait(false);

            if (existente is not null)
                throw new DomainException("Ya existe un usuario con ese correo electronico.");

            var usuario = Usuario.Crear(
                peticion.Nombre,
                correo,
                hasheador.Hashear(peticion.Contrasena),
                peticion.Rol);

            usuarios.Agregar(usuario);

            await unitOfWork.GuardarCambiosAsync(cancelacion).ConfigureAwait(false);

            return UsuarioDto.Desde(usuario);
        }
    }
}
