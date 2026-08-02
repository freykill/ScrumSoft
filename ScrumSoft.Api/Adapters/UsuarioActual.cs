using System.Security.Claims;
using ScrumSoft.Application.Common;
using ScrumSoft.Domain.Enums;

namespace ScrumSoft.Api.Adapters
{
    // Adaptador de IUsuarioActual: traduce los claims del token a datos simples.
    // Gracias a esto, Application no referencia ASP.NET y sus casos de uso se
    // pueden probar pasandoles un usuario falso.
    public sealed class UsuarioActual(IHttpContextAccessor acceso) : IUsuarioActual
    {
        public Guid Id =>
            Guid.TryParse(Buscar(ClaimTypes.NameIdentifier), out var id) ? id : Guid.Empty;

        public bool EstaAutenticado =>
            acceso.HttpContext?.User.Identity?.IsAuthenticated ?? false;

        public bool EsAdministrador =>
            string.Equals(Buscar(ClaimTypes.Role), nameof(RolUsuario.Administrador), StringComparison.Ordinal);

        private string? Buscar(string tipo) => acceso.HttpContext?.User.FindFirst(tipo)?.Value;
    }
}
