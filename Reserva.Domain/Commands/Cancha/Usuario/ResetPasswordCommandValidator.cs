using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reserva.Domain.Commands.Cancha.Usuario
{
    public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator()
        {
            RuleFor(x => x.ResetPasswordDto.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.ResetPasswordDto.Token).NotEmpty();
            RuleFor(x => x.ResetPasswordDto.NewPassword).NotEmpty().MinimumLength(6);
        }
    }
}
