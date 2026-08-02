namespace ScrumSoft.Application.Ports
{
    // Existe para que ni el dominio ni la aplicacion dependan de una libreria de hash concreta.
    public interface IHasheadorDeContrasenas
    {
        string Hashear(string contrasena);

        bool Verificar(string contrasena, string hash);
    }
}
