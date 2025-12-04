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
            string telefonoProveedor = ObtenerTelefonoProveedor(cancha);

            string conceptoPago = GenerarConceptoPago(cancha, reserva);

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
            if (cancha.IdProveedorNavigation?.IdUsuarioNavigation?.PhoneNumber != null)
            {
                return cancha.IdProveedorNavigation.IdUsuarioNavigation.PhoneNumber;
            }

            return _configuration.GetValue<string>("Pago:TelefonoPlin")
                ?? _configuration.GetValue<string>("Pago:TelefonoYape")
                ?? "901269594";
        }

        private string GenerarConceptoPago(Cancha cancha, Entity.Reserva reserva)
        {
            var horas = reserva.DetalleReserva
            .OrderBy(d => d.HoraInicio).Select(d => d.HoraInicio.ToString(@"hh\:mm"));

            var horasTexto = string.Join(", ", horas);

            return $"Reserva {cancha.Nombre} - {reserva.FechaReserva:dd/MM/yyyy} {horasTexto}";
        }
    }
}
