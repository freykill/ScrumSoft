using ScrumSoft.Application.Common;
using ScrumSoft.Application.Mediador;
using ScrumSoft.Application.Ports;

namespace ScrumSoft.Application.Tareas
{
    public sealed class EliminarTareaHandler(
        AccesoAProyectos acceso,
        ITareaRepository tareas,
        IUnitOfWork unitOfWork,
        INotificadorDeTablero notificador) : IManejador<EliminarTareaComando, Unidad>
    {
        public async Task<Unidad> ManejarAsync(EliminarTareaComando peticion, CancellationToken cancelacion)
        {
            ArgumentNullException.ThrowIfNull(peticion);

            var proyecto = await acceso.ObtenerConAccesoAsync(peticion.IdProyecto, cancelacion).ConfigureAwait(false);

            var tarea = await tareas.ObtenerPorIdAsync(peticion.IdTarea, cancelacion).ConfigureAwait(false)
                ?? throw new RecursoNoEncontradoException("Tarea", peticion.IdTarea);

            if (proyecto.Columnas.All(c => c.Id != tarea.IdColumna))
                throw new AccesoDenegadoException("La tarea no pertenece a este proyecto.");

            tarea.MarcarComoEliminada();

            await unitOfWork.GuardarCambiosAsync(cancelacion).ConfigureAwait(false);

            await notificador.TareaEliminadaAsync(proyecto.Id, tarea.Id, cancelacion).ConfigureAwait(false);

            return Unidad.Valor;
        }
    }
}
