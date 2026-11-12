namespace Reserva.Dto.Dbo.Pago;

public class RegistrarMovimientoPagoDto
{
    public int IdPago { get; set; }
    public Guid IdOperacion { get; set; }
    public int Revision { get; set; }
    public decimal Monto { get; set; }
    public string? CodigoMetodoPago { get; set; }
    public string? Motivo { get; set; }
}

public class GetMovimientoPagoDto
{
    public int IdMovimientoPago { get; set; }
    public string Tipo { get; set; } = null!;
    public decimal Importe { get; set; }
    public decimal MontoAnterior { get; set; }
    public decimal MontoResultante { get; set; }
    public string? MetodoPago { get; set; }
    public string? Motivo { get; set; }
    public string Usuario { get; set; } = null!;
    public DateTimeOffset Fecha { get; set; }
}

public class GetHistorialPagoDto
{
    public int IdPago { get; set; }
    public int Revision { get; set; }
    public decimal Monto { get; set; }
    public decimal MontoPagado { get; set; }
    public decimal Saldo { get; set; }
    public bool PuedeCobrar { get; set; }
    public bool PuedeCorregir { get; set; }
    public List<GetMovimientoPagoDto> Movimientos { get; set; } = new();
}
