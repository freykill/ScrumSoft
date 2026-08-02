namespace ScrumSoft.Application.Mediador
{
    // Marca un comando o una consulta y declara que tipo devuelve.
    // Equivale a IRequest<TResponse> de MediatR.
    public interface IPeticion<out TRespuesta>
    {
    }

    // Atajo para las peticiones que no devuelven datos.
    public interface IPeticion : IPeticion<Unidad>
    {
    }
}
