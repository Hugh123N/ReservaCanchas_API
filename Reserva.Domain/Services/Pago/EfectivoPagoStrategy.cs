using Reserva.Entity;

namespace Reserva.Domain.Services.Pago
{
    /// <summary>
    /// Estrategia de pago para Efectivo
    /// El cliente paga en el establecimiento, no requiere datos adicionales
    /// </summary>
    public class EfectivoPagoStrategy : IPagoStrategy
    {
        public async Task<PagoStrategyResult> ProcesarPagoAsync(Entity.Pago pago, Cancha cancha, Entity.Reserva reserva)
        {
            return await Task.FromResult(new PagoStrategyResult
            {
                RequiereConfirmacion = true,
                InformacionAdicional = $"Reserva registrada. El cliente debe pagar S/ {pago.Monto:F2} en efectivo " +
                                      $"en el establecimiento antes del horario establecido de la fecha {reserva.Fecha:dd/MM/yyyy}"
            });
        }
    }
}
