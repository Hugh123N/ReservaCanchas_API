namespace Reserva.Dto.Dbo.HorarioCancha
{
    public class SearchHorarioCanchaFilterDto
    {
        public DateTimeOffset? FechaDesde { get; set; }
        public DateTimeOffset? FechaHasta { get; set; }
        public int? IdHorarioCancha { get; set; }
        public bool? Activo { get; set; }
    }
}
