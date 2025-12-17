namespace Reserva.Dto.Dbo.Reserva
{
    public class SearchReservaFilterDto
    {
        public string? CodigoEstado { get; set; }
        public DateTimeOffset? FechaDesde { get; set; }
        public DateTimeOffset? FechaHasta { get; set; }
        public string? CodigoEstadoPago { get; set; }
        public string? NombreCancha { get; set; }
        public string? SearchText { get; set; } // Busca por código reserva, nombre cliente o teléfono cliente
        public int? IdProveedor { get; set; }
    }
}
