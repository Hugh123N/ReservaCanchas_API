using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Reserva.Domain.Commands.Base;
using Reserva.Entity;

namespace Reserva.Domain.Commands.Cancha.Usuario
{
    public class CreateUsuarioCommandValidator : CommandValidatorBase<CreateUsuarioCommand>
    {
        UserManager<ApplicationUser> _userManager;

        public CreateUsuarioCommandValidator(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;

            RuleFor(x => x.CreateDto)
            .NotNull().WithMessage("El objeto CreateDto no puede ser nulo.");

            When(x => x.CreateDto != null, () =>
            {
                RuleFor(x => x.CreateDto.UserName)
                    .NotEmpty().WithMessage("El nombre es obligatorio.")
                    .MaximumLength(100).WithMessage("El nombre no debe exceder los 100 caracteres.")
                    .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]*$").WithMessage("El nombre solo debe contener letras y espacios.");

                RuleFor(x => x.CreateDto.LastName)
                    .NotEmpty().WithMessage("El apellido es obligatorio.")
                    .MaximumLength(100).WithMessage("El apellido no debe exceder los 100 caracteres.")
                    .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]*$").WithMessage("El apellido solo debe contener letras y espacios.");

                RuleFor(x => x.CreateDto.Email)
                    .NotEmpty().WithMessage("El correo electrónico es obligatorio.")
                    .EmailAddress().WithMessage("El correo electrónico no tiene un formato válido.")
                    .MaximumLength(100)
                    .MustAsync(CorreoNoExiste)
                    .WithMessage("El correo electrónico ya está registrado.");
                    
                RuleFor(x => x.CreateDto.PhoneNumber)
                    .Matches(@"^\d{9}$").WithMessage("El número de teléfono debe tener 9 dígitos.")
                    .MustAsync(TelefonoNoExiste)
                    .WithMessage("El número de teléfono ya está registrado.");

                RuleFor(x => x.CreateDto.Password)
                    .NotEmpty().WithMessage("La contraseña es obligatoria.")
                    .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres.")
                    .Matches("[A-Z]").WithMessage("La contraseña debe contener al menos una letra mayúscula.")
                    .Matches("[a-z]").WithMessage("La contraseña debe contener al menos una letra minúscula.")
                    .Matches("[0-9]").WithMessage("La contraseña debe contener al menos un número.");
            });
        }

        private async Task<bool> CorreoNoExiste(string email, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user == null;
        }

        private async Task<bool> TelefonoNoExiste(string telefono, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByLoginAsync("PhoneNumber", telefono);
            return user == null;
        }
    }
}
