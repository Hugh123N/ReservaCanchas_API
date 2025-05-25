namespace Reserva.Dto.Cancha.EstadoProveedor
{
    public class SelectEstadoProveedorFilterDto
    {
        public DateTimeOffset? FechaDesde { get; set; }
        public DateTimeOffset? FechaHasta { get; set; }
        public int? IdEstadoProveedor { get; set; }
        public bool? Activo { get; set; }
    }
}
