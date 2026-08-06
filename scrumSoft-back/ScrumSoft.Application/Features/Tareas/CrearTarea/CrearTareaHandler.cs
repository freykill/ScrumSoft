using ScrumSoft.Application.Common;
using ScrumSoft.Application.Mediador;
using ScrumSoft.Application.Ports;
using ScrumSoft.Domain.Common;
using ScrumSoft.Domain.Entities;

namespace ScrumSoft.Application.Tareas
{
    public sealed class CrearTareaHandler(
        AccesoAProyectos acceso,
        ITareaRepository tareas,
        IUnitOfWork unitOfWork,
        INotificadorDeTablero notificador) : IManejador<CrearTareaComando, TareaDto>
    {
        public async Task<TareaDto> ManejarAsync(CrearTareaComando peticion, CancellationToken cancelacion)
        {
            ArgumentNullException.ThrowIfNull(peticion);

            var proyecto = await acceso.ObtenerConAccesoAsync(peticion.IdProyecto, cancelacion).ConfigureAwait(false);

            if (proyecto.Columnas.All(c => c.Id != peticion.IdColumna))
                throw new RecursoNoEncontradoException("Columna", peticion.IdColumna);

            // Sin esto un id inexistente reventaria contra la clave foranea con un 500,
            // y uno valido pero ajeno al equipo se aceptaria en silencio. Los miembros
            // ya vienen cargados con el proyecto: no cuesta una consulta extra.
            if (peticion.IdResponsable is { } responsable && !proyecto.EsMiembro(responsable))
                throw new DomainException("El responsable no pertenece al equipo del proyecto.");

            var existentes = await tareas
                .ListarPorColumnaAsync(peticion.IdColumna, cancelacion)
                .ConfigureAwait(false);

            // Al final de la columna, con hueco para insertar despues sin renumerar.
            var orden = existentes.Count == 0
                ? CalculadoraDeOrden.Salto
                : existentes.Max(t => t.Orden) + CalculadoraDeOrden.Salto;

            var tarea = Tarea.Crear(
                peticion.IdColumna,
                peticion.Titulo,
                peticion.Descripcion,
                peticion.Prioridad,
                orden,
                peticion.IdResponsable);

            tareas.Agregar(tarea);
            await unitOfWork.GuardarCambiosAsync(cancelacion).ConfigureAwait(false);

            var dto = TareaDto.Desde(tarea);
            await notificador.TareaCreadaAsync(proyecto.Id, dto, cancelacion).ConfigureAwait(false);

            return dto;
        }
    }
}
