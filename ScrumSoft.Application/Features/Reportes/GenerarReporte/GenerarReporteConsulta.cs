using ScrumSoft.Application.Mediador;
using ScrumSoft.Application.Ports;

namespace ScrumSoft.Application.Reportes
{
    public sealed record ArchivoDeReporte
    {
        public required byte[] Contenido { get; init; }

        public required string NombreDeArchivo { get; init; }

        public required string TipoDeContenido { get; init; }
    }

    public sealed record GenerarReporteConsulta : IPeticion<ArchivoDeReporte>
    {
        public required Guid IdProyecto { get; init; }

        public required FormatoDeReporte Formato { get; init; }
    }
}
