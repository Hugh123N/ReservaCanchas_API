namespace Reserva.Domain.Services.Pago
{
    /// <summary>
    /// Resultado de la ejecución de una estrategia de pago
    /// </summary>
    public class PagoStrategyResult
    {
        /// <summary>
        /// Código QR en formato Base64 (solo para Yape/Plin)
        /// </summary>
        public string? QrCodeBase64 { get; set; }

        /// <summary>
        /// Texto del QR generado (solo para Yape/Plin)
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
        /// Titular de la cuenta (solo para Transferencia)
        /// </summary>
        public string? TitularCuenta { get; set; }

        /// <summary>
        /// Información adicional del método de pago
        /// </summary>
        public string? InformacionAdicional { get; set; }

        /// <summary>
        /// Indica si el pago requiere confirmación posterior (true para todos excepto Efectivo en algunos casos)
        /// </summary>
        public bool RequiereConfirmacion { get; set; } = true;
    }
}
