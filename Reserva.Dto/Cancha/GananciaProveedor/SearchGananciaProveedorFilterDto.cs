namespace Reserva.Dto.Cancha.GananciaProveedor
{
    public class SearchGananciaProveedorFilterDto
    {
        public DateTimeOffset? FechaDesde { get; set; }
        public DateTimeOffset? FechaHasta { get; set; }
        public int? IdGananciaProveedor { get; set; }
        public bool? Activo { get; set; }
    }
}
