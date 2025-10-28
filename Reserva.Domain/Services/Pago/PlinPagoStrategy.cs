using Microsoft.Extensions.Configuration;
using Reserva.Entity;

namespace Reserva.Domain.Services.Pago
{
    /// <summary>
    /// Estrategia de pago para Plin
    /// Genera código QR con los datos del pago
    /// </summary>
    public class PlinPagoStrategy : IPagoStrategy
    {
        private readonly IConfiguration _configuration;
        private readonly QrCodeService _qrCodeService;

        public PlinPagoStrategy(IConfiguration configuration)
        {
            _configuration = configuration;
            _qrCodeService = new QrCodeService();
        }

        public async Task<PagoStrategyResult> ProcesarPagoAsync(
            Entity.Pago pago,
            Cancha cancha,
            Entity.Reserva reserva)
        {
            // Obtener teléfono del proveedor o usar el configurado por defecto
            string telefonoProveedor = ObtenerTelefonoProveedor(cancha);

            // Generar concepto del pago
            string conceptoPago = GenerarConceptoPago(cancha, reserva);

            // Generar QR de Plin
            var (qrBase64, qrText) = await _qrCodeService.GenerarQrPlin(
                telefonoProveedor,
                pago.Monto,
                conceptoPago
            );

            return new PagoStrategyResult
            {
                QrCodeBase64 = qrBase64,
                QrText = qrText,
                RequiereConfirmacion = true,
                InformacionAdicional = $"Escanea el código QR con tu app Plin y envía S/ {pago.Monto:F2} al número {telefonoProveedor}"
            };
        }

        private string ObtenerTelefonoProveedor(Cancha cancha)
        {
            // Intentar obtener el teléfono del proveedor asociado a la cancha
            if (cancha.IdProveedorNavigation?.IdProveedorNavigation?.PhoneNumber != null)
            {
                return cancha.IdProveedorNavigation.IdProveedorNavigation.PhoneNumber;
            }

            // Si no hay teléfono, usar el configurado en appsettings (puede ser el mismo que Yape o diferente)
            return _configuration.GetValue<string>("Pago:TelefonoPlin")
                ?? _configuration.GetValue<string>("Pago:TelefonoYape")
                ?? "901269594";
        }

        private string GenerarConceptoPago(Cancha cancha, Entity.Reserva reserva)
        {
            return $"Reserva {cancha.Nombre} - {reserva.Fecha:dd/MM/yyyy} {reserva.HoraInicio:HH:mm}-{reserva.HoraFin:HH:mm}";
        }
    }
}
