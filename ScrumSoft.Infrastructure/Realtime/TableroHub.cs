using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ScrumSoft.Application.Ports;

namespace ScrumSoft.Infrastructure.Realtime
{
    // Canal de tiempo real. Cada tablero es un grupo: una sesion solo recibe
    // eventos de los tableros a los que se suscribio (requisito 6.7).
    //
    // Authorize exige el mismo token JWT que el resto de la API (requisito 6.2).
    [Authorize]
    public sealed class TableroHub(
        IProyectoRepository proyectos,
        RegistroDePresencia presencia) : Hub
    {
        public async Task Suscribirse(Guid idProyecto)
        {
            var usuario = UsuarioDeLaSesion();

            // Estar autenticado no basta: sin esta comprobacion cualquier sesion
            // podria suscribirse al guid de un proyecto ajeno y ver sus tareas en vivo.
            //
            // No se usa AccesoAProyectos porque depende de IUsuarioActual, que lee la
            // identidad del IHttpContextAccessor: dentro de un hub eso no es fiable,
            // porque la llamada no corre dentro de la peticion HTTP original. El hub
            // tiene su propia identidad en Context.User, que si esta siempre poblada.
            var proyecto = await proyectos
                .ObtenerPorIdAsync(idProyecto, Context.ConnectionAborted)
                .ConfigureAwait(false)
                ?? throw new HubException("El proyecto no existe.");

            if (!proyecto.EsMiembro(usuario.IdUsuario))
                throw new HubException("No pertenece a este proyecto.");

            await Groups.AddToGroupAsync(Context.ConnectionId, NombreDeGrupo(idProyecto)).ConfigureAwait(false);

            await AnunciarAsync(
                idProyecto,
                presencia.Entrar(idProyecto, Context.ConnectionId, usuario)).ConfigureAwait(false);
        }

        public async Task Desuscribirse(Guid idProyecto)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, NombreDeGrupo(idProyecto)).ConfigureAwait(false);

            await AnunciarAsync(
                idProyecto,
                presencia.Salir(idProyecto, Context.ConnectionId)).ConfigureAwait(false);
        }

        // Cerrar la pestana no llama a Desuscribirse: el navegador solo corta. Esta es
        // la unica limpieza que ocurre siempre, y sin ella la lista de conectados se
        // llenaria de fantasmas.
        // El nombre del parametro lo impone la firma de Hub, no se traduce.
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            foreach (var idProyecto in presencia.Abandonar(Context.ConnectionId))
                await AnunciarAsync(idProyecto, presencia.Conectados(idProyecto)).ConfigureAwait(false);

            await base.OnDisconnectedAsync(exception).ConfigureAwait(false);
        }

        // Un solo lugar decide como se nombra el grupo: si cambia, cambia para
        // el hub y para el notificador a la vez.
        public static string NombreDeGrupo(Guid idProyecto) => $"tablero-{idProyecto}";

        private Task AnunciarAsync(Guid idProyecto, IReadOnlyList<UsuarioConectado> conectados) =>
            Clients.Group(NombreDeGrupo(idProyecto)).SendAsync("UsuariosConectados", conectados);

        // El nombre sale del token, asi que armar la lista no consulta la base.
        private UsuarioConectado UsuarioDeLaSesion()
        {
            var id = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(id, out var idUsuario))
                throw new HubException("La sesion no es valida.");

            return new UsuarioConectado
            {
                IdUsuario = idUsuario,
                Nombre = Context.User?.FindFirst(ClaimTypes.Name)?.Value ?? "Sin nombre"
            };
        }
    }
}
