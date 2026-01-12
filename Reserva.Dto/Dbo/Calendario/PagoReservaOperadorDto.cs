namespace Reserva.Dto.Dbo.Calendario
{
    /// <summary>
    /// DTO que representa los datos de pago para una reserva de operador
    /// </summary>
    public class PagoReservaOperadorDto
    {
        /// <summary>
        /// Monto total de la reserva
        /// </summary>
        public decimal MontoTotal { get; set; }

        /// <summary>
        /// Monto pagado (solo para reservas inmediatas)
        /// </summary>
        public decimal? MontoPagado { get; set; }

        /// <summary>
        /// Código de método de pago (por defecto: "02" = Efectivo)
        /// </summary>
        public string CodigoMetodoPago { get; set; } = "02";

        /// <summary>
        /// Código de operación (opcional)
        /// </summary>
        public string? CodigoOperacion { get; set; }

        /// <summary>
        /// Número de referencia (opcional)
        /// </summary>
        public string? NumeroReferencia { get; set; }
    }
}
