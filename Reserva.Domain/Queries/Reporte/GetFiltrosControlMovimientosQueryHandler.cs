using AutoMapper;
using Reserva.Domain.Queries.Base;
using Reserva.Domain.Services.Reporte;
using Reserva.Dto.Base;
using Reserva.Dto.Reporte;

namespace Reserva.Domain.Queries.Reporte;

public class GetFiltrosControlMovimientosQueryHandler(IMapper mapper, ControlMovimientosService service)
    : QueryHandlerBase<GetFiltrosControlMovimientosQuery, FiltrosControlMovimientosDto>(mapper)
{
    protected override Task<ResponseDto<FiltrosControlMovimientosDto>> HandleQuery(GetFiltrosControlMovimientosQuery request, CancellationToken cancellationToken)
        => service.FiltrosAsync(cancellationToken);
}
