using ScrumSoft.Application.Common;
using ScrumSoft.Application.Mediador;
using ScrumSoft.Application.Ports;
using ScrumSoft.Domain.Common;

namespace ScrumSoft.Application.Usuarios
{
    public sealed class ActualizarUsuarioHandler(
        IUsuarioRepository usuarios,
        IUsuarioActual usuarioActual,
        IUnitOfWork unitOfWork) : IManejador<ActualizarUsuarioComando, UsuarioDto>
    {
        public async Task<UsuarioDto> ManejarAsync(
            ActualizarUsuarioComando peticion,
            CancellationToken cancelacion)
        {
            ArgumentNullException.ThrowIfNull(peticion);

            var usuario = await usuarios
                .ObtenerPorIdAsync(peticion.Id, cancelacion)
                .ConfigureAwait(false)
                ?? throw new RecursoNoEncontradoException("Usuario", peticion.Id);

            // Crear usuarios es cosa exclusiva del administrador. Si el ultimo que
            // queda pudiera degradarse a si mismo, nadie podria dar de alta a nadie
            // nunca mas y solo se saldria de ahi tocando la base a mano. Como nadie
            // puede cambiar su propio rol, siempre sobrevive al menos un administrador.
            if (peticion.Id == usuarioActual.Id && peticion.Rol != usuario.Rol)
                throw new DomainException("No puede cambiar su propio rol.");

            usuario.Renombrar(peticion.Nombre);
            usuario.CambiarRol(peticion.Rol);

            await unitOfWork.GuardarCambiosAsync(cancelacion).ConfigureAwait(false);

            return UsuarioDto.Desde(usuario);
        }
    }
}
