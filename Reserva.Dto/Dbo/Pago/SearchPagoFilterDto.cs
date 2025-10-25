namespace Reserva.Dto.Dbo.Pago
{
    public class SearchPagoFilterDto
    {
        public DateTimeOffset? FechaDesde { get; set; }
        public DateTimeOffset? FechaHasta { get; set; }
        public int? IdPago { get; set; }
        public bool? Activo { get; set; }
    }
}
