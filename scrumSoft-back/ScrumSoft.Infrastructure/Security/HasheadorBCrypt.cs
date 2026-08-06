using ScrumSoft.Application.Ports;

namespace ScrumSoft.Infrastructure.Security
{
    // Adaptador de IHasheadorDeContrasenas.
    // BCrypt genera un salt aleatorio por contrasena y lo guarda dentro del propio hash,
    // por eso no hace falta una columna de salt aparte.
    public sealed class HasheadorBCrypt : IHasheadorDeContrasenas
    {
        // Cada incremento duplica el trabajo de calcular el hash. 12 es un punto
        // razonable hoy: lento para quien intenta fuerza bruta, imperceptible al iniciar sesion.
        private const int FactorDeTrabajo = 12;

        public string Hashear(string contrasena) =>
            BCrypt.Net.BCrypt.HashPassword(contrasena, FactorDeTrabajo);

        public bool Verificar(string contrasena, string hash) =>
            BCrypt.Net.BCrypt.Verify(contrasena, hash);
    }
}
