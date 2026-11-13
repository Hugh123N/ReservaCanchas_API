using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Reserva.Api.Controllers.Reporte;
using Reserva.Application.Abstractions.Reporte;
using Reserva.Common;
using Reserva.Dto.Base;
using Reserva.Dto.Reporte;
using Xunit;

namespace Reserva.Api.Tests.Reporte;

public class ReporteControllerTests
{
    [Theory]
    [InlineData(nameof(ReporteController.Economico), Constants.Role.Proveedor)]
    [InlineData(nameof(ReporteController.EconomicoExcel), Constants.Role.Proveedor)]
    [InlineData(nameof(ReporteController.Demanda), Constants.Role.Proveedor)]
    [InlineData(nameof(ReporteController.DemandaExcel), Constants.Role.Proveedor)]
    [InlineData(nameof(ReporteController.AgendaOperador), Constants.Role.Operador)]
    [InlineData(nameof(ReporteController.AgendaOperadorExcel), Constants.Role.Operador)]
    public void ConsultaYExcel_ExigenBearerYElRolDelReporte(string action, string role)
    {
        var attribute = typeof(ReporteController).GetMethod(action)!.GetCustomAttribute<AuthorizeAttribute>()!;
        Assert.NotNull(attribute);
        Assert.Equal(JwtBearerDefaults.AuthenticationScheme, attribute.AuthenticationSchemes);
        Assert.Equal(role, attribute.Roles);
    }

    [Fact]
    public async Task ExportacionRechazada_NoDevuelveUnArchivoConUnErrorDentro()
    {
        var application = new Mock<IReporteApplication>();
        var failure = new ResponseDto<ArchivoReporteDto>();
        failure.AddErrorResult("Cancha fuera de tus permisos.");
        application.Setup(a => a.Exportar(TipoReporte.AgendaOperador, It.IsAny<FiltroReporteDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failure);
        var result = await new ReporteController(application.Object).AgendaOperadorExcel(new(), default);
        Assert.IsType<BadRequestObjectResult>(result);
    }
}
