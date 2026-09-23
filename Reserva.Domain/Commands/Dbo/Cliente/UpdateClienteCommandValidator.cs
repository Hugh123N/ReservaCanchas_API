using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.Dbo.Cliente
{
    public class UpdateClienteCommandValidator : CommandValidatorBase<UpdateClienteCommand>
    {
        private readonly IRepository<Entity.Cliente> _repositoryBase;
        public UpdateClienteCommandValidator(IRepository<Entity.Cliente> repositoryBase)
        {
            _repositoryBase = repositoryBase;

            RequiredInformation(x => x.UpdateDto).DependentRules(() =>
            {
                RequiredField(x => x.UpdateDto.IdCliente, Resources.Dbo.Operador.IdOperador)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UpdateDto.IdCliente)
                            .MustAsync(ValidateExistenceAsync)
                            .WithCustomValidationMessage();
                    });
                //RequiredString(x => x.UpdateDto.Codigo, Resources.Dbo.Cliente.Codigo, 5, 10);
                //RequiredField(x => x.UpdateDto.FechaIngreso, Resources.Dbo.Cliente.FechaIngreso);
            });
        }

        protected async Task<bool> ValidateExistenceAsync(UpdateClienteCommand command, int id, ValidationContext<UpdateClienteCommand> context, CancellationToken cancellationToken)
        {
            var exists = await _repositoryBase.FindAll().Where(x => x.IdCliente == id).AnyAsync(cancellationToken);
            if (!exists) return CustomValidationMessage(context, Resources.Common.UpdateRecordNotFound);
            return true;
        }
    }
}
