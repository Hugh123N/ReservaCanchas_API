using Reserva.Dto.Base;
using Reserva.Dto.Reporte;

namespace Reserva.Application.Abstractions.Reporte;

public interface IReporteApplication
{
    Task<ResponseDto<FiltrosControlMovimientosDto>> FiltrosControl(CancellationToken cancellationToken);
    Task<ResponseDto<ControlMovimientosDto>> Control(FiltroControlMovimientosDto filtro, CancellationToken cancellationToken);
    Task<ResponseDto<ArchivoReporteDto>> ExportarControl(FiltroControlMovimientosDto filtro, CancellationToken cancellationToken);
    Task<ResponseDto<FiltrosReporteDto>> Filtros(CancellationToken cancellationToken);
    Task<ResponseDto<ReporteDto>> Consultar(TipoReporte tipo, FiltroReporteDto filtro, CancellationToken cancellationToken);
    Task<ResponseDto<ArchivoReporteDto>> Exportar(TipoReporte tipo, FiltroReporteDto filtro, CancellationToken cancellationToken);
}
