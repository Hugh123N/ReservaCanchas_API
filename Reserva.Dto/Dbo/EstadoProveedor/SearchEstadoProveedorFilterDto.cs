namespace Reserva.Dto.Dbo.EstadoProveedor
{
    public class SearchEstadoProveedorFilterDto
    {
        public DateTimeOffset? FechaDesde { get; set; }
        public DateTimeOffset? FechaHasta { get; set; }
        public int? IdEstadoProveedor { get; set; }
        public bool? Activo { get; set; }
    }
}
