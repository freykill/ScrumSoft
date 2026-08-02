using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ScrumSoft.Infrastructure.Realtime
{
    // Canal de tiempo real. Cada tablero es un grupo: una sesion solo recibe
    // eventos de los tableros a los que se suscribio (requisito 6.7).
    //
    // Authorize exige el mismo token JWT que el resto de la API (requisito 6.2).
    [Authorize]
    public sealed class TableroHub : Hub
    {
        public Task Suscribirse(Guid idProyecto) =>
            Groups.AddToGroupAsync(Context.ConnectionId, NombreDeGrupo(idProyecto));

        public Task Desuscribirse(Guid idProyecto) =>
            Groups.RemoveFromGroupAsync(Context.ConnectionId, NombreDeGrupo(idProyecto));

        // Un solo lugar decide como se nombra el grupo: si cambia, cambia para
        // el hub y para el notificador a la vez.
        public static string NombreDeGrupo(Guid idProyecto) => $"tablero-{idProyecto}";
    }
}
