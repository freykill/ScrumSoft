using ScrumSoft.Application.Common;
using ScrumSoft.Application.Mediador;
using ScrumSoft.Application.Ports;

namespace ScrumSoft.Application.Columnas
{
    public sealed class RenombrarColumnaHandler(
        AccesoAProyectos acceso,
        IUnitOfWork unitOfWork,
        INotificadorDeTablero notificador) : IManejador<RenombrarColumnaComando, ColumnaDto>
    {
        public async Task<ColumnaDto> ManejarAsync(
            RenombrarColumnaComando peticion,
            CancellationToken cancelacion)
        {
            ArgumentNullException.ThrowIfNull(peticion);

            var proyecto = await acceso.ObtenerConAccesoAsync(peticion.IdProyecto, cancelacion).ConfigureAwait(false);

            var columna = proyecto.RenombrarColumna(peticion.IdColumna, peticion.Nombre);

            await unitOfWork.GuardarCambiosAsync(cancelacion).ConfigureAwait(false);

            await notificador
                .ColumnasActualizadasAsync(proyecto.Id, ColumnaDto.Desde(proyecto.Columnas), cancelacion)
                .ConfigureAwait(false);

            return ColumnaDto.Desde(columna);
        }
    }
}
