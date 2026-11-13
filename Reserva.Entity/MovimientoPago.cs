namespace Reserva.Entity;

public class MovimientoPago
{
    public int IdMovimientoPago { get; set; }
    public int IdPago { get; set; }
    public Guid IdOperacion { get; set; }
    public string Tipo { get; set; } = null!;
    public decimal Importe { get; set; }
    public decimal MontoAnterior { get; set; }
    public decimal MontoResultante { get; set; }
    public int? IdMetodoPago { get; set; }
    public Guid? IdUsuarioResponsable { get; set; }
    public string? RolResponsable { get; set; }
    public string? ReferenciaOperacion { get; set; }
    public string? Motivo { get; set; }
    public string UserNameCreate { get; set; } = null!;
    public DateTimeOffset CreateDate { get; set; }
    public bool Activo { get; set; } = true;
    public virtual Pago IdPagoNavigation { get; set; } = null!;
    public virtual MetodoPago? IdMetodoPagoNavigation { get; set; }
    public virtual AspNetUsers? IdUsuarioResponsableNavigation { get; set; }
}
