using ClosedXML.Excel;

namespace Reserva.Common.Excel;

public record ColumnaExcel(string Titulo, string Formato = "texto");

public static class ExportadorExcel
{
    public const string ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    public static byte[] Generar(string titulo, IReadOnlyList<ColumnaExcel> columnas,
        IEnumerable<IReadOnlyList<object?>> filas, IReadOnlyList<(string Etiqueta, object? Valor, string Formato)> resumen)
    {
        if (columnas.Count == 0) throw new ArgumentException("El archivo requiere columnas.", nameof(columnas));
        using var libro = new XLWorkbook();
        var hojaResumen = libro.AddWorksheet("Resumen");
        hojaResumen.Cell(1, 1).Value = titulo;
        hojaResumen.Range(1, 1, 1, 2).Merge().Style.Font.SetBold().Font.SetFontSize(14);
        var filaResumen = 3;
        foreach (var item in resumen)
        {
            hojaResumen.Cell(filaResumen, 1).Value = item.Etiqueta;
            Escribir(hojaResumen.Cell(filaResumen, 2), item.Valor, item.Formato);
            filaResumen++;
        }
        hojaResumen.Column(1).Width = 29;
        hojaResumen.Column(2).Width = 75;
        hojaResumen.Column(2).Style.Alignment.WrapText = true;

        var detalle = libro.AddWorksheet("Detalle");
        detalle.Cell(1, 1).Value = titulo;
        detalle.Range(1, 1, 1, columnas.Count).Merge().Style.Font.SetBold().Font.SetFontSize(14);
        const int cabecera = 3;
        for (var c = 0; c < columnas.Count; c++)
        {
            detalle.Cell(cabecera, c + 1).Value = columnas[c].Titulo;
            detalle.Column(c + 1).Width = Math.Clamp(columnas[c].Titulo.Length + 5, 16, 36);
        }
        detalle.Range(cabecera, 1, cabecera, columnas.Count).Style
            .Fill.SetBackgroundColor(XLColor.FromHtml("#0F766E")).Font.SetFontColor(XLColor.White).Font.SetBold();
        var fila = cabecera + 1;
        foreach (var valores in filas)
        {
            if (valores.Count != columnas.Count) throw new ArgumentException("La fila no coincide con las columnas.", nameof(filas));
            for (var c = 0; c < columnas.Count; c++) Escribir(detalle.Cell(fila, c + 1), valores[c], columnas[c].Formato);
            fila++;
        }
        detalle.Range(cabecera, 1, Math.Max(cabecera, fila - 1), columnas.Count).SetAutoFilter();
        detalle.SheetView.FreezeRows(cabecera);
        using var stream = new MemoryStream();
        libro.SaveAs(stream);
        return stream.ToArray();
    }

    private static void Escribir(IXLCell celda, object? valor, string formato)
    {
        // Los textos se escriben como valores, nunca como fórmulas de Excel.
        switch (valor)
        {
            case null: return;
            case decimal numero: celda.Value = (double)numero; break;
            case int numero: celda.Value = numero; break;
            case long numero: celda.Value = numero; break;
            case double numero: celda.Value = numero; break;
            case DateOnly fecha: celda.Value = fecha.ToDateTime(TimeOnly.MinValue); break;
            case DateTime fecha: celda.Value = fecha; break;
            case DateTimeOffset fecha: celda.Value = fecha.DateTime; break;
            case bool booleano: celda.Value = booleano; break;
            default: celda.Value = valor.ToString() ?? string.Empty; break;
        }
        celda.Style.NumberFormat.Format = formato switch
        {
            "moneda" => "\"S/ \"#,##0.00;[Red]-\"S/ \"#,##0.00",
            "horas" => "0.00",
            "numero" => "0",
            "fecha" => "dd/mm/yyyy",
            "fechaHora" => "dd/mm/yyyy hh:mm",
            _ => "@"
        };
    }
}
