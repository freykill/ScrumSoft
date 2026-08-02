using ScrumSoft.Application.Common;
using ScrumSoft.Application.Mediador;

namespace ScrumSoft.Application.Proyectos
{
    public sealed class ActualizarProyectoHandler(
        AccesoAProyectos acceso,
        IUnitOfWork unitOfWork) : IManejador<ActualizarProyectoComando, ProyectoDto>
    {
        public async Task<ProyectoDto> ManejarAsync(
            ActualizarProyectoComando peticion,
            CancellationToken cancelacion)
        {
            ArgumentNullException.ThrowIfNull(peticion);

            var proyecto = await acceso.ObtenerConAccesoAsync(peticion.Id, cancelacion).ConfigureAwait(false);

            proyecto.Actualizar(
                peticion.Nombre,
                peticion.Descripcion,
                peticion.FechaInicio,
                peticion.FechaFinPrevista);

            proyecto.CambiarEstadoProyecto(peticion.EstadoProyecto);

            await unitOfWork.GuardarCambiosAsync(cancelacion).ConfigureAwait(false);

            return ProyectoDto.Desde(proyecto);
        }
    }
}
