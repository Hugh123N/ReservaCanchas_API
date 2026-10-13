using FluentValidation;
using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.Usuario
{
    public class ChangeEmailConfirmCommandValidator : CommandValidatorBase<ChangeEmailConfirmCommand>
    {
        public ChangeEmailConfirmCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("El usuario es requerido.");
            RuleFor(x => x.Email).NotEmpty().WithMessage("El correo es requerido.").EmailAddress();
            RuleFor(x => x.Token).NotEmpty().WithMessage("El token es requerido.");
        }
    }
}
