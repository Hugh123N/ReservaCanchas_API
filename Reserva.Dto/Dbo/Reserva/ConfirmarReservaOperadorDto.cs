namespace Reserva.Dto.Dbo.Reserva
{
    /// <summary>
    /// DTO para que el operador confirme una reserva pendiente
    /// Permite registrar adelanto o pago completo
    /// </summary>
    public class ConfirmarReservaOperadorDto
    {
        /// <summary>
        /// ID de la reserva a confirmar
        /// </summary>
        public int IdReserva { get; set; }

        /// <summary>
        /// Monto del adelanto (opcional). Si es null o 0, queda todo pendiente.
        /// Si es igual o mayor al monto total, se marca como pagado completo.
        /// </summary>
        public decimal? MontoAdelanto { get; set; }

        /// <summary>
        /// Número de recibo o comprobante (opcional)
        /// </summary>
        public string? NumeroRecibo { get; set; }

        /// <summary>
        /// Observaciones del operador sobre el pago
        /// </summary>
        public string? ObservacionOperador { get; set; }
    }
}
