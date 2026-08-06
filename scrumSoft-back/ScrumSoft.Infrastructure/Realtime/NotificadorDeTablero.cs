using Microsoft.AspNetCore.SignalR;
using ScrumSoft.Application.Columnas;
using ScrumSoft.Application.Ports;
using ScrumSoft.Application.Tareas;

namespace ScrumSoft.Infrastructure.Realtime
{
    // Adaptador de INotificadorDeTablero. Los casos de uso solo dicen "esto cambio";
    // que viaje por SignalR y a que grupo se decide aqui.
    //
    // Los nombres de los eventos ("TareaCreada", "TareaMovida"...) son el contrato
    // con el frontend: son los que Angular va a escuchar.
    public sealed class NotificadorDeTablero(IHubContext<TableroHub> hub) : INotificadorDeTablero
    {
        public Task TareaCreadaAsync(Guid idProyecto, TareaDto tarea, CancellationToken cancelacion = default) =>
            Enviar(idProyecto, "TareaCreada", tarea, cancelacion);

        public Task TareaActualizadaAsync(Guid idProyecto, TareaDto tarea, CancellationToken cancelacion = default) =>
            Enviar(idProyecto, "TareaActualizada", tarea, cancelacion);

        public Task TareaMovidaAsync(Guid idProyecto, TareaDto tarea, CancellationToken cancelacion = default) =>
            Enviar(idProyecto, "TareaMovida", tarea, cancelacion);

        public Task TareaEliminadaAsync(Guid idProyecto, Guid idTarea, CancellationToken cancelacion = default) =>
            Enviar(idProyecto, "TareaEliminada", idTarea, cancelacion);

        public Task ColumnasActualizadasAsync(
            Guid idProyecto,
            IReadOnlyList<ColumnaDto> columnas,
            CancellationToken cancelacion = default) =>
            Enviar(idProyecto, "ColumnasActualizadas", columnas, cancelacion);

        private Task Enviar(Guid idProyecto, string evento, object carga, CancellationToken cancelacion) =>
            hub.Clients
                .Group(TableroHub.NombreDeGrupo(idProyecto))
                .SendAsync(evento, carga, cancelacion);
    }
}
