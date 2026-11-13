using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Services.Reporte;
using Reserva.Repository.Data;
using Xunit;

namespace Reserva.Domain.Tests.Reporte;

public class ReporteSqlTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Consultas_SeTraducenEnSqlServerYMySqlSinAbrirConexion(bool mysql)
    {
        var options = new DbContextOptionsBuilder<ReservaCanchasContext>();
        if (mysql) options.UseMySql("Server=localhost;Database=ReportesModelo", new MySqlServerVersion(new Version(8, 4, 7)));
        else options.UseSqlServer("Server=(local);Database=ReportesModelo;Trusted_Connection=True;TrustServerCertificate=True");
        using var context = new ReservaCanchasContext(options.Options);
        var inicio = new DateTimeOffset(2026, 11, 1, 0, 0, 0, TimeSpan.Zero);
        var ids = new List<int> { 3 };
        var reservas = ReporteConsultas.Reservas(context.Reserva.AsNoTracking(), ids, inicio, inicio.AddMonths(1), 25001).ToQueryString();
        var detalles = ReporteConsultas.Detalles(context.DetalleReserva.AsNoTracking(), ids, inicio, inicio.AddMonths(1)).ToQueryString();
        var franjas = ReporteConsultas.Configurados(context.HorarioCancha.AsNoTracking(), ids).ToQueryString();
        Assert.Contains("WHERE", reservas);
        Assert.Contains("idCancha", reservas);
        Assert.Contains("WHERE", detalles);
        Assert.Contains("WHERE", franjas);
    }
}
