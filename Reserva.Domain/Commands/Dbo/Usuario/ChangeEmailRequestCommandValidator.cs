using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Reserva.Domain.Commands.Base;
using Reserva.Entity;
using Reserva.Repository.Security;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Reserva.Domain.Commands.Dbo.Usuario
{
    public class ChangeEmailRequestCommandValidator : CommandValidatorBase<ChangeEmailRequestCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserIdentity _userIdentity;

        public ChangeEmailRequestCommandValidator(
            UserManager<ApplicationUser> userManager,
            IUserIdentity userIdentity)
        {
            _userManager = userManager;
            _userIdentity = userIdentity;

            RuleFor(x => x.NewEmail)
                .NotEmpty().WithMessage("El correo es requerido.")
                .MaximumLength(256).WithMessage("El correo no debe exceder los 256 caracteres.")
                .EmailAddress().WithMessage("El correo no tiene un formato válido.")
                .MustAsync(BeDifferentFromCurrentEmail)
                .WithMessage("El correo ingresado es tu correo actual.");

            RuleFor(x => x.Host)
                .NotEmpty().WithMessage("El host es requerido.")
                // El host se usa para armar el enlace de confirmación: no debe llevar esquema ni ruta
                .Must(host => host == null || (!host.Contains("://") && !host.Contains('/')))
                .WithMessage("El host no es válido.");
        }

        // Si no se puede identificar al usuario se omite la regla: el handler responde con el error correspondiente
        private async Task<bool> BeDifferentFromCurrentEmail(
            ChangeEmailRequestCommand command,
            string newEmail,
            ValidationContext<ChangeEmailRequestCommand> context,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(newEmail))
                return true;

            var userId = _userIdentity.GetCurrentUserId();
            if (userId == null)
                return true;

            var user = await _userManager.FindByIdAsync(userId.Value.ToString());
            if (user?.Email == null)
                return true;

            return !string.Equals(user.Email.Trim(), newEmail.Trim(), StringComparison.OrdinalIgnoreCase);
        }
    }
}
