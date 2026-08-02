using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ScrumSoft.Domain.Common;

namespace ScrumSoft.Infrastructure.Persistence.Interceptors
{
    // Rellena FechaCreacion y FechaActualizacion en el momento de guardar.
    // Va aqui y no en las entidades porque hay 16 metodos que modifican estado:
    // ponerlo en cada uno seria repetirlo 16 veces y poder olvidarlo en el 17.
    public sealed class InterceptorDeAuditoria(IClock reloj) : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(eventData);

            Sellar(eventData.Context);

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            ArgumentNullException.ThrowIfNull(eventData);

            Sellar(eventData.Context);

            return base.SavingChanges(eventData, result);
        }

        private void Sellar(DbContext? contexto)
        {
            if (contexto is null)
                return;

            var ahora = reloj.UtcNow;

            // EF ya sabe que cambio: esa informacion la necesita para decidir
            // si genera INSERT o UPDATE. Aqui solo se aprovecha.
            foreach (var entrada in contexto.ChangeTracker.Entries<IAuditable>())
            {
                if (entrada.State == EntityState.Added)
                    entrada.Entity.RegistrarCreacion(ahora);

                if (entrada.State is EntityState.Added or EntityState.Modified)
                    entrada.Entity.RegistrarActualizacion(ahora);
            }
        }
    }
}
