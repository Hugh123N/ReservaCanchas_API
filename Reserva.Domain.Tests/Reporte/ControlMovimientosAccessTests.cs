using Moq;
using Reserva.Common;
using Reserva.Domain.Services.Reporte;
using Reserva.Dto.Reporte;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Security;
using Xunit;

namespace Reserva.Domain.Tests.Reporte;

public class ControlMovimientosAccessTests
{
    [Theory]
    [InlineData(Constants.Role.Operador)]
    [InlineData(Constants.Role.Cliente)]
    [InlineData(Constants.Role.Admin)]
    public async Task SoloProveedor_PuedeConsultarORecibirElCatalogo(string role)
    {
        var identity = new Mock<IUserIdentity>();
        identity.Setup(i => i.GetUserRole()).Returns(role);
        var movimientos = new Mock<IRepository<Entity.MovimientoPago>>();
        var access = new ReporteAccessService(identity.Object, Mock.Of<IRepository<Entity.Cancha>>(), Mock.Of<IRepository<Entity.Operador>>(), Mock.Of<IRepository<Entity.OperadorCancha>>());
        var service = new ControlMovimientosService(access, movimientos.Object, Mock.Of<IRepository<Entity.MetodoPago>>(), Mock.Of<IRepository<Entity.Operador>>(), identity.Object);
        Assert.False((await service.FiltrosAsync(default)).IsValid);
        Assert.False((await service.ObtenerAsync(new(), default)).IsValid);
        movimientos.Verify(r => r.FindAllAsNoTracking(), Times.Never);
    }
}
