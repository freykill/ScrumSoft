using System.Globalization;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ScrumSoft.Application.Ports;
using ScrumSoft.Application.Reportes;

namespace ScrumSoft.Infrastructure.Reportes
{
    // Adaptador de IExportadorDeReporte para PDF (QuestPDF, exigido por el enunciado).
    // Recibe el reporte ya armado y solo se ocupa de dibujarlo: no consulta nada.
    public sealed class ExportadorPdf : IExportadorDeReporte
    {
        public FormatoDeReporte Formato => FormatoDeReporte.Pdf;

        public string TipoDeContenido => "application/pdf";

        public string Extension => ".pdf";

        public byte[] Exportar(ReporteProyectoDto reporte)
        {
            ArgumentNullException.ThrowIfNull(reporte);

            return Document.Create(documento =>
            {
                documento.Page(pagina =>
                {
                    pagina.Size(PageSizes.A4);
                    pagina.Margin(2, Unit.Centimetre);
                    pagina.DefaultTextStyle(t => t.FontSize(10));

                    pagina.Header().Element(c => DibujarEncabezado(c, reporte));
                    pagina.Content().PaddingVertical(15).Element(c => DibujarTabla(c, reporte));

                    pagina.Footer().AlignCenter().Text(t =>
                    {
                        t.CurrentPageNumber();
                        t.Span(" de ");
                        t.TotalPages();
                    });
                });
            }).GeneratePdf();
        }

        // Requisito 6.8: encabezado con datos del proyecto y fecha de generacion.
        private static void DibujarEncabezado(IContainer contenedor, ReporteProyectoDto reporte)
        {
            contenedor.Column(columna =>
            {
                columna.Item().Text(reporte.Nombre).FontSize(18).SemiBold();

                if (!string.IsNullOrWhiteSpace(reporte.Descripcion))
                    columna.Item().PaddingTop(3).Text(reporte.Descripcion).FontSize(10).Light();

                columna.Item().PaddingTop(8).Text(t =>
                {
                    t.Span("Estado: ").SemiBold();
                    t.Span(reporte.EstadoProyecto.ToString());
                    t.Span("     Inicio: ").SemiBold();
                    t.Span(reporte.FechaInicio.ToString("dd/MM/yyyy", null));

                    if (reporte.FechaFinPrevista is { } fin)
                    {
                        t.Span("     Fin previsto: ").SemiBold();
                        t.Span(fin.ToString("dd/MM/yyyy", null));
                    }
                });

                columna.Item().Text(t =>
                {
                    t.Span("Generado: ").SemiBold();
                    t.Span(reporte.FechaGeneracion.ToString("dd/MM/yyyy HH:mm 'UTC'", null));
                    t.Span("     Tareas: ").SemiBold();
                    t.Span(reporte.Tareas.Count.ToString(CultureInfo.InvariantCulture));
                });

                columna.Item().PaddingTop(8).LineHorizontal(1).LineColor(Colors.Grey.Medium);
            });
        }

        // Requisito 6.8: tabla de tareas con columna, responsable y prioridad.
        private static void DibujarTabla(IContainer contenedor, ReporteProyectoDto reporte)
        {
            contenedor.Table(tabla =>
            {
                tabla.ColumnsDefinition(c =>
                {
                    c.RelativeColumn(4);   // Titulo
                    c.RelativeColumn(2);   // Columna
                    c.RelativeColumn(2);   // Responsable
                    c.RelativeColumn(2);   // Prioridad
                });

                tabla.Header(encabezado =>
                {
                    encabezado.Cell().Element(Titulo).Text("Tarea");
                    encabezado.Cell().Element(Titulo).Text("Columna");
                    encabezado.Cell().Element(Titulo).Text("Responsable");
                    encabezado.Cell().Element(Titulo).Text("Prioridad");
                });

                foreach (var fila in reporte.Tareas)
                {
                    tabla.Cell().Element(Celda).Text(fila.Titulo);
                    tabla.Cell().Element(Celda).Text(fila.Columna);
                    tabla.Cell().Element(Celda).Text(fila.Responsable);
                    tabla.Cell().Element(Celda).Text(fila.Prioridad.ToString());
                }

                static IContainer Titulo(IContainer c) => c
                    .Background(Colors.Grey.Lighten3)
                    .BorderBottom(1)
                    .BorderColor(Colors.Grey.Medium)
                    .PaddingVertical(5)
                    .PaddingHorizontal(4)
                    .DefaultTextStyle(t => t.SemiBold());

                static IContainer Celda(IContainer c) => c
                    .BorderBottom(1)
                    .BorderColor(Colors.Grey.Lighten2)
                    .PaddingVertical(4)
                    .PaddingHorizontal(4);
            });
        }
    }
}
