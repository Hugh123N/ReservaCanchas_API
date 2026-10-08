using AutoMapper;
using Google.Apis.Logging;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using Reserva.Domain.Commands.Base;
using Reserva.Domain.Services.Notificacion;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;
using System.Net.Mail;

namespace Reserva.Domain.Commands.Email
{
    public class SendEmailCommandHandler : CommandHandlerBase<SendEmailCommand>
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<NotificacionService> _logger;

        public SendEmailCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            SendEmailCommandValidator validator,
            IConfiguration configuration,
            ILogger<NotificacionService> logger
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public override async Task<ResponseDto> HandleCommand(SendEmailCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();

            var emailSettings = _configuration.GetSection("EmailSettings");

            string fromName = emailSettings["FromName"]!;
            string fromEmail = emailSettings["FromEmail"]!;
            string fromPassword = emailSettings["FromPassword"]!;
            string smtpLogin = emailSettings["SmtpLogin"]!;
            string smtpServer = emailSettings["SmtpServer"]!;
            int smtpPort = int.Parse(emailSettings["SmtpPort"]!);

            var subject = ReplaceParams(request.EmailDto.EmailCode, request.EmailDto.SubjectParams);
            string body;
            if (request.EmailDto.BodyParams.TryGetValue("{BODY}", out var htmlBody))
            {
                body = htmlBody;
            }
            else
            {
                // Si no hay HTML definido, usar EmailCode como base del cuerpo y reemplazar parámetros
                body = ReplaceParams(request.EmailDto.EmailCode, request.EmailDto.BodyParams);
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromName, fromEmail));

            if (request.EmailDto.ToEmails != null)
            {
                foreach (var to in request.EmailDto.ToEmails)
                    message.To.Add(MailboxAddress.Parse(to));
            }

            if (request.EmailDto.CcEmails != null)
            {
                foreach (var cc in request.EmailDto.CcEmails)
                    message.Cc.Add(MailboxAddress.Parse(cc));
            }

            message.Subject = subject;
            message.Body = new TextPart("html") { Text = body };
            try
            {
                using var smtp = new MailKit.Net.Smtp.SmtpClient();
                await smtp.ConnectAsync(smtpServer, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(smtpLogin, fromPassword);
                await smtp.SendAsync(message);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error al enviar el correo: ");
            } 

            response.AddOkResult(
                string.IsNullOrWhiteSpace(request.EmailDto.SuccesMessage)
                    ? "Correo enviado exitosamente."
                    : request.EmailDto.SuccesMessage
            );

            return response;
        }

        private static string ReplaceParams(string text, Dictionary<string, string>? parameters)
        {
            if (string.IsNullOrEmpty(text) || parameters == null)
                return text;

            foreach (var kvp in parameters)
            {
                text = text.Replace(kvp.Key, kvp.Value);
            }

            return text;
        }
    }
}
