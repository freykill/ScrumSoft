using ScrumSoft.Application.Reportes;

namespace ScrumSoft.Application.Ports
{
    public enum FormatoDeReporte
    {
        Pdf,
        Excel
    }

    // Requisito 6.8: agregar un tercer formato es escribir una implementacion nueva
    // y registrarla. Ninguna de las existentes se toca.
    public interface IExportadorDeReporte
    {
        FormatoDeReporte Formato { get; }

        string TipoDeContenido { get; }

        // Con punto incluido: ".pdf", ".xlsx"
        string Extension { get; }

        byte[] Exportar(ReporteProyectoDto reporte);
    }
}
