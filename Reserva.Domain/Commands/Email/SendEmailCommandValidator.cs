using FluentValidation;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;

namespace Reserva.Domain.Commands.Email
{
    public class SendEmailCommandValidator : CommandValidatorBase<SendEmailCommand>
    {
        public SendEmailCommandValidator(IConfiguration configuration)
        {
            RequiredInformation(x => x.EmailDto)
                .DependentRules(() =>
                {
                    RuleFor(x => x.EmailDto.EmailCode)
                        .NotEmpty()
                        .WithMessage("El asunto o cuerpo del correo no puede estar vacío.");
                })
                .DependentRules(() =>
                {
                    RuleFor(x => x.EmailDto.ToEmails)
                        .Cascade(CascadeMode.Stop)
                        .NotEmpty().WithMessage("Debe especificar al menos un destinatario.")
                        .Must(ValidateEmailsFormat).WithMessage("Uno o más correos en 'ToEmails' son inválidos.");

                    RuleFor(x => x.EmailDto.CcEmails)
                        .Must(ValidateEmailsFormat).When(x => x.EmailDto.CcEmails != null)
                        .WithMessage("Uno o más correos en 'CcEmails' son inválidos.");
                });
        }

        private static bool ValidateEmailsFormat(IEnumerable<string>? emails)
        {
            if (emails == null)
                return true;

            foreach (var email in emails)
            {
                if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
                    return false;
            }

            return true;
        }

        private static bool IsValidEmail(string email)
        {
            // Validación simple con System.Net.Mail
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
