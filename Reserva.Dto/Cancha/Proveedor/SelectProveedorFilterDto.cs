namespace Reserva.Dto.Cancha.Proveedor
{
    public class SelectProveedorFilterDto
    {
        public DateTimeOffset? FechaDesde { get; set; }
        public DateTimeOffset? FechaHasta { get; set; }
        public int? IdProveedor { get; set; }
        public bool? Activo { get; set; }
    }
}
