using Reserva.Dto.Base;

namespace Reserva.Dto.Dbo.Reserva;

public class IndicadoresReservasDto
{
    public int Total { get; set; }
    public int Pendientes { get; set; }
    public int Confirmadas { get; set; }
    public int Expiradas { get; set; }
    public int Canceladas { get; set; }
    public int Anuladas { get; set; }
    public int ConDevolucion { get; set; }
    public int NoPresentadas { get; set; }
    public int Completadas { get; set; }
    public decimal SaldoVigente { get; set; }
}

public class SearchReservasResultDto(IEnumerable<ReservaClienteDto> items, IndicadoresReservasDto indicadores, int page, int pageSize)
    : SearchResultDto<ReservaClienteDto>(items, indicadores.Total, page, pageSize)
{
    public IndicadoresReservasDto Indicadores { get; set; } = indicadores;
}
