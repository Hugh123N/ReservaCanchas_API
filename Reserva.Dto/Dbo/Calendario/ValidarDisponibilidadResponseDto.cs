namespace Reserva.Dto.Dbo.Calendario
{
    /// <summary>
    /// DTO Response de validación de disponibilidad de horarios
    /// </summary>
    public class ValidarDisponibilidadResponseDto
    {
        /// <summary>
        /// Indica si todos los horarios están disponibles
        /// </summary>
        public bool TodosDisponibles { get; set; }

        /// <summary>
        /// Lista de horarios no disponibles (si aplica)
        /// </summary>
        public List<HorarioNoDisponibleDto> HorariosNoDisponibles { get; set; } = new List<HorarioNoDisponibleDto>();

        /// <summary>
        /// Mensaje descriptivo del resultado
        /// </summary>
        public string Mensaje { get; set; } = null!;
    }
}
