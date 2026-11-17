using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Services.Reporte;
using Reserva.Dto.Reporte;
using Reserva.Repository.Data;
using Xunit;

namespace Reserva.Domain.Tests.Reporte;

public class MovimientosReporteConsultasTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ConsultaDeMovimientos_SeTraduceEnAmbosProveedores(bool mysql)
    {
        var options = new DbContextOptionsBuilder<ReservaCanchasContext>();
        if (mysql) options.UseMySql("Server=localhost;Database=ReportesModelo", new MySqlServerVersion(new Version(8, 4, 7)));
        else options.UseSqlServer("Server=(local);Database=ReportesModelo;Trusted_Connection=True;TrustServerCertificate=True");
        using var context = new ReservaCanchasContext(options.Options);
        var desde = new DateTimeOffset(2026, 11, 13, 5, 0, 0, TimeSpan.Zero);
        var filtro = new FiltroMovimientosReporteDto { IdUsuarioResponsable = Guid.NewGuid(), IdMetodoPago = 2 };
        var query = MovimientosReporteConsultas.Periodo(context.MovimientoPago.AsNoTracking(), new() { 3 }, desde, desde.AddDays(1), filtro).Take(50001);
        Assert.Contains("WHERE", query.ToQueryString());
        Assert.Contains("IdUsuarioResponsable", query.ToQueryString());
    }

    [Fact]
    public void FiltroMetodo_NoAtribuyeUnaCorreccionGlobalAlMedioDelCobroOriginal()
    {
        var reserva = new Entity.Reserva { IdCancha = 3, CodigoReserva = "RES-10" };
        var pago = new Entity.Pago { IdReserva = 10, IdReservaNavigation = reserva };
        var desde = new DateTimeOffset(2026, 11, 13, 5, 0, 0, TimeSpan.Zero);
        var data = new[]
        {
            new Entity.MovimientoPago { IdMovimientoPago = 1, Tipo = "COBRO", Importe = 10, IdMetodoPago = 2, Activo = true, CreateDate = desde.AddHours(1), IdPagoNavigation = pago },
            new Entity.MovimientoPago { IdMovimientoPago = 2, Tipo = "COBRO", Importe = 20, IdMetodoPago = 4, Activo = true, CreateDate = desde.AddHours(1), IdPagoNavigation = pago },
            new Entity.MovimientoPago { IdMovimientoPago = 3, Tipo = "CORRECCION", Importe = -5, Activo = true, CreateDate = desde.AddHours(1), IdPagoNavigation = pago }
        };
        var seleccion = MovimientosReporteConsultas.Periodo(data.AsQueryable(), new() { 3 }, desde, desde.AddDays(1), new() { IdMetodoPago = 2 }).ToList();
        Assert.Equal(new[] { 1, 3 }, seleccion.Select(m => m.IdMovimiento));
        Assert.Empty(MovimientosReporteConsultas.Periodo(data.AsQueryable(), new() { 99 }, desde, desde.AddDays(1), new()).ToList());
    }
}
