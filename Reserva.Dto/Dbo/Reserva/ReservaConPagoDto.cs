using Reserva.Dto.Dbo.Pago;

namespace Reserva.Dto.Dbo.Reserva
{
    /// <summary>
    /// DTO que contiene la información completa de una reserva con su pago
    /// Para sistema de pre-reserva con pago en efectivo coordinado por operador
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
        /// Código único de la reserva (ej: RES-2024-0001)
        /// </summary>
        public string? CodigoReserva { get; set; }

        /// <summary>
        /// Duración de la pre-reserva en horas
        /// </summary>
        public int DuracionPreReservaHoras { get; set; }

        /// <summary>
        /// Fecha y hora de expiración de la pre-reserva
        /// </summary>
        public DateTimeOffset? FechaExpiracionPreReserva { get; set; }

        /// <summary>
        /// Teléfono de la cancha para coordinar el pago
        /// </summary>
        public string? TelefonoCancha { get; set; }

        /// <summary>
        /// Nombre del operador/proveedor
        /// </summary>
        public string? NombreOperador { get; set; }

        /// <summary>
        /// QR code en formato Base64 (OBSOLETO - Solo se usaba para Yape/Plin)
        /// </summary>
        [Obsolete("Ya no se usa. Solo se acepta pago en efectivo.")]
        public string? QrCodeBase64 { get; set; }

        /// <summary>
        /// Texto plano contenido en el QR (OBSOLETO - Solo se usaba para Yape/Plin)
        /// </summary>
        [Obsolete("Ya no se usa. Solo se acepta pago en efectivo.")]
        public string? QrText { get; set; }

        /// <summary>
        /// Número de cuenta bancaria (OBSOLETO - Solo se usaba para Transferencia)
        /// </summary>
        [Obsolete("Ya no se usa. Solo se acepta pago en efectivo.")]
        public string? NumeroCuenta { get; set; }

        /// <summary>
        /// CCI - Código de Cuenta Interbancario (OBSOLETO - Solo se usaba para Transferencia)
        /// </summary>
        [Obsolete("Ya no se usa. Solo se acepta pago en efectivo.")]
        public string? CCI { get; set; }

        /// <summary>
        /// Nombre del banco (OBSOLETO - Solo se usaba para Transferencia)
        /// </summary>
        [Obsolete("Ya no se usa. Solo se acepta pago en efectivo.")]
        public string? NombreBanco { get; set; }

        /// <summary>
        /// Titular de la cuenta bancaria (OBSOLETO - Solo se usaba para Transferencia)
        /// </summary>
        [Obsolete("Ya no se usa. Solo se acepta pago en efectivo.")]
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
        /// Minutos que tiene el usuario para completar el pago (OBSOLETO - Ahora se usa duracionPreReservaHoras)
        /// </summary>
        [Obsolete("Usar DuracionPreReservaHoras en su lugar.")]
        public int MinutosExpiracion { get; set; }

        /// <summary>
        /// Fecha y hora de expiración del pago (OBSOLETO - Ahora se usa FechaExpiracionPreReserva)
        /// </summary>
        [Obsolete("Usar FechaExpiracionPreReserva en su lugar.")]
        public DateTimeOffset FechaExpiracion { get; set; }

        /// <summary>
        /// Información adicional del método de pago
        /// </summary>
        public string? InformacionAdicional { get; set; }
    }
}
