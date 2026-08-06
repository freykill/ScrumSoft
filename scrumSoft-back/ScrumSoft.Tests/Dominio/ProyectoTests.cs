using ScrumSoft.Domain.Common;
using ScrumSoft.Domain.Entities;
using Xunit;

namespace ScrumSoft.Tests.Dominio
{
    // Reglas que el proyecto hace cumplir por si mismo, sin pasar por la base de datos.
    public sealed class ProyectoTests
    {
        // Requisito 6.4: la regla vive en el backend, no en la interfaz.
        [Fact]
        public void EliminarColumnaConTareas_NoSePermite()
        {
            var proyecto = CrearProyecto();
            var columna = proyecto.AgregarColumna("Pendiente");

            var error = Assert.Throws<DomainException>(
                () => proyecto.EliminarColumna(columna.Id, contieneTareas: true));

            Assert.Equal("No se puede eliminar una columna que contiene tareas.", error.Message);
            Assert.Single(proyecto.Columnas);
        }

        private static Proyecto CrearProyecto() =>
            Proyecto.Crear("Tablero de prueba", null, new DateOnly(2026, 1, 1), null);
    }
}
