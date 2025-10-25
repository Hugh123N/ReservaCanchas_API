namespace Reserva.Dto.Dbo.TipoProveedor
{
    public class SelectTipoProveedorFilterDto
    {
        public DateTimeOffset? FechaDesde { get; set; }
        public DateTimeOffset? FechaHasta { get; set; }
        public int? IdTipoProveedor { get; set; }
        public bool? Activo { get; set; }
    }
}
