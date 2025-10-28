using Microsoft.Extensions.Configuration;
using Reserva.Entity;

namespace Reserva.Domain.Services.Pago
{
    /// <summary>
    /// Estrategia de pago para Transferencia Bancaria
    /// Devuelve los datos de cuenta bancaria del proveedor
    /// </summary>
    public class TransferenciaPagoStrategy : IPagoStrategy
    {
        private readonly IConfiguration _configuration;

        public TransferenciaPagoStrategy(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<PagoStrategyResult> ProcesarPagoAsync(
            Entity.Pago pago,
            Cancha cancha,
            Entity.Reserva reserva)
        {
            // Obtener datos bancarios del proveedor
            var datosBancarios = ObtenerDatosBancarios(cancha);

            return await Task.FromResult(new PagoStrategyResult
            {
                NumeroCuenta = datosBancarios.NumeroCuenta,
                CCI = datosBancarios.CCI,
                NombreBanco = datosBancarios.NombreBanco,
                TitularCuenta = datosBancarios.TitularCuenta,
                RequiereConfirmacion = true,
                InformacionAdicional = $"Realiza la transferencia de S/ {pago.Monto:F2} a la cuenta indicada. " +
                                      $"Una vez completada, ingresa el número de operación para confirmar tu reserva."
            });
        }

        private DatosBancariosDto ObtenerDatosBancarios(Cancha cancha)
        {
            // TODO: Estos datos deberían venir de la entidad Proveedor
            // Por ahora, usar configuración por defecto
            // En el futuro, agregar campos a la tabla Proveedor: NumeroCuenta, CCI, NombreBanco, TitularCuenta

            var datosBancarios = new DatosBancariosDto();

            // Intentar obtener de configuración
            datosBancarios.NumeroCuenta = _configuration.GetValue<string>("Pago:Transferencia:NumeroCuenta")
                ?? "PENDIENTE DE CONFIGURAR";
            datosBancarios.CCI = _configuration.GetValue<string>("Pago:Transferencia:CCI")
                ?? "PENDIENTE DE CONFIGURAR";
            datosBancarios.NombreBanco = _configuration.GetValue<string>("Pago:Transferencia:NombreBanco")
                ?? "Banco BCP";
            datosBancarios.TitularCuenta = _configuration.GetValue<string>("Pago:Transferencia:TitularCuenta")
                ?? cancha.IdProveedorNavigation?.IdProveedorNavigation?.UserName
                ?? "PENDIENTE DE CONFIGURAR";

            return datosBancarios;
        }

        private class DatosBancariosDto
        {
            public string NumeroCuenta { get; set; } = string.Empty;
            public string CCI { get; set; } = string.Empty;
            public string NombreBanco { get; set; } = string.Empty;
            public string TitularCuenta { get; set; } = string.Empty;
        }
    }
}
