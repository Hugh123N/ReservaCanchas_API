using AutoMapper;
using Reserva.Domain.Queries.Base;
using Reserva.Domain.Services.Reporte;
using Reserva.Dto.Base;
using Reserva.Dto.Reporte;

namespace Reserva.Domain.Queries.Reporte;

public class GetControlMovimientosQueryHandler(IMapper mapper, GetControlMovimientosQueryValidator validator, ControlMovimientosService service)
    : QueryHandlerBase<GetControlMovimientosQuery, ControlMovimientosDto>(mapper, validator)
{
    protected override Task<ResponseDto<ControlMovimientosDto>> HandleQuery(GetControlMovimientosQuery request, CancellationToken cancellationToken)
        => service.ObtenerAsync(request.Filtro, cancellationToken);
}
