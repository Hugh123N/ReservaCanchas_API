using ClosedXML.Excel;
using MediatR;
using Moq;
using Reserva.Application.Reporte;
using Reserva.Domain.Queries.Reporte;
using Reserva.Dto.Base;
using Reserva.Dto.Reporte;
using Xunit;

namespace Reserva.Api.Tests.Reporte;

public class ReporteExportacionTests
{
    [Fact]
    public async Task Excel_ReutilizaElMismoTipoYFiltrosYExportaTodasLasFilas()
    {
        var mediator = new Mock<IMediator>();
        var reporte = new ReporteDto { Titulo = "Agenda", FechaDesde = new(2026, 11, 13), FechaHasta = new(2026, 11, 13),
            Columnas = new() { new("codigo", "Código") },
            Filas = Enumerable.Range(1, 25).Select(i => new Dictionary<string, object?> { ["codigo"] = $"RES-{i}" }).ToList() };
        mediator.Setup(m => m.Send(It.IsAny<GetReporteQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ResponseDto<ReporteDto>(reporte));
        var filtro = new FiltroReporteDto { IdCancha = 3, FechaDesde = new(2026, 11, 13), FechaHasta = new(2026, 11, 13), EstadoReserva = "VIGENTES" };

        var result = await new ReporteApplication(mediator.Object).Exportar(TipoReporte.AgendaOperador, filtro, default);

        Assert.True(result.IsValid);
        Assert.EndsWith(".xlsx", result.Data!.Nombre);
        using var libro = new XLWorkbook(new MemoryStream(result.Data.Contenido));
        Assert.Equal("RES-25", libro.Worksheet("Detalle").Cell(28, 1).GetString());
        mediator.Verify(m => m.Send(It.Is<GetReporteQuery>(q => q.Tipo == TipoReporte.AgendaOperador && q.Filtro == filtro),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
