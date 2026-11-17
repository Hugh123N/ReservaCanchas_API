using Reserva.Dto.Reporte;
using Reserva.Repository.Utils;

namespace Reserva.Domain.Services.Reporte;

public static class ReportePeriodo
{
    public static (DateTimeOffset Desde, DateTimeOffset Hasta) LimitesUtc(List<CanchaReporteDto> canchas, DateOnly desde, DateOnly hasta)
    {
        var limites = canchas.Select(c => TimezoneUtils.ObtenerZonaHoraria(c.ZonaHoraria)).Select(z => (
            Desde: DateTimeHelper.NormalizarFechaLocal(desde.ToDateTime(TimeOnly.MinValue), z).ToUniversalTime(),
            Hasta: DateTimeHelper.NormalizarFechaLocal(hasta.AddDays(1).ToDateTime(TimeOnly.MinValue), z).ToUniversalTime())).ToList();
        return (limites.Min(l => l.Desde), limites.Max(l => l.Hasta));
    }
}
