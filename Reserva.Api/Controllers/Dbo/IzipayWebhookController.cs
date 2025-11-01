using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Reserva.Common;
using Reserva.Domain.Services.Izipay;
using Reserva.Repository.Abstractions.Base;
using System.Text;
using System.Text.Json;

namespace Reserva.Api.Controllers.Dbo
{
    /// <summary>
    /// Controller para recibir notificaciones IPN/Webhook de Izipay
    /// cuando cambia el estado de un pago
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class IzipayWebhookController : ControllerBase
    {
        private readonly IRepository<Entity.Pago> _pagoRepository;
        private readonly IRepository<Entity.Reserva> _reservaRepository;
        private readonly IRepository<Entity.EstadoPago> _estadoPagoRepository;
        private readonly IRepository<Entity.EstadoReserva> _estadoReservaRepository;
        private readonly IzipayService _izipayService;
        private readonly ILogger<IzipayWebhookController> _logger;

        public IzipayWebhookController(
            IRepository<Entity.Pago> pagoRepository,
            IRepository<Entity.Reserva> reservaRepository,
            IRepository<Entity.EstadoPago> estadoPagoRepository,
            IRepository<Entity.EstadoReserva> estadoReservaRepository,
            IzipayService izipayService,
            ILogger<IzipayWebhookController> logger)
        {
            _pagoRepository = pagoRepository;
            _reservaRepository = reservaRepository;
            _estadoPagoRepository = estadoPagoRepository;
            _estadoReservaRepository = estadoReservaRepository;
            _izipayService = izipayService;
            _logger = logger;
        }

        /// <summary>
        /// Endpoint para recibir notificaciones de Izipay
        /// POST /api/IzipayWebhook/notification
        /// </summary>
        [HttpPost("notification")]
        public async Task<IActionResult> HandleNotification()
        {
            try
            {
                // 1. Leer el body del request
                using var reader = new StreamReader(Request.Body, Encoding.UTF8);
                var requestBody = await reader.ReadToEndAsync();

                _logger.LogInformation("Webhook de Izipay recibido: {Body}", requestBody);

                // 2. Validar firma HMAC-SHA256 para asegurar que viene de Izipay
                if (!Request.Headers.TryGetValue("X-Signature", out StringValues signature))
                {
                    _logger.LogWarning("Webhook sin firma recibido");
                    return BadRequest(new { error = "Firma no presente" });
                }

                if (!_izipayService.ValidateWebhookSignature(requestBody, signature!))
                {
                    _logger.LogWarning("Webhook con firma inválida recibido");
                    return Unauthorized(new { error = "Firma inválida" });
                }

                // 3. Deserializar la notificación
                var notification = JsonSerializer.Deserialize<IzipayWebhookNotification>(
                    requestBody,
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        PropertyNameCaseInsensitive = true
                    });

                if (notification == null)
                {
                    _logger.LogError("No se pudo deserializar el webhook");
                    return BadRequest(new { error = "Formato inválido" });
                }

                // 4. Buscar el pago por TransactionId
                var pago = await _pagoRepository.GetByAsync(
                    p => p.IzipayTransactionId == notification.TransactionId,
                    p => p.IdReservaNavigation!,
                    p => p.IdEstadoPagoNavigation
                );

                if (pago == null)
                {
                    _logger.LogWarning("Pago no encontrado para TransactionId: {TransactionId}",
                        notification.TransactionId);
                    return NotFound(new { error = "Pago no encontrado" });
                }

                _logger.LogInformation("Pago encontrado: IdPago={IdPago}, Estado={Estado}",
                    pago.IdPago, notification.Status);

                // 5. Procesar según el estado del pago
                await ProcessPaymentStatusAsync(pago, notification);

                // 6. Responder OK a Izipay
                return Ok(new { message = "Notificación procesada correctamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar webhook de Izipay");
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }

        private async Task ProcessPaymentStatusAsync(
            Entity.Pago pago,
            IzipayWebhookNotification notification)
        {
            switch (notification.Status?.ToUpper())
            {
                case "PAID":
                case "ACCEPTED":
                case "SUCCESS":
                    await MarcarPagoComoPagado(pago, notification);
                    break;

                case "FAILED":
                case "REJECTED":
                case "CANCELLED":
                    await MarcarPagoComoRechazado(pago, notification);
                    break;

                case "PENDING":
                    // Mantener estado pendiente, no hacer nada
                    _logger.LogInformation("Pago {IdPago} sigue pendiente", pago.IdPago);
                    break;

                default:
                    _logger.LogWarning("Estado desconocido recibido: {Status}", notification.Status);
                    break;
            }
        }

        private async Task MarcarPagoComoPagado(
            Entity.Pago pago,
            IzipayWebhookNotification notification)
        {
            // Solo actualizar si aún no está pagado
            if (pago.IdEstadoPagoNavigation.Codigo == Constants.ESTADO_PAGO.Pagado)
            {
                _logger.LogInformation("Pago {IdPago} ya estaba marcado como PAGADO", pago.IdPago);
                return;
            }

            var estadoPagado = await _estadoPagoRepository.GetByAsync(
                e => e.Codigo == Constants.ESTADO_PAGO.Pagado);

            if (estadoPagado == null)
            {
                _logger.LogError("Estado PAGADO no encontrado en base de datos");
                return;
            }

            // Actualizar pago
            pago.IdEstadoPago = estadoPagado.IdEstadoPago;
            pago.CodigoOperacion = notification.OperationNumber ?? notification.TransactionId;
            pago.MontoAdelanto = _izipayService.ConvertFromCents(notification.Amount);
            pago.MontoPendiente = 0;

            await _pagoRepository.UpdateAsync(pago);
            await _pagoRepository.SaveAsync();

            _logger.LogInformation("Pago {IdPago} marcado como PAGADO", pago.IdPago);

            // Confirmar reserva si existe
            if (pago.IdReserva.HasValue && pago.IdReservaNavigation != null)
            {
                await ConfirmarReserva(pago.IdReservaNavigation);
            }
        }

        private async Task MarcarPagoComoRechazado(
            Entity.Pago pago,
            IzipayWebhookNotification notification)
        {
            var estadoRechazado = await _estadoPagoRepository.GetByAsync(
                e => e.Codigo == Constants.ESTADO_PAGO.Rechazado);

            if (estadoRechazado == null)
            {
                _logger.LogError("Estado RECHAZADO no encontrado en base de datos");
                return;
            }

            // Actualizar pago
            pago.IdEstadoPago = estadoRechazado.IdEstadoPago;
            pago.NumeroReferencia = $"RECHAZADO: {notification.Status}";

            await _pagoRepository.UpdateAsync(pago);
            await _pagoRepository.SaveAsync();

            _logger.LogInformation("Pago {IdPago} marcado como RECHAZADO", pago.IdPago);

            // Cancelar reserva si existe
            if (pago.IdReserva.HasValue && pago.IdReservaNavigation != null)
            {
                await CancelarReserva(pago.IdReservaNavigation);
            }
        }

        private async Task ConfirmarReserva(Entity.Reserva reserva)
        {
            if (reserva.IdEstadoReservaNavigation.Codigo == Constants.ESTADO_RESERVA.Confirmado)
            {
                return; // Ya está confirmada
            }

            var estadoConfirmado = await _estadoReservaRepository.GetByAsync(
                e => e.Codigo == Constants.ESTADO_RESERVA.Confirmado);

            if (estadoConfirmado != null)
            {
                reserva.IdEstadoReserva = estadoConfirmado.IdEstadoReserva;
                await _reservaRepository.UpdateAsync(reserva);
                await _reservaRepository.SaveAsync();

                _logger.LogInformation("Reserva {IdReserva} confirmada automáticamente",
                    reserva.IdReserva);
            }
        }

        private async Task CancelarReserva(Entity.Reserva reserva)
        {
            var estadoCancelado = await _estadoReservaRepository.GetByAsync(
                e => e.Codigo == Constants.ESTADO_RESERVA.Cancelado);

            if (estadoCancelado != null)
            {
                reserva.IdEstadoReserva = estadoCancelado.IdEstadoReserva;
                await _reservaRepository.UpdateAsync(reserva);
                await _reservaRepository.SaveAsync();

                _logger.LogInformation("Reserva {IdReserva} cancelada por pago rechazado",
                    reserva.IdReserva);
            }
        }
    }
}
