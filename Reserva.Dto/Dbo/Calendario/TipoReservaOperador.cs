namespace Reserva.Dto.Dbo.Calendario
{
    /// <summary>
    /// Tipo de reserva creada por operador
    /// </summary>
    public enum TipoReservaOperador
    {
        /// <summary>
        /// Reserva normal - Estado inicial: PENDIENTE (requiere confirmación posterior)
        /// </summary>
        Normal = 1,

        /// <summary>
        /// Reserva inmediata - Estado inicial: CONFIRMADO (con pago inmediato)
        /// </summary>
        Inmediata = 2
    }
}
