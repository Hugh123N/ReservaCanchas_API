using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Moq;
using Reserva.Common;
using Reserva.Domain.Queries.Dbo.Dashboard;
using Reserva.Domain.Services.Dashboard;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Data;
using Reserva.Repository.Security;
using Xunit;

namespace Reserva.Domain.Tests.Dashboard;

public class DashboardAdminTests
{
    private static readonly DateTimeOffset Ahora = new(2026, 11, 17, 15, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Cobros_ExcluyenPendientesOtrasMonedasYDuplicadosInclusoEntreMeses()
    {
        var pagos = new[]
        {
            Pago(1, 100, "cargo-1", Ahora.AddMonths(-1)), Pago(2, 100, "cargo-1", Ahora),
            Pago(3, 50, "cargo-2", Ahora), Pago(4, 999, "pendiente", Ahora, estado: 9),
            Pago(5, 999, "usd", Ahora, moneda: "USD"), Pago(6, 999, "futuro", Ahora.AddDays(1)),
            Pago(7, 999, "inactivo", Ahora, activo: false), Pago(8, 20, null, Ahora), Pago(9, 30, null, Ahora)
        };
        var estados = new[] { new Entity.EstadoPago { IdEstadoPago = 7, Codigo = Constants.ESTADO_PAGO.Pagado } };
        var confirmados = DashboardAdminConsultas.PagosConfirmados(pagos.AsQueryable(), estados.AsQueryable(), Ahora).ToList();
        Assert.Equal(new[] { 1, 3, 8, 9 }, confirmados.Select(p => p.IdPagoPlan));
        Assert.Equal(200, confirmados.Sum(p => p.Monto));
        // El cobro no depende del estado actual de la suscripción o del proveedor.
        Assert.All(confirmados, p => Assert.Null(p.IdProveedorPlanNavigation));
    }

    [Fact]
    public void Meses_RespetanMedianochePeruRellenanCerosYComparanElMismoTramo()
    {
        var periodo = new DashboardAdminPeriodo(Ahora, 6);
        Assert.Equal(new DateTimeOffset(2026, 11, 1, 5, 0, 0, TimeSpan.Zero), periodo.InicioMes);
        Assert.Equal(Ahora.AddMonths(-1), periodo.FinComparableAnterior);
        var fechas = new[]
        {
            new DashboardAdminFechaValor { Fecha = periodo.InicioMes.AddTicks(-1), Valor = 30 },
            new DashboardAdminFechaValor { Fecha = periodo.InicioMes, Valor = 40 },
            new DashboardAdminFechaValor { Fecha = periodo.Inicios[0].AddDays(-1), Valor = 999 },
            new DashboardAdminFechaValor { Fecha = Ahora.AddDays(1), Valor = 999 }
        };
        var meses = periodo.CompletarMeses(DashboardAdminConsultas.Mensuales(fechas.AsQueryable(), periodo), Array.Empty<DashboardAdminMesValor>());
        Assert.Equal(6, meses.Count);
        Assert.Equal(30, meses[^2].Cobrado);
        Assert.Equal(40, meses[^1].Cobrado);
        Assert.Equal(0, meses[0].Cobrado);
        Assert.True(meses[^1].EnCurso);
        Assert.Null(DashboardAdminPeriodo.Variacion(100, 0));
        Assert.Equal(-50m, DashboardAdminPeriodo.Variacion(50, 100));
        var enero = new DashboardAdminPeriodo(new(2027, 1, 31, 15, 0, 0, TimeSpan.Zero), 12);
        Assert.Equal(12, enero.CompletarMeses(Array.Empty<DashboardAdminMesValor>(), Array.Empty<DashboardAdminMesValor>()).Count);
        var marzo = new DashboardAdminPeriodo(new(2026, 3, 31, 15, 0, 0, TimeSpan.Zero), 6);
        Assert.Equal(28, marzo.FinComparableAnterior.Day);
    }

    [Fact]
    public void Planes_CuentanProveedoresUnicosSeparanPruebasYExcluyenInactivosYVencidos()
    {
        var proveedores = Enumerable.Range(1, 8).Select(i => new Entity.Proveedor
            { IdProveedor = i, Activo = i != 8, Nombres = "Proveedor", Apellidos = i.ToString(), IdEstadoProveedorNavigation = new() { Codigo = Constants.ESTADO_PROVEEDOR.Aprobado } }).AsQueryable();
        var planes = new[]
        {
            Plan(1, 1), Plan(2, 1), Plan(3, 2, prueba: true),
            Plan(4, 3, estado: Constants.ESTADO_PROV_PLAN.GRACE), Plan(5, 4, estado: Constants.ESTADO_PROV_PLAN.SUSPENDED),
            Plan(6, 5, estado: Constants.ESTADO_PROV_PLAN.PENDING), Plan(7, 6, fin: Ahora.AddDays(-1)),
            Plan(8, 7, cancelacion: true), Plan(9, 8)
        }.AsQueryable();
        var actuales = DashboardAdminConsultas.PlanesActuales(planes, proveedores, Ahora);
        var resumen = DashboardAdminConsultas.ResumenPlanes(actuales, Ahora.AddDays(7)).Single();
        Assert.Equal(2, resumen.Pago);
        Assert.Equal(1, resumen.Prueba);
        Assert.Equal(1, resumen.PruebasPorTerminar);
        Assert.Equal(1, resumen.EnGracia);
        Assert.Equal(1, resumen.Suspendidos);
        Assert.Equal(1, resumen.Cancelaciones);
        Assert.Equal(1, resumen.Vencimientos); // La cancelación programada no se duplica en vencimientos.
        Assert.Equal(2, DashboardAdminConsultas.DistribucionPlanes(actuales).Single().Cantidad);
        Assert.Equal(2, DashboardAdminConsultas.CasosPlanes(actuales, "CANCELACIONES", Ahora.AddDays(7)).Single().Fecha!.Value.Subtract(Ahora).Days);
    }

    [Fact]
    public void Canchas_PendientesNoSonAprobadasYSoloIncluyenNegociosActivos()
    {
        Entity.Cancha Cancha(int id, string estado, bool proveedorActivo = true) => new()
        {
            IdCancha = id, Nombre = "Cancha " + id, Activo = true, CreateDate = Ahora.AddDays(-id),
            IdProveedorNavigation = new() { Activo = proveedorActivo, Nombres = "Dueño", Apellidos = "Cancha", IdEstadoProveedorNavigation = new() { Codigo = Constants.ESTADO_PROVEEDOR.Aprobado } },
            IdEstadoCanchaNavigation = new() { Codigo = estado }
        };
        var canchas = new[] { Cancha(1, Constants.ESTADO_CANCHA.Aprobado), Cancha(2, Constants.ESTADO_CANCHA.Pendiente),
            Cancha(3, Constants.ESTADO_CANCHA.Pendiente, false), Cancha(4, Constants.ESTADO_CANCHA.Pendiente) }.AsQueryable();
        Assert.Equal(new[] { "Cancha 4", "Cancha 2" }, DashboardAdminConsultas.CanchasPendientes(canchas).Select(c => c.Cancha));
    }

    [Theory]
    [InlineData("Proveedor", 6)]
    [InlineData("Operador", 6)]
    [InlineData("Admin", 7)]
    public async Task Handler_RechazaRolOPeriodoAntesDeConsultarDatos(string rol, int meses)
    {
        var identity = new Mock<IUserIdentity>();
        identity.Setup(i => i.GetCurrentUserClaims()).Returns(new[] { new Claim("Roles", rol) });
        var handler = new GetDashboardAdminQueryHandler(new Mock<IRepository<Entity.Proveedor>>(MockBehavior.Strict).Object,
            new Mock<IRepository<Entity.ProveedorPlan>>(MockBehavior.Strict).Object, new Mock<IRepository<Entity.PagoPlan>>(MockBehavior.Strict).Object,
            new Mock<IRepository<Entity.EstadoPago>>(MockBehavior.Strict).Object, new Mock<IRepository<Entity.Cancha>>(MockBehavior.Strict).Object, identity.Object);
        Assert.False((await handler.Handle(new GetDashboardAdminQuery(meses), default)).IsValid);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Agregados_SeTraducenEnSqlServerYMySql(bool mysql)
    {
        var options = new DbContextOptionsBuilder<ReservaCanchasContext>();
        if (mysql) options.UseMySql("Server=localhost;Database=DashboardModelo", new MySqlServerVersion(new Version(8, 4, 7)));
        else options.UseSqlServer("Server=(local);Database=DashboardModelo;Trusted_Connection=True;TrustServerCertificate=True");
        using var context = new ReservaCanchasContext(options.Options);
        var periodo = new DashboardAdminPeriodo(Ahora, 12);
        var pagos = DashboardAdminConsultas.PagosConfirmados(context.PagoPlan, context.EstadoPago, Ahora);
        var planes = DashboardAdminConsultas.PlanesActuales(context.ProveedorPlan, context.Proveedor, Ahora);
        Assert.Contains("WHERE", pagos.ToQueryString());
        Assert.Contains("GROUP BY", DashboardAdminConsultas.Mensuales(pagos.Select(p => new DashboardAdminFechaValor
            { Fecha = p.FechaPago!.Value, Valor = p.Monto }), periodo).ToQueryString());
        Assert.Contains("GROUP BY", DashboardAdminConsultas.Mensuales(context.Proveedor.Select(p => new DashboardAdminFechaValor
            { Fecha = p.CreateDate, Valor = 1 }), periodo).ToQueryString());
        Assert.Contains("SELECT", DashboardAdminConsultas.ResumenPlanes(planes, Ahora.AddDays(7)).ToQueryString());
        Assert.Contains("GROUP BY", DashboardAdminConsultas.DistribucionPlanes(planes).ToQueryString());
        Assert.Contains("SELECT", DashboardAdminConsultas.CasosPlanes(planes, "GRACIA", Ahora.AddDays(7)).Take(5).ToQueryString());
        Assert.Contains("SELECT", DashboardAdminConsultas.CanchasPendientes(context.Cancha).Take(5).ToQueryString());
    }

    private static Entity.PagoPlan Pago(int id, decimal monto, string? cargo, DateTimeOffset fecha, int estado = 7, string moneda = "PEN", bool activo = true) =>
        new() { IdPagoPlan = id, Monto = monto, CulqiChargeId = cargo, FechaPago = fecha, IdEstadoPago = estado, Moneda = moneda, Activo = activo };

    private static Entity.ProveedorPlan Plan(int id, int proveedor, bool prueba = false, string estado = "ACTIVE", DateTimeOffset? fin = null, bool cancelacion = false) =>
        new()
        {
            IdProveedorPlan = id, IdProveedor = proveedor, IdPlane = 1, Activo = true, EsActual = true, EsPruebaGratis = prueba,
            Estado = estado, FechaInicio = Ahora.AddDays(-20), FechaFin = fin ?? Ahora.AddDays(2), CancelAtPeriodEnd = cancelacion,
            IdPlaneNavigation = new() { Nombre = "Profesional" }, IdPlanTarifaNavigation = new() { Precio = 99 }
        };
}
