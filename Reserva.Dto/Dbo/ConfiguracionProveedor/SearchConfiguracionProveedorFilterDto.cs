namespace Reserva.Dto.Dbo.ConfiguracionProveedor
{
    public class SearchConfiguracionProveedorFilterDto
    {
        public DateTimeOffset? FechaDesde { get; set; }
        public DateTimeOffset? FechaHasta { get; set; }
        public int? IdConfiguracionProveedor { get; set; }
    }
}
