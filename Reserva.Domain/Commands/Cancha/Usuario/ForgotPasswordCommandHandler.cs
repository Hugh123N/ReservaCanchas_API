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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace Reserva.Domain.Commands.Cancha.Usuario
{
    public class ForgotPasswordCommandHandler : CommandHandlerBase<ForgotPasswordCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ForgotPasswordCommandHandler> _logger;
        private readonly IConfiguration _configuration;

        public ForgotPasswordCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            ILogger<ForgotPasswordCommandHandler> logger,
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager
        ) : base(unitOfWork, mapper, mediator)
        {
            _configuration = configuration;
            _userManager = userManager;
            _logger = logger;
        }

        public override async Task<ResponseDto> HandleCommand(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                response.AddErrorResult("Correo no registrado.");
                return response;
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var tokenCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var email = WebUtility.UrlDecode(user.Email);

            // Construcción de enlace al frontend
            var frontUrl = _configuration.GetValue<string>("SecurityOptions:FrontUrl") ?? "";
            frontUrl = frontUrl.Replace("{host}", request.Host);

            //var callbackUrl = $"{frontUrl}/#/user/reset-password?email={request.Email}&token={token}";
            var callbackUrl = $"{frontUrl}/#/user/reset-password?email={email}&token={tokenCode}";

            // Enviar correo
            try
            {
                await SendResetPasswordEmail(user, callbackUrl);
                response.AddOkResult("Correo de recuperación enviado.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                response.AddErrorResult("Ocurrió un error al enviar el correo.");
            }

            return response;
        }

        private async Task SendResetPasswordEmail(ApplicationUser user, string callbackUrl)
        {
            var logo = _configuration.GetValue<string>("SecurityOptions:FrontUrlLogo") ?? "";

            var emailDto = new SendEmailDto
            {
                EmailCode = $"Hola {user.UserName}, restablece tu contraseña",
                ToEmails = new List<string> { user.Email! },
                BodyParams = new Dictionary<string, string>
                {
                    {
                        "{BODY}", $@"
                            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #eee; border-radius: 10px; background-color: #f9f9f9;'>
                                <div style='text-align: center; margin-bottom: 20px;'>
                                    <img src='{logo}' alt='Logo' style='max-height: 60px;' />
                                </div>
                                <h2 style='color: #333;'>Hola {user.UserName},</h2>
                                <p style='color: #555; font-size: 16px;'>Recibimos una solicitud para restablecer tu contraseña.</p>
                                <p style='color: #555; font-size: 16px;'>Haz clic en el siguiente botón para continuar:</p>
                                <div style='text-align: center; margin: 30px 0;'>
                                    <a href='{callbackUrl}' style='background-color: #007bff; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; font-size: 16px;'>Restablecer contraseña</a>
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
