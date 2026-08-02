using ClosedXML.Excel;
using ScrumSoft.Application.Ports;
using ScrumSoft.Application.Reportes;

namespace ScrumSoft.Infrastructure.Reportes
{
    // Adaptador de IExportadorDeReporte para Excel (ClosedXML, licencia MIT).
    // Recibe exactamente el mismo ReporteProyectoDto que el exportador de PDF.
    public sealed class ExportadorExcel : IExportadorDeReporte
    {
        private const int FilaEncabezadoTabla = 7;

        public FormatoDeReporte Formato => FormatoDeReporte.Excel;

        public string TipoDeContenido =>
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        public string Extension => ".xlsx";

        public byte[] Exportar(ReporteProyectoDto reporte)
        {
            ArgumentNullException.ThrowIfNull(reporte);

            using var libro = new XLWorkbook();
            var hoja = libro.Worksheets.Add("Reporte");

            EscribirEncabezado(hoja, reporte);
            EscribirTabla(hoja, reporte);

            // Requisito 6.8: anchos de columna adecuados.
            hoja.Columns().AdjustToContents();

            using var flujo = new MemoryStream();
            libro.SaveAs(flujo);

            return flujo.ToArray();
        }

        private static void EscribirEncabezado(IXLWorksheet hoja, ReporteProyectoDto reporte)
        {
            hoja.Cell(1, 1).Value = reporte.Nombre;
            hoja.Cell(1, 1).Style.Font.SetBold().Font.SetFontSize(14);

            hoja.Cell(2, 1).Value = reporte.Descripcion ?? string.Empty;

            hoja.Cell(3, 1).Value = "Estado";
            hoja.Cell(3, 2).Value = reporte.EstadoProyecto.ToString();

            hoja.Cell(4, 1).Value = "Fecha de inicio";
            hoja.Cell(4, 2).Value = reporte.FechaInicio.ToDateTime(TimeOnly.MinValue);
            hoja.Cell(4, 2).Style.DateFormat.Format = "dd/MM/yyyy";

            hoja.Cell(5, 1).Value = "Generado";
            hoja.Cell(5, 2).Value = reporte.FechaGeneracion.UtcDateTime;
            hoja.Cell(5, 2).Style.DateFormat.Format = "dd/MM/yyyy HH:mm";

            hoja.Range(3, 1, 5, 1).Style.Font.SetBold();
        }

        private static void EscribirTabla(IXLWorksheet hoja, ReporteProyectoDto reporte)
        {
            var titulos = new[] { "Tarea", "Columna", "Responsable", "Prioridad" };

            for (var i = 0; i < titulos.Length; i++)
            {
                var celda = hoja.Cell(FilaEncabezadoTabla, i + 1);
                celda.Value = titulos[i];
                celda.Style.Font.SetBold();
                celda.Style.Fill.BackgroundColor = XLColor.LightGray;
            }

            var fila = FilaEncabezadoTabla + 1;

            foreach (var tarea in reporte.Tareas)
            {
                hoja.Cell(fila, 1).Value = tarea.Titulo;
                hoja.Cell(fila, 2).Value = tarea.Columna;
                hoja.Cell(fila, 3).Value = tarea.Responsable;
                hoja.Cell(fila, 4).Value = tarea.Prioridad.ToString();
                fila++;
            }

            // Deja los titulos siempre visibles al desplazarse.
            hoja.SheetView.FreezeRows(FilaEncabezadoTabla);
        }
    }
}
