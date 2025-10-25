namespace Reserva.Dto.Dbo.CanchaFavorita
{
    public class SearchCanchaFavoritaFilterDto
    {
        public DateTimeOffset? FechaDesde { get; set; }
        public DateTimeOffset? FechaHasta { get; set; }
        public int? IdCanchaFavorita { get; set; }
        public bool? Activo { get; set; }
    }
}
