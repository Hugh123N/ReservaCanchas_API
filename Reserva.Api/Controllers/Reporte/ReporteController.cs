using Microsoft.AspNetCore.Mvc;
using Reserva.Application.Abstractions.Reporte;
using Reserva.Common;
using Reserva.Common.Excel;
using Reserva.Dto.Base;
using Reserva.Dto.Reporte;

namespace Reserva.Api.Controllers.Reporte;

[ApiController]
[Route("api/Reporte")]
public class ReporteController(IReporteApplication application) : ControllerBase
{
    [Security.Authorize(Roles = Constants.Role.Proveedor + "," + Constants.Role.Operador)]
    [HttpGet("filtros")]
    public Task<ResponseDto<FiltrosReporteDto>> Filtros(CancellationToken cancellationToken)
        => application.Filtros(cancellationToken);

    [Security.Authorize(Roles = Constants.Role.Proveedor)]
    [HttpPost("economico")]
    public Task<ResponseDto<ReporteDto>> Economico(FiltroReporteDto filtro, CancellationToken cancellationToken)
        => application.Consultar(TipoReporte.Economico, filtro, cancellationToken);

    [Security.Authorize(Roles = Constants.Role.Proveedor)]
    [HttpPost("demanda")]
    public Task<ResponseDto<ReporteDto>> Demanda(FiltroReporteDto filtro, CancellationToken cancellationToken)
        => application.Consultar(TipoReporte.Demanda, filtro, cancellationToken);

    [Security.Authorize(Roles = Constants.Role.Operador)]
    [HttpPost("agenda-operador")]
    public Task<ResponseDto<ReporteDto>> AgendaOperador(FiltroReporteDto filtro, CancellationToken cancellationToken)
        => application.Consultar(TipoReporte.AgendaOperador, filtro, cancellationToken);

    [Security.Authorize(Roles = Constants.Role.Proveedor)]
    [HttpPost("economico/excel")]
    public async Task<IActionResult> EconomicoExcel(FiltroReporteDto filtro, CancellationToken cancellationToken)
        => Archivo(await application.Exportar(TipoReporte.Economico, filtro, cancellationToken));

    [Security.Authorize(Roles = Constants.Role.Proveedor)]
    [HttpPost("demanda/excel")]
    public async Task<IActionResult> DemandaExcel(FiltroReporteDto filtro, CancellationToken cancellationToken)
        => Archivo(await application.Exportar(TipoReporte.Demanda, filtro, cancellationToken));

    [Security.Authorize(Roles = Constants.Role.Operador)]
    [HttpPost("agenda-operador/excel")]
    public async Task<IActionResult> AgendaOperadorExcel(FiltroReporteDto filtro, CancellationToken cancellationToken)
        => Archivo(await application.Exportar(TipoReporte.AgendaOperador, filtro, cancellationToken));

    private IActionResult Archivo(ResponseDto<ArchivoReporteDto> response)
        => response.IsValid && response.Data != null
            ? File(response.Data.Contenido, ExportadorExcel.ContentType, response.Data.Nombre)
            : BadRequest(response);
}
