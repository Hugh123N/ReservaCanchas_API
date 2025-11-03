using Microsoft.Extensions.Configuration;
using Reserva.Entity;

namespace Reserva.Domain.Services.Pago
{
    /// <summary>
    /// Estrategia de pago para Yape
    /// Genera código QR con los datos del pago
    /// </summary>
    public class YapePagoStrategy : IPagoStrategy
    {
        private readonly IConfiguration _configuration;
        private readonly QrCodeService _qrCodeService;

        public YapePagoStrategy(IConfiguration configuration)
        {
            _configuration = configuration;
            _qrCodeService = new QrCodeService();
        }

        public async Task<PagoStrategyResult> ProcesarPagoAsync(Entity.Pago pago, Cancha cancha, Entity.Reserva reserva)
        {
            string telefonoProveedor = ObtenerTelefonoProveedor(cancha);

            string conceptoPago = GenerarConceptoPago(cancha, reserva);

            var (qrBase64, qrText) = await _qrCodeService.GenerarQrYape(
                telefonoProveedor,
                pago.Monto,
                conceptoPago
            );

            return new PagoStrategyResult
            {
                QrCodeBase64 = qrBase64,
                QrText = qrText,
                RequiereConfirmacion = true,
                InformacionAdicional = $"Escanea el código QR con tu app Yape y envía S/ {pago.Monto:F2} al número {telefonoProveedor}"
            };
        }

        private string ObtenerTelefonoProveedor(Cancha cancha)
        {
            if (cancha.IdProveedorNavigation?.IdUsuarioNavigation?.PhoneNumber != null)
            {
                return cancha.IdProveedorNavigation.IdUsuarioNavigation.PhoneNumber;
            }

            // Si no hay teléfono, usar el configurado en appsettings
            return _configuration.GetValue<string>("Pago:TelefonoYape") ?? "901269594";
        }

        private string GenerarConceptoPago(Cancha cancha, Entity.Reserva reserva)
        {
            var horas = reserva.ReservaDetalle
            .OrderBy(d => d.HoraInicio).Select(d => d.HoraInicio.ToString(@"hh\:mm"));

            var horasTexto = string.Join(", ", horas);

            return $"Reserva {cancha.Nombre} - {reserva.Fecha:dd/MM/yyyy} {horasTexto}";
        }
    }
}
