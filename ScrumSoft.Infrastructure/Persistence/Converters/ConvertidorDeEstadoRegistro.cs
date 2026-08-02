using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ScrumSoft.Domain.Common;

namespace ScrumSoft.Infrastructure.Persistence.Converters
{
    // En C# es un enum legible; en PostgreSQL es un char(1): A, I o E.
    public sealed class ConvertidorDeEstadoRegistro : ValueConverter<EstadoRegistro, string>
    {
        public ConvertidorDeEstadoRegistro()
            : base(
                estado => ADominio(estado),
                texto => ADominio(texto))
        {
        }

        private static string ADominio(EstadoRegistro estado) => estado switch
        {
            EstadoRegistro.Activo => "A",
            EstadoRegistro.Inactivo => "I",
            _ => "E"
        };

        private static EstadoRegistro ADominio(string texto) => texto switch
        {
            "A" => EstadoRegistro.Activo,
            "I" => EstadoRegistro.Inactivo,
            _ => EstadoRegistro.Eliminado
        };
    }
}
