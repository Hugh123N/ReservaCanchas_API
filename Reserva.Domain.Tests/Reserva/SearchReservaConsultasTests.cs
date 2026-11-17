using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Queries.Dbo.Reserva;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Reserva;
using Reserva.Dto.Reporte;
using Reserva.Repository.Data;
using Xunit;

namespace Reserva.Domain.Tests.Reserva;

public class SearchReservaConsultasTests
{
    private static readonly List<CanchaReporteDto> Alcance = new() { new(3, "Norte", "America/Lima", true) };

    [Fact]
    public void Indicadores_ConsideranTodoElFiltroYNoSoloLaPaginaNiElProveedorEnviadoPorElCliente()
    {
        var reservas = Enumerable.Range(1, 25).Select(i => Crear(i)).Append(Crear(99, cancha: 99)).AsQueryable();
        var query = SearchReservaConsultas.Filtrar(reservas, Array.Empty<Entity.Pago>().AsQueryable(), Alcance,
            new() { IdProveedor = 999, CodigoEstadoPago = Constants.ESTADO_PAGO.Pendiente });
        var indicadores = SearchReservaConsultas.Indicadores(query).Single();
        var pagina = SearchReservaConsultas.Ordenar(query, null).Take(12).ToList();
        Assert.Equal(25, indicadores.Total);
        Assert.Equal(25, indicadores.Pendientes);
        Assert.Equal(2500, indicadores.SaldoVigente);
        Assert.Equal(12, pagina.Count);
        Assert.All(pagina, r => Assert.Equal(3, r.IdCancha));
    }

    [Fact]
    public void Estados_AnuladasSonSubconjuntoDeCanceladasYNoGeneranSaldo()
    {
        var vigente = Crear(1, estado: Constants.ESTADO_RESERVA.Confirmado);
        var anulada = Crear(2); anulada.Activo = false;
        var cancelada = Crear(3, estado: Constants.ESTADO_RESERVA.Cancelado);
        var pagos = new[]
        {
            new Entity.Pago { IdReserva = 1, Monto = 100, MontoAdelanto = 40, Activo = true,
                IdEstadoPagoNavigation = new() { Codigo = Constants.ESTADO_PAGO.Parcial, Nombre = "Parcial" } },
            new Entity.Pago { IdReserva = 3, Monto = 100, MontoAdelanto = 80, MontoReembolso = 60, Activo = true,
                IdEstadoPagoNavigation = new() { Codigo = Constants.ESTADO_PAGO.Cancelado, Nombre = "Cancelado" } }
        };
        var query = SearchReservaConsultas.Filtrar(new[] { vigente, anulada, cancelada }.AsQueryable(), pagos.AsQueryable(), Alcance, new());
        var resumen = SearchReservaConsultas.Indicadores(query).Single();
        Assert.Equal(3, resumen.Total);
        Assert.Equal(2, resumen.Canceladas);
        Assert.Equal(1, resumen.Anuladas);
        Assert.Equal(1, resumen.ConDevolucion);
        Assert.Equal(60, resumen.SaldoVigente);
        var soloAnuladas = SearchReservaConsultas.Filtrar(new[] { vigente, anulada, cancelada }.AsQueryable(), pagos.AsQueryable(), Alcance, new() { CodigoEstado = "ANULADAS" }).Single();
        Assert.Equal(2, soloAnuladas.IdReserva);
        Assert.Equal(0, soloAnuladas.MontoPendiente);
    }

    [Fact]
    public void Fechas_SonDiasLocalesInclusivosYElTextoEncuentraApellidos()
    {
        var antes = Crear(1); antes.FechaReserva = new(2026, 11, 13, 4, 59, 0, TimeSpan.Zero);
        var dentro = Crear(2); dentro.FechaReserva = new(2026, 11, 14, 4, 59, 0, TimeSpan.Zero);
        var despues = Crear(3); despues.FechaReserva = new(2026, 11, 14, 5, 0, 0, TimeSpan.Zero);
        var fila = SearchReservaConsultas.Filtrar(new[] { antes, dentro, despues }.AsQueryable(), Array.Empty<Entity.Pago>().AsQueryable(), Alcance,
            new() { FechaDesdeLocal = new(2026, 11, 13), FechaHastaLocal = new(2026, 11, 13), SearchText = "Pérez" }).Single();
        Assert.Equal(2, fila.IdReserva);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void SearchPaginadoYAgregados_SeTraducenEnSqlServerYMySql(bool mysql)
    {
        var options = new DbContextOptionsBuilder<ReservaCanchasContext>();
        if (mysql) options.UseMySql("Server=localhost;Database=SearchModelo", new MySqlServerVersion(new Version(8, 4, 7)));
        else options.UseSqlServer("Server=(local);Database=SearchModelo;Trusted_Connection=True;TrustServerCertificate=True");
        using var context = new ReservaCanchasContext(options.Options);
        var query = SearchReservaConsultas.Filtrar(context.Reserva.AsNoTracking(), context.Pago.AsNoTracking(), Alcance,
            new() { FechaDesdeLocal = new(2026, 11, 13), FechaHastaLocal = new(2026, 11, 13) });
        var suma = SearchReservaConsultas.Indicadores(query).ToQueryString();
        var pagina = SearchReservaConsultas.Ordenar(query, new[] { new SortParamsDto { Property = "createDate", Direction = "desc" } }).Skip(12).Take(12).ToQueryString();
        Assert.Contains("SUM", suma);
        Assert.Contains("LEFT JOIN", suma);
        Assert.Contains("WHERE", pagina);
    }

    [Fact]
    public async Task Validador_ConservaLaValidacionComunDePaginaYValidaRangosYEstados()
    {
        var validator = new SearchReservaQueryValidator();
        var parametros = new SearchParamsDto<SearchReservaFilterDto> { Page = new() { Page = 1, PageSize = 12 }, Filter = new() };
        Assert.True((await validator.ValidateAsync(new SearchReservaQuery(parametros))).IsValid);
        parametros.Page.Page = 0;
        Assert.False((await validator.ValidateAsync(new SearchReservaQuery(parametros))).IsValid);
        parametros.Page.Page = 1;
        parametros.Filter.FechaDesdeLocal = new(2026, 11, 14); parametros.Filter.FechaHastaLocal = new(2026, 11, 13);
        Assert.False((await validator.ValidateAsync(new SearchReservaQuery(parametros))).IsValid);
        parametros.Filter.FechaHastaLocal = new(2026, 11, 14); parametros.Filter.CodigoEstado = "ANULADAS";
        Assert.True((await validator.ValidateAsync(new SearchReservaQuery(parametros))).IsValid);
    }

    private static Entity.Reserva Crear(int id, int cancha = 3, string estado = Constants.ESTADO_RESERVA.Pendiente) => new()
    {
        IdReserva = id, IdCancha = cancha, Activo = true, CodigoReserva = $"RES-{id}", MontoTotal = 100,
        FechaReserva = new(2026, 11, 13, 15, 0, 0, TimeSpan.Zero), CreateDate = new(2026, 11, 12, 15, 0, 0, TimeSpan.Zero),
        IdCanchaNavigation = new() { IdCancha = cancha, Nombre = "Norte", Activo = true },
        IdClienteNavigation = new() { Nombres = "Ana", Apellidos = "Pérez", Telefono = "999888777" },
        IdEstadoReservaNavigation = new() { Codigo = estado, Nombre = estado }
    };
}
