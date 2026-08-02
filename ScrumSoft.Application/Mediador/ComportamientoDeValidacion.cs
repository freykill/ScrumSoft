using FluentValidation;

namespace ScrumSoft.Application.Mediador
{
    // Valida toda peticion antes de que llegue a su manejador. Se escribe una vez
    // y aplica a todos los casos de uso: ningun manejador vuelve a validar.
    public sealed class ComportamientoDeValidacion<TPeticion, TRespuesta>(
        IEnumerable<IValidator<TPeticion>> validadores)
        : IComportamiento<TPeticion, TRespuesta>
        where TPeticion : IPeticion<TRespuesta>
    {
        public async Task<TRespuesta> ManejarAsync(
            TPeticion peticion,
            SiguienteEnLaCadena<TRespuesta> siguiente,
            CancellationToken cancelacion)
        {
            ArgumentNullException.ThrowIfNull(siguiente);

            var aplicables = validadores.ToList();

            // Una consulta puede no tener validador: se deja pasar sin ceremonia.
            if (aplicables.Count == 0)
                return await siguiente().ConfigureAwait(false);

            var contexto = new ValidationContext<TPeticion>(peticion);

            var resultados = await Task
                .WhenAll(aplicables.Select(v => v.ValidateAsync(contexto, cancelacion)))
                .ConfigureAwait(false);

            var fallos = resultados.SelectMany(r => r.Errors).Where(f => f is not null).ToList();

            if (fallos.Count > 0)
                throw new ValidationException(fallos);

            return await siguiente().ConfigureAwait(false);
        }
    }
}
