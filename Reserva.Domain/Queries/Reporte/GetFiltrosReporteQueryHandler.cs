using AutoMapper;
using Reserva.Domain.Queries.Base;
using Reserva.Domain.Services.Reporte;
using Reserva.Dto.Base;
using Reserva.Dto.Reporte;

namespace Reserva.Domain.Queries.Reporte;

public class GetFiltrosReporteQueryHandler(IMapper mapper, ReporteService service)
    : QueryHandlerBase<GetFiltrosReporteQuery, FiltrosReporteDto>(mapper)
{
    protected override Task<ResponseDto<FiltrosReporteDto>> HandleQuery(GetFiltrosReporteQuery request, CancellationToken cancellationToken)
        => service.ObtenerFiltrosAsync();
}
