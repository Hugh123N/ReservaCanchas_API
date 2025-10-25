namespace Reserva.Dto.Dbo.IntentoLogin
{
    public class SelectIntentoLoginFilterDto
    {
        public DateTimeOffset? FechaDesde { get; set; }
        public DateTimeOffset? FechaHasta { get; set; }
        public int? IdIntentoLogin { get; set; }
        public bool? Activo { get; set; }
    }
}
