namespace ScrumSoft.Infrastructure.Security
{
    // Se rellena desde la seccion "Jwt" de la configuracion.
    // La Clave nunca viene de appsettings.json: llega por secretos de usuario
    // en desarrollo, o por la variable de entorno Jwt__Clave al desplegar.
    public sealed class OpcionesDeJwt
    {
        public const string Seccion = "Jwt";

        // Minimo 32 caracteres: HS256 firma con una clave de al menos 256 bits.
        public string Clave { get; set; } = string.Empty;

        public string Emisor { get; set; } = "ScrumSoft";

        public string Audiencia { get; set; } = "ScrumSoftClientes";

        public int MinutosDeVigencia { get; set; } = 60;
    }
}
