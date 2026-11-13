using Reserva.Domain.Queries.Base;
using Reserva.Dto.Reporte;

namespace Reserva.Domain.Queries.Reporte;

public class GetReporteQuery(TipoReporte tipo, FiltroReporteDto filtro) : QueryBase<ReporteDto>
{
    public TipoReporte Tipo { get; } = tipo;
    public FiltroReporteDto Filtro { get; } = filtro;
}
