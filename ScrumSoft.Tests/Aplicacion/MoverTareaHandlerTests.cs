using NSubstitute;
using ScrumSoft.Application.Common;
using ScrumSoft.Application.Ports;
using ScrumSoft.Application.Tareas;
using ScrumSoft.Domain.Entities;
using ScrumSoft.Domain.Enums;
using Xunit;

namespace ScrumSoft.Tests.Aplicacion
{
    // Requisito 6.6: el arrastre solo informa entre que dos tarjetas se solto la
    // tarea. La posicion la calcula el servidor, y aqui se cubre ese calculo de
    // punta a punta, incluida la renumeracion cuando se agotan los huecos.
    public sealed class MoverTareaHandlerTests
    {
        private readonly IProyectoRepository _proyectos = Substitute.For<IProyectoRepository>();
        private readonly IUsuarioActual _usuarioActual = Substitute.For<IUsuarioActual>();
        private readonly ITareaRepository _tareas = Substitute.For<ITareaRepository>();
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
        private readonly INotificadorDeTablero _notificador = Substitute.For<INotificadorDeTablero>();

        private readonly Guid _idUsuario = Guid.NewGuid();

        public MoverTareaHandlerTests() => _usuarioActual.Id.Returns(_idUsuario);

        [Fact]
        public async Task MoverEntreDosTareas_GuardaElPuntoMedioYAvisaPorTiempoReal()
        {
            var (proyecto, columna) = CrearProyectoConColumna();

            var anterior = CrearTarea(columna.Id, "Primera", 1000);
            var siguiente = CrearTarea(columna.Id, "Tercera", 3000);
            var movida = CrearTarea(columna.Id, "La que se arrastra", 9000);

            var handler = ArmarHandler(proyecto, movida, [anterior, siguiente, movida]);

            var dto = await handler.ManejarAsync(
                ComandoDeArrastre(proyecto, movida, columna, anterior, siguiente),
                CancellationToken.None);

            Assert.Equal(2000, dto.Orden);
            await _unitOfWork.Received(1).GuardarCambiosAsync(Arg.Any<CancellationToken>());

            // Requisito 6.7: el movimiento viaja a las demas sesiones del tablero.
            await _notificador
                .Received(1)
                .TareaMovidaAsync(proyecto.Id, Arg.Any<TareaDto>(), Arg.Any<CancellationToken>());
        }

        // El caso limite del requisito: entre 1000 y 1001 no cabe ningun entero.
        // El handler renumera la columna de mil en mil y recalcula sobre las
        // posiciones nuevas, en lugar de fallar o dejar dos tareas empatadas.
        [Fact]
        public async Task SinHuecoEntreVecinas_RenumeraLaColumnaYUbicaLaTarea()
        {
            var (proyecto, columna) = CrearProyectoConColumna();

            var anterior = CrearTarea(columna.Id, "Pegada arriba", 1000);
            var siguiente = CrearTarea(columna.Id, "Pegada abajo", 1001);
            var movida = CrearTarea(columna.Id, "La que se arrastra", 9000);

            var handler = ArmarHandler(proyecto, movida, [anterior, siguiente, movida]);

            var dto = await handler.ManejarAsync(
                ComandoDeArrastre(proyecto, movida, columna, anterior, siguiente),
                CancellationToken.None);

            // Las vecinas quedaron equiespaciadas...
            Assert.Equal(1000, anterior.Orden);
            Assert.Equal(2000, siguiente.Orden);

            // ...y la tarea aterrizo en el hueco que se acaba de abrir entre ellas.
            Assert.Equal(1500, dto.Orden);
            Assert.True(anterior.Orden < movida.Orden && movida.Orden < siguiente.Orden);
        }

        // ----------------------------------------------------------------

        private static MoverTareaComando ComandoDeArrastre(
            Proyecto proyecto,
            Tarea movida,
            Columna destino,
            Tarea anterior,
            Tarea siguiente) =>
            new()
            {
                IdProyecto = proyecto.Id,
                IdTarea = movida.Id,
                IdColumnaDestino = destino.Id,
                IdTareaAnterior = anterior.Id,
                IdTareaSiguiente = siguiente.Id
            };

        private (Proyecto Proyecto, Columna Columna) CrearProyectoConColumna()
        {
            var proyecto = Proyecto.Crear(
                "Tablero de prueba",
                null,
                new DateOnly(2026, 1, 1),
                null);

            proyecto.AgregarMiembro(_idUsuario, DateTimeOffset.UnixEpoch);

            return (proyecto, proyecto.AgregarColumna("Pendiente"));
        }

        private static Tarea CrearTarea(Guid idColumna, string titulo, int orden) =>
            Tarea.Crear(idColumna, titulo, null, Prioridad.Media, orden);

        private MoverTareaHandler ArmarHandler(
            Proyecto proyecto,
            Tarea movida,
            IReadOnlyList<Tarea> enLaColumnaDestino)
        {
            _proyectos.ObtenerPorIdAsync(proyecto.Id, Arg.Any<CancellationToken>()).Returns(proyecto);
            _tareas.ObtenerPorIdAsync(movida.Id, Arg.Any<CancellationToken>()).Returns(movida);
            _tareas
                .ListarPorColumnaAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                .Returns(enLaColumnaDestino);

            return new MoverTareaHandler(
                new AccesoAProyectos(_proyectos, _usuarioActual),
                _tareas,
                _unitOfWork,
                _notificador);
        }
    }
}
