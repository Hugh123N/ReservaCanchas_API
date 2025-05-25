namespace Reserva.Dto.Cancha.DiaSemana
{
    public class SelectDiaSemanaFilterDto
    {
        public DateTimeOffset? FechaDesde { get; set; }
        public DateTimeOffset? FechaHasta { get; set; }
        public int? IdDiaSemana { get; set; }
        public bool? Activo { get; set; }
    }
}
