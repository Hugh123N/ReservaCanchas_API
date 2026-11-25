using Microsoft.Extensions.Logging;
using Reserva.Domain.Commands.Email;
using Reserva.Dto.Dbo.Reclamacion;
using Reserva.Dto.Email;
using System.Globalization;
using System.Net;

namespace Reserva.Domain.Services.Notificacion
{
    public partial class NotificacionService
    {
        public async Task<bool> NotificarReclamacionRegistradaAsync(GetReclamacionDto reclamacionDto)
        {
            var body = ConstruirEmailReclamacionRegistrada(reclamacionDto);
            var copiaEnviada = await EnviarCorreoReclamacionAsync(reclamacionDto.Email,
                $"Constancia {reclamacionDto.NumeroHoja} - ReservaFast", body);
            await EnviarCorreoReclamacionAsync(reclamacionDto.ComercioEmail,
                $"Nueva hoja {reclamacionDto.NumeroHoja} - ReservaFast", body);
            return copiaEnviada;
        }

        private async Task<bool> EnviarCorreoReclamacionAsync(string destinatario, string asunto, string htmlBody)
        {
            try
            {
                var result = await _mediator.Send(new SendEmailCommand(new SendEmailDto
                {
                    ToEmails = [destinatario],
                    EmailCode = asunto,
                    SubjectParams = new(),
                    BodyParams = new() { ["{BODY}"] = htmlBody }
                }));
                return result.IsValid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "No se pudo enviar una comunicación del Libro de Reclamaciones.");
                return false;
            }
        }

        private static string ConstruirEmailReclamacionRegistrada(GetReclamacionDto reclamacionDto)
        {
            static string Fila(string titulo, string? valor) => $"<dt><strong>{WebUtility.HtmlEncode(titulo)}</strong></dt><dd style='margin:0 0 12px;white-space:pre-wrap'>{WebUtility.HtmlEncode(valor ?? "No indicado")}</dd>";
            var peru = CultureInfo.GetCultureInfo("es-PE");
            var h = reclamacionDto;
            return "<h1>Libro de Reclamaciones - Hoja de reclamación</h1><dl>" +
                Fila("Número de hoja", h.NumeroHoja) + Fila("Fecha de registro (Perú)", h.FechaRegistro.ToOffset(TimeSpan.FromHours(-5)).ToString("dd/MM/yyyy HH:mm", peru)) +
                Fila("Comercio", $"{h.ComercioNombre} / {h.ComercioTitular}") + Fila("RUC", h.ComercioRuc) + Fila("Dirección del comercio", h.ComercioDireccion) +
                Fila("Contacto del comercio", $"{h.ComercioTelefono} / {h.ComercioEmail}") +
                "</dl><h2>1. Identificación del consumidor</h2><dl>" +
                Fila("Nombres y apellidos", h.Nombres) + Fila("Tipo de documento", h.TipDoc switch { "1" => "DNI", "4" => "Carné de extranjería", _ => "Pasaporte" }) +
                Fila("Número de documento", h.NroDoc) + Fila("Domicilio", h.Domicilio) + Fila("Email", h.Email) + Fila("Teléfono", h.Telefono) +
                Fila("Consumidor menor de edad", h.EsMenor ? "Sí" : "No") + Fila("Padre, madre o representante", h.Representante) +
                "</dl><h2>2. Identificación del servicio</h2><dl>" + Fila("Tipo de bien", "Servicio") + Fila("Descripción", h.DescripcionServicio) +
                Fila("Monto reclamado", h.MontoReclamado?.ToString("C2", peru)) + Fila("Referencia de operación", h.ReferenciaOperacion) +
                "</dl><h2>3. Reclamación y pedido</h2><dl>" + Fila("Tipo", h.Tipo) + Fila("Detalle", h.Detalle) + Fila("Pedido", h.Pedido) +
                "</dl><h2>4. Observaciones y acciones del proveedor</h2><p>Pendiente de atención.</p>" +
                "<p>Reclamo: disconformidad relacionada con el servicio. Queja: disconformidad relacionada con la atención.</p>" +
                "<p>Plazo máximo de respuesta: 15 días hábiles improrrogables. Esta constancia de recepción no sustituye la respuesta al caso.</p>" +
                "<p>La presentación de esta hoja no constituye una denuncia ante Indecopi ni impide acudir a otros mecanismos de solución de controversias.</p>";
        }
    }
}
