using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Reserva.Domain.Commands.Base;
using Reserva.Domain.Commands.Email;
using Reserva.Dto.Base;
using Reserva.Dto.Email;
using Reserva.Entity;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;
using Reserva.Repository.Security;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Reserva.Domain.Commands.Dbo.Usuario
{
    public class ChangeEmailRequestCommandHandler : CommandHandlerBase<ChangeEmailRequestCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserIdentity _userIdentity;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ChangeEmailRequestCommandHandler> _logger;

        public ChangeEmailRequestCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            ChangeEmailRequestCommandValidator validator,
            UserManager<ApplicationUser> userManager,
            IUserIdentity userIdentity,
            IConfiguration configuration,
            ILogger<ChangeEmailRequestCommandHandler> logger
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _userManager = userManager;
            _userIdentity = userIdentity;
            _configuration = configuration;
            _logger = logger;
        }

        public override async Task<ResponseDto> HandleCommand(ChangeEmailRequestCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();

            var userId = _userIdentity.GetCurrentUserId();
            if (userId == null)
            {
                response.AddErrorResult("No se pudo identificar al usuario.");
                return response;
            }

            var user = await _userManager.FindByIdAsync(userId.Value.ToString());
            if (user == null)
            {
                response.AddErrorResult("Usuario no encontrado.");
                return response;
            }

            var newEmail = request.NewEmail.Trim();

            // El login acepta UserName o Email: se bloquea si el nuevo correo ya se usa en cualquiera de los dos
            var existingByEmail = await _userManager.FindByEmailAsync(newEmail);
            if (existingByEmail != null && existingByEmail.Id != user.Id)
            {
                response.AddErrorResult("El correo ya está registrado en otra cuenta.");
                return response;
            }

            var existingByUserName = await _userManager.FindByNameAsync(newEmail);
            if (existingByUserName != null && existingByUserName.Id != user.Id)
            {
                response.AddErrorResult("El correo ya está registrado en otra cuenta.");
                return response;
            }

            var token = await _userManager.GenerateUserTokenAsync(user, TokenOptions.DefaultProvider, GetChangeEmailPurpose(newEmail));
            var tokenCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var frontUrl = _configuration.GetValue<string>("SecurityOptions:FrontUrl") ?? "";
            frontUrl = frontUrl.Replace("{host}", request.Host);

            var callbackUrl = $"{frontUrl}/cambiar-email/confirm?userId={user.Id}&email={Uri.EscapeDataString(newEmail)}&token={tokenCode}";

            try
            {
                await SendChangeEmailEmail(user, newEmail, callbackUrl);
                response.AddOkResult("Correo de confirmación enviado.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                response.AddErrorResult("Ocurrió un error al enviar el correo.");
            }

            return response;
        }

        // El token queda atado al correo solicitado: no sirve para confirmar otro correo diferente
        private static string GetChangeEmailPurpose(string newEmail)
            => $"ChangeEmail:{newEmail}";

        private async Task SendChangeEmailEmail(ApplicationUser user, string newEmail, string callbackUrl)
        {
            var logo = _configuration.GetValue<string>("SecurityOptions:FrontUrlLogo") ?? "";
            var nombreCompleto = $"{user.FirstName} {user.LastName}";

            var emailDto = new SendEmailDto
            {
                EmailCode = $"Hola {nombreCompleto}, confirma tu nuevo correo",
                ToEmails = new List<string> { newEmail },
                BodyParams = new Dictionary<string, string>
                {
                    {
                        "{BODY}", $@"
                            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #eee; border-radius: 10px; background-color: #f9f9f9;'>
                                <div style='text-align: center; margin-bottom: 20px;'>
                                    <img src='{logo}' alt='Logo' style='max-height: 60px;' />
                                </div>
                                <h2 style='color: #333;'>Hola {nombreCompleto},</h2>
                                <p style='color: #555; font-size: 16px;'>Recibimos una solicitud para cambiar tu correo electrónico a <strong>{newEmail}</strong>.</p>
                                <p style='color: #555; font-size: 16px;'>Haz clic en el siguiente botón para confirmar:</p>
                                <div style='text-align: center; margin: 30px 0;'>
                                    <a href='{callbackUrl}' style='background-color: #007bff; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; font-size: 16px;'>Confirmar cambio de correo</a>
                                </div>
                                <p style='color: #999; font-size: 14px;'>Si no solicitaste este cambio, puedes ignorar este correo.</p>
                                <p style='color: #ccc; font-size: 12px;'>Este enlace expirará en 1 hora.</p>
                            </div>"
                    }
                }
            };

            await _mediator.Send(new SendEmailCommand(emailDto));
        }
    }
}
