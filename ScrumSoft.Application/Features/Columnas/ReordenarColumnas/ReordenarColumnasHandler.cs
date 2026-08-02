using ScrumSoft.Application.Common;
using ScrumSoft.Application.Mediador;
using ScrumSoft.Application.Ports;

namespace ScrumSoft.Application.Columnas
{
    public sealed class ReordenarColumnasHandler(
        AccesoAProyectos acceso,
        IUnitOfWork unitOfWork,
        INotificadorDeTablero notificador)
        : IManejador<ReordenarColumnasComando, IReadOnlyList<ColumnaDto>>
    {
        public async Task<IReadOnlyList<ColumnaDto>> ManejarAsync(
            ReordenarColumnasComando peticion,
            CancellationToken cancelacion)
        {
            ArgumentNullException.ThrowIfNull(peticion);

            var proyecto = await acceso.ObtenerConAccesoAsync(peticion.IdProyecto, cancelacion).ConfigureAwait(false);

            // El dominio valida que la lista coincida con el tablero y renumera 1000, 2000, 3000...
            proyecto.ReordenarColumnas(peticion.IdsEnOrden);

            await unitOfWork.GuardarCambiosAsync(cancelacion).ConfigureAwait(false);

            var columnas = ColumnaDto.Desde(proyecto.Columnas);
            await notificador.ColumnasActualizadasAsync(proyecto.Id, columnas, cancelacion).ConfigureAwait(false);

            return columnas;
        }
    }
}
