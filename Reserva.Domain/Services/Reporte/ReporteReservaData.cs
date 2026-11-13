namespace Reserva.Domain.Services.Reporte;

public class ReportePagoData
{
    public decimal Total { get; set; }
    public decimal Cobrado { get; set; }
    public decimal Devuelto { get; set; }
    public string EstadoCodigo { get; set; } = string.Empty;
    public string EstadoNombre { get; set; } = string.Empty;
}

public class ReporteReservaData
{
    public int IdReserva { get; set; }
    public int IdCancha { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Cliente { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public DateTimeOffset FechaReserva { get; set; }
    public DateOnly FechaLocal { get; set; }
    public TimeOnly HoraLocal { get; set; }
    public decimal MontoTotal { get; set; }
    public bool Activo { get; set; }
    public string EstadoCodigo { get; set; } = string.Empty;
    public string EstadoNombre { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
    public ReportePagoData? Pago { get; set; }
}

public record ReporteHorarioData(int IdReserva, int IdCancha, int IdHorarioCancha, TimeOnly Inicio, TimeOnly Fin)
{
    public decimal Horas => (decimal)((Fin.ToTimeSpan() - Inicio.ToTimeSpan()).TotalMinutes + (Fin <= Inicio ? 1440 : 0)) / 60;
}

public record FranjaReporteData(int IdCancha, int DiaSemana, TimeOnly Inicio, TimeOnly Fin);
