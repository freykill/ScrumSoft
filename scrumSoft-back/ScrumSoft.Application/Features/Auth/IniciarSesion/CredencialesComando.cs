using ScrumSoft.Application.Mediador;

namespace ScrumSoft.Application.Auth
{
    public sealed record CredencialesComando : IPeticion<SesionDto>
    {
        public required string CorreoElectronico { get; init; }

        // En claro. Solo vive durante esta peticion: nunca se guarda ni se registra en logs.
        public required string Contrasena { get; init; }
    }
}
