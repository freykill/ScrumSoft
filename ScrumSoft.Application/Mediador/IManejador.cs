namespace ScrumSoft.Application.Mediador
{
    // Ejecuta una peticion concreta. Un manejador por peticion.
    public interface IManejador<in TPeticion, TRespuesta>
        where TPeticion : IPeticion<TRespuesta>
    {
        Task<TRespuesta> ManejarAsync(TPeticion peticion, CancellationToken cancelacion);
    }
}
