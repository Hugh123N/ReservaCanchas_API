using System.Data;
using System.Linq.Expressions;
using AutoMapper;
using MediatR;
using Moq;
using Reserva.Common;
using Reserva.Domain.Commands.Dbo.Calendario;
using Reserva.Domain.Services.Pago;
using Reserva.Dto.Dbo.Calendario;
using Reserva.Entity;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;
using Reserva.Repository.Security;
using Xunit;

namespace Reserva.Domain.Tests.Pago;

public class CrearReservaMovimientoTests
{
    [Theory]
    [InlineData(TipoReservaOperador.Normal)]
    [InlineData(TipoReservaOperador.Inmediata)]
    public async Task CrearReserva_SoloCreaPagoYMovimientoCuandoRecibeDinero(TipoReservaOperador tipo)
    {
        var usuario = Guid.NewGuid();
        var identidad = new Mock<IUserIdentity>();
        identidad.Setup(i => i.GetCurrentUserId()).Returns(usuario);
        identidad.Setup(i => i.GetUserRole()).Returns(Constants.Role.Proveedor);
        identidad.Setup(i => i.GetCurrentUserIdNegocio()).Returns(9);
        var cliente = new Cliente { IdCliente = 1, Activo = true, TipoDoc = "DNI", NroDoc = "12345678", Nombres = "Cliente", Apellidos = "Prueba" };
        var clientes = Repository(new[] { cliente });
        var cancha = new Cancha { IdCancha = 3, IdProveedor = 9, Activo = true, ZonaHoraria = "America/Lima",
            IdProveedorNavigation = new Proveedor { ConfiguracionProveedor = new ConfiguracionProveedor { PorcentajeAdelantoMinimo = 50 } } };
        var canchas = Repository(new[] { cancha });
        var horario = new HorarioCancha { IdHorarioCancha = 10, IdCancha = 3, IdDiaSemana = 1, PrecioHora = 100, Activo = true,
            IdHoraInicioNavigation = new Hora { Hora1 = new TimeOnly(10, 0) },
            IdHoraFinNavigation = new Hora { Hora1 = new TimeOnly(11, 0) } };
        var horarios = Repository(new[] { horario });
        var reservas = Repository(Array.Empty<Entity.Reserva>());
        Entity.Reserva? creada = null;
        reservas.Setup(r => r.AddAsync(It.IsAny<Entity.Reserva>())).Returns((Entity.Reserva r) =>
        {
            creada = r;
            r.IdReserva = 7;
            return Task.FromResult<Entity.Reserva?>(r);
        });
        reservas.Setup(r => r.ExecuteScalarSPAsync<string>(It.IsAny<string>(), It.IsAny<IDbDataParameter[]>()))
            .ReturnsAsync("RES-2026-001");
        var estadosReserva = Repository(new[] {
            new EstadoReserva { IdEstadoReserva = 1, Codigo = Constants.ESTADO_RESERVA.Pendiente, Activo = true },
            new EstadoReserva { IdEstadoReserva = 2, Codigo = Constants.ESTADO_RESERVA.Confirmado, Activo = true } });
        var estadosPago = Repository(new[] { new EstadoPago { IdEstadoPago = 2, Codigo = Constants.ESTADO_PAGO.Parcial, Activo = true } });
        var metodos = Repository(new[] { new MetodoPago { IdMetodoPago = 4, Codigo = "04", Activo = true } });
        var operadores = Repository(Array.Empty<Operador>());
        var access = new PagoReservaAccessService(identidad.Object, operadores.Object, Repository(Array.Empty<OperadorCancha>()).Object);
        var movimientos = new MovimientoPagoService(Repository(Array.Empty<Entity.Pago>()).Object,
            Repository(Array.Empty<MovimientoPago>()).Object, metodos.Object, estadosPago.Object, estadosReserva.Object,
            access, Repository(Array.Empty<ConfiguracionProveedor>()).Object, reservas.Object);
        var mapper = new MapperConfiguration(c => c.AddMaps(typeof(CrearReservaOperadorCommandHandler).Assembly)).CreateMapper();
        var handler = new CrearReservaOperadorCommandHandler(Mock.Of<IUnitOfWork>(), mapper, Mock.Of<IMediator>(),
            new CrearReservaOperadorCommandValidator(clientes.Object, canchas.Object), clientes.Object, reservas.Object,
            Repository(Array.Empty<DetalleReserva>()).Object, horarios.Object, canchas.Object, estadosReserva.Object,
            estadosPago.Object, identidad.Object, Mock.Of<IDbParameterFactory>(), operadores.Object, movimientos, access);
        var operacion = Guid.NewGuid();
        var dto = new CrearReservaOperadorRequestDto { TipoReserva = tipo, IdCancha = 3, IdTipoDeporte = 1,
            Cliente = new ClienteReservaDto { IdCliente = 1, TipoDoc = "DNI", NroDoc = "12345678", Telefono = "999888777" },
            Horarios = new List<BloqueHorarioDto> { new() { Fecha = new DateTimeOffset(2026, 12, 7, 0, 0, 0, TimeSpan.FromHours(-5)),
                IdHorarioCanchaInicio = 10, IdHorarioCanchaFin = 10 } },
            Pago = new PagoReservaOperadorDto { MontoTotal = 100, MontoPagado = tipo == TipoReservaOperador.Normal ? 0 : 50,
                CodigoMetodoPago = "04", IdOperacion = operacion, ReferenciaOperacion = "YAPE-123" } };

        var respuesta = await handler.HandleCommand(new CrearReservaOperadorCommand { RequestDto = dto }, default);

        Assert.True(respuesta.IsValid, string.Join(";", respuesta.Messages.Select(m => m.Message)));
        Assert.NotNull(creada);
        Assert.Equal(100, creada.MontoTotal);
        if (tipo == TipoReservaOperador.Normal)
        {
            Assert.Empty(creada.Pago);
            Assert.Null(respuesta.Data.IdPago);
            Assert.Equal(1, creada.IdEstadoReserva);
        }
        else
        {
            var pago = Assert.Single(creada.Pago);
            Assert.Equal(50, pago.MontoAdelanto);
            Assert.Equal(50, pago.MontoPendiente);
            var movimiento = Assert.Single(pago.MovimientoPago);
            Assert.Equal(usuario, movimiento.IdUsuarioResponsable);
            Assert.Equal(Constants.Role.Proveedor, movimiento.RolResponsable);
            Assert.Equal(4, movimiento.IdMetodoPago);
            Assert.Equal(operacion, movimiento.IdOperacion);
            Assert.Equal("YAPE-123", movimiento.ReferenciaOperacion);
            Assert.Null(creada.FechaExpiracionPreReserva);
        }
    }

    private static Mock<IRepository<T>> Repository<T>(IEnumerable<T> items) where T : class
    {
        var mock = new Mock<IRepository<T>>();
        mock.Setup(r => r.GetByAsync(It.IsAny<Expression<Func<T, bool>>>())).Returns((Expression<Func<T, bool>> p) =>
            Task.FromResult(items.FirstOrDefault(p.Compile())));
        mock.Setup(r => r.GetByAsync(It.IsAny<Expression<Func<T, bool>>>(), It.IsAny<Expression<Func<T, object>>[]>()))
            .Returns((Expression<Func<T, bool>> p, Expression<Func<T, object>>[] _) => Task.FromResult(items.FirstOrDefault(p.Compile())));
        mock.Setup(r => r.FindByAsync(It.IsAny<Expression<Func<T, bool>>>(), It.IsAny<Expression<Func<T, object>>[]>()))
            .Returns((Expression<Func<T, bool>> p, Expression<Func<T, object>>[] _) => Task.FromResult(items.Where(p.Compile())));
        return mock;
    }
}
