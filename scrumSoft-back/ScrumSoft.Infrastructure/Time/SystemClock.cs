using ScrumSoft.Domain.Common;

namespace ScrumSoft.Infrastructure.Time
{
    // Adaptador de IClock: el reloj de verdad. En los tests se sustituye por uno
    // con una fecha fija, y por eso ninguna entidad llama a DateTime.Now.
    public sealed class SystemClock : IClock
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}
