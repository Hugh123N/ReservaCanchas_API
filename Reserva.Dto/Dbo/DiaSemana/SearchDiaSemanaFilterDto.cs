namespace Reserva.Dto.Dbo.DiaSemana
{
    public class SearchDiaSemanaFilterDto
    {
        public DateTimeOffset? FechaDesde { get; set; }
        public DateTimeOffset? FechaHasta { get; set; }
        public int? IdDiaSemana { get; set; }
        public bool? Activo { get; set; }
    }
}
