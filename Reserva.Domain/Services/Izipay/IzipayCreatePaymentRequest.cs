namespace Reserva.Domain.Services.Izipay
{
    /// <summary>
    /// Request para crear un pago en Izipay
    /// Endpoint: POST https://api.micuentaweb.pe/api-payment/V4/Charge/CreatePayment
    /// </summary>
    public class IzipayCreatePaymentRequest
    {
        /// <summary>
        /// Monto en céntimos (50.00 soles = 5000)
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// Código de moneda ISO 4217 (PEN para soles peruanos)
        /// </summary>
        public string Currency { get; set; } = "PEN";

        /// <summary>
        /// Identificador único de la orden en tu sistema
        /// </summary>
        public string OrderId { get; set; } = null!;

        /// <summary>
        /// Datos del cliente
        /// </summary>
        public IzipayCustomer Customer { get; set; } = null!;

        /// <summary>
        /// Método de pago específico (YAPE, PLIN, etc.)
        /// Opcional: si no se especifica, Izipay muestra todas las opciones
        /// </summary>
        public string? PaymentMethodType { get; set; }

        /// <summary>
        /// Descripción de la transacción
        /// </summary>
        public string? Description { get; set; }
    }

    public class IzipayCustomer
    {
        /// <summary>
        /// Email del cliente
        /// </summary>
        public string Email { get; set; } = null!;

        /// <summary>
        /// Referencia del cliente en tu sistema (puede ser el GUID del usuario)
        /// </summary>
        public string? Reference { get; set; }

        /// <summary>
        /// Nombre completo del cliente (opcional)
        /// </summary>
        public string? BillingDetails { get; set; }
    }
}
