using ScrumSoft.Application.Common;
using ScrumSoft.Application.Mediador;
using ScrumSoft.Application.Ports;
using ScrumSoft.Domain.Common;

namespace ScrumSoft.Application.Proyectos
{
    // Incorporar a alguien al equipo del proyecto. Es idempotente: agregar dos veces
    // al mismo usuario deja el equipo igual y devuelve la membresia que ya existia.
    public sealed class AgregarMiembroHandler(
        AccesoAProyectos acceso,
        IUsuarioRepository usuarios,
        IUnitOfWork unitOfWork,
        IClock reloj) : IManejador<AgregarMiembroComando, MiembroDto>
    {
        public async Task<MiembroDto> ManejarAsync(
            AgregarMiembroComando peticion,
            CancellationToken cancelacion)
        {
            ArgumentNullException.ThrowIfNull(peticion);

            var proyecto = await acceso
                .ObtenerConAccesoAsync(peticion.IdProyecto, cancelacion)
                .ConfigureAwait(false);

            // El usuario vive en otro agregado: hay que comprobar que existe antes
            // de guardar una membresia que apunte al vacio.
            var usuario = await usuarios
                .ObtenerPorIdAsync(peticion.IdUsuario, cancelacion)
                .ConfigureAwait(false)
                ?? throw new RecursoNoEncontradoException("Usuario", peticion.IdUsuario);

            proyecto.AgregarMiembro(usuario.Id, reloj.UtcNow);

            await unitOfWork.GuardarCambiosAsync(cancelacion).ConfigureAwait(false);

            var membresia = proyecto.Miembros.First(m => m.IdUsuario == usuario.Id);

            return MiembroDto.Desde(membresia, usuario);
        }
    }
}
