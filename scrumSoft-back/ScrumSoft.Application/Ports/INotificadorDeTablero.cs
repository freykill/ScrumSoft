using ScrumSoft.Application.Columnas;
using ScrumSoft.Application.Tareas;

namespace ScrumSoft.Application.Ports
{
    // Puerto hacia el canal de tiempo real. Los casos de uso avisan que algo cambio;
    // que sea SignalR, WebSocket o SSE es decision del adaptador.
    // Cada aviso viaja solo a las sesiones suscritas a ese proyecto (requisito 6.7).
    public interface INotificadorDeTablero
    {
        Task TareaCreadaAsync(Guid idProyecto, TareaDto tarea, CancellationToken cancelacion = default);

        Task TareaActualizadaAsync(Guid idProyecto, TareaDto tarea, CancellationToken cancelacion = default);

        Task TareaMovidaAsync(Guid idProyecto, TareaDto tarea, CancellationToken cancelacion = default);

        Task TareaEliminadaAsync(Guid idProyecto, Guid idTarea, CancellationToken cancelacion = default);

        Task ColumnasActualizadasAsync(
            Guid idProyecto,
            IReadOnlyList<ColumnaDto> columnas,
            CancellationToken cancelacion = default);
    }
}
