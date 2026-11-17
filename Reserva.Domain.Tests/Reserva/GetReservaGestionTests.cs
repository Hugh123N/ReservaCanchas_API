using System.Linq.Expressions;
using AutoMapper;
using Moq;
using Reserva.Common;
using Reserva.Domain.Queries.Dbo.Reserva;
using Reserva.Domain.Services.Reporte;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Security;
using Xunit;

namespace Reserva.Domain.Tests.Reserva;

public class GetReservaGestionTests
{
    [Theory]
    [InlineData(Constants.Role.Proveedor, 3, true)]
    [InlineData(Constants.Role.Operador, 3, true)]
    [InlineData(Constants.Role.Proveedor, 99, false)]
    [InlineData(Constants.Role.Operador, 99, false)]
    public async Task Ojito_LeeAnuladasSoloDentroDelAlcanceDelUsuario(string role, int canchaReserva, bool permitido)
    {
        var usuario = Guid.NewGuid();
        var identity = new Mock<IUserIdentity>();
        identity.Setup(i => i.GetCurrentUserId()).Returns(usuario);
        identity.Setup(i => i.GetCurrentUserIdNegocio()).Returns(role == Constants.Role.Proveedor ? 9 : 7);
        identity.Setup(i => i.GetUserRole()).Returns(role);
        var canchas = Repo(new[] { new Entity.Cancha { IdCancha = 3, IdProveedor = 9, Nombre = "Norte", ZonaHoraria = "America/Lima", Activo = true } });
        var operadores = Repo(new[] { new Entity.Operador { IdOperador = 7, IdProveedor = 9, IdUsuario = usuario.ToString(), Activo = true } });
        var asignaciones = Repo(new[] { new Entity.OperadorCancha { IdOperador = 7, IdCancha = 3, Activo = true } });
        var access = new ReporteAccessService(identity.Object, canchas.Object, operadores.Object, asignaciones.Object);
        var reserva = new Entity.Reserva { IdReserva = 10, IdCancha = canchaReserva, Activo = false, CodigoReserva = "RES-10",
            IdCanchaNavigation = new Entity.Cancha { IdCancha = canchaReserva, IdProveedor = 9 },
            IdEstadoReservaNavigation = new Entity.EstadoReserva { Codigo = Constants.ESTADO_RESERVA.Pendiente } };
        var reservas = new Mock<IRepository<Entity.Reserva>>();
        reservas.Setup(r => r.GetByAsNoTrackingAsync(It.IsAny<Expression<Func<Entity.Reserva, bool>>>(),
            It.IsAny<Expression<Func<Entity.Reserva, object>>[]>()))
            .Returns((Expression<Func<Entity.Reserva, bool>> filtro, Expression<Func<Entity.Reserva, object>>[] _) =>
                Task.FromResult(filtro.Compile()(reserva) ? reserva : null));
        var detalles = new Mock<IRepository<Entity.DetalleReserva>>();
        detalles.Setup(r => r.FindByAsNoTrackingAsync(It.IsAny<Expression<Func<Entity.DetalleReserva, bool>>>(),
            It.IsAny<Expression<Func<Entity.DetalleReserva, object>>[]>())).ReturnsAsync(Array.Empty<Entity.DetalleReserva>());
        var mapper = new MapperConfiguration(c => c.AddMaps(typeof(GetReservaQueryHandler).Assembly)).CreateMapper();
        var handler = new GetReservaQueryHandler(mapper, new GetReservaQueryValidator(reservas.Object), reservas.Object,
            detalles.Object, Mock.Of<IRepository<Entity.Pago>>(), Mock.Of<IRepository<Entity.ConfiguracionProveedor>>(), access);

        var resultado = await handler.Handle(new GetReservaQuery(10, gestion: true), default);

        Assert.Equal(permitido, resultado.IsValid);
        if (permitido) Assert.True(resultado.Data!.Anulada);
        else Assert.Null(resultado.Data);
    }

    private static Mock<IRepository<T>> Repo<T>(IEnumerable<T> data) where T : class
    {
        var mock = new Mock<IRepository<T>>();
        mock.Setup(r => r.GetByAsNoTrackingAsync(It.IsAny<Expression<Func<T, bool>>>()))
            .Returns((Expression<Func<T, bool>> p) => Task.FromResult(data.FirstOrDefault(p.Compile())));
        mock.Setup(r => r.FindByAsNoTrackingAsync(It.IsAny<Expression<Func<T, bool>>>()))
            .Returns((Expression<Func<T, bool>> p) => Task.FromResult(data.Where(p.Compile())));
        return mock;
    }
}
