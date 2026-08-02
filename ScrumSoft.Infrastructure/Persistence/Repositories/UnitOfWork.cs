using ScrumSoft.Application.Common;

namespace ScrumSoft.Infrastructure.Persistence.Repositories
{
    // Adaptador de IUnitOfWork. SaveChangesAsync de EF ya es una transaccion:
    // envia todos los cambios pendientes juntos y revierte si alguno falla.
    public sealed class UnitOfWork(ScrumSoftDbContext contexto) : IUnitOfWork
    {
        public Task<int> GuardarCambiosAsync(CancellationToken cancelacion = default) =>
            contexto.SaveChangesAsync(cancelacion);
    }
}
