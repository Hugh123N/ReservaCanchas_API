using Reserva.Dto.Base;
using Reserva.Dto.Reporte;

namespace Reserva.Application.Abstractions.Reporte;

public interface IReporteApplication
{
    Task<ResponseDto<FiltrosReporteDto>> Filtros(CancellationToken cancellationToken);
    Task<ResponseDto<ReporteDto>> Consultar(TipoReporte tipo, FiltroReporteDto filtro, CancellationToken cancellationToken);
    Task<ResponseDto<ArchivoReporteDto>> Exportar(TipoReporte tipo, FiltroReporteDto filtro, CancellationToken cancellationToken);
}
