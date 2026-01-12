namespace Reserva.Dto.Dbo.Calendario
{
    public class BloqueHorarioDto
    {
        public DateTimeOffset Fecha { get; set; }
        public int IdHorarioCanchaInicio { get; set; }
        public int IdHorarioCanchaFin { get; set; }
        public decimal PrecioHora { get; set; }
    }
}
