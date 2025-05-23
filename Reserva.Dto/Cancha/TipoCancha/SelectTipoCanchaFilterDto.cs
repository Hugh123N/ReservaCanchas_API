namespace Reserva.Dto.Cancha.TipoCancha
{
    public class SelectTipoCanchaFilterDto
    {
        public DateTimeOffset? FechaDesde { get; set; }
        public DateTimeOffset? FechaHasta { get; set; }
        public int? IdTipoCancha { get; set; }
        public bool? Activo { get; set; }
    }
}
