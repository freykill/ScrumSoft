using ScrumSoft.Application.Mediador;
using ScrumSoft.Application.Ports;
using ScrumSoft.Domain.Enums;

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

        // Los mismos filtros del tablero. Si el usuario descarga el reporte con el
        // tablero filtrado, el archivo trae justo lo que esta viendo en pantalla.
        public Guid? IdResponsable { get; init; }

        public Prioridad? Prioridad { get; init; }
    }
}
