using ScrumSoft.Application.Common;
using ScrumSoft.Application.Mediador;

namespace ScrumSoft.Application.Proyectos
{
    // Borrado logico: la fila queda con estado 'E' y deja de aparecer en las consultas.
    public sealed class EliminarProyectoHandler(
        AccesoAProyectos acceso,
        IUnitOfWork unitOfWork) : IManejador<EliminarProyectoComando, Unidad>
    {
        public async Task<Unidad> ManejarAsync(
            EliminarProyectoComando peticion,
            CancellationToken cancelacion)
        {
            ArgumentNullException.ThrowIfNull(peticion);

            var proyecto = await acceso.ObtenerConAccesoAsync(peticion.Id, cancelacion).ConfigureAwait(false);

            proyecto.MarcarComoEliminada();

            await unitOfWork.GuardarCambiosAsync(cancelacion).ConfigureAwait(false);

            return Unidad.Valor;
        }
    }
}
