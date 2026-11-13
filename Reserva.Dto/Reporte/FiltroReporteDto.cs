namespace Reserva.Dto.Reporte;

public enum TipoReporte { Economico, Demanda, AgendaOperador }

public class FiltroReporteDto
{
    public DateOnly FechaDesde { get; set; }
    public DateOnly FechaHasta { get; set; }
    public int? IdCancha { get; set; }
    public string? EstadoReserva { get; set; }
    public string? EstadoPago { get; set; }
}
