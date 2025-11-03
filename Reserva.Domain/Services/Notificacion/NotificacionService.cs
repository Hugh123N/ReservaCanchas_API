using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Reserva.Domain.Commands.Email;
using Reserva.Domain.Services.WhatsApp;
using Reserva.Dto.Email;
using Reserva.Entity;
using System.Text;

namespace Reserva.Domain.Services.Notificacion
{
    public class NotificacionService : INotificacionService
    {
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;
        private readonly ILogger<NotificacionService> _logger;
        private readonly IWhatsAppService _whatsAppService;

        public NotificacionService(
            IMediator mediator,
            IConfiguration configuration,
            ILogger<NotificacionService> logger,
            IWhatsAppService whatsAppService)
        {
            _mediator = mediator;
            _configuration = configuration;
            _logger = logger;
            _whatsAppService = whatsAppService;
        }

        public async Task NotificarNuevaReservaPendienteAsync(
            Entity.Reserva reserva,
            Cancha cancha,
            AspNetUsers cliente,
            List<Operador> operadores)
        {
            try
            {
                var htmlBody = ConstruirEmailNuevaReservaPendiente(reserva, cancha, cliente);

                var emailsOperadores = operadores
                    .Where(o => o.IdUsuarioNavigation?.Email != null)
                    .Select(o => o.IdUsuarioNavigation.Email!)
                    .ToList();

                if (emailsOperadores.Any())
                {
                    var emailDto = new SendEmailDto
                    {
                        EmailCode = "NUEVA_RESERVA_PENDIENTE",
                        ToEmails = emailsOperadores,
                        SubjectParams = new Dictionary<string, string>
                        {
                            { "NUEVA_RESERVA_PENDIENTE", $"🔔 Nueva Reserva Pendiente - {reserva.CodigoReserva}" }
                        },
                        BodyParams = new Dictionary<string, string>
                        {
                            { "{BODY}", htmlBody }
                        },
                        SuccesMessage = "Notificación enviada a operadores"
                    };

                    await _mediator.Send(new SendEmailCommand(emailDto));
                }

                // TODO: Enviar WhatsApp a operadores
                await EnviarWhatsAppOperadoresAsync(operadores, reserva, cancha, cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al notificar nueva reserva pendiente: {CodigoReserva}", reserva.CodigoReserva);
            }
        }

        public async Task NotificarReservaConfirmadaAsync(
            Entity.Reserva reserva,
            Cancha cancha,
            AspNetUsers cliente,
            Entity.Pago pago)
        {
            try
            {
                var htmlBody = ConstruirEmailReservaConfirmada(reserva, cancha, pago);

                if (!string.IsNullOrEmpty(cliente.Email))
                {
                    var emailDto = new SendEmailDto
                    {
                        EmailCode = "RESERVA_CONFIRMADA",
                        ToEmails = new[] { cliente.Email },
                        SubjectParams = new Dictionary<string, string>
                        {
                            { "RESERVA_CONFIRMADA", $"✅ Reserva Confirmada - {reserva.CodigoReserva}" }
                        },
                        BodyParams = new Dictionary<string, string>
                        {
                            { "{BODY}", htmlBody }
                        },
                        SuccesMessage = "Confirmación enviada al cliente"
                    };

                    await _mediator.Send(new SendEmailCommand(emailDto));
                }

                // TODO: Enviar WhatsApp al cliente
                await EnviarWhatsAppClienteAsync(cliente, reserva, cancha, pago);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al notificar reserva confirmada: {CodigoReserva}", reserva.CodigoReserva);
            }
        }

        public async Task NotificarReservaProximaExpirarAsync(
            Entity.Reserva reserva,
            Cancha cancha,
            AspNetUsers cliente,
            List<Operador> operadores)
        {
            try
            {
                var horasRestantes = reserva.FechaExpiracionPreReserva.HasValue
                    ? (reserva.FechaExpiracionPreReserva.Value - DateTimeOffset.Now).TotalHours
                    : 0;

                var htmlBody = ConstruirEmailReservaProximaExpirar(reserva, cancha, cliente, horasRestantes);

                var emailsOperadores = operadores
                    .Where(o => o.IdUsuarioNavigation?.Email != null)
                    .Select(o => o.IdUsuarioNavigation.Email!)
                    .ToList();

                if (emailsOperadores.Any())
                {
                    var emailDto = new SendEmailDto
                    {
                        EmailCode = "RESERVA_PROXIMA_EXPIRAR",
                        ToEmails = emailsOperadores,
                        SubjectParams = new Dictionary<string, string>
                        {
                            { "RESERVA_PROXIMA_EXPIRAR", $"⚠️ Reserva Próxima a Expirar - {reserva.CodigoReserva}" }
                        },
                        BodyParams = new Dictionary<string, string>
                        {
                            { "{BODY}", htmlBody }
                        },
                        SuccesMessage = "Alerta enviada a operadores"
                    };

                    await _mediator.Send(new SendEmailCommand(emailDto));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al notificar reserva próxima a expirar: {CodigoReserva}", reserva.CodigoReserva);
            }
        }

        public async Task NotificarReservaExpiradaAsync(
            Entity.Reserva reserva,
            Cancha cancha,
            List<Operador> operadores)
        {
            try
            {
                var htmlBody = ConstruirEmailReservaExpirada(reserva, cancha);

                var emailsOperadores = operadores
                    .Where(o => o.IdUsuarioNavigation?.Email != null)
                    .Select(o => o.IdUsuarioNavigation.Email!)
                    .ToList();

                if (emailsOperadores.Any())
                {
                    var emailDto = new SendEmailDto
                    {
                        EmailCode = "RESERVA_EXPIRADA",
                        ToEmails = emailsOperadores,
                        SubjectParams = new Dictionary<string, string>
                        {
                            { "RESERVA_EXPIRADA", $"❌ Reserva Expirada - {reserva.CodigoReserva}" }
                        },
                        BodyParams = new Dictionary<string, string>
                        {
                            { "{BODY}", htmlBody }
                        },
                        SuccesMessage = "Notificación de expiración enviada"
                    };

                    await _mediator.Send(new SendEmailCommand(emailDto));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al notificar reserva expirada: {CodigoReserva}", reserva.CodigoReserva);
            }
        }

        public async Task NotificarReservaCanceladaAsync(
            Entity.Reserva reserva,
            AspNetUsers cliente,
            string motivo)
        {
            try
            {
                var htmlBody = ConstruirEmailReservaCancelada(reserva, motivo);

                if (!string.IsNullOrEmpty(cliente.Email))
                {
                    var emailDto = new SendEmailDto
                    {
                        EmailCode = "RESERVA_CANCELADA",
                        ToEmails = new[] { cliente.Email },
                        SubjectParams = new Dictionary<string, string>
                        {
                            { "RESERVA_CANCELADA", $"❌ Reserva Cancelada - {reserva.CodigoReserva}" }
                        },
                        BodyParams = new Dictionary<string, string>
                        {
                            { "{BODY}", htmlBody }
                        },
                        SuccesMessage = "Cancelación notificada al cliente"
                    };

                    await _mediator.Send(new SendEmailCommand(emailDto));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al notificar reserva cancelada: {CodigoReserva}", reserva.CodigoReserva);
            }
        }

        #region Construcción de Plantillas HTML

        private string ConstruirEmailNuevaReservaPendiente(Entity.Reserva reserva, Cancha cancha, AspNetUsers cliente)
        {
            var horasRestantes = reserva.FechaExpiracionPreReserva.HasValue
                ? (reserva.FechaExpiracionPreReserva.Value - DateTimeOffset.Now).TotalHours
                : 0;

            var horarios = reserva.ReservaDetalle != null && reserva.ReservaDetalle.Any()
                ? $"{reserva.ReservaDetalle.Min(d => d.HoraInicio):HH:mm} - {reserva.ReservaDetalle.Max(d => d.HoraFin):HH:mm}"
                : "No especificado";

            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: #4CAF50; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
        .content {{ background: #f9f9f9; padding: 20px; border: 1px solid #ddd; }}
        .info-box {{ background: white; padding: 15px; margin: 10px 0; border-left: 4px solid #4CAF50; }}
        .alert-box {{ background: #fff3cd; padding: 15px; margin: 10px 0; border-left: 4px solid #ffc107; }}
        .footer {{ text-align: center; margin-top: 20px; font-size: 12px; color: #666; }}
        .btn {{ display: inline-block; padding: 10px 20px; background: #4CAF50; color: white; text-decoration: none; border-radius: 5px; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h2>🔔 Nueva Reserva Pendiente</h2>
        </div>
        <div class=""content"">
            <p>Hola, tienes una nueva reserva pendiente que requiere tu atención.</p>

            <div class=""info-box"">
                <h3>📋 Detalles de la Reserva</h3>
                <p><strong>Código:</strong> {reserva.CodigoReserva}</p>
                <p><strong>Cancha:</strong> {cancha.Nombre}</p>
                <p><strong>Fecha:</strong> {reserva.Fecha:dddd, dd/MM/yyyy}</p>
                <p><strong>Horario:</strong> {horarios}</p>
                <p><strong>Monto:</strong> S/ {reserva.Monto:F2}</p>
            </div>

            <div class=""info-box"">
                <h3>👤 Datos del Cliente</h3>
                <p><strong>Nombre:</strong> {cliente.FirstName} {cliente.LastName}</p>
                <p><strong>Email:</strong> {cliente.Email}</p>
                <p><strong>Teléfono:</strong> {cliente.PhoneNumber ?? "No proporcionado"}</p>
            </div>

            <div class=""alert-box"">
                <h3>⏰ Tiempo de Expiración</h3>
                <p><strong>Expira en:</strong> {horasRestantes:F1} horas</p>
                <p><strong>Fecha límite:</strong> {reserva.FechaExpiracionPreReserva:dd/MM/yyyy HH:mm}</p>
                <p>⚠️ Si no se confirma el pago antes de esta fecha, la reserva se cancelará automáticamente.</p>
            </div>

            <div style=""text-align: center; margin: 20px 0;"">
                <p><strong>👉 Acción Requerida:</strong> Contacta al cliente para coordinar el pago.</p>
            </div>
        </div>
        <div class=""footer"">
            <p>Este es un mensaje automático del sistema de reservas.</p>
        </div>
    </div>
</body>
</html>";
        }

        private string ConstruirEmailReservaConfirmada(Entity.Reserva reserva, Cancha cancha, Entity.Pago pago)
        {
            var horarios = reserva.ReservaDetalle != null && reserva.ReservaDetalle.Any()
                ? $"{reserva.ReservaDetalle.Min(d => d.HoraInicio):HH:mm} - {reserva.ReservaDetalle.Max(d => d.HoraFin):HH:mm}"
                : "No especificado";

            var mensajePago = pago.MontoPendiente > 0
                ? $@"<p><strong>💰 Estado del Pago:</strong></p>
                     <p>Adelanto: S/ {pago.MontoAdelanto:F2}</p>
                     <p>Pendiente: S/ {pago.MontoPendiente:F2}</p>
                     <p class=""alert"">⚠️ Recuerda completar el pago pendiente antes del día de tu reserva.</p>"
                : "<p><strong>✅ Pago Completo</strong></p>";

            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: #4CAF50; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
        .content {{ background: #f9f9f9; padding: 20px; border: 1px solid #ddd; }}
        .success-box {{ background: #d4edda; padding: 15px; margin: 10px 0; border-left: 4px solid #28a745; border-radius: 5px; }}
        .info-box {{ background: white; padding: 15px; margin: 10px 0; border-left: 4px solid #4CAF50; }}
        .alert {{ color: #856404; background-color: #fff3cd; padding: 10px; border-radius: 5px; margin: 10px 0; }}
        .footer {{ text-align: center; margin-top: 20px; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h2>✅ ¡Reserva Confirmada!</h2>
        </div>
        <div class=""content"">
            <div class=""success-box"">
                <h3>🎉 Tu reserva ha sido confirmada exitosamente</h3>
                <p>Código de Reserva: <strong>{reserva.CodigoReserva}</strong></p>
            </div>

            <div class=""info-box"">
                <h3>📋 Detalles de tu Reserva</h3>
                <p><strong>Cancha:</strong> {cancha.Nombre}</p>
                <p><strong>Fecha:</strong> {reserva.Fecha:dddd, dd/MM/yyyy}</p>
                <p><strong>Horario:</strong> {horarios}</p>
                <p><strong>Dirección:</strong> {cancha.Direccion ?? "Ver en la app"}</p>
                <p><strong>Monto Total:</strong> S/ {reserva.Monto:F2}</p>
            </div>

            <div class=""info-box"">
                {mensajePago}
            </div>

            <div style=""text-align: center; margin: 20px 0;"">
                <p>📱 Presenta este código el día de tu reserva: <strong style=""font-size: 20px; color: #4CAF50;"">{reserva.CodigoReserva}</strong></p>
            </div>
        </div>
        <div class=""footer"">
            <p>¡Gracias por confiar en nosotros! ¡Disfruta tu partido!</p>
        </div>
    </div>
</body>
</html>";
        }

        private string ConstruirEmailReservaProximaExpirar(Entity.Reserva reserva, Cancha cancha, AspNetUsers cliente, double horasRestantes)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: #ff9800; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
        .alert-box {{ background: #fff3cd; padding: 20px; margin: 10px 0; border-left: 4px solid #ffc107; }}
        .content {{ background: #f9f9f9; padding: 20px; border: 1px solid #ddd; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h2>⚠️ Reserva Próxima a Expirar</h2>
        </div>
        <div class=""content"">
            <div class=""alert-box"">
                <h3>⏰ Acción Urgente Requerida</h3>
                <p><strong>Código:</strong> {reserva.CodigoReserva}</p>
                <p><strong>Cancha:</strong> {cancha.Nombre}</p>
                <p><strong>Cliente:</strong> {cliente.FirstName} {cliente.LastName}</p>
                <p><strong>Teléfono:</strong> {cliente.PhoneNumber}</p>
                <p><strong>Tiempo restante:</strong> {horasRestantes:F1} horas</p>
                <p><strong>Expira:</strong> {reserva.FechaExpiracionPreReserva:dd/MM/yyyy HH:mm}</p>

                <p style=""margin-top: 15px; font-weight: bold; color: #d32f2f;"">
                    ⚠️ Esta reserva se cancelará automáticamente si no se confirma el pago antes de la fecha límite.
                </p>
                <p>👉 Contacta al cliente de inmediato para confirmar el pago.</p>
            </div>
        </div>
    </div>
</body>
</html>";
        }

        private string ConstruirEmailReservaExpirada(Entity.Reserva reserva, Cancha cancha)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: #f44336; color: white; padding: 20px; text-align: center; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h2>❌ Reserva Expirada</h2>
        </div>
        <div style=""padding: 20px;"">
            <p>La siguiente reserva ha expirado y ha sido cancelada automáticamente:</p>
            <p><strong>Código:</strong> {reserva.CodigoReserva}</p>
            <p><strong>Cancha:</strong> {cancha.Nombre}</p>
            <p>El horario ahora está disponible para nuevas reservas.</p>
        </div>
    </div>
</body>
</html>";
        }

        private string ConstruirEmailReservaCancelada(Entity.Reserva reserva, string motivo)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
    </style>
</head>
<body>
    <div class=""container"">
        <h2>❌ Reserva Cancelada</h2>
        <p>Tu reserva <strong>{reserva.CodigoReserva}</strong> ha sido cancelada.</p>
        {(!string.IsNullOrEmpty(motivo) ? $"<p><strong>Motivo:</strong> {motivo}</p>" : "")}
        <p>Si tienes dudas, contacta con el proveedor de la cancha.</p>
    </div>
</body>
</html>";
        }

        #endregion

        #region WhatsApp (Stub - Por implementar)

        private async Task EnviarWhatsAppOperadoresAsync(List<Operador> operadores, Entity.Reserva reserva, Cancha cancha, AspNetUsers cliente)
        {
            try
            {
                var telefonosOperadores = operadores
                    .Where(o => !string.IsNullOrWhiteSpace(o.IdUsuarioNavigation?.PhoneNumber))
                    .Select(o => o.IdUsuarioNavigation!.PhoneNumber!)
                    .ToList();

                if (!telefonosOperadores.Any())
                {
                    _logger.LogWarning("No hay teléfonos válidos de operadores para enviar WhatsApp");
                    return;
                }

                var mensaje = $"🔔 *Nueva Reserva Pendiente*\n\n" +
                             $"📋 Código: *{reserva.CodigoReserva}*\n" +
                             $"⚽ Cancha: {cancha.Nombre}\n" +
                             $"📅 Fecha: {reserva.Fecha:dd/MM/yyyy}\n" +
                             $"💰 Monto: S/ {reserva.Monto:F2}\n\n" +
                             $"👤 *Cliente:*\n" +
                             $"Nombre: {cliente.FirstName} {cliente.LastName}\n" +
                             $"Teléfono: {cliente.PhoneNumber}\n" +
                             $"Email: {cliente.Email}\n\n" +
                             $"⏰ Expira: {reserva.FechaExpiracionPreReserva:dd/MM/yyyy HH:mm}\n\n" +
                             $"Por favor, contacta al cliente para coordinar el pago.";

                var enviados = await _whatsAppService.SendBulkTextMessageAsync(telefonosOperadores, mensaje);

                _logger.LogInformation(
                    "WhatsApp enviado a {Enviados}/{Total} operadores para reserva {CodigoReserva}",
                    enviados,
                    telefonosOperadores.Count,
                    reserva.CodigoReserva);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar WhatsApp a operadores para reserva {CodigoReserva}", reserva.CodigoReserva);
            }
        }

        private async Task EnviarWhatsAppClienteAsync(AspNetUsers cliente, Entity.Reserva reserva, Cancha cancha, Entity.Pago pago)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cliente.PhoneNumber))
                {
                    _logger.LogWarning("Cliente {Email} no tiene teléfono registrado", cliente.Email);
                    return;
                }

                var estadoPago = pago.MontoAdelanto >= pago.Monto ? "✅ PAGADO" :
                                pago.MontoAdelanto > 0 ? $"⏳ PARCIAL (Adelanto: S/ {pago.MontoAdelanto:F2})" :
                                "⏳ PENDIENTE";

                var mensaje = $"✅ *Reserva Confirmada*\n\n" +
                             $"¡Hola {cliente.FirstName}! Tu reserva ha sido confirmada.\n\n" +
                             $"📋 Código: *{reserva.CodigoReserva}*\n" +
                             $"⚽ Cancha: {cancha.Nombre}\n" +
                             $"📍 Dirección: {cancha.Direccion}\n" +
                             $"📅 Fecha: {reserva.Fecha:dd/MM/yyyy}\n" +
                             $"💰 Monto Total: S/ {pago.Monto:F2}\n" +
                             $"💳 Estado Pago: {estadoPago}\n";

                if (pago.MontoPendiente > 0)
                {
                    mensaje += $"\n⚠️ Pendiente: S/ {pago.MontoPendiente:F2}";
                }

                mensaje += $"\n\n📞 Teléfono cancha: {cancha.TelefonoCancha ?? "No disponible"}\n\n" +
                          $"¡Nos vemos en la cancha! ⚽";

                var enviado = await _whatsAppService.SendTextMessageAsync(cliente.PhoneNumber, mensaje);

                if (enviado)
                {
                    _logger.LogInformation(
                        "WhatsApp enviado a cliente {Email} para reserva {CodigoReserva}",
                        cliente.Email,
                        reserva.CodigoReserva);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar WhatsApp a cliente para reserva {CodigoReserva}", reserva.CodigoReserva);
            }
        }

        #endregion
    }
}
