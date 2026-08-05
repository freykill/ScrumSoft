using ScrumSoft.Application.Common;
using ScrumSoft.Application.Mediador;
using ScrumSoft.Application.Ports;

namespace ScrumSoft.Application.Proyectos
{
    public sealed class ListarMiembrosHandler(
        AccesoAProyectos acceso,
        IUsuarioRepository usuarios) : IManejador<ListarMiembrosConsulta, IReadOnlyList<MiembroDto>>
    {
        public async Task<IReadOnlyList<MiembroDto>> ManejarAsync(
            ListarMiembrosConsulta peticion,
            CancellationToken cancelacion)
        {
            ArgumentNullException.ThrowIfNull(peticion);

            var proyecto = await acceso
                .ObtenerConAccesoAsync(peticion.IdProyecto, cancelacion)
                .ConfigureAwait(false);

            // La membresia solo guarda el id: los datos de la persona viven en el
            // otro agregado y se traen en una sola consulta, no una por miembro.
            var ids = proyecto.Miembros.Select(m => m.IdUsuario).ToList();

            var personas = await usuarios
                .ListarPorIdsAsync(ids, cancelacion)
                .ConfigureAwait(false);

            var porId = personas.ToDictionary(u => u.Id);

            return [.. proyecto.Miembros
                .Where(m => porId.ContainsKey(m.IdUsuario))
                .Select(m => MiembroDto.Desde(m, porId[m.IdUsuario]))
                .OrderBy(m => m.Nombre)];
        }
    }
}
