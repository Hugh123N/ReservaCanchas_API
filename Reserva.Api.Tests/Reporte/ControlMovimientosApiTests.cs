using System.Reflection;
using ClosedXML.Excel;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Moq;
using Reserva.Api.Controllers.Reporte;
using Reserva.Application.Reporte;
using Reserva.Common;
using Reserva.Domain.Queries.Reporte;
using Reserva.Domain.Services.Reporte;
using Reserva.Dto.Base;
using Reserva.Dto.Reporte;
using Xunit;

namespace Reserva.Api.Tests.Reporte;

public class ControlMovimientosApiTests
{
    [Theory]
    [InlineData(nameof(ReporteController.FiltrosControl))]
    [InlineData(nameof(ReporteController.Control))]
    [InlineData(nameof(ReporteController.ControlExcel))]
    public void Endpoints_ExigenProveedorYBearer(string accion)
    {
        var atributo = typeof(ReporteController).GetMethod(accion)!.GetCustomAttribute<AuthorizeAttribute>()!;
        Assert.Equal(Constants.Role.Proveedor, atributo.Roles);
        Assert.Equal(JwtBearerDefaults.AuthenticationScheme, atributo.AuthenticationSchemes);
    }

    [Fact]
    public async Task Excel_ConservaMovimientosSinHojaNiConsultaDeSituacionDeReservas()
    {
        var responsable = Guid.NewGuid();
        var fecha = new DateOnly(2026, 11, 13);
        var filtro = new FiltroControlMovimientosDto
        {
            Movimientos = new() { FechaDesde = fecha, FechaHasta = fecha, IdCancha = 3, IdUsuarioResponsable = responsable }
        };
        var movimientos = new List<MovimientoReporteData>
        {
            new() { IdMovimiento = 1, IdReserva = 10, IdCancha = 3, Tipo = "COBRO", Importe = 50, CodigoReserva = "RES-10", IdUsuarioResponsable = responsable,
                NombreResponsable = "Operador", RolResponsable = Constants.Role.Operador, FechaLocal = new(2026, 11, 13, 10, 0, 0), FechaReservaLocal = fecha.AddDays(3) },
            new() { IdMovimiento = 2, IdReserva = 10, IdCancha = 3, Tipo = "CORRECCION", Importe = -10, CodigoReserva = "RES-10", IdUsuarioResponsable = responsable,
                NombreResponsable = "Operador", RolResponsable = Constants.Role.Proveedor, FechaLocal = new(2026, 11, 13, 11, 0, 0), FechaReservaLocal = fecha.AddDays(3) }
        };
        var canchas = new List<CanchaReporteDto> { new(3, "Cancha Norte", "America/Lima", true) };
        var data = ControlMovimientosBuilder.Construir(movimientos, canchas, filtro.Movimientos);
        var mediator = new Mock<IMediator>();
        mediator.Setup(m => m.Send(It.IsAny<GetControlMovimientosQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ResponseDto<ControlMovimientosDto>(data));

        var resultado = await new ReporteApplication(mediator.Object).ExportarControl(filtro, default);

        Assert.True(resultado.IsValid);
        using var libro = new XLWorkbook(new MemoryStream(resultado.Data!.Contenido));
        Assert.Equal(new[] { "Resumen", "Responsables", "Metodos", "Movimientos" }, libro.Worksheets.Select(h => h.Name));
        mediator.Verify(m => m.Send(It.IsAny<GetReporteQuery>(), It.IsAny<CancellationToken>()), Times.Never);
        var hoja = libro.Worksheet("Movimientos");
        int Columna(string titulo) => hoja.Row(3).CellsUsed().Single(c => c.GetString() == titulo).Address.ColumnNumber;
        var filas = hoja.RowsUsed().Where(r => r.RowNumber() > 3).ToList();
        Assert.All(filas, r => Assert.Equal("Cancha Norte", r.Cell(Columna("Cancha")).GetString()));
        Assert.All(filas, r => Assert.Equal("Operador", r.Cell(Columna("Responsable")).GetString()));
        Assert.All(libro.Worksheets, h => Assert.DoesNotContain(h.CellsUsed(), c =>
            c.GetString() == "ID responsable" || c.GetString() == responsable.ToString()));
        Assert.Equal(50, filas.Sum(r => r.Cell(Columna("Entrada monetaria")).GetDouble()));
        Assert.Equal(-10, filas.Sum(r => r.Cell(Columna("Ajuste de registro")).GetDouble()));
        mediator.Verify(m => m.Send(It.Is<GetControlMovimientosQuery>(q => q.Filtro == filtro), It.IsAny<CancellationToken>()), Times.Once);
    }
}
