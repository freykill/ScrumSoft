namespace ScrumSoft.Application.Mediador
{
    // Continua la cadena hacia el siguiente comportamiento, o hacia el manejador.
    public delegate Task<TRespuesta> SiguienteEnLaCadena<TRespuesta>();

    // Envuelve la ejecucion de toda peticion. Es donde viven las preocupaciones
    // transversales: validacion, registro, transacciones.
    public interface IComportamiento<in TPeticion, TRespuesta>
        where TPeticion : IPeticion<TRespuesta>
    {
        Task<TRespuesta> ManejarAsync(
            TPeticion peticion,
            SiguienteEnLaCadena<TRespuesta> siguiente,
            CancellationToken cancelacion);
    }
}
