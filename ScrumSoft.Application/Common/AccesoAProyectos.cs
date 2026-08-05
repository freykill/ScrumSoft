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

            // Una sola regla para todos, sin importar el rol: se toca el proyecto
            // si se esta en su equipo. Un administrador que necesite entrar a uno
            // ajeno se agrega como miembro, y esa alta queda con fecha en
            // proyecto_usuarios. Antes se saltaba esta comprobacion, con lo que
            // podia leer y borrar cualquier proyecto sin dejar rastro y sin que
            // ese proyecto le apareciera nunca en su propia lista.
            if (!proyecto.EsMiembro(usuarioActual.Id))
                throw new AccesoDenegadoException("No pertenece a este proyecto.");

            return proyecto;
        }
    }
}
