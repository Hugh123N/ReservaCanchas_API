namespace Reserva.Dto.Dbo.Proveedor
{
    public class SearchProveedorFilterDto
    {
        public DateTimeOffset? FechaDesde { get; set; }
        public DateTimeOffset? FechaHasta { get; set; }
        public Guid? IdProveedor { get; set; }
        public bool? Activo { get; set; }
    }
}
