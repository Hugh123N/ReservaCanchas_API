using Reserva.Domain.Queries.Base;
using Reserva.Dto.Reporte;

namespace Reserva.Domain.Queries.Reporte;

public class GetControlMovimientosQuery(FiltroControlMovimientosDto filtro) : QueryBase<ControlMovimientosDto>
{
    public FiltroControlMovimientosDto Filtro { get; } = filtro;
}
