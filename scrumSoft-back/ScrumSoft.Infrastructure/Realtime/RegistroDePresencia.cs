using System.Collections.Concurrent;

namespace ScrumSoft.Infrastructure.Realtime
{
    public sealed record UsuarioConectado
    {
        public required Guid IdUsuario { get; init; }

        public required string Nombre { get; init; }
    }

    // Lleva la cuenta de quien esta viendo cada tablero en este momento.
    //
    // SignalR sabe que conexiones hay en un grupo pero no permite consultarlas,
    // asi que el registro se lleva aparte. Se guarda por conexion y no por usuario
    // a proposito: una misma persona puede tener el tablero abierto en dos pestanas,
    // y cerrar una no significa que se haya ido.
    //
    // Vive en memoria: un reinicio de la API vacia la lista y cada sesion la
    // recibe de nuevo al reconectarse. Con varias instancias del servidor haria
    // falta un backplane compartido, pero para un solo proceso esto sobra.
    public sealed class RegistroDePresencia
    {
        // idProyecto -> (idConexion -> quien es)
        private readonly ConcurrentDictionary<Guid, ConcurrentDictionary<string, UsuarioConectado>> _porTablero = new();

        public IReadOnlyList<UsuarioConectado> Entrar(Guid idProyecto, string idConexion, UsuarioConectado usuario)
        {
            ArgumentNullException.ThrowIfNull(usuario);

            var conexiones = _porTablero.GetOrAdd(idProyecto, _ => new ConcurrentDictionary<string, UsuarioConectado>());
            conexiones[idConexion] = usuario;

            return Distintos(conexiones);
        }

        public IReadOnlyList<UsuarioConectado> Salir(Guid idProyecto, string idConexion)
        {
            if (!_porTablero.TryGetValue(idProyecto, out var conexiones))
                return [];

            conexiones.TryRemove(idConexion, out _);

            return Distintos(conexiones);
        }

        // Saca la conexion de todos los tableros y devuelve cuales quedaron afectados,
        // para que el hub avise solo a esos. Se usa al caerse la conexion, donde no
        // se sabe a que tableros estaba suscrita.
        public IReadOnlyList<Guid> Abandonar(string idConexion)
        {
            var afectados = new List<Guid>();

            foreach (var (idProyecto, conexiones) in _porTablero)
            {
                if (conexiones.TryRemove(idConexion, out _))
                    afectados.Add(idProyecto);
            }

            return afectados;
        }

        public IReadOnlyList<UsuarioConectado> Conectados(Guid idProyecto) =>
            _porTablero.TryGetValue(idProyecto, out var conexiones) ? Distintos(conexiones) : [];

        // Una persona con tres pestanas abiertas es una sola entrada en la lista.
        private static IReadOnlyList<UsuarioConectado> Distintos(
            ConcurrentDictionary<string, UsuarioConectado> conexiones) =>
            [.. conexiones.Values
                .GroupBy(u => u.IdUsuario)
                .Select(g => g.First())
                .OrderBy(u => u.Nombre)];
    }
}
