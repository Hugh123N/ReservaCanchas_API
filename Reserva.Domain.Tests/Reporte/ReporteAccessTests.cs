using System.Linq.Expressions;
using Moq;
using Reserva.Common;
using Reserva.Domain.Services.Reporte;
using Reserva.Dto.Reporte;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Security;
using Xunit;

namespace Reserva.Domain.Tests.Reporte;

public class ReporteAccessTests
{
    [Fact]
    public async Task Proveedor_SoloVeSusCanchasIncluidasLasInactivasParaElHistorico()
    {
        var s = new Scenario(Constants.Role.Proveedor);
        var canchas = await s.Access.ObtenerCanchasAsync();
        Assert.Equal(new[] { 3, 4 }, canchas!.Select(c => c.IdCancha));
        Assert.False(canchas[1].Activo);
        Assert.True(s.Access.TieneRol(TipoReporte.Economico));
        Assert.False(s.Access.TieneRol(TipoReporte.AgendaOperador));
    }

    [Fact]
    public async Task Operador_SoloVeAsignacionesActivasYDeSuProveedor()
    {
        var s = new Scenario(Constants.Role.Operador);
        var canchas = await s.Access.ObtenerCanchasAsync();
        Assert.Single(canchas!);
        Assert.Equal(3, canchas[0].IdCancha);
        Assert.True(s.Access.TieneRol(TipoReporte.AgendaOperador));
        Assert.False(s.Access.TieneRol(TipoReporte.Economico));
        Assert.False(s.Access.TieneRol(TipoReporte.Demanda));
    }

    [Fact]
    public async Task Operador_NoPuedeUsarElIdNegocioDeOtroUsuario()
    {
        var s = new Scenario(Constants.Role.Operador);
        s.Identity.Setup(i => i.GetCurrentUserId()).Returns(Guid.NewGuid());
        Assert.Null(await s.Access.ObtenerCanchasAsync());
    }

    [Fact]
    public async Task FiltroDeCanchaAjena_SeRechazaAntesDeConsultarReservas()
    {
        var s = new Scenario(Constants.Role.Proveedor);
        var reservas = new Mock<IRepository<Entity.Reserva>>();
        var service = new ReporteService(s.Access, reservas.Object, Mock.Of<IRepository<Entity.DetalleReserva>>(),
            Mock.Of<IRepository<Entity.HorarioCancha>>(), Mock.Of<IRepository<Entity.EstadoReserva>>(), Mock.Of<IRepository<Entity.EstadoPago>>());
        var response = await service.ObtenerAsync(TipoReporte.Economico, new FiltroReporteDto
            { IdCancha = 99, FechaDesde = new(2026, 11, 1), FechaHasta = new(2026, 11, 13) }, default);
        Assert.False(response.IsValid);
        reservas.Verify(r => r.FindAllAsNoTracking(), Times.Never);
    }

    private sealed class Scenario
    {
        public Mock<IUserIdentity> Identity { get; } = new();
        public ReporteAccessService Access { get; }
        public Scenario(string role)
        {
            var usuario = Guid.NewGuid();
            Identity.Setup(i => i.GetCurrentUserId()).Returns(usuario);
            Identity.Setup(i => i.GetUserRole()).Returns(role);
            Identity.Setup(i => i.GetCurrentUserIdNegocio()).Returns(role == Constants.Role.Proveedor ? 9 : 7);
            var canchas = Repositorio(new[] { new Entity.Cancha { IdCancha = 3, IdProveedor = 9, Nombre = "A", Activo = true },
                new Entity.Cancha { IdCancha = 4, IdProveedor = 9, Nombre = "B", Activo = false },
                new Entity.Cancha { IdCancha = 99, IdProveedor = 88, Nombre = "Ajena", Activo = true } });
            var operadores = Repositorio(new[] { new Entity.Operador { IdOperador = 7, IdProveedor = 9, IdUsuario = usuario.ToString(), Activo = true } });
            var asignaciones = Repositorio(new[] { new Entity.OperadorCancha { IdOperador = 7, IdCancha = 3, Activo = true },
                new Entity.OperadorCancha { IdOperador = 7, IdCancha = 4, Activo = false },
                new Entity.OperadorCancha { IdOperador = 7, IdCancha = 99, Activo = true } });
            Access = new(Identity.Object, canchas.Object, operadores.Object, asignaciones.Object);
        }

        private static Mock<IRepository<T>> Repositorio<T>(IEnumerable<T> data) where T : class
        {
            var mock = new Mock<IRepository<T>>();
            mock.Setup(r => r.GetByAsNoTrackingAsync(It.IsAny<Expression<Func<T, bool>>>()))
                .Returns((Expression<Func<T, bool>> p) => Task.FromResult(data.FirstOrDefault(p.Compile())));
            mock.Setup(r => r.FindByAsNoTrackingAsync(It.IsAny<Expression<Func<T, bool>>>()))
                .Returns((Expression<Func<T, bool>> p) => Task.FromResult(data.Where(p.Compile())));
            return mock;
        }
    }
}
