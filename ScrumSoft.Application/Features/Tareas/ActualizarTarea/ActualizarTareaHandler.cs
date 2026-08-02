using ScrumSoft.Application.Common;
using ScrumSoft.Application.Mediador;
using ScrumSoft.Application.Ports;

namespace ScrumSoft.Application.Tareas
{
    public sealed class ActualizarTareaHandler(
        AccesoAProyectos acceso,
        ITareaRepository tareas,
        IUnitOfWork unitOfWork,
        INotificadorDeTablero notificador) : IManejador<ActualizarTareaComando, TareaDto>
    {
        public async Task<TareaDto> ManejarAsync(ActualizarTareaComando peticion, CancellationToken cancelacion)
        {
            ArgumentNullException.ThrowIfNull(peticion);

            var proyecto = await acceso.ObtenerConAccesoAsync(peticion.IdProyecto, cancelacion).ConfigureAwait(false);

            var tarea = await tareas.ObtenerPorIdAsync(peticion.IdTarea, cancelacion).ConfigureAwait(false)
                ?? throw new RecursoNoEncontradoException("Tarea", peticion.IdTarea);

            // Que la tarea exista no basta: tiene que ser de una columna de ESTE proyecto.
            if (proyecto.Columnas.All(c => c.Id != tarea.IdColumna))
                throw new AccesoDenegadoException("La tarea no pertenece a este proyecto.");

            tarea.Actualizar(peticion.Titulo, peticion.Descripcion, peticion.Prioridad);

            if (peticion.IdResponsable is { } responsable)
                tarea.Asignar(responsable);
            else
                tarea.Desasignar();

            await unitOfWork.GuardarCambiosAsync(cancelacion).ConfigureAwait(false);

            var dto = TareaDto.Desde(tarea);
            await notificador.TareaActualizadaAsync(proyecto.Id, dto, cancelacion).ConfigureAwait(false);

            return dto;
        }
    }
}
