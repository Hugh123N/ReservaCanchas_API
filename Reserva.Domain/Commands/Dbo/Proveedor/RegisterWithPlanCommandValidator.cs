using FluentValidation;
using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.Proveedor
{
    public class RegisterWithPlanCommandValidator : CommandValidatorBase<RegisterWithPlanCommand>
    {
        public RegisterWithPlanCommandValidator()
        {
            RequiredInformation(x => x.Dto).DependentRules(() =>
            {
                RuleFor(x => x.Dto.Nombre)
                    .NotEmpty().WithMessage("El nombre es requerido.")
                    .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

                RuleFor(x => x.Dto.Apellidos)
                    .NotEmpty().WithMessage("Los apellidos son requeridos.")
                    .MaximumLength(100).WithMessage("Los apellidos no pueden superar los 100 caracteres.");

                RuleFor(x => x.Dto.Email)
                    .NotEmpty().WithMessage("El email es requerido.")
                    .EmailAddress().WithMessage("El email no es válido.")
                    .MaximumLength(256).WithMessage("El email no puede superar los 256 caracteres.");

                RuleFor(x => x.Dto.UserName)
                    .NotEmpty().WithMessage("El nombre de usuario es requerido.")
                    .MaximumLength(256).WithMessage("El nombre de usuario no puede superar los 256 caracteres.");

                RuleFor(x => x.Dto.Password)
                    .NotEmpty().WithMessage("La contraseña es requerida.")
                    .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres.");

                RuleFor(x => x.Dto.ConfirmPassword)
                    .NotEmpty().WithMessage("La confirmación de contraseña es requerida.")
                    .Equal(x => x.Dto.Password).WithMessage("Las contraseñas no coinciden.");

                RuleFor(x => x.Dto.IdPlane)
                    .GreaterThan(0).WithMessage("El plan es requerido.");

                RuleFor(x => x.Dto.IdPlanTarifa)
                    .GreaterThan(0).WithMessage("La tarifa del plan es requerida.");
            });
        }
    }
}
