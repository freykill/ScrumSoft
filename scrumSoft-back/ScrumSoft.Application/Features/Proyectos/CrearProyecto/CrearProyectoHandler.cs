using ScrumSoft.Application.Common;
using ScrumSoft.Application.Mediador;
using ScrumSoft.Application.Ports;
using ScrumSoft.Domain.Common;
using ScrumSoft.Domain.Entities;

namespace ScrumSoft.Application.Proyectos
{
    public sealed class CrearProyectoHandler(
         IProyectoRepository proyectos,
         IUnitOfWork unitOfWork,
         IUsuarioActual usuarioActual,
         IClock reloj) : IManejador<CrearProyectoComando, ProyectoDto>
    {
        private static readonly string[] FlujoPorDefecto = ["Por hacer", "En progreso", "Hecho"];

        public async Task<ProyectoDto> ManejarAsync(
            CrearProyectoComando peticion,
            CancellationToken cancelacion)
        {
            ArgumentNullException.ThrowIfNull(peticion);

            var proyecto = Proyecto.Crear(
                peticion.Nombre,
                peticion.Descripcion,
                peticion.FechaInicio,
                peticion.FechaFinPrevista);

            var nombres = peticion.Columnas is { Count: > 0 } pedidas ? pedidas : FlujoPorDefecto;

            foreach (var nombre in nombres)
                proyecto.AgregarColumna(nombre);

            proyecto.AgregarMiembro(usuarioActual.Id, reloj.UtcNow);

            proyectos.Agregar(proyecto);
            await unitOfWork.GuardarCambiosAsync(cancelacion).ConfigureAwait(false);

            return ProyectoDto.Desde(proyecto);
        }
    }
}
