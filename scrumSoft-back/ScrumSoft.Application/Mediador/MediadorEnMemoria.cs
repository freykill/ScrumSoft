using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace ScrumSoft.Application.Mediador
{
    // Despacha en el mismo proceso: no hay cola ni bus, solo resolucion en el contenedor.
    // La reflexion ocurre una sola vez por tipo de peticion y queda cacheada; a partir
    // de ahi todo es fuertemente tipado y no se usa dynamic en ningun momento.
    public sealed class MediadorEnMemoria(IServiceProvider proveedor) : IMediador
    {
        private static readonly ConcurrentDictionary<Type, object> EnvoltoriosPorTipo = new();

        public Task<TRespuesta> EnviarAsync<TRespuesta>(
            IPeticion<TRespuesta> peticion,
            CancellationToken cancelacion = default)
        {
            ArgumentNullException.ThrowIfNull(peticion);

            // La peticion esta tipada como IPeticion<TRespuesta>, pero en tiempo de ejecucion
            // es un tipo concreto (CrearProyectoComando, por ejemplo). El envoltorio recupera
            // ese tipo concreto para poder pedirle al contenedor IManejador<Concreto, TRespuesta>.
            // El diccionario guarda object porque adentro caben envoltorios de tipos
            // distintos, uno por cada comando. Solo tienen "object" en comun.
            var crudo = EnvoltoriosPorTipo.GetOrAdd(
                peticion.GetType(),
                tipoDePeticion => Activator.CreateInstance(
                    typeof(Envoltorio<,>).MakeGenericType(tipoDePeticion, typeof(TRespuesta)))!);

            // Se sostiene por la clase base, que es el unico tipo que este metodo
            // puede nombrar: aqui no se conoce CrearProyectoComando.
            var envoltorio = (EnvoltorioBase<TRespuesta>)crudo;

            return envoltorio.ManejarAsync(peticion, proveedor, cancelacion);
        }

        private abstract class EnvoltorioBase<TRespuesta>
        {
            public abstract Task<TRespuesta> ManejarAsync(
                object peticion,
                IServiceProvider proveedor,
                CancellationToken cancelacion);
        }

        private sealed class Envoltorio<TPeticion, TRespuesta> : EnvoltorioBase<TRespuesta>
            where TPeticion : IPeticion<TRespuesta>
        {
            public override Task<TRespuesta> ManejarAsync(
                object peticion,
                IServiceProvider proveedor,
                CancellationToken cancelacion)
            {
                var tipada = (TPeticion)peticion;

                var manejador = proveedor.GetRequiredService<IManejador<TPeticion, TRespuesta>>();

                SiguienteEnLaCadena<TRespuesta> cadena =
                    () => manejador.ManejarAsync(tipada, cancelacion);

                // Se recorren al reves para que el primero registrado quede como el mas externo.
                var comportamientos = proveedor
                    .GetServices<IComportamiento<TPeticion, TRespuesta>>()
                    .Reverse();

                foreach (var comportamiento in comportamientos)
                {
                    var siguiente = cadena;
                    var actual = comportamiento;
                    cadena = () => actual.ManejarAsync(tipada, siguiente, cancelacion);
                }

                return cadena();
            }
        }
    }
}
