using ScrumSoft.Domain.Entities;

namespace ScrumSoft.Application.Ports
{
    public sealed record TokenDeAcceso(string Valor, DateTimeOffset ExpiraEn);

    // La aplicacion pide un token para un usuario; como se firma y con que clave
    // es asunto del adaptador.
    public interface IGeneradorDeTokens
    {
        TokenDeAcceso Generar(Usuario usuario);
    }
}
