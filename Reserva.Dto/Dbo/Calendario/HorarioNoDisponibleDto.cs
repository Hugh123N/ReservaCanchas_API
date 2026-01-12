namespace Reserva.Dto.Dbo.Calendario
{
    /// <summary>
    /// DTO que representa un horario no disponible
    /// </summary>
    public class HorarioNoDisponibleDto
    {
        /// <summary>
        /// Fecha del horario no disponible
        /// </summary>
        public DateTimeOffset Fecha { get; set; }

        /// <summary>
        /// ID de la hora de inicio
        /// </summary>
        public int IdHoraInicio { get; set; }

        /// <summary>
        /// ID de la hora de fin
        /// </summary>
        public int IdHoraFin { get; set; }

        /// <summary>
        /// Hora de inicio en formato texto
        /// </summary>
        public string HoraInicio { get; set; } = null!;

        /// <summary>
        /// Hora de fin en formato texto
        /// </summary>
        public string HoraFin { get; set; } = null!;

        /// <summary>
        /// Motivo de no disponibilidad
        /// </summary>
        public string Motivo { get; set; } = null!;
    }
}
