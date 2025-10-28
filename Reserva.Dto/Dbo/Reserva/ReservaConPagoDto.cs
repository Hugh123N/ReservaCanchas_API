using Reserva.Dto.Dbo.Pago;

namespace Reserva.Dto.Dbo.Reserva
{
    /// <summary>
    /// DTO que contiene la información completa de una reserva con su pago
    /// Incluye datos específicos según el método de pago (QR para Yape/Plin, datos bancarios para Transferencia, etc.)
    /// </summary>
    public class ReservaConPagoDto
    {
        /// <summary>
        /// Datos de la reserva creada
        /// </summary>
        public GetReservaDto Reserva { get; set; } = null!;

        /// <summary>
        /// Datos del pago pendiente asociado a la reserva
        /// </summary>
        public GetPagoDto Pago { get; set; } = null!;

        /// <summary>
        /// QR code en formato Base64 (solo para Yape/Plin)
        /// </summary>
        public string? QrCodeBase64 { get; set; }

        /// <summary>
        /// Texto plano contenido en el QR (solo para Yape/Plin)
        /// </summary>
        public string? QrText { get; set; }

        /// <summary>
        /// Número de cuenta bancaria (solo para Transferencia)
        /// </summary>
        public string? NumeroCuenta { get; set; }

        /// <summary>
        /// CCI - Código de Cuenta Interbancario (solo para Transferencia)
        /// </summary>
        public string? CCI { get; set; }

        /// <summary>
        /// Nombre del banco (solo para Transferencia)
        /// </summary>
        public string? NombreBanco { get; set; }

        /// <summary>
        /// Titular de la cuenta bancaria (solo para Transferencia)
        /// </summary>
        public string? TitularCuenta { get; set; }

        /// <summary>
        /// Método de pago seleccionado
        /// </summary>
        public string MetodoPago { get; set; } = null!;

        /// <summary>
        /// Monto a pagar en formato string (ej: "45.00")
        /// </summary>
        public string MontoFormateado { get; set; } = null!;

        /// <summary>
        /// Moneda del pago (ej: "PEN")
        /// </summary>
        public string Moneda { get; set; } = null!;

        /// <summary>
        /// Minutos que tiene el usuario para completar el pago
        /// </summary>
        public int MinutosExpiracion { get; set; }

        /// <summary>
        /// Fecha y hora de expiración del pago
        /// </summary>
        public DateTimeOffset FechaExpiracion { get; set; }

        /// <summary>
        /// Información adicional del método de pago
        /// </summary>
        public string? InformacionAdicional { get; set; }
    }
}
