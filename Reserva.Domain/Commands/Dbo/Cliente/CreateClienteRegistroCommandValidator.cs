using FluentValidation;
using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.Cliente
{
    public class CreateClienteRegistroCommandValidator : CommandValidatorBase<CreateClienteRegistroCommand>
    {
        public CreateClienteRegistroCommandValidator()
        {
            RuleFor(x => x.CreateDto)
                .NotNull().WithMessage("El objeto CreateDto no puede ser nulo.");

            When(x => x.CreateDto != null, () =>
            {
                RuleFor(x => x.CreateDto.FirstName)
                    .NotEmpty().WithMessage("El nombre es obligatorio.")
                    .MaximumLength(100).WithMessage("El nombre no debe exceder los 100 caracteres.")
                    .Matches(@"^[\p{L}\s]+$").WithMessage("El nombre solo debe contener letras y espacios.");

                RuleFor(x => x.CreateDto.LastName)
                    .NotEmpty().WithMessage("El apellido es obligatorio.")
                    .MaximumLength(100).WithMessage("El apellido no debe exceder los 100 caracteres.")
                    .Matches(@"^[\p{L}\s]+$").WithMessage("El apellido solo debe contener letras y espacios.");

                RuleFor(x => x.CreateDto.Email)
                    .NotEmpty().WithMessage("El correo electrónico es obligatorio.")
                    .EmailAddress().WithMessage("El correo electrónico no tiene un formato válido.")
                    .MaximumLength(100).WithMessage("El correo electrónico no debe exceder los 100 caracteres.");

                RuleFor(x => x.CreateDto.PhoneNumber)
                    .MaximumLength(20).WithMessage("El teléfono no debe exceder los 20 caracteres.");

                RuleFor(x => x.CreateDto.UserName)
                    .MaximumLength(100).WithMessage("El nombre de usuario no debe exceder los 100 caracteres.");

                RuleFor(x => x.CreateDto.Password)
                    .NotEmpty().WithMessage("La contraseña es obligatoria.")
                    .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres.");

                RuleFor(x => x.CreateDto.ConfirmPassword)
                    .NotEmpty().WithMessage("La confirmación de contraseña es obligatoria.")
                    .Equal(x => x.CreateDto.Password).WithMessage("Las contraseñas no coinciden.");
            });
        }
    }
}
