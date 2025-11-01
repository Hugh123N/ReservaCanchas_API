namespace Reserva.Domain.Services.Izipay
{
    /// <summary>
    /// Respuesta de Izipay al crear un pago
    /// </summary>
    public class IzipayCreatePaymentResponse
    {
        /// <summary>
        /// Estado de la operación (SUCCESS, ERROR)
        /// </summary>
        public string Status { get; set; } = null!;

        /// <summary>
        /// Datos de la respuesta
        /// </summary>
        public IzipayAnswer? Answer { get; set; }

        /// <summary>
        /// Mensaje de error si status = ERROR
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Código de error si status = ERROR
        /// </summary>
        public string? ErrorCode { get; set; }
    }

    public class IzipayAnswer
    {
        /// <summary>
        /// Token del formulario de pago (necesario para el frontend)
        /// </summary>
        public string FormToken { get; set; } = null!;

        /// <summary>
        /// ID de la transacción en Izipay
        /// </summary>
        public string? TransactionId { get; set; }

        /// <summary>
        /// URL de la página de pago (para redirección)
        /// </summary>
        public string? PaymentUrl { get; set; }

        /// <summary>
        /// Datos del QR si el método es Yape/Plin
        /// </summary>
        public string? QrData { get; set; }
    }
}
