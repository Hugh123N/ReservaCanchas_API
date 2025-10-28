using Reserva.Entity;

namespace Reserva.Domain.Services.Pago
{
    /// <summary>
    /// Interfaz que define el contrato para las estrategias de procesamiento de pago
    /// </summary>
    public interface IPagoStrategy
    {
        /// <summary>
        /// Procesa el pago según la estrategia específica del método de pago
        /// </summary>
        /// <param name="pago">Entidad de pago a procesar</param>
        /// <param name="cancha">Cancha asociada a la reserva</param>
        /// <param name="reserva">Reserva asociada al pago</param>
        /// <returns>Resultado con datos específicos del método de pago</returns>
        Task<PagoStrategyResult> ProcesarPagoAsync(Entity.Pago pago, Entity.Cancha cancha, Entity.Reserva reserva);

    }
}
