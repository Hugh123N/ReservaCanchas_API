namespace Reserva.Dto.Dbo.Calendario
{
    /// <summary>
    /// DTO para solicitar la disponibilidad semanal de una cancha
    /// </summary>
    public class DisponibilidadSemanalRequestDto
    {
        public int IdCancha { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }
}
