namespace Reserva.Dto.Cancha.CanchaFavorita
{
    public class SelectCanchaFavoritaFilterDto
    {
        public DateTimeOffset? FechaDesde { get; set; }
        public DateTimeOffset? FechaHasta { get; set; }
        public int? IdCanchaFavorita { get; set; }
        public bool? Activo { get; set; }
    }
}
