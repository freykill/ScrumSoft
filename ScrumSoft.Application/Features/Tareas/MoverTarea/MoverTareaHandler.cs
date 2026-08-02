using ScrumSoft.Application.Common;
using ScrumSoft.Application.Mediador;
using ScrumSoft.Application.Ports;
using ScrumSoft.Domain.Common;
using ScrumSoft.Domain.Entities;

namespace ScrumSoft.Application.Tareas
{
    // Mueve una tarea entre columnas o dentro de la misma y persiste su posicion.
    // Normalmente actualiza una sola fila; solo renumera la columna cuando se agotan
    // los huecos entre dos vecinos consecutivos.
    public sealed class MoverTareaHandler(
        AccesoAProyectos acceso,
        ITareaRepository tareas,
        IUnitOfWork unitOfWork,
        INotificadorDeTablero notificador) : IManejador<MoverTareaComando, TareaDto>
    {
        public async Task<TareaDto> ManejarAsync(MoverTareaComando peticion, CancellationToken cancelacion)
        {
            ArgumentNullException.ThrowIfNull(peticion);

            var proyecto = await acceso.ObtenerConAccesoAsync(peticion.IdProyecto, cancelacion).ConfigureAwait(false);

            if (proyecto.Columnas.All(c => c.Id != peticion.IdColumnaDestino))
                throw new RecursoNoEncontradoException("Columna", peticion.IdColumnaDestino);

            var tarea = await tareas.ObtenerPorIdAsync(peticion.IdTarea, cancelacion).ConfigureAwait(false)
                ?? throw new RecursoNoEncontradoException("Tarea", peticion.IdTarea);

            if (proyecto.Columnas.All(c => c.Id != tarea.IdColumna))
                throw new AccesoDenegadoException("La tarea no pertenece a este proyecto.");

            var destino = await tareas
                .ListarPorColumnaAsync(peticion.IdColumnaDestino, cancelacion)
                .ConfigureAwait(false);

            var orden = CalcularOrden(destino, peticion, tarea);

            tarea.MoverA(peticion.IdColumnaDestino, orden);

            await unitOfWork.GuardarCambiosAsync(cancelacion).ConfigureAwait(false);

            var dto = TareaDto.Desde(tarea);
            await notificador.TareaMovidaAsync(proyecto.Id, dto, cancelacion).ConfigureAwait(false);

            return dto;
        }

        // Busca el hueco entre los dos vecinos. Si no queda espacio, renumera la columna
        // con posiciones equiespaciadas y vuelve a intentarlo sobre las posiciones nuevas.
        private static int CalcularOrden(
            IReadOnlyList<Tarea> destino,
            MoverTareaComando peticion,
            Tarea tarea)
        {
            var vecinas = destino.Where(t => t.Id != tarea.Id).OrderBy(t => t.Orden).ToList();

            var (anterior, siguiente) = Vecinos(vecinas, peticion);

            if (CalculadoraDeOrden.TryCalcular(anterior, siguiente, out var orden))
                return orden;

            for (var i = 0; i < vecinas.Count; i++)
                vecinas[i].Reposicionar(CalculadoraDeOrden.PosicionEn(i));

            var (anteriorRenumerado, siguienteRenumerado) = Vecinos(vecinas, peticion);

            return CalculadoraDeOrden.TryCalcular(anteriorRenumerado, siguienteRenumerado, out var ordenFinal)
                ? ordenFinal
                : throw new DomainException("No fue posible calcular la posicion de la tarea.");
        }

        private static (int? Anterior, int? Siguiente) Vecinos(
            IReadOnlyList<Tarea> vecinas,
            MoverTareaComando peticion)
        {
            var anterior = peticion.IdTareaAnterior is { } idAnterior
                ? vecinas.FirstOrDefault(t => t.Id == idAnterior)?.Orden
                : null;

            var siguiente = peticion.IdTareaSiguiente is { } idSiguiente
                ? vecinas.FirstOrDefault(t => t.Id == idSiguiente)?.Orden
                : null;

            return (anterior, siguiente);
        }
    }
}
