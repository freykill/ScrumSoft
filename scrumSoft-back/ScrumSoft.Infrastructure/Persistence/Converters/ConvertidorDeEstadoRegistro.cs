using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ScrumSoft.Domain.Common;

namespace ScrumSoft.Infrastructure.Persistence.Converters
{
    // En C# el estado es un enum legible; en PostgreSQL es un char(1): A, I o E.
    // EF llama estas dos funciones al guardar y al leer, en cada sentido.
    public sealed class ConvertidorDeEstadoRegistro : ValueConverter<EstadoRegistro, string>
    {
        public ConvertidorDeEstadoRegistro()
            : base(
                estado => ATexto(estado),
                texto => AEstado(texto))
        {
        }

        private static string ATexto(EstadoRegistro estado) => estado switch
        {
            EstadoRegistro.Activo => "A",
            EstadoRegistro.Inactivo => "I",
            _ => "E"
        };

        private static EstadoRegistro AEstado(string texto) => texto switch
        {
            "A" => EstadoRegistro.Activo,
            "I" => EstadoRegistro.Inactivo,
            _ => EstadoRegistro.Eliminado
        };
    }
}
