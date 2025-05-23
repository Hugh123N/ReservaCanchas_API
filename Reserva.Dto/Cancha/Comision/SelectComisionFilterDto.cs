namespace Reserva.Dto.Cancha.Comision
{
    public class SelectComisionFilterDto
    {
        public DateTimeOffset? FechaDesde { get; set; }
        public DateTimeOffset? FechaHasta { get; set; }
        public int? IdComision { get; set; }
        public bool? Activo { get; set; }
    }
}
