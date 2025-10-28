using Microsoft.Extensions.Configuration;

namespace Reserva.Domain.Services.Pago
{
    /// <summary>
    /// Factory para crear la estrategia de pago adecuada según el método de pago
    /// </summary>
    public class PagoStrategyFactory
    {
        private readonly IConfiguration _configuration;

        public PagoStrategyFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Obtiene la estrategia de pago correspondiente al código del método de pago
        /// </summary>
        /// <param name="codigoMetodoPago">Código del método de pago (de Constants.METODO_PAGO)</param>
        /// <returns>Instancia de la estrategia correspondiente</returns>
        /// <exception cref="ArgumentException">Si el método de pago no está soportado</exception>
        public IPagoStrategy ObtenerEstrategia(string codigoMetodoPago)
        {
            return codigoMetodoPago switch
            {
                Common.Constants.METODO_PAGO.Yape => new YapePagoStrategy(_configuration),
                Common.Constants.METODO_PAGO.Plin => new PlinPagoStrategy(_configuration),
                Common.Constants.METODO_PAGO.Transferencia => new TransferenciaPagoStrategy(_configuration),
                Common.Constants.METODO_PAGO.Efectivo => new EfectivoPagoStrategy(),
                _ => throw new ArgumentException($"Método de pago '{codigoMetodoPago}' no está soportado", nameof(codigoMetodoPago))
            };
        }

        /// <summary>
        /// Verifica si un método de pago está soportado
        /// </summary>
        /// <param name="codigoMetodoPago">Código del método de pago</param>
        /// <returns>true si está soportado, false en caso contrario</returns>
        public bool EsMetodoPagoSoportado(string codigoMetodoPago)
        {
            return codigoMetodoPago switch
            {
                Common.Constants.METODO_PAGO.Yape => true,
                Common.Constants.METODO_PAGO.Plin => true,
                Common.Constants.METODO_PAGO.Transferencia => true,
                Common.Constants.METODO_PAGO.Efectivo => true,
                Common.Constants.METODO_PAGO.Tarjeta => false, // No implementado aún
                _ => false
            };
        }
    }
}
