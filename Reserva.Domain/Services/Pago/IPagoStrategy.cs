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
        Task<PagoStrategyResult> ProcesarPagoAsync(Entity.Pago pago, Entity.Cancha cancha, Entity.Reserva reserva);

    }
}
