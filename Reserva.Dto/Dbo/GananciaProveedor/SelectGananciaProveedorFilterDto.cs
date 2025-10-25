namespace Reserva.Dto.Dbo.GananciaProveedor
{
    public class SelectGananciaProveedorFilterDto
    {
        public DateTimeOffset? FechaDesde { get; set; }
        public DateTimeOffset? FechaHasta { get; set; }
        public int? IdGananciaProveedor { get; set; }
        public bool? Activo { get; set; }
    }
}
