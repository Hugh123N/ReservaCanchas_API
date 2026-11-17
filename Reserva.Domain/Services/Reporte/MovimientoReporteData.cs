namespace Reserva.Domain.Services.Reporte;

public class MovimientoReporteData
{
    public int IdMovimiento { get; set; }
    public int IdReserva { get; set; }
    public int IdCancha { get; set; }
    public string CodigoReserva { get; set; } = "";
    public DateTimeOffset FechaUtc { get; set; }
    public DateTime FechaLocal { get; set; }
    public DateTimeOffset FechaReservaUtc { get; set; }
    public DateOnly FechaReservaLocal { get; set; }
    public Guid? IdUsuarioResponsable { get; set; }
    public string? RolResponsable { get; set; }
    public string? NombreResponsable { get; set; }
    public string? NombreAuditoria { get; set; }
    public bool UsuarioActivo { get; set; }
    public string Tipo { get; set; } = "";
    public decimal Importe { get; set; }
    public decimal Anterior { get; set; }
    public decimal Resultante { get; set; }
    public int? IdMetodoPago { get; set; }
    public string? Metodo { get; set; }
    public string? Referencia { get; set; }
    public string? Motivo { get; set; }
}
