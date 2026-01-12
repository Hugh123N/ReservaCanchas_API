namespace Reserva.Dto.Dbo.Calendario
{
    /// <summary>
    /// DTO Request para validar la disponibilidad de horarios
    /// </summary>
    public class ValidarDisponibilidadRequestDto
    {
        /// <summary>
        /// ID de la cancha
        /// </summary>
        public int IdCancha { get; set; }

        /// <summary>
        /// Lista de bloques de horarios a validar
        /// </summary>
        public List<BloqueHorarioDto> Horarios { get; set; } = new List<BloqueHorarioDto>();
    }
}
