using Reserva.Common;
using Reserva.Domain.Helpers;
using Reserva.Domain.Services.Reporte;
using Reserva.Dto.Dbo.Reserva;
using Reserva.Dto.Reporte;
using Reserva.Repository.Utils;

namespace Reserva.Domain.Queries.Dbo.Reserva;

public static class SearchReservaPaginaMapper
{
    public static void Completar(List<ReservaClienteDto> filas, IEnumerable<Entity.DetalleReserva> detalles,
        IEnumerable<MovimientoReporteData> reembolsos, List<CanchaReporteDto> canchas)
    {
        var porReserva = detalles.GroupBy(d => d.IdReserva).ToDictionary(g => g.Key, g => g.ToList());
        var ultimos = reembolsos.Where(m => m.Tipo == "REEMBOLSO").GroupBy(m => m.IdReserva).ToDictionary(g => g.Key,
            g => g.OrderByDescending(m => m.FechaUtc).ThenByDescending(m => m.IdMovimiento).First());
        var zonas = canchas.ToDictionary(c => c.IdCancha, c => TimezoneUtils.ObtenerZonaHoraria(c.ZonaHoraria));
        foreach (var fila in filas)
        {
            fila.FechaLocal = DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(fila.Fecha, zonas[fila.IdCancha]).DateTime);
            fila.Horarios = porReserva.TryGetValue(fila.IdReserva, out var horarios) ? HorarioHelper.AgruparHorariosDesdeDetalles(horarios) : new();
            fila.ResponsableUltimoReembolso = ultimos.TryGetValue(fila.IdReserva, out var movimiento) ? ControlMovimientosBuilder.Responsable(movimiento) :
                fila.MontoReembolso > 0 ? "Histórico sin responsable verificable" : null;
            fila.FechaUltimoReembolso = movimiento == null ? null : TimeZoneInfo.ConvertTime(movimiento.FechaUtc, zonas[fila.IdCancha]).DateTime;
            fila.EventoReserva = fila.Anulada || fila.CodigoEstadoReserva == Constants.ESTADO_RESERVA.Cancelado ? "Sin evento específico de anulación/cancelación registrado" :
                fila.CodigoEstadoReserva == Constants.ESTADO_RESERVA.Expirado ? "Estado expirado; proceso automático, sin evento individual" : null;
        }
    }
}
