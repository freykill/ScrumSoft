namespace ScrumSoft.Application.Common
{
    public interface IUnitOfWork
    {
        Task<int> GuardarCambiosAsync(CancellationToken cancelacion = default);
    }
}
