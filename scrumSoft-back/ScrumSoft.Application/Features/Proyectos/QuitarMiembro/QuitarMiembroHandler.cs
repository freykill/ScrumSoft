using ScrumSoft.Application.Common;
using ScrumSoft.Application.Mediador;
using ScrumSoft.Application.Ports;

namespace ScrumSoft.Application.Proyectos
{
    public sealed class QuitarMiembroHandler(
        AccesoAProyectos acceso,
        IUnitOfWork unitOfWork) : IManejador<QuitarMiembroComando, Unidad>
    {
        public async Task<Unidad> ManejarAsync(
            QuitarMiembroComando peticion,
            CancellationToken cancelacion)
        {
            ArgumentNullException.ThrowIfNull(peticion);

            var proyecto = await acceso
                .ObtenerConAccesoAsync(peticion.IdProyecto, cancelacion)
                .ConfigureAwait(false);

            // Quitar a quien no esta es un 404, no un exito silencioso: el cliente
            // creia estar viendo un equipo que ya cambio.
            if (!proyecto.EsMiembro(peticion.IdUsuario))
                throw new RecursoNoEncontradoException("Miembro", peticion.IdUsuario);

            // El dominio impide dejar el proyecto sin nadie.
            proyecto.QuitarMiembro(peticion.IdUsuario);

            await unitOfWork.GuardarCambiosAsync(cancelacion).ConfigureAwait(false);

            return Unidad.Valor;
        }
    }
}
