using System.Linq.Expressions;
using System.Security.Claims;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using Reserva.Common;
using Reserva.Domain.Commands.Dbo.Cancha;
using Reserva.Domain.Commands.Dbo.Reserva;
using Reserva.Domain.Services.Culqi;
using Reserva.Domain.Services.GestionAdmin;
using Reserva.Domain.Services.Pago;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Cancha;
using Reserva.Dto.Dbo.Reserva;
using Reserva.Dto.GestionAdmin;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Data;
using Reserva.Repository.Security;
using Xunit;

namespace Reserva.Domain.Tests.GestionAdmin;

public class GestionAdminTests
{
    [Theory]
    [InlineData("SUSPENDIDO", true, 4)]
    [InlineData("INACTIVO", false, 2)]
    public async Task RestringirProveedor_PoneTodasLasCanchasPendientesYCancelaRenovacionSinBorrarReservas(string estado, bool activo, int idEstado)
    {
        var s = new Escenario();
        var finPlan = s.Plan.FechaFin;
        var resultado = await s.Service.CambiarProveedorAsync(7, new() { Estado = estado, Version = s.Version });
        Assert.True(resultado.IsValid);
        Assert.Equal(activo, s.Proveedor.Activo);
        Assert.Equal(idEstado, s.Proveedor.IdEstadoProveedor);
        Assert.All(s.Canchas, c => { Assert.Equal(2, c.IdEstadoCancha); Assert.True(c.Activo); });
        Assert.False(s.Plan.AutoRenovacion);
        Assert.True(s.Plan.CancelAtPeriodEnd);
        Assert.Null(s.Plan.FechaProximoCobro);
        Assert.Equal(finPlan, s.Plan.FechaFin);
        Assert.Equal("ACTIVE", s.Plan.Estado);
        s.Culqi.Verify(c => c.CancelSubscriptionAsync("sub-actual"), Times.Once);
        s.Reservas.Verify(r => r.UpdateAsync(It.IsAny<Entity.Reserva>()), Times.Never);
        s.Reservas.Verify(r => r.DeleteAsync(It.IsAny<Entity.Reserva>()), Times.Never);
    }

    [Fact]
    public async Task SiFallaLaPasarela_NoAplicaElCambioLocalNiDaLaRenovacionPorCancelada()
    {
        var s = new Escenario();
        s.Culqi.Setup(c => c.CancelSubscriptionAsync(It.IsAny<string>())).ReturnsAsync(false);
        Assert.False((await s.Service.CambiarProveedorAsync(7, new() { Estado = "SUSPENDIDO", Version = s.Version })).IsValid);
        Assert.Equal(2, s.Proveedor.IdEstadoProveedor);
        Assert.True(s.Plan.AutoRenovacion);
        Assert.False(s.Plan.CancelAtPeriodEnd);
        Assert.Equal(1, s.Canchas[0].IdEstadoCancha);
        s.Proveedores.Verify(r => r.UpdateAsync(It.IsAny<Entity.Proveedor>()), Times.Never);
    }

    [Fact]
    public async Task Reactivar_NoApruebaCanchasNiReactivaRenovaciones()
    {
        var s = new Escenario();
        s.Proveedor.Activo = false;
        s.Canchas.ForEach(c => c.IdEstadoCancha = 2);
        s.Plan.AutoRenovacion = false;
        s.Plan.CancelAtPeriodEnd = true;
        Assert.True((await s.Service.CambiarProveedorAsync(7, new() { Estado = "ACTIVO", Version = s.Version })).IsValid);
        Assert.True(s.Proveedor.Activo);
        Assert.All(s.Canchas, c => Assert.Equal(2, c.IdEstadoCancha));
        Assert.False(s.Plan.AutoRenovacion);
        Assert.True(s.Plan.CancelAtPeriodEnd);
        s.Culqi.Verify(c => c.CancelSubscriptionAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task VersionAnteriorONoAdmin_NoEscribenNiLlamanALaPasarela()
    {
        var s = new Escenario();
        Assert.False((await s.Service.CambiarProveedorAsync(7, new() { Estado = "SUSPENDIDO", Version = s.Version.AddMinutes(-1) })).IsValid);
        s.Identity.Setup(i => i.GetCurrentUserClaims()).Returns(new[] { new Claim("Roles", "Proveedor") });
        Assert.False((await s.Service.CambiarProveedorAsync(7, new() { Estado = "SUSPENDIDO", Version = s.Version })).IsValid);
        s.Proveedores.Verify(r => r.UpdateAsync(It.IsAny<Entity.Proveedor>()), Times.Never);
        s.Culqi.Verify(c => c.CancelSubscriptionAsync(It.IsAny<string>()), Times.Never);
    }

    [Theory]
    [InlineData("02", "01", true)]
    [InlineData("02", "03", true)]
    [InlineData("03", "01", false)]
    [InlineData("01", "04", true)]
    [InlineData("04", "02", true)]
    public async Task RevisionAdmin_SoloPermiteLasTransicionesDefinidas(string actual, string destino, bool permitido)
    {
        var s = new Escenario();
        s.Canchas[0].IdEstadoCanchaNavigation = s.EstadosCancha.Single(e => e.Codigo == actual);
        s.Canchas[0].IdEstadoCancha = s.Canchas[0].IdEstadoCanchaNavigation.IdEstadoCancha;
        var resultado = await s.Service.CambiarCanchaAsync(1, new() { Estado = destino, Version = s.Version });
        Assert.Equal(permitido, resultado.IsValid);
        if (permitido) Assert.Equal(int.Parse(destino), s.Canchas[0].IdEstadoCancha);
    }

    [Fact]
    public async Task Reenvio_SoloPropietarioHabilitadoYCanchaRechazada()
    {
        var s = new Escenario("Proveedor");
        s.Canchas[0].IdEstadoCanchaNavigation = s.EstadosCancha.Single(e => e.Codigo == "03");
        s.Canchas[0].IdEstadoCancha = 3;
        Assert.True((await s.Service.CambiarCanchaAsync(1, new() { Estado = "02", Version = s.Version }, true)).IsValid);
        s.Proveedor.Activo = false;
        Assert.False((await s.Service.CambiarCanchaAsync(1, new() { Estado = "02", Version = s.Version }, true)).IsValid);
        s.Proveedor.Activo = true;
        s.Canchas[0].IdProveedor = 99;
        Assert.False((await s.Service.CambiarCanchaAsync(1, new() { Estado = "02", Version = s.Version }, true)).IsValid);
    }

    [Fact]
    public async Task Pendiente_SePuedeGestionarPeroNoPublicarYElNegocioRestringidoNoCrea()
    {
        var s = new Escenario("Proveedor");
        var cancha = s.Canchas[0];
        cancha.IdEstadoCanchaNavigation = s.EstadosCancha.Single(e => e.Codigo == "02");
        var pagoAccess = new PagoReservaAccessService(s.Identity.Object, Mock.Of<IRepository<Entity.Operador>>(), Mock.Of<IRepository<Entity.OperadorCancha>>());
        Assert.True(await pagoAccess.PuedeAccederCanchaAsync(cancha));
        Assert.False(NegocioReglas.EsPublicable(cancha));
        cancha.IdEstadoCanchaNavigation = s.EstadosCancha.Single(e => e.Codigo == "01");
        Assert.True(NegocioReglas.EsPublicable(cancha));
        s.Proveedor.IdEstadoProveedorNavigation = new() { Codigo = "04" };
        Assert.False(NegocioReglas.EsPublicable(cancha));
        var llamado = false;
        var behavior = new NegocioRestringidoBehavior<CreateCanchaCommand, ResponseDto<GetCanchaDto>>(s.Identity.Object, s.Access);
        var resultado = await behavior.Handle(new CreateCanchaCommand(new()), _ => { llamado = true; return Task.FromResult(new ResponseDto<GetCanchaDto>()); }, default);
        Assert.False(resultado.IsValid);
        Assert.False(llamado);
        var permitido = new NegocioRestringidoBehavior<DeleteReservaCommand, ResponseDto>(s.Identity.Object, s.Access);
        Assert.True((await permitido.Handle(new DeleteReservaCommand(1), _ => Task.FromResult(new ResponseDto()), default)).IsValid);
    }

    [Fact]
    public void EditarCancha_NoPermiteAutoAprobarseNiCambiarDeProveedor()
    {
        var mapper = new MapperConfiguration(c => c.AddProfile<global::Reserva.Domain.Mapping.Cancha.CanchaProfile>()).CreateMapper();
        var cancha = new Entity.Cancha { IdProveedor = 7, IdEstadoCancha = 3 };
        mapper.Map(new UpdateCanchaDto { IdProveedor = 99, IdEstadoCancha = 1, Nombre = "Corregida" }, cancha);
        Assert.Equal(7, cancha.IdProveedor);
        Assert.Equal(3, cancha.IdEstadoCancha);
        Assert.Equal("Corregida", cancha.Nombre);
    }

    [Fact]
    public async Task RestriccionDeNegocio_NoConfundeElContextoClienteDeLaMismaCuenta()
    {
        var s = new Escenario("Proveedor");
        s.Proveedor.Activo = false;
        s.Identity.Setup(i => i.GetCurrentUserClaims()).Returns(new[] { new Claim("Roles", "Proveedor"), new Claim("ApplicationCode", "Cliente") });
        Assert.Null(await s.Access.ProveedorActualAsync());
        var behavior = new NegocioRestringidoBehavior<CreateReservaCommand, ResponseDto<ReservaConPagoDto>>(s.Identity.Object, s.Access);
        var llamado = false;
        var resultado = await behavior.Handle(new CreateReservaCommand(new()), _ =>
        {
            llamado = true;
            return Task.FromResult(new ResponseDto<ReservaConPagoDto>());
        }, default);
        Assert.True(resultado.IsValid);
        Assert.True(llamado);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void GestionYPublicacion_SeTraducenEnAmbosMotores(bool mysql)
    {
        var options = new DbContextOptionsBuilder<ReservaCanchasContext>();
        if (mysql) options.UseMySql("Server=localhost;Database=GestionModelo", new MySqlServerVersion(new Version(8, 4, 7)));
        else options.UseSqlServer("Server=(local);Database=GestionModelo;Trusted_Connection=True;TrustServerCertificate=True");
        using var context = new ReservaCanchasContext(options.Options);
        Assert.Contains("WHERE", context.Cancha.Where(NegocioReglas.Publicable).ToQueryString());
        Assert.Contains("SELECT", GestionAdminConsultas.Proveedores(context.Proveedor, context.ProveedorPlan, context.Reserva, DateTimeOffset.UtcNow)
            .Where(p => p.Estado == "SUSPENDIDO" && p.Nombre.Contains("Norte")).OrderBy(p => p.IdProveedor).Skip(20).Take(20).ToQueryString());
        Assert.Contains("SELECT", GestionAdminConsultas.Canchas(context.Cancha).Where(c => c.Estado == "02").Take(20).ToQueryString());
        Assert.True(context.Model.FindEntityType(typeof(Entity.Cancha))!.FindProperty(nameof(Entity.Cancha.IdEstadoCancha))!.IsConcurrencyToken);
        Assert.True(context.Model.FindEntityType(typeof(Entity.Proveedor))!.FindProperty(nameof(Entity.Proveedor.UpdateDate))!.IsConcurrencyToken);
    }

    private sealed class Escenario
    {
        public readonly DateTimeOffset Version = new(2026, 11, 17, 15, 0, 0, TimeSpan.Zero);
        public readonly Mock<IUserIdentity> Identity = new();
        public readonly Mock<ICulqiService> Culqi = new();
        public readonly Mock<IRepository<Entity.Proveedor>> Proveedores;
        public readonly Mock<IRepository<Entity.Reserva>> Reservas = Repo(new List<Entity.Reserva>());
        public readonly Entity.Proveedor Proveedor;
        public readonly Entity.ProveedorPlan Plan;
        public readonly List<Entity.Cancha> Canchas;
        public readonly List<Entity.EstadoCancha> EstadosCancha;
        public readonly NegocioAccessService Access;
        public readonly GestionAdminService Service;

        public Escenario(string rol = "Admin")
        {
            var usuario = Guid.NewGuid();
            Identity.Setup(i => i.GetUserRole()).Returns(rol);
            Identity.Setup(i => i.GetCurrentUserId()).Returns(usuario);
            Identity.Setup(i => i.GetCurrentUserIdNegocio()).Returns(7);
            Identity.Setup(i => i.GetCurrentUserClaims()).Returns(new[] { new Claim("Roles", rol) });
            var estados = new List<Entity.EstadoProveedor> { new() { IdEstadoProveedor = 2, Codigo = "02", Activo = true }, new() { IdEstadoProveedor = 4, Codigo = "04", Activo = true } };
            Proveedor = new() { IdProveedor = 7, IdUsuario = usuario.ToString(), Activo = true, IdEstadoProveedor = 2, IdEstadoProveedorNavigation = estados[0], CreateDate = Version };
            Proveedores = Repo(new List<Entity.Proveedor> { Proveedor });
            EstadosCancha = Enumerable.Range(1, 4).Select(i => new Entity.EstadoCancha { IdEstadoCancha = i, Codigo = i.ToString("00"), Activo = true }).ToList();
            Canchas = new() { new() { IdCancha = 1, IdProveedor = 7, IdProveedorNavigation = Proveedor, Activo = true, IdEstadoCancha = 1, IdEstadoCanchaNavigation = EstadosCancha[0], CreateDate = Version },
                new() { IdCancha = 2, IdProveedor = 7, Activo = true, IdEstadoCancha = 3, CreateDate = Version } };
            Plan = new() { IdProveedor = 7, Activo = true, EsActual = true, AutoRenovacion = true, CulqiSubscriptionId = "sub-actual", FechaFin = Version.AddMonths(1), FechaProximoCobro = Version.AddMonths(1), Estado = "ACTIVE" };
            var planes = Repo(new List<Entity.ProveedorPlan> { Plan });
            Culqi.Setup(c => c.CancelSubscriptionAsync(It.IsAny<string>())).ReturnsAsync(true);
            Access = new(Identity.Object, Proveedores.Object, Mock.Of<IRepository<Entity.Operador>>(), Mock.Of<IRepository<Entity.OperadorCancha>>());
            Service = new(Proveedores.Object, planes.Object, Repo(Canchas).Object, Reservas.Object, Repo(estados).Object, Repo(EstadosCancha).Object,
                Access, new CancelarRenovacionService(Culqi.Object, planes.Object));
        }
    }

    private static Mock<IRepository<T>> Repo<T>(List<T> datos) where T : class
    {
        var repo = new Mock<IRepository<T>>();
        repo.Setup(r => r.GetByAsync(It.IsAny<Expression<Func<T, bool>>>())).ReturnsAsync((Expression<Func<T, bool>> filtro) => datos.FirstOrDefault(filtro.Compile()));
        repo.Setup(r => r.GetByAsync(It.IsAny<Expression<Func<T, bool>>>(), It.IsAny<Expression<Func<T, object>>[]>()))
            .ReturnsAsync((Expression<Func<T, bool>> filtro, Expression<Func<T, object>>[] _) => datos.FirstOrDefault(filtro.Compile()));
        repo.Setup(r => r.GetByAsNoTrackingAsync(It.IsAny<Expression<Func<T, bool>>>())).ReturnsAsync((Expression<Func<T, bool>> filtro) => datos.FirstOrDefault(filtro.Compile()));
        repo.Setup(r => r.GetByAsNoTrackingAsync(It.IsAny<Expression<Func<T, bool>>>(), It.IsAny<Expression<Func<T, object>>[]>()))
            .ReturnsAsync((Expression<Func<T, bool>> filtro, Expression<Func<T, object>>[] _) => datos.FirstOrDefault(filtro.Compile()));
        repo.Setup(r => r.FindByAsync(It.IsAny<Expression<Func<T, bool>>>())).ReturnsAsync((Expression<Func<T, bool>> filtro) => datos.Where(filtro.Compile()).ToList());
        repo.Setup(r => r.UpdateAsync(It.IsAny<T>())).ReturnsAsync((T dato) => dato);
        return repo;
    }
}
