using ScrumSoft.Application.Ports;
using ScrumSoft.Domain.Entities;

namespace ScrumSoft.Application.Common
{
    // Resuelve la pregunta que se repite en casi todos los casos de uso:
    // traer un proyecto y comprobar que el usuario de la sesion puede tocarlo.
    public sealed class AccesoAProyectos(
        IProyectoRepository proyectos,
        IUsuarioActual usuarioActual)
    {
        public async Task<Proyecto> ObtenerConAccesoAsync(
            Guid idProyecto,
            CancellationToken cancelacion = default)
        {
            var proyecto = await proyectos.ObtenerPorIdAsync(idProyecto, cancelacion)
                ?? throw new RecursoNoEncontradoException("Proyecto", idProyecto);

            if (!usuarioActual.EsAdministrador && !proyecto.EsMiembro(usuarioActual.Id))
                throw new AccesoDenegadoException("No pertenece a este proyecto.");

            return proyecto;
        }
    }
}
