namespace ScrumSoft.Application.Mediador
{
    // Recibe una peticion, arma la cadena de comportamientos y la entrega a su manejador.
    public interface IMediador
    {
        Task<TRespuesta> EnviarAsync<TRespuesta>(
            IPeticion<TRespuesta> peticion,
            CancellationToken cancelacion = default);
    }
}
