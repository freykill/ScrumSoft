using ScrumSoft.Application.Common;
using ScrumSoft.Application.Mediador;
using ScrumSoft.Application.Ports;

namespace ScrumSoft.Application.Columnas
{
    // Requisito 6.4: no se permite eliminar una columna que contenga tareas.
    public sealed class EliminarColumnaHandler(
        AccesoAProyectos acceso,
        ITareaRepository tareas,
        IUnitOfWork unitOfWork,
        INotificadorDeTablero notificador) : IManejador<EliminarColumnaComando, Unidad>
    {
        public async Task<Unidad> ManejarAsync(
            EliminarColumnaComando peticion,
            CancellationToken cancelacion)
        {
            ArgumentNullException.ThrowIfNull(peticion);

            var proyecto = await acceso.ObtenerConAccesoAsync(peticion.IdProyecto, cancelacion).ConfigureAwait(false);

            // Las tareas viven en otro agregado: el proyecto no puede consultarlas
            // por si mismo, asi que el caso de uso trae el dato y el dominio decide.
            var contieneTareas = await tareas
                .ExisteAlgunaEnColumnaAsync(peticion.IdColumna, cancelacion)
                .ConfigureAwait(false);

            proyecto.EliminarColumna(peticion.IdColumna, contieneTareas);

            await unitOfWork.GuardarCambiosAsync(cancelacion).ConfigureAwait(false);

            await notificador
                .ColumnasActualizadasAsync(proyecto.Id, ColumnaDto.Desde(proyecto.Columnas), cancelacion)
                .ConfigureAwait(false);

            return Unidad.Valor;
        }
    }
}
