using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Reserva.Common;
using Reserva.Domain.Commands.Dbo.Pago;
using Reserva.Domain.Commands.Dbo.Reserva;
using AutoMapper;
using MediatR;
using Reserva.Domain.Services.Pago;
using Reserva.Dto.Dbo.Pago;
using Reserva.Dto.Dbo.Reserva;
using Reserva.Entity;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Data;
using Reserva.Repository.Security;
using Reserva.Repository.Abstractions.Transactions;
using Xunit;

namespace Reserva.Domain.Tests.Pago;

public class MovimientoPagoTests
{
    [Theory]
    [InlineData(40, 2, Constants.ESTADO_RESERVA.Pendiente)]
    [InlineData(50, 2, Constants.ESTADO_RESERVA.Confirmado)]
    [InlineData(100, 1, Constants.ESTADO_RESERVA.Confirmado)]
    public async Task PrimerCobroPendiente_PersisteCabeceraCompletaConEstadoValidoYSuMovimiento(
        decimal importe, int idEstadoPago, string estadoReserva)
    {
        var s = new Scenario(sinPago: true, porcentajeMinimo: 50);
        var dto = new RegistrarMovimientoPagoDto { IdPago = 0, IdReserva = 2, IdOperacion = Guid.NewGuid(),
            Revision = 0, Monto = importe, CodigoMetodoPago = "04", ReferenciaOperacion = "YAPE-PRIMERO" };

        var respuesta = await s.Service.RegistrarAsync(dto, false);

        Assert.True(respuesta.IsValid);
        var escritura = Assert.Single(s.InsercionesPago);
        Assert.Equal(idEstadoPago, escritura.Estado);
        Assert.Equal(importe, escritura.Cobrado);
        Assert.Equal(100 - importe, escritura.Pendiente);
        Assert.Equal(1, escritura.Movimientos);
        var pago = Assert.Single(s.Payments);
        Assert.True(pago.IdPago > 0);
        Assert.Equal(pago.IdPago, respuesta.Data.IdPago);
        Assert.Equal(estadoReserva, s.Reservation.IdEstadoReservaNavigation.Codigo);
        var movimiento = Assert.Single(s.Movements);
        Assert.Equal(pago.IdPago, movimiento.IdPago);
        Assert.Equal(importe, movimiento.Importe);
        Assert.Equal("YAPE-PRIMERO", movimiento.ReferenciaOperacion);
        s.Pagos.Verify(p => p.UpdateAsync(It.IsAny<Entity.Pago>()), Times.Never);
        Assert.True((await s.Service.RegistrarAsync(dto, false)).IsValid);
        Assert.Single(s.InsercionesPago);
        Assert.Single(s.Movements);
    }

    [Fact]
    public async Task PrimerCobro_CreaCabeceraYMovimientoConResponsableSinSaldoHistorico()
    {
        var s = new Scenario(sinPago: true);
        var dto = new RegistrarMovimientoPagoDto { IdReserva = 2, IdOperacion = Guid.NewGuid(),
            Monto = 40, CodigoMetodoPago = "02", ReferenciaOperacion = " REC-123 " };
        Assert.True((await s.Service.RegistrarAsync(dto, false)).IsValid);
        var pago = Assert.Single(s.Payments);
        Assert.Equal(100, pago.Monto);
        Assert.Equal(40, pago.MontoAdelanto);
        Assert.Equal(60, pago.MontoPendiente);
        var movimiento = Assert.Single(s.Movements);
        Assert.Equal("COBRO", movimiento.Tipo);
        Assert.Equal(s.Identity.Object.GetCurrentUserId(), movimiento.IdUsuarioResponsable);
        Assert.Equal(Constants.Role.Proveedor, movimiento.RolResponsable);
        Assert.Equal("REC-123", movimiento.ReferenciaOperacion);
        Assert.True((await s.Service.RegistrarAsync(dto, false)).IsValid);
        Assert.Single(s.Payments);
        Assert.Single(s.Movements);
    }

    [Fact]
    public async Task ReservaSinPago_ConsultarHistorialNoCreaCabecera()
    {
        var s = new Scenario(sinPago: true);
        var respuesta = await s.Service.ObtenerPorReservaAsync(2);
        Assert.True(respuesta.IsValid);
        Assert.Equal(0, respuesta.Data.IdPago);
        Assert.Equal(100, respuesta.Data.Saldo);
        Assert.True(respuesta.Data.PuedeCobrar);
        Assert.Empty(s.Payments);
        Assert.Empty(s.Movements);
    }

    [Fact]
    public async Task CobrosDeDistintosUsuarios_ConservanResponsablesYMetodosIndividuales()
    {
        var s = new Scenario(operador: true);
        Assert.True((await s.Service.RegistrarAsync(s.Request(20), false)).IsValid);
        var operador = s.Movements.Last();
        s.Identity.Setup(i => i.GetCurrentUserId()).Returns(Guid.NewGuid());
        s.Identity.Setup(i => i.GetUserRole()).Returns(Constants.Role.Proveedor);
        s.Identity.Setup(i => i.GetCurrentUserIdNegocio()).Returns(9);
        var saldo = s.Request(30);
        saldo.CodigoMetodoPago = "04";
        Assert.True((await s.Service.RegistrarAsync(saldo, false)).IsValid);
        var dueno = s.Movements.Last();
        Assert.NotEqual(operador.IdUsuarioResponsable, dueno.IdUsuarioResponsable);
        Assert.Equal(Constants.Role.Operador, operador.RolResponsable);
        Assert.Equal(Constants.Role.Proveedor, dueno.RolResponsable);
        Assert.Equal(2, operador.IdMetodoPago);
        Assert.Equal(4, dueno.IdMetodoPago);
        Assert.Equal(100, s.Payment.MontoAdelanto);
    }

    [Fact]
    public async Task Reembolso_GuardaMetodoResponsableYTotalDevueltoSinReducirCobros()
    {
        var s = new Scenario();
        await s.Service.RegistrarReembolsoAsync(s.Payment, 20, 4, Guid.NewGuid(), "Cancelación", "YAPE-123");
        Assert.Equal(50, s.Payment.MontoAdelanto);
        Assert.Equal(20, s.Payment.MontoReembolso);
        var movimiento = s.Movements.Last();
        Assert.Equal("REEMBOLSO", movimiento.Tipo);
        Assert.Equal(20, movimiento.Importe);
        Assert.Equal(4, movimiento.IdMetodoPago);
        Assert.Equal(s.Identity.Object.GetCurrentUserId(), movimiento.IdUsuarioResponsable);
        Assert.Equal("YAPE-123", movimiento.ReferenciaOperacion);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            s.Service.RegistrarReembolsoAsync(s.Payment, 31, 2, Guid.NewGuid(), null, null));
    }

    [Fact]
    public async Task Correccion_NoPuedeBajarDeLoYaReembolsado()
    {
        var s = new Scenario();
        s.Payment.MontoReembolso = 30;
        Assert.False((await s.Service.RegistrarAsync(s.Request(20, "Error de registro"), true)).IsValid);
        Assert.Empty(s.Movements);
    }

    [Fact]
    public async Task CancelarConReembolso_ActualizaCabeceraYReintentarNoDuplicaDevolucion()
    {
        var s = new Scenario();
        s.Reservation.IdCanchaNavigation.IdProveedorNavigation = new Proveedor { ConfiguracionProveedor =
            new ConfiguracionProveedor { TiempoLimiteCancelacion = 12, PorcentajeDevolucionCompleto = 100 } };
        var reservas = new Mock<IRepository<Entity.Reserva>>();
        reservas.Setup(r => r.GetByAsync(It.IsAny<Expression<Func<Entity.Reserva, bool>>>(),
            It.IsAny<Expression<Func<Entity.Reserva, object>>[]>())).ReturnsAsync(s.Reservation);
        var pagos = new Mock<IRepository<Entity.Pago>>();
        pagos.Setup(r => r.GetByAsync(It.IsAny<Expression<Func<Entity.Pago, bool>>>(),
            It.IsAny<Expression<Func<Entity.Pago, object>>[]>())).ReturnsAsync(s.Payment);
        var estadosReserva = new Mock<IRepository<EstadoReserva>>();
        estadosReserva.Setup(r => r.GetByAsNoTrackingAsync(It.IsAny<Expression<Func<EstadoReserva, bool>>>()))
            .ReturnsAsync(new EstadoReserva { IdEstadoReserva = 3, Codigo = Constants.ESTADO_RESERVA.Cancelado });
        var estadosPago = new Mock<IRepository<EstadoPago>>();
        estadosPago.Setup(r => r.GetByAsNoTrackingAsync(It.IsAny<Expression<Func<EstadoPago, bool>>>()))
            .ReturnsAsync(new EstadoPago { IdEstadoPago = 4, Codigo = Constants.ESTADO_PAGO.Cancelado });
        var mapper = new MapperConfiguration(c => c.AddMaps(typeof(LiberarReservaOperadorCommandHandler).Assembly)).CreateMapper();
        var handler = new LiberarReservaOperadorCommandHandler(Mock.Of<IUnitOfWork>(), mapper, Mock.Of<IMediator>(),
            new LiberarReservaOperadorCommandValidator(), reservas.Object, estadosReserva.Object, pagos.Object,
            estadosPago.Object, s.Access, s.Service);
        var dto = new LiberarReservaOperadorDto { IdReserva = 2, RevisionPago = 0, IdOperacion = Guid.NewGuid(),
            MontoReembolso = 50, CodigoMetodoPago = "02", ReferenciaOperacion = "REC-DEV-1" };
        Assert.True((await handler.HandleCommand(new LiberarReservaOperadorCommand(dto), default)).IsValid);
        Assert.Equal(50, s.Payment.MontoReembolso);
        Assert.Equal(50, s.Payment.MontoAdelanto);
        Assert.Equal(0, s.Payment.MontoPendiente);
        Assert.Equal(Constants.ESTADO_RESERVA.Cancelado, s.Reservation.IdEstadoReservaNavigation.Codigo);
        Assert.True((await handler.HandleCommand(new LiberarReservaOperadorCommand(dto), default)).IsValid);
        Assert.Single(s.Movements.Where(m => m.Tipo == "REEMBOLSO"));
        dto.ReferenciaOperacion = "OTRA";
        Assert.False((await handler.HandleCommand(new LiberarReservaOperadorCommand(dto), default)).IsValid);
        Assert.Equal(50, s.Payment.MontoReembolso);
    }
    [Fact]
    public async Task Cobro_IncrementaSinSobrescribirYConservaSaldoPrevio()
    {
        var s = new Scenario();
        var response = await s.Service.RegistrarAsync(s.Request(20), false);
        Assert.True(response.IsValid);
        Assert.Equal(70, s.Payment.MontoAdelanto);
        Assert.Equal(30, s.Payment.MontoPendiente);
        Assert.Equal(1, s.Payment.Revision);
        Assert.Equal(new[] { "SALDO_INICIAL", "COBRO" }, s.Movements.Select(m => m.Tipo));
        Assert.Equal(50, s.Movements[0].MontoResultante);
        Assert.Equal(20, s.Movements[1].Importe);
    }

    [Theory]
    [InlineData(40)]
    [InlineData(60)]
    [InlineData(0)]
    public async Task Proveedor_CorrigeAcumuladoConDiferenciaSinEliminarOriginal(decimal correcto)
    {
        var s = new Scenario();
        var response = await s.Service.RegistrarAsync(s.Request(correcto, "Error al digitar el adelanto"), true);
        Assert.True(response.IsValid);
        Assert.Equal(correcto, s.Payment.MontoAdelanto);
        Assert.Equal(100 - correcto, s.Payment.MontoPendiente);
        Assert.Equal("CORRECCION", s.Movements[1].Tipo);
        Assert.Equal(correcto - 50, s.Movements[1].Importe);
        Assert.Equal(50, s.Movements[1].MontoAnterior);
    }

    [Fact]
    public async Task Operador_PuedeCobrarPeroNoCorregir()
    {
        var s = new Scenario(operador: true);
        Assert.False((await s.Service.RegistrarAsync(s.Request(40, "Error de registro"), true)).IsValid);
        Assert.True((await s.Service.RegistrarAsync(s.Request(10), false)).IsValid);
        Assert.Equal(60, s.Payment.MontoAdelanto);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task OtroNegocioOSinCanchaAsignada_NoPuedeConsultarNiCobrar(bool operador)
    {
        var s = new Scenario(operador);
        if (operador) s.Assignments.Clear();
        else s.Identity.Setup(i => i.GetCurrentUserIdNegocio()).Returns(999);
        Assert.False((await s.Service.ObtenerAsync(1)).IsValid);
        Assert.False((await s.Service.RegistrarAsync(s.Request(10), false)).IsValid);
        Assert.Empty(s.Movements);
    }

    [Fact]
    public async Task ReintentarMismaOperacion_NoDuplicaCobro()
    {
        var s = new Scenario();
        var dto = s.Request(20);
        Assert.True((await s.Service.RegistrarAsync(dto, false)).IsValid);
        Assert.True((await s.Service.RegistrarAsync(dto, false)).IsValid);
        Assert.Equal(70, s.Payment.MontoAdelanto);
        Assert.Equal(2, s.Movements.Count);
        dto.Monto = 10;
        Assert.False((await s.Service.RegistrarAsync(dto, false)).IsValid);
    }

    [Fact]
    public async Task OperacionConVersionAnterior_NoSobrescribeOtroCobro()
    {
        var s = new Scenario();
        var primero = s.Request(20);
        var segundo = s.Request(10);
        Assert.True((await s.Service.RegistrarAsync(primero, false)).IsValid);
        Assert.False((await s.Service.RegistrarAsync(segundo, false)).IsValid);
        Assert.Equal(70, s.Payment.MontoAdelanto);
    }

    [Theory]
    [InlineData(51, false)]
    [InlineData(101, true)]
    [InlineData(-1, true)]
    [InlineData(50, true)]
    public async Task RechazaSobrepagoOResultadoInvalido(decimal monto, bool correccion)
    {
        var s = new Scenario();
        Assert.False((await s.Service.RegistrarAsync(s.Request(monto, correccion ? "Error de registro" : null), correccion)).IsValid);
        Assert.Empty(s.Movements);
        Assert.Equal(50, s.Payment.MontoAdelanto);
    }

    [Theory]
    [InlineData(Constants.ESTADO_RESERVA.Cancelado)]
    [InlineData(Constants.ESTADO_RESERVA.Expirado)]
    public async Task ReservaNoVigente_NoRecibeCobros(string estado)
    {
        var s = new Scenario();
        s.Reservation.IdEstadoReservaNavigation.Codigo = estado;
        Assert.False((await s.Service.RegistrarAsync(s.Request(10), false)).IsValid);
    }

    [Fact]
    public async Task ReservaConfirmada_NoSeBloqueaPorAntiguaFechaDePreReserva()
    {
        var s = new Scenario();
        s.Reservation.FechaExpiracionPreReserva = DateTimeOffset.UtcNow.AddDays(-1);
        Assert.True((await s.Service.RegistrarAsync(s.Request(10), false)).IsValid);
    }

    [Theory]
    [InlineData(-1, false, null)]
    [InlineData(0, false, null)]
    [InlineData(10.001, false, null)]
    [InlineData(40, true, "")]
    [InlineData(40, true, "    x")]
    public async Task Validador_RechazaImportesYMotivosInvalidos(double monto, bool correccion, string? motivo)
    {
        var dto = new Scenario().Request((decimal)monto, motivo);
        var result = await new RegistrarMovimientoPagoCommandValidator().ValidateAsync(new RegistrarMovimientoPagoCommand(dto, correccion));
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Modelo_ConfiguraVersionConcurrenteYOperacionUnica()
    {
        using var context = new ReservaCanchasContext(new DbContextOptionsBuilder<ReservaCanchasContext>()
            .UseSqlServer("Server=(local);Database=Modelo;Trusted_Connection=True;TrustServerCertificate=True").Options);
        Assert.True(context.Model.FindEntityType(typeof(Entity.Pago))!.FindProperty(nameof(Entity.Pago.Revision))!.IsConcurrencyToken);
        Assert.Contains(context.Model.FindEntityType(typeof(MovimientoPago))!.GetIndexes(),
            index => index.IsUnique && index.Properties.Single().Name == nameof(MovimientoPago.IdOperacion));
    }

    private sealed class Scenario
    {
        public Mock<IUserIdentity> Identity { get; } = new();
        public List<MovimientoPago> Movements { get; } = new();
        public List<OperadorCancha> Assignments { get; } = new() { new() { IdOperador = 7, IdCancha = 3, Activo = true } };
        public Entity.Reserva Reservation { get; }
        public Entity.Pago Payment { get; }
        public List<Entity.Pago> Payments { get; } = new();
        public List<(int Estado, decimal Cobrado, decimal Pendiente, int Movimientos)> InsercionesPago { get; } = new();
        public Mock<IRepository<Entity.Pago>> Pagos { get; }
        public MovimientoPagoService Service { get; }
        public PagoReservaAccessService Access { get; }

        public Scenario(bool operador = false, bool sinPago = false, decimal porcentajeMinimo = 0)
        {
            var usuario = Guid.NewGuid();
            Identity.Setup(i => i.GetCurrentUserId()).Returns(usuario);
            Identity.Setup(i => i.GetCurrentUserIdNegocio()).Returns(operador ? 7 : 9);
            Identity.Setup(i => i.GetUserRole()).Returns(operador ? Constants.Role.Operador : Constants.Role.Proveedor);
            Reservation = new Entity.Reserva
            {
                IdReserva = 2, IdCancha = 3, Activo = true, MontoTotal = 100, FechaReserva = DateTimeOffset.UtcNow.AddDays(1),
                IdCanchaNavigation = new Cancha { IdCancha = 3, IdProveedor = 9, ZonaHoraria = "America/Lima" },
                IdEstadoReservaNavigation = new EstadoReserva {
                    Codigo = sinPago ? Constants.ESTADO_RESERVA.Pendiente : Constants.ESTADO_RESERVA.Confirmado }
            };
            Payment = new Entity.Pago
            {
                IdPago = 1, IdReserva = 2, Monto = 100, MontoAdelanto = 50, MontoPendiente = 50,
                Activo = true, IdReservaNavigation = Reservation,
                IdEstadoPagoNavigation = new EstadoPago { Codigo = Constants.ESTADO_PAGO.Parcial }
            };
            if (!sinPago) { Payments.Add(Payment); Reservation.Pago.Add(Payment); }
            var payments = Repository(Payments);
            Pagos = payments;
            payments.Setup(r => r.GetByAsync(It.IsAny<Expression<Func<Entity.Pago, bool>>>(),
                It.IsAny<Expression<Func<Entity.Pago, object>>[]>()))
                .Returns((Expression<Func<Entity.Pago, bool>> p, Expression<Func<Entity.Pago, object>>[] _) =>
                    Task.FromResult(Payments.FirstOrDefault(p.Compile())));
            payments.Setup(r => r.AddAsync(It.IsAny<Entity.Pago>())).Returns((Entity.Pago pago) =>
            {
                // AddAsync persiste inmediatamente: la FK y los importes deben ser válidos en este punto.
                if (pago.IdEstadoPago <= 0) throw new DbUpdateException("FK Pago.EstadoPago: no se puede insertar el estado cero.");
                InsercionesPago.Add((pago.IdEstadoPago, pago.MontoAdelanto, pago.MontoPendiente, pago.MovimientoPago.Count));
                pago.IdPago = 100 + Payments.Count;
                Payments.Add(pago);
                foreach (var movimiento in pago.MovimientoPago)
                {
                    movimiento.IdPago = pago.IdPago;
                    movimiento.IdMovimientoPago = Movements.Count + 1;
                    Movements.Add(movimiento);
                }
                return Task.FromResult<Entity.Pago?>(pago);
            });
            var movements = Repository(Movements);
            movements.Setup(r => r.AddAsync(It.IsAny<MovimientoPago>())).Returns((MovimientoPago m) =>
            {
                m.IdMovimientoPago = Movements.Count + 1;
                Movements.Add(m);
                return Task.FromResult<MovimientoPago?>(m);
            });
            var operators = Repository(new[] { new Operador { IdOperador = 7, IdProveedor = 9, IdUsuario = usuario.ToString(), Activo = true } });
            var methods = Repository(new[] { new MetodoPago { IdMetodoPago = 2, Codigo = "02", Nombre = "Efectivo", Activo = true },
                new MetodoPago { IdMetodoPago = 4, Codigo = "04", Nombre = "Yape", Activo = true } });
            var states = Repository(new[] { new EstadoPago { IdEstadoPago = 2, Codigo = Constants.ESTADO_PAGO.Parcial, Activo = true },
                new EstadoPago { IdEstadoPago = 1, Codigo = Constants.ESTADO_PAGO.Pagado, Activo = true },
                new EstadoPago { IdEstadoPago = 3, Codigo = Constants.ESTADO_PAGO.Pendiente, Activo = true } });
            var reservas = Repository(new[] { Reservation });
            reservas.Setup(r => r.GetByAsync(It.IsAny<Expression<Func<Entity.Reserva, bool>>>(),
                It.IsAny<Expression<Func<Entity.Reserva, object>>[]>())).ReturnsAsync(Reservation);
            Access = new PagoReservaAccessService(Identity.Object, operators.Object, Repository(Assignments).Object);
            Service = new MovimientoPagoService(payments.Object, movements.Object, methods.Object, states.Object,
                Repository(new[] { new EstadoReserva { IdEstadoReserva = 2, Codigo = Constants.ESTADO_RESERVA.Confirmado, Activo = true } }).Object,
                Access,
                Repository(new[] { new ConfiguracionProveedor { IdProveedor = 9, Activo = true,
                    PorcentajeAdelantoMinimo = porcentajeMinimo } }).Object, reservas.Object);
        }

        public RegistrarMovimientoPagoDto Request(decimal monto, string? motivo = null) => new()
        {
            IdPago = 1, IdOperacion = Guid.NewGuid(), Revision = Payment.Revision,
            Monto = monto, Motivo = motivo, CodigoMetodoPago = "02"
        };

        private static Mock<IRepository<T>> Repository<T>(IEnumerable<T> items) where T : class
        {
            var mock = new Mock<IRepository<T>>();
            mock.Setup(r => r.GetByAsync(It.IsAny<Expression<Func<T, bool>>>()))
                .Returns((Expression<Func<T, bool>> p) => Task.FromResult(items.FirstOrDefault(p.Compile())));
            mock.Setup(r => r.GetByAsNoTrackingAsync(It.IsAny<Expression<Func<T, bool>>>()))
                .Returns((Expression<Func<T, bool>> p) => Task.FromResult(items.FirstOrDefault(p.Compile())));
            mock.Setup(r => r.FindByAsync(It.IsAny<Expression<Func<T, bool>>>()))
                .Returns((Expression<Func<T, bool>> p) => Task.FromResult(items.Where(p.Compile())));
            mock.Setup(r => r.FindByAsNoTrackingAsync(It.IsAny<Expression<Func<T, bool>>>()))
                .Returns((Expression<Func<T, bool>> p) => Task.FromResult(items.Where(p.Compile())));
            return mock;
        }
    }
}
