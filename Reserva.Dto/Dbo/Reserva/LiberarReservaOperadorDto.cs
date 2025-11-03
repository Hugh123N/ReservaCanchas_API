namespace Reserva.Dto.Dbo.Reserva
{
    /// <summary>
    /// DTO para que el operador libere/cancele una reserva pendiente
    /// </summary>
    public class LiberarReservaOperadorDto
    {
        /// <summary>
        /// ID de la reserva a liberar
        /// </summary>
        public int IdReserva { get; set; }

        /// <summary>
        /// Motivo de la cancelación/liberación
        /// </summary>
        public string? MotivoLiberacion { get; set; }
    }
}
