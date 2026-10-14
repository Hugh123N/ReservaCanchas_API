using FluentValidation;
using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.Operador
{
    public class CreateOperadorCommandValidator : CommandValidatorBase<CreateOperadorCommand>
    {
        public CreateOperadorCommandValidator()
        {
            RequiredInformation(x => x.CreateDto).DependentRules(() =>
            {
                RuleFor(x => x.CreateDto.UserName)
                    .NotEmpty().WithMessage("El nombre de usuario es requerido.")
                    .MinimumLength(3).WithMessage("El nombre de usuario debe tener al menos 3 caracteres.")
                    .MaximumLength(50).WithMessage("El nombre de usuario no debe exceder los 50 caracteres.")
                    .Matches(@"^[a-zA-Z0-9._-]+$").WithMessage("El nombre de usuario solo puede contener letras, números, puntos, guiones y guiones bajos.");
            });
        }
    }
}
