using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Entity;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Dbo.Cliente
{
    public class UpdateClienteCommandValidator : CommandValidatorBase<UpdateClienteCommand>
    {
        private readonly IRepository<Entity.Cliente> _repositoryBase;
        private readonly UserManager<ApplicationUser> _userManager;

        public UpdateClienteCommandValidator(
            IRepository<Entity.Cliente> repositoryBase,
            UserManager<ApplicationUser> userManager)
        {
            _repositoryBase = repositoryBase;
            _userManager = userManager;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.IdCliente, "IdCliente")
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.IdCliente)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });

                RequiredString(x => x.UpdateDto.Nombres, "Nombres", null, 100);

                RequiredString(x => x.UpdateDto.Apellidos, "Apellidos", null, 100);

                RuleFor(x => x.UpdateDto.Email)
                    .NotEmpty().WithMessage("El correo electrónico es obligatorio.")
                    .EmailAddress().WithMessage("El correo electrónico no tiene un formato válido.")
                    .MaximumLength(100).WithMessage("El correo electrónico no debe exceder los 100 caracteres.")
                    .MustAsync(EmailNoEnUsoEnOtroUsuario).WithMessage("El correo electrónico ya está registrado.");

                RuleFor(x => x.UpdateDto.Telefono)
                    .MaximumLength(20).WithMessage("El teléfono no debe exceder los 20 caracteres.")
                    .When(x => !string.IsNullOrEmpty(x.UpdateDto.Telefono));

                // El username no se recibe: el cliente conserva el existente
            });
        }

        protected async Task<bool> ValidateExistenceAsync(UpdateClienteCommand command, int id, ValidationContext<UpdateClienteCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdCliente == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.UpdateRecordNotFound);
            return true;
        }

        private async Task<bool> EmailNoEnUsoEnOtroUsuario(UpdateClienteCommand command, string? email, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(email)) return true;

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return true;

            var cliente = await _repositoryBase.FindAll().AsNoTracking()
                .FirstOrDefaultAsync(x => x.IdCliente == command.UpdateDto.IdCliente, cancellationToken);

            return cliente != null && string.Equals(cliente.UserId, user.Id.ToString(), StringComparison.OrdinalIgnoreCase);
        }
    }
}
