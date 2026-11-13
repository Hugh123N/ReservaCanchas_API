using System.Linq.Expressions;
using AutoMapper;
using Moq;
using Reserva.Common;
using Reserva.Domain.Commands.Dbo.Reserva;
using Reserva.Domain.Commands.Dbo.Pago;
using Reserva.Domain.Services.Pago;
using Reserva.Dto.Dbo.Pago;
using Reserva.Dto.Dbo.Reserva;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;
using Reserva.Repository.Security;
using Xunit;

namespace Reserva.Domain.Tests.Pago;

public class ReservaPagoGuardTests
{
    [Fact]
    public async Task EditarReservaPendienteSinPago_NoCreaCabeceraNiDinero()
    {
        var s = new Scenario();
        s.Reservation.Pago.Clear();
        s.Reservation.MontoTotal = 100;
        s.Reservation.IdEstadoReservaNavigation.Codigo = Constants.ESTADO_RESERVA.Pendiente;
        var dto = s.Request();
        dto.RevisionPago = 0;
        dto.PagoReserva.Clear();
        dto.Observaciones = "Todavía sin cobro";
        var resultado = await s.Handler.HandleCommand(new(dto), default);
        Assert.True(resultado.IsValid);
        Assert.Null(resultado.Data.PagoReserva);
        Assert.Equal(100, resultado.Data.MontoTotal);
        Assert.Empty(s.Reservation.Pago);
    }
    [Theory]
    [InlineData(40)]
    [InlineData(60)]
    public async Task EditarReserva_NoPermiteReemplazarAcumulado(decimal montoEnviado)
    {
        var s = new Scenario();
        var dto = s.Request();
        dto.PagoReserva[0].MontoAdelanto = montoEnviado;
        Assert.False((await s.Handler.HandleCommand(new(dto), default)).IsValid);
        Assert.Equal(50, s.Payment.MontoAdelanto);
        s.Reservas.Verify(r => r.UpdateAsync(It.IsAny<Entity.Reserva>()), Times.Never);
    }

    [Fact]
    public async Task EditarReserva_ConVersionViejaNoGuarda()
    {
        var s = new Scenario();
        var dto = s.Request();
        dto.RevisionPago = 0;
        Assert.False((await s.Handler.HandleCommand(new(dto), default)).IsValid);
    }

    [Fact]
    public async Task ReducirHorarios_NoPermiteTotalMenorQueLoCobrado()
    {
        var s = new Scenario();
        s.Horarios.Setup(r => r.FindByAsync(It.IsAny<Expression<Func<Entity.HorarioCancha, bool>>>(),
            It.IsAny<Expression<Func<Entity.HorarioCancha, object>>[]>()))
            .ReturnsAsync(new[] { new Entity.HorarioCancha { IdHorarioCancha = 20, PrecioHora = 30 } });
        var dto = s.Request();
        dto.Horarios.Add(new() { Fecha = s.Reservation.FechaReserva, IdHorarioCanchaInicio = 20, IdHorarioCanchaFin = 20 });
        Assert.False((await s.Handler.HandleCommand(new(dto), default)).IsValid);
        Assert.Equal(50, s.Payment.MontoAdelanto);
        Assert.Equal(100, s.Payment.Monto);
        s.Reservas.Verify(r => r.UpdateAsync(It.IsAny<Entity.Reserva>()), Times.Never);
    }

    [Fact]
    public async Task EditarObservaciones_ConservaDineroYPrecioAunqueElClienteEnvieOtroTotal()
    {
        var s = new Scenario();
        var dto = s.Request();
        dto.Observaciones = "Nueva observación";
        dto.PagoReserva[0].Monto = 1;
        Assert.True((await s.Handler.HandleCommand(new(dto), default)).IsValid);
        Assert.Equal(50, s.Payment.MontoAdelanto);
        Assert.Equal(100, s.Payment.Monto);
        Assert.Equal(50, s.Payment.MontoPendiente);
        Assert.Equal("Nueva observación", s.Reservation.Observaciones);
        Assert.Equal(3, s.Payment.Revision);
    }

    [Fact]
    public async Task Update_DevuelveDetalleCompletoYReutilizaLosHorariosYaCargados()
    {
        var s = new Scenario();
        s.Reservation.CodigoReserva = "RES-002";
        s.Reservation.IdCanchaNavigation.Nombre = "Cancha 3";
        s.Reservation.IdClienteNavigation = new Entity.Cliente { Nombres = "Cliente", Apellidos = "Prueba", Telefono = "999888777" };
        s.Reservation.DetalleReserva.Add(new Entity.DetalleReserva { IdDetalleReserva = 1, IdHorarioCancha = 10, Activo = true });
        var seleccionado = new Entity.HorarioCancha
        {
            IdHorarioCancha = 20, PrecioHora = 120,
            IdHoraInicioNavigation = new Entity.Hora { Hora1 = new TimeOnly(13, 0) },
            IdHoraFinNavigation = new Entity.Hora { Hora1 = new TimeOnly(14, 0) }
        };
        s.Horarios.Setup(r => r.FindByAsync(It.IsAny<Expression<Func<Entity.HorarioCancha, bool>>>(),
            It.IsAny<Expression<Func<Entity.HorarioCancha, object>>[]>())).ReturnsAsync(new[] { seleccionado });
        s.EstadosPago.Setup(r => r.GetByAsync(It.IsAny<Expression<Func<Entity.EstadoPago, bool>>>()))
            .ReturnsAsync(new Entity.EstadoPago { IdEstadoPago = 2, Codigo = Constants.ESTADO_PAGO.Parcial, Nombre = "Parcial", Activo = true });
        var dto = s.Request();
        dto.Observaciones = "Horario actualizado";
        dto.Horarios.Add(new() { Fecha = s.Reservation.FechaReserva, IdHorarioCanchaInicio = 20, IdHorarioCanchaFin = 20 });

        var response = await s.Handler.HandleCommand(new(dto), default);

        Assert.True(response.IsValid);
        Assert.Equal("RES-002", response.Data.CodigoReserva);
        Assert.Equal("Cancha 3", response.Data.NombreCancha);
        Assert.Equal("Cliente Prueba", response.Data.NombreCliente);
        Assert.Equal("999888777", response.Data.NumeroCliente);
        Assert.Equal("Horario actualizado", response.Data.Observaciones);
        Assert.Equal(120, response.Data.PagoReserva!.Monto);
        Assert.Equal(50, response.Data.PagoReserva.MontoAdelanto);
        Assert.Equal(70, response.Data.PagoReserva.MontoPendiente);
        Assert.Equal(3, response.Data.PagoReserva.Revision);
        Assert.Equal(Constants.ESTADO_PAGO.Parcial, response.Data.PagoReserva.EstadoPago!.Codigo);
        var horario = Assert.Single(response.Data.Horarios);
        Assert.Equal(20, horario.IdHorarioCancha);
        Assert.Equal(new TimeOnly(13, 0), horario.HoraInicio);
        Assert.Equal(new TimeOnly(14, 0), horario.HoraFin);
        Assert.Equal(13, response.Data.FechaReserva.Hour);
        s.Horarios.Verify(r => r.FindByAsync(It.IsAny<Expression<Func<Entity.HorarioCancha, bool>>>(),
            It.IsAny<Expression<Func<Entity.HorarioCancha, object>>[]>()), Times.Once);
        s.Reservas.Verify(r => r.GetByAsync(It.IsAny<Expression<Func<Entity.Reserva, bool>>>(),
            It.IsAny<Expression<Func<Entity.Reserva, object>>[]>()), Times.Once);
    }

    [Fact]
    public async Task ActualizarPagoGenerico_NoPermiteEludirFlujoDeCobros()
    {
        var payments = new Mock<IRepository<Entity.Pago>>();
        payments.Setup(r => r.GetByAsync(It.IsAny<Expression<Func<Entity.Pago, bool>>>()))
            .ReturnsAsync(new Entity.Pago { IdPago = 1, IdReserva = 2, MontoAdelanto = 50 });
        var handler = new UpdatePagoCommandHandler(Mock.Of<IUnitOfWork>(), Mock.Of<IMapper>(),
            new UpdatePagoCommandValidator(payments.Object), payments.Object);
        var response = await handler.HandleCommand(new UpdatePagoCommand(new UpdatePagoDto { IdPago = 1, MontoAdelanto = 10 }), default);
        Assert.False(response.IsValid);
        payments.Verify(r => r.UpdateAsync(It.IsAny<Entity.Pago>()), Times.Never);
    }

    [Fact]
    public async Task CancelarSimple_NoOcultaReservasConDineroRecibido()
    {
        var s = new Scenario();
        var handler = new DeleteReservaCommandHandler(Mock.Of<IUnitOfWork>(), Mock.Of<IMapper>(),
            new DeleteReservaCommandValidator(s.Reservas.Object), s.Reservas.Object, s.Access);
        Assert.False((await handler.HandleCommand(new DeleteReservaCommand(2), default)).IsValid);
        Assert.True(s.Reservation.Activo);
    }

    private sealed class Scenario
    {
        public Entity.Pago Payment { get; } = new() { IdPago = 1, IdReserva = 2, Monto = 100,
            MontoAdelanto = 50, MontoPendiente = 50, Revision = 2, Activo = true };
        public Entity.Reserva Reservation { get; }
        public Mock<IRepository<Entity.Reserva>> Reservas { get; } = new();
        public Mock<IRepository<Entity.HorarioCancha>> Horarios { get; } = new();
        public Mock<IRepository<Entity.EstadoPago>> EstadosPago { get; } = new();
        public PagoReservaAccessService Access { get; }
        public UpdateReservaCommandHandler Handler { get; }

        public Scenario()
        {
            Reservation = new Entity.Reserva
            {
                IdReserva = 2, IdCancha = 3, FechaReserva = DateTimeOffset.UtcNow.AddDays(1), Activo = true,
                Pago = new List<Entity.Pago> { Payment },
                IdCanchaNavigation = new Entity.Cancha { IdCancha = 3, IdProveedor = 9, ZonaHoraria = "America/Lima" },
                IdEstadoReservaNavigation = new Entity.EstadoReserva { Codigo = Constants.ESTADO_RESERVA.Confirmado }
            };
            Reservas.Setup(r => r.GetByAsync(It.IsAny<Expression<Func<Entity.Reserva, bool>>>(),
                It.IsAny<Expression<Func<Entity.Reserva, object>>[]>())).ReturnsAsync(Reservation);
            var identity = new Mock<IUserIdentity>();
            identity.Setup(i => i.GetCurrentUserId()).Returns(Guid.NewGuid());
            identity.Setup(i => i.GetCurrentUserIdNegocio()).Returns(9);
            identity.Setup(i => i.GetUserRole()).Returns(Constants.Role.Proveedor);
            Access = new PagoReservaAccessService(identity.Object, Mock.Of<IRepository<Entity.Operador>>(), Mock.Of<IRepository<Entity.OperadorCancha>>());
            var mapper = new MapperConfiguration(config => config.AddMaps(typeof(UpdateReservaCommandHandler).Assembly)).CreateMapper();
            Handler = new UpdateReservaCommandHandler(Mock.Of<IUnitOfWork>(), mapper,
                new UpdateReservaCommandValidator(Reservas.Object), Reservas.Object, Mock.Of<IRepository<Entity.DetalleReserva>>(),
                Horarios.Object, EstadosPago.Object, Access);
        }

        public UpdateReservaDto Request() => new()
        {
            IdReserva = 2, IdCancha = 3, FechaReserva = Reservation.FechaReserva, RevisionPago = 2,
            PagoReserva = new List<UpdatePagoDto> { new() { IdPago = 1, Monto = 100, MontoAdelanto = 50 } }
        };
    }
}
