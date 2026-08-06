using ScrumSoft.Domain.Enums;

namespace ScrumSoft.Application.Auth
{
    public sealed record SesionDto
    {
        public required string Token { get; init; }

        public required DateTimeOffset ExpiraEn { get; init; }

        public required Guid IdUsuario { get; init; }

        public required string Nombre { get; init; }

        public required RolUsuario Rol { get; init; }
    }
}
