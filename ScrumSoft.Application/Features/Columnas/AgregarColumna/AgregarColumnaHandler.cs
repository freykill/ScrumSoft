using ScrumSoft.Application.Common;
using ScrumSoft.Application.Mediador;
using ScrumSoft.Application.Ports;

namespace ScrumSoft.Application.Columnas
{
    public sealed class AgregarColumnaHandler(
        AccesoAProyectos acceso,
        IUnitOfWork unitOfWork,
        INotificadorDeTablero notificador) : IManejador<AgregarColumnaComando, ColumnaDto>
    {
        public async Task<ColumnaDto> ManejarAsync(
            AgregarColumnaComando peticion,
            CancellationToken cancelacion)
        {
            ArgumentNullException.ThrowIfNull(peticion);

            var proyecto = await acceso.ObtenerConAccesoAsync(peticion.IdProyecto, cancelacion);

            // El dominio calcula el orden: al final del tablero, con hueco de 1000.
            var columna = proyecto.AgregarColumna(peticion.Nombre);

            await unitOfWork.GuardarCambiosAsync(cancelacion);

            await notificador
                .ColumnasActualizadasAsync(proyecto.Id, ColumnaDto.Desde(proyecto.Columnas), cancelacion);

            return ColumnaDto.Desde(columna);
        }
    }
}
