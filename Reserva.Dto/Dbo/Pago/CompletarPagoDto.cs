namespace Reserva.Dto.Dbo.Pago
{
    /// <summary>
    /// DTO para completar un pago que tiene adelanto (solo efectivo)
    /// </summary>
    public class CompletarPagoDto
    {
        public int IdPago { get; set; }
        public decimal MontoRestante { get; set; }
        public string? NumeroRecibo { get; set; }
    }
}
