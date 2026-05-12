using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Reserva.Domain.Commands.Base;
using Reserva.Entity;

namespace Reserva.Domain.Commands.Dbo.Usuario
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
                RuleFor(x => x.CreateDto.FirstName)
                    .NotEmpty().WithMessage("El nombre es obligatorio.")
                    .MaximumLength(100).WithMessage("El nombre no debe exceder los 100 caracteres.")
                    .Matches(@"^[\p{L}\s]+$").WithMessage("El nombre solo debe contener letras y espacios.");

                RuleFor(x => x.CreateDto.LastName)
                    .NotEmpty().WithMessage("El apellido es obligatorio.")
                    .MaximumLength(100).WithMessage("El apellido no debe exceder los 100 caracteres.")
                    .Matches(@"^[\p{L}\s]+$").WithMessage("El apellido solo debe contener letras y espacios.");

                RuleFor(x => x.CreateDto.Email)
                    //.NotEmpty().WithMessage("El correo electronico es obligatorio.")
                    .EmailAddress().WithMessage("El correo electronico no tiene un formato valido.")
                    .MaximumLength(100)
                    .MustAsync(CorreoNoExiste)
                    .WithMessage("El correo electronico ya esta registrado.");
                    
                /*RuleFor(x => x.CreateDto.PhoneNumber)
                    .Matches(@"^\d{9}$").WithMessage("El n�mero de tel�fono debe tener 9 d�gitos.")
                    .MustAsync(TelefonoNoExiste)
                    .WithMessage("El n�mero de tel�fono ya est� registrado.");*/

            });
        }

        private async Task<bool> CorreoNoExiste(string? email, CancellationToken cancellationToken)
        {
            if (email != null) { 
                var user = await _userManager.FindByEmailAsync(email);
                return user == null;
            }
            return true;
        }

        private async Task<bool> TelefonoNoExiste(string telefono, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByLoginAsync("PhoneNumber", telefono);
            return user == null;
        }
    }
}
