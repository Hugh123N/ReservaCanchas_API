using AutoMapper;
using Reserva.Domain.Queries.Base;
using Reserva.Domain.Services.Reporte;
using Reserva.Dto.Base;
using Reserva.Dto.Reporte;

namespace Reserva.Domain.Queries.Reporte;

public class GetReporteQueryHandler(IMapper mapper, GetReporteQueryValidator validator, ReporteService service)
    : QueryHandlerBase<GetReporteQuery, ReporteDto>(mapper, validator)
{
    protected override Task<ResponseDto<ReporteDto>> HandleQuery(GetReporteQuery request, CancellationToken cancellationToken)
        => service.ObtenerAsync(request.Tipo, request.Filtro, cancellationToken);
}
