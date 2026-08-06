using ScrumSoft.Domain.Common;
using Xunit;

namespace ScrumSoft.Tests.Dominio
{
    // Requisito 6.9: prueba obligatoria del calculo de la nueva posicion de una
    // tarea al reordenarla.
    //
    // Las posiciones no son 1, 2, 3: van de mil en mil. Insertar entre dos tarjetas
    // es entonces escribir una sola fila con el punto medio, en lugar de renumerar
    // la columna entera en cada arrastre.
    public sealed class CalculadoraDeOrdenTests
    {
        [Fact]
        public void EntreDosTareas_DevuelveElPuntoMedio()
        {
            var pudo = CalculadoraDeOrden.TryCalcular(1000, 2000, out var orden);

            Assert.True(pudo);
            Assert.Equal(1500, orden);
        }

        // Los enteros no se dividen indefinidamente: entre 1000 y 1001 no cabe nada.
        // La calculadora avisa con false y el caso de uso renumera la columna. Sin esa
        // señal dos tareas quedarian empatadas y el orden se corromperia en silencio.
        [Fact]
        public void SinHuecoEntreVecinas_NoCalcula()
        {
            var pudo = CalculadoraDeOrden.TryCalcular(1000, 1001, out _);

            Assert.False(pudo);
        }
    }
}
