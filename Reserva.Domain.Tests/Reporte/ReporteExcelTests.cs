using ClosedXML.Excel;
using Reserva.Common.Excel;
using Xunit;

namespace Reserva.Domain.Tests.Reporte;

public class ReporteExcelTests
{
    [Fact]
    public void Excel_ConservaNumerosFechasTelefonosYTextosSinEjecutarFormulas()
    {
        var bytes = ExportadorExcel.Generar("Prueba", new[] { new ColumnaExcel("Importe", "moneda"),
            new ColumnaExcel("Fecha", "fecha"), new ColumnaExcel("Teléfono"), new ColumnaExcel("Cliente") },
            new[] { (IReadOnlyList<object?>)new object?[] { 123.45m, new DateOnly(2026, 11, 13), "001234567", "=SUM(1,2)" } },
            new[] { ("Total", (object?)123.45m, "moneda") });
        using var libro = new XLWorkbook(new MemoryStream(bytes));
        Assert.Equal(2, libro.Worksheets.Count);
        var hoja = libro.Worksheet("Detalle");
        Assert.Equal(XLDataType.Number, hoja.Cell(4, 1).DataType);
        Assert.Equal(123.45, hoja.Cell(4, 1).GetDouble(), 2);
        Assert.Equal(new DateTime(2026, 11, 13), hoja.Cell(4, 2).GetDateTime());
        Assert.Equal("001234567", hoja.Cell(4, 3).GetString());
        Assert.False(hoja.Cell(4, 4).HasFormula);
        Assert.Equal("=SUM(1,2)", hoja.Cell(4, 4).GetString());
    }

    [Fact]
    public void ExcelSinDatos_ConservaCabeceraYResumen()
    {
        var bytes = ExportadorExcel.Generar("Sin reservas", new[] { new ColumnaExcel("Código") },
            Array.Empty<IReadOnlyList<object?>>(), new[] { ("Reservas", (object?)0, "numero") });
        using var libro = new XLWorkbook(new MemoryStream(bytes));
        Assert.Equal("Código", libro.Worksheet("Detalle").Cell(3, 1).GetString());
        Assert.Equal(0, libro.Worksheet("Resumen").Cell(3, 2).GetDouble());
    }
}
